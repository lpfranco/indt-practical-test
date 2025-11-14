# INDT Practical Test

![Build](https://github.com/lpfranco/indt-practical-test/actions/workflows/dotnet.yml/badge.svg)
![Tests](https://img.shields.io/badge/tests-passing-brightgreen)
![Docker](https://img.shields.io/badge/docker-ready-blue)
![.NET](https://img.shields.io/badge/.NET-7.0-blue)

Repositório contendo dois microserviços em .NET 8: **ContractService** e **ProposalService**, orquestrados com Docker Compose.  
Servem como teste prático para criação e gestão de propostas e contratos.

---

## 📂 Estrutura do projeto

```
/
├── ContractService/
│   └── src/ContractService/        # Projeto .NET do serviço de contrato
│       ├── ContractService.sln
│       └── Dockerfile
│
├── ProposalService/
│   └── src/ProposalService/        # Projeto .NET do serviço de proposta
│       ├── ProposalService.sln
│       └── Dockerfile
│
└── docker-compose.yml               # Orquestração de todos os contêineres
```

---

## 🚀 Pré-requisitos

- [Docker](https://www.docker.com/products/docker-desktop)  
- [Docker Compose](https://docs.docker.com/compose/)  
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (opcional, para rodar sem Docker)  

---

## 🐳 Rodando com Docker

1. Clone o repositório:

```bash
git clone https://github.com/lpfranco/indt-practical-test.git
cd indt-practical-test
```

2. Suba todos os serviços:

```bash
docker-compose up --build
```

3. Acesse os serviços:

- **Proposal Service**: [http://localhost:5002](http://localhost:5002)  
- **Contract Service**: [http://localhost:5001](http://localhost:5001)  

---

## 🔧 Rodando sem Docker (local)

1. Abra o `.sln` no Visual Studio ou VS Code.  
2. Execute cada projeto individualmente:

```bash
cd ProposalService/src/ProposalService
dotnet run

cd ContractService/src/ContractService
dotnet run
```

> Certifique-se de que o RabbitMQ está rodando localmente ou altere as variáveis de ambiente.

---

## ⚙️ Configuração de ambiente

As variáveis de ambiente padrão no `docker-compose.yml` são:

```env
RABBITMQ_HOST=rabbitmq
RABBITMQ_USER=guest
RABBITMQ_PASS=guest
ASPNETCORE_ENVIRONMENT=Development
```

> Pode criar um arquivo `.env` na raiz do repositório para customizar essas variáveis.

---

## 📦 Build das imagens Docker

Para apenas construir as imagens:

```bash
docker-compose build
```

Para um serviço específico:

```bash
docker-compose build proposal-service-api
docker-compose build contract-service-api
```

---

## 🧪 Testes e Cobertura

Para rodar testes unitários:

```bash
cd ProposalService/src/ProposalService.Tests
dotnet test --logger "console;verbosity=detailed"

cd ContractService/src/ContractService.Tests
dotnet test --logger "console;verbosity=detailed"
```

---

## 📝 Endpoints da API

### ProposalService

| Método | Endpoint                   | Descrição                     |
|--------|---------------------------|--------------------------------|
| POST   | `/api/proposals`           | Criar nova proposta            |
| PUT    | `/api/proposals/{id}/status` | Alterar status da proposta     |
| GET    | `/api/proposals/{id}`     | Obter detalhes da proposta     |

### ContractService

| Método | Endpoint           | Descrição                 |
|--------|-----------------|---------------------------|
| POST   | `/api/contracts` | Criar novo contrato       |
| GET    | `/api/contracts/{id}` | Obter detalhes do contrato |

> Você pode usar [Proposal Swagger](http://localhost:5002/swagger) ou [Contract Swagger](http://localhost:5001/swagger) para documentação interativa.

---

## 🔄 Exemplo de chamadas HTTP (cURL)

Criar proposta:

```bash
curl -X POST http://localhost:5002/api/proposals \
-H "Content-Type: application/json" \
-d '{"customerName":"João Silva","amount":5000}'
```

Alterar status de proposta:

```bash
curl -X PUT http://localhost:5002/api/proposals/<proposalId>/status \
-H "Content-Type: application/json" \
-d '{"newStatus":"Aprovada"}'
```

Criar contrato:

```bash
curl -X POST http://localhost:5001/api/contracts \
-H "Content-Type: application/json" \
-d '{"proposalId":"<proposalId>"}'
```

---

## 🔗 Fluxo dos microserviços

1. **ProposalService** cria propostas e altera status.  
2. **ContractService** consome eventos e cria contratos quando a proposta é aprovada.  
3. **RabbitMQ** é usado para comunicação assíncrona entre os serviços.

---

## 📚 Referências

- [Microservices .NET com Docker Compose](https://learn.microsoft.com/pt-br/dotnet/architecture/microservices/multi-container-microservice-net-applications/multi-container-applications-docker-compose)  
- [Workflow Docker para .NET](https://learn.microsoft.com/pt-br/dotnet/architecture/microservices/docker-application-development-process/docker-app-development-workflow)  

---


