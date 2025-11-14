using ContractService.Application.Behaviors;
using ContractService.Application.Handlers;
using ContractService.Application.Ports;
using ContractService.Application.Validators;
using ContractService.Infrastructure;
using ContractService.Infrastructure.Messaging.Consumers;
using ContractService.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Database
// ------------------------------------------------------------
builder.Services.AddDbContext<ContractDbContext>(opt =>
    opt.UseInMemoryDatabase("ContractDb"));

// ------------------------------------------------------------
// Repositories / Cache
// ------------------------------------------------------------
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddSingleton<IProposalStatusCacheRepository, ProposalStatusCacheRepository>();

// ------------------------------------------------------------
// RabbitMQ
// ------------------------------------------------------------
var rabbitHost = builder.Configuration.GetValue<string>("RabbitMQ:HostName") ?? "rabbitmq";
var rabbitUser = builder.Configuration.GetValue<string>("RabbitMQ:UserName") ?? "guest";
var rabbitPass = builder.Configuration.GetValue<string>("RabbitMQ:Password") ?? "guest";

builder.Services.AddSingleton<IConnection>(_ =>
{
    var factory = new ConnectionFactory
    {
        HostName = rabbitHost,
        UserName = rabbitUser,
        Password = rabbitPass,
        DispatchConsumersAsync = true
    };

    return factory.CreateConnection();
});

// Hosted consumer
builder.Services.AddHostedService<ProposalStatusChangedConsumer>();

// ------------------------------------------------------------
// MediatR + Validation
// ------------------------------------------------------------
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateContractHandler).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(CreateContractValidator).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// ------------------------------------------------------------
// Controllers + Swagger
// ------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Contract API",
        Version = "v1",
        Description = "Contract API - Indt."
    });
});

// ------------------------------------------------------------
// Web Host
// ------------------------------------------------------------
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
});

// ------------------------------------------------------------
// App Pipeline
// ------------------------------------------------------------
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contract API DEV");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
