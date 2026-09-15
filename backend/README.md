# 🚀 API de Solicitações de Suporte - Backend

> Backend do desafio técnico Fullstack para cadastro, acompanhamento e conclusão de solicitações internas de suporte.

O escopo desta entrega é o **backend**, desenvolvido com **.NET 10**, **ASP.NET Core Web API**, **Dapper** e **Microsoft SQL Server**.

## 📋 Visão geral

A API permite cadastrar, consultar, listar, filtrar, pesquisar, atualizar e excluir solicitações internas, além de oferecer autenticação JWT, documentação OpenAPI e health check com SQL Server.

### 🛠 Stack principal

| Tecnologia | Uso |
|---|---|
| .NET 10 / ASP.NET Core | API REST |
| SQL Server 2022 Developer | Banco de dados |
| Dapper | Persistência |
| FluentValidation | Validação de entrada |
| JwtBearer | Autenticação JWT |
| xUnit | Testes |
| NSubstitute | Mocks |
| Bogus | Massa de dados |
| WebApplicationFactory | Testes HTTP |
| OpenAPI / Swagger | Contrato e documentação |

## 🧭 Requisitos do desafio

| Requisito | Status |
|---|:---:|
| Criar solicitação | ✅ |
| Consultar por ID | ✅ |
| Listar com filtros, busca e paginação | ✅ |
| Atualizar prioridade e status | ✅ |
| Excluir apenas solicitações abertas | ✅ |
| Data de conclusão automática | ✅ |
| `404 Not Found` para registros inexistentes | ✅ |
| SQL Server + migration versionada | ✅ |
| Dapper com SQL parametrizado | ✅ |
| Índice para filtros/ordenação | ✅ |
| Testes unitários e integração | ✅ |
| JWT Bearer | ✅ |
| OpenAPI / Swagger | ✅ |
| Logs estruturados + Correlation ID | ✅ |
| Health Check com SQL Server | ✅ |

## 🏛 Arquitetura

O projeto utiliza **Pragmatic Layered Clean Architecture**:

```text
Api / Presentation
        |
        v
Application
        |
        v
Domain

Infrastructure
    |
    +--> implementa portas da Application
    +--> persiste/mapeia entidades do Domain
```

```mermaid
flowchart TD
    Api["Api / Presentation"] --> Application["Application / Use Cases"]
    Application --> Domain["Domain / Business Rules"]
    Api -->|"Dependency Composition"| Infrastructure["Infrastructure / Dapper + SQL Server"]
    Infrastructure -->|"Implements Ports"| Application
    Infrastructure --> Domain
```

| Camada | Responsabilidade |
|---|---|
| `Api` | HTTP, autenticação, OpenAPI, middleware e tratamento de erros |
| `Application` | Casos de uso, DTOs, validação e orquestração |
| `Domain` | Entidade, enums e regras de negócio |
| `Infrastructure` | Dapper, SQL Server, repositório e health check |

A interface `ISupportRequestRepository` pertence à **Application** e sua implementação Dapper à **Infrastructure**.

Foram evitadas abstrações sem necessidade para o escopo do desafio, como MediatR, AutoMapper, Generic Repository, EF Core, Domain Events e microsserviços.

# Executando localmente

## Pré-requisitos

- .NET 10 SDK
- Docker Desktop
- PowerShell 7
- Git
- Postman, opcional

Implementação validada com **.NET SDK 10.0.300**.

## 1. Subir o SQL Server

Se o container já existir:

```powershell
docker start sql_testes
```

No primeiro uso:

```powershell
$env:MSSQL_SA_PASSWORD = Read-Host 'Senha local do SQL Server' -MaskInput

docker run --name sql_testes `
  -e ACCEPT_EULA=Y `
  -e MSSQL_SA_PASSWORD `
  -e MSSQL_PID=Developer `
  -p 1433:1433 `
  -v sql_testes_data:/var/opt/mssql `
  -d mcr.microsoft.com/mssql/server:2022-latest

Remove-Item Env:MSSQL_SA_PASSWORD
```

O volume `sql_testes_data` preserva os dados entre reinicializações.

## 2. Configurar `.env`

```powershell
Copy-Item .env.example .env
```

Exemplo:

```dotenv
ASPNETCORE_ENVIRONMENT=Development
PORT=3000
DATABASE_URL=Server=localhost,1433;Database=SupportRequestsDb;User Id=sa;Password=<SENHA_LOCAL>;Encrypt=False;TrustServerCertificate=True;
JWT_SECRET=<SECRET_LOCAL>
JWT_ISSUER=Challenge-Fullstack-Nextjs-Dotnet-SQLServer
JWT_AUDIENCE=Challenge-Fullstack-Nextjs-Dotnet-SQLServer
```

> O `.env` contém credenciais locais e não deve ser versionado. O `.env.example` contém somente placeholders.

## 3. Aplicar a migration

A migration está em:

```text
database/migrations/001_create_database_and_solicitacoes.sql
```

```powershell
$env:SQLCMDPASSWORD = Read-Host 'Senha atual do usuário sa' -MaskInput

docker exec -e SQLCMDPASSWORD sql_testes `
  /opt/mssql-tools18/bin/sqlcmd `
  -S localhost -U sa -C -b -Q "SELECT 1"

docker cp database/migrations/001_create_database_and_solicitacoes.sql `
  sql_testes:/tmp/001.sql

docker exec -e SQLCMDPASSWORD sql_testes `
  /opt/mssql-tools18/bin/sqlcmd `
  -S localhost -U sa -C -b -i /tmp/001.sql

Remove-Item Env:SQLCMDPASSWORD
```

A migration cria o banco `SupportRequestsDb`, a tabela `dbo.Solicitacoes`, restrições e índice. Ela é idempotente para a estrutura prevista pelo desafio.

## 4. Restaurar, compilar e executar

```powershell
dotnet restore Challenge.SupportRequests.sln
dotnet build Challenge.SupportRequests.sln --no-restore
dotnet run --project src/Challenge.SupportRequests.Api --launch-profile http
```

A API ficará disponível em:

```text
http://localhost:3000
```

# Documentação da API

| Recurso | URL |
|---|---|
| Health Check | http://localhost:3000/api/health |
| Swagger UI | http://localhost:3000/swagger |
| OpenAPI 3.1 JSON | http://localhost:3000/openapi/v1.json |

# Autenticação JWT

As rotas de negócio utilizam **ASP.NET Core JwtBearer**.

Não há endpoint de login ou emissão de token porque esse fluxo está fora do escopo do desafio. Para execução local é utilizado um JWT pré-gerado de longa duração.

A API valida:

- assinatura `HS256`;
- `JWT_SECRET`;
- `issuer`;
- `audience`;
- expiração;
- algoritmo permitido.

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwcm9qZWN0IjoiQ2hhbGxlbmdlLUZ1bGxzdGFjay1OZXh0anMtRG90bmV0LVNRTFNlcnZlciIsInVzZXIiOiJGaWxpcGVIRyIsImVtYWlsIjoiZmlsaXBlaC5nb25jYWx2ZXNAZ21haWwuY29tIiwiaXNzIjoiQ2hhbGxlbmdlLUZ1bGxzdGFjay1OZXh0anMtRG90bmV0LVNRTFNlcnZlciIsImF1ZCI6IkNoYWxsZW5nZS1GdWxsc3RhY2stTmV4dGpzLURvdG5ldC1TUUxTZXJ2ZXIiLCJleHAiOjQ5Mzg2MjQwMDB9.Egkf-JvxC62nDNvHaNyMu0n-hmgwbPKNaJNyYniWgmw
```

`/api/health` e a documentação OpenAPI são públicos.

Em produção, a estratégia seria evoluída para tokens curtos, rotação de chaves e Identity Provider.

# Endpoints

| Método | Endpoint | Operação |
|---|---|---|
| `GET` | `/api/health` | Health check |
| `POST` | `/api/support-requests` | Criar solicitação |
| `GET` | `/api/support-requests/{id}` | Consultar por ID |
| `GET` | `/api/support-requests` | Listar, pesquisar e filtrar |
| `PATCH` | `/api/support-requests/{id}/priority` | Alterar prioridade |
| `PATCH` | `/api/support-requests/{id}/status` | Alterar status |
| `DELETE` | `/api/support-requests/{id}` | Excluir solicitação aberta |

## Exemplo de criação

```json
{
  "title": "Impressora indisponível",
  "description": "A impressora do financeiro não está imprimindo.",
  "requester": "Maria Silva",
  "priority": "high"
}
```

## Filtros e paginação

```http
GET /api/support-requests?status=open&priority=high&search=impressora&page=1&limit=20
```

| Parâmetro | Regra |
|---|---|
| `status` | `open`, `inProgress`, `completed` |
| `priority` | `low`, `medium`, `high` |
| `search` | título, descrição e solicitante |
| `page` | padrão `1`, mínimo `1` |
| `limit` | padrão `100`, máximo `100` |

Ordenação padrão:

```sql
ORDER BY DataCriacao DESC, IdSolicitacao DESC
```

# Regras de negócio

- título, descrição e solicitante são obrigatórios;
- prioridade aceita `low`, `medium` ou `high`;
- uma nova solicitação inicia como `open`;
- ao concluir, `completedAt` é preenchido automaticamente;
- repetir `completed` preserva a data de conclusão;
- `completed -> open` é proibido;
- `completed -> inProgress` é permitido e limpa `completedAt`;
- somente solicitações `open` podem ser excluídas;
- registros inexistentes retornam `404 Not Found`;
- conflitos de regra retornam `409 Conflict`.

As regras de transição e exclusão ficam no **Domain**, não nos controllers.

# Banco de dados

Banco: `SupportRequestsDb`

Tabela: `dbo.Solicitacoes`

| Coluna | Tipo | Nulo |
|---|---|:---:|
| `IdSolicitacao` | `BIGINT IDENTITY` | Não |
| `Titulo` | `NVARCHAR(200)` | Não |
| `Descricao` | `NVARCHAR(4000)` | Não |
| `Solicitante` | `NVARCHAR(200)` | Não |
| `Prioridade` | `TINYINT` | Não |
| `Status` | `TINYINT` | Não |
| `DataCriacao` | `DATETIME2(3)` | Não |
| `DataConclusao` | `DATETIME2(3)` | Sim |

O banco possui `PK`, `CHECK` para prioridade e status, `DEFAULT SYSUTCDATETIME()` e índice composto:

```sql
CREATE INDEX IX_Solicitacoes_Status_Prioridade_DataCriacao
ON dbo.Solicitacoes
(
    Status,
    Prioridade,
    DataCriacao DESC,
    IdSolicitacao DESC
);
```

O índice favorece principalmente consultas filtradas por **status + prioridade** mantendo a ordenação das solicitações mais recentes.

A pesquisa textual utiliza `LIKE '%texto%'`; para volumes maiores, Full-Text Search seria uma evolução possível.

Todas as consultas executadas pela aplicação são parametrizadas pelo Dapper.

# Testes

A solução cobre:

- regras de domínio;
- casos de uso;
- validações;
- endpoints HTTP;
- autenticação JWT;
- OpenAPI;
- correlação e erros;
- persistência com SQL Server real.

## Suíte padrão

```powershell
dotnet test Challenge.SupportRequests.sln
```

## Teste HTTP

```powershell
dotnet test tests/Challenge.SupportRequests.IntegrationTests `
  --filter 'Category!=SqlServer'
```

## Teste com SQL Server real

O teste cria um banco temporário `SupportRequestsTest_<GUID>`, valida a persistência e remove o banco ao finalizar.

```powershell
$env:SUPPORT_REQUESTS_TEST_SQL = Read-Host `
  'Connection string do SQL Server local apontando para master' `
  -MaskInput

try {
    dotnet test tests/Challenge.SupportRequests.IntegrationTests `
      --filter 'Category=SqlServer'
}
finally {
    Remove-Item Env:SUPPORT_REQUESTS_TEST_SQL -ErrorAction SilentlyContinue
}
```

Na verificação registrada durante a implementação:

- **16 testes unitários**;
- **26 testes HTTP**;
- **1 teste de integração com SQL Server real**.

# Verificação completa

```powershell
dotnet restore Challenge.SupportRequests.sln
dotnet build Challenge.SupportRequests.sln --no-restore
dotnet test Challenge.SupportRequests.sln --no-build
dotnet format Challenge.SupportRequests.sln --verify-no-changes
```

O arquivo `VERIFICATION.md` contém evidências complementares.

# Erros e observabilidade

A API utiliza **Problem Details compatível com RFC 9457**.

| HTTP | Situação |
|---|---|
| `400` | Entrada inválida |
| `401` | JWT ausente ou inválido |
| `404` | Solicitação não encontrada |
| `409` | Conflito de regra de negócio |
| `500` | Erro inesperado |
| `503` | SQL Server indisponível |

Também foram implementados logs estruturados, `X-Correlation-ID`, medição de duração das requisições e health check do SQL Server.

# Decisões técnicas

- **Dapper:** SQL explícito, parametrizado e fácil de avaliar.
- **DDD Lite:** entidade com comportamento sem adicionar complexidade desnecessária.
- **Casos de uso explícitos:** operações claras na Application, sem dispatcher.
- **Migration SQL versionada:** suficiente para o escopo atual.
- **KISS/YAGNI:** sem MediatR, AutoMapper, EF Core ou infraestrutura não necessária ao desafio.

# Tempo dedicado e uso de IA

Tempo aproximado dedicado: **8 horas**.

O **OpenAI Codex** foi utilizado como ferramenta de apoio na análise de requisitos, estruturação inicial, implementação assistida, criação de cenários de teste, investigação de falhas e documentação.

Todas as decisões arquiteturais, regras de negócio e alterações produzidas com auxílio de IA foram revisadas pelo desenvolvedor. A solução foi compilada e as suíítes de testes foram executadas para validação da entrega.
