FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ../../ContractService.sln ./
COPY ./ContractService.API/*.csproj ./ContractService.API/
COPY ./ContractService.Domain/*.csproj ./ContractService.Domain/
COPY ./ContractService.Application/*.csproj ./ContractService.Application/
COPY ./ContractService.Infrastructure/*.csproj ./ContractService.Infrastructure/
COPY ./ContractService.Test/*.csproj ./ContractService.Test/

RUN dotnet restore ContractService.sln

COPY ./ContractService.API ./ContractService.API
COPY ./ContractService.Application ./ContractService.Application
COPY ./ContractService.Domain ./ContractService.Domain
COPY ./ContractService.Infrastructure ./ContractService.Infrastructure
COPY ./ContractService.Test ./ContractService.Test

WORKDIR /app/ContractService.API
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/ContractService.API/out .

ENTRYPOINT ["dotnet", "ContractService.API.dll"]