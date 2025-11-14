FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ../../ProposalService.sln ./
COPY ./ProposalService.API/*.csproj ./ProposalService.API/
COPY ./ProposalService.Domain/*.csproj ./ProposalService.Domain/
COPY ./ProposalService.Application/*.csproj ./ProposalService.Application/
COPY ./ProposalService.Infrastructure/*.csproj ./ProposalService.Infrastructure/

RUN dotnet restore ProposalService.sln

COPY ./ProposalService.API ./ProposalService.API
COPY ./ProposalService.Application ./ProposalService.Application
COPY ./ProposalService.Domain ./ProposalService.Domain
COPY ./ProposalService.Infrastructure ./ProposalService.Infrastructure

WORKDIR /app/ProposalService.API
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/ProposalService.API/out .

ENTRYPOINT ["dotnet", "ProposalService.API.dll"]