using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ProposalService.API.Filters;
using ProposalService.Application.Behaviors;
using ProposalService.Application.Commands;
using ProposalService.Application.Handlers;
using ProposalService.Application.Ports;
using ProposalService.Application.Validators;
using ProposalService.Infrastructure;
using ProposalService.Infrastructure.Messaging;
using ProposalService.Infrastructure.Repositories;
using RabbitMQ.Client;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ProposalDbContext>(opt => opt.UseInMemoryDatabase("ProposalDb"));

builder.Services.AddScoped<IProposalRepository, ProposalRepository>();


//rabbit
// Configurações opcionais vindas do appsettings.json ou variáveis de ambiente
var rabbitHost = builder.Configuration.GetValue<string>("RabbitMQ:HostName") ?? "rabbitmq";
var rabbitUser = builder.Configuration.GetValue<string>("RabbitMQ:UserName") ?? "guest";
var rabbitPass = builder.Configuration.GetValue<string>("RabbitMQ:Password") ?? "guest";

builder.Services.AddSingleton<IConnection>(_ =>
{
    var factory = new ConnectionFactory
    {
        HostName = rabbitHost,
        UserName = rabbitUser,
        Password = rabbitPass
    };
    return factory.CreateConnection();
});

builder.Services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();
//------------------------------------------------------------------------------
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); 
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProposalHandler).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(CreateProposalCommandValidator).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Proposal API",
        Version = "v1",
        Description = "Proposal API - Indt."
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proposal API DEV");
});


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
