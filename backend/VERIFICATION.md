# Backend verification report

Verified on 2026-09-14.

## Required quality gate

| Command | Result |
|---|---|
| dotnet restore backend/Challenge.SupportRequests.sln | PASS |
| dotnet build backend/Challenge.SupportRequests.sln --no-restore | PASS — 0 warnings, 0 errors |
| dotnet test backend/Challenge.SupportRequests.sln --no-build | PASS — 16 unit + 26 HTTP integration tests; 1 opt-in SQL test skipped by default |
| dotnet format backend/Challenge.SupportRequests.sln --verify-no-changes | PASS |

The opt-in SQL test was also executed separately against local SQL Server: **1 passed, 0 failed**. Total executed successful test cases: **43**.

## Real environment verification

- Docker Desktop was initially stopped/starting, then became available.
- Created `sql_testes` with SQL Server 2022 Developer and persistent `sql_testes_data`, port 1433.
- SQL readiness query passed.
- Applied `001_create_database_and_solicitacoes.sql` to `SupportRequestsDb`.
- An isolated test database exercised migration twice, Portuguese-to-English mapping, UTC timestamps, status/priority filters, search across text fields, literal wildcard handling, injection-like input, deterministic pagination/counts, updates, deletion and the priority CHECK constraint. The test database was removed.
- Started the actual API on localhost:3000 using ephemeral test configuration.
- `/api/health`, `/swagger`, `/swagger-json`, `/swagger-yaml`: 200.
- Missing bearer: 401. With a synthetic short-lived test JWT: create 201, completion 200, completed -> open 409, completed -> inProgress -> open 200, deletion 204.
- Removed the temporary support request and stopped the API process. SQL Server remains running.

## Checklist evaluation

Reviewed `engineering/11-ENGINEERING-CHECKLIST.md` and `project/11-ACCEPTANCE-CRITERIA.md`.

| Area | Evaluation and evidence |
|---|---|
| Architecture | PASS: four production projects, inward references, dependency-free Domain, Application repository port, thin controllers |
| Required use cases | PASS: create, get, list/filter/search/page, priority update, status update, open-only delete |
| State rules | PASS: server completion time, idempotent completion, completed-to-open conflict, documented inProgress reopening |
| Input validation | PASS: required/max-length text rules, Portuguese FluentValidation messages, enum strings, explicit pagination bounds |
| HTTP and errors | PASS: English camelCase contracts, known status responses, centralized Problem Details, safe unexpected error details |
| Persistence | PASS: Dapper/SqlClient, parameterized values, SQL migration, PK/CHECK/index, deterministic OFFSET/FETCH and COUNT_BIG |
| Authentication implementation | PASS: cryptographic JWT validation with HS256, issuer/audience/expiration checks, missing/wrong/expired/malformed token rejection |
| Evaluator's actual JWT | NOT VERIFIED: the local secret is not committed to Git and was not used in the tracked verification files; a local challenge secret can be supplied in the developer environment only |
| Documentation routes | PASS: JSON, YAML and interactive Swagger UI; bearer security in OpenAPI |
| Tests | PASS: mandatory unit/HTTP suite plus opt-in real SQL verification; default suite has no external service requirement |
| Secrets | PASS: .env ignored; no supplied secret/token stored in generated tracked files; no full-token or connection-string logging |
| Observability | PASS: structured request logs, safe correlation header/scope/Problem Details, SQL readiness |
| Documentation | PASS: README covers setup, decisions, index limits, authentication, tests, known limitations, future work, time placeholder and truthful AI disclosure |
| Git | PASS: filipe-hg; logical local commits; no push; preexisting authoring documents remain untouched/untracked |
| Optional CI / Compose | N/A: not implemented; real SQL integration chosen as the additional differential |

## Remaining local setup

Copy `backend/.env.example` to `backend/.env` and configure `DATABASE_URL` plus a local `JWT_SECRET`, `JWT_ISSUER` and `JWT_AUDIENCE`, then run:

```powershell
dotnet run --project backend/src/Challenge.SupportRequests.Api
```

The backend implementation and automated quality gate pass with cryptographic JWT validation against the configured local secret. Known design limits remain documented in README: no optimistic concurrency, separate list/count reads and simple wildcard search.
