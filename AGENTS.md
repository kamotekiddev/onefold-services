# Repository guidance for AI agents

## Project map

Onefold is a .NET 10 ASP.NET Core API for authentication, exercises, workout templates, and workout sessions. The solution is `Onefold.sln`.

- `Onefold/`: HTTP controllers, startup/composition in `Program.cs`, the global exception filter, OpenAPI, configuration, and Dockerfile.
- `Application/`: feature handlers, request/response types, DTOs, FluentValidation validators, and authentication/persistence abstractions. Depends on Domain.
- `Domain/`: entities, aggregate behavior, and domain exceptions. Keep it independent of HTTP and persistence frameworks.
- `Infrastructure/`: EF Core/Npgsql persistence, repositories, JWT services, current-user implementation, migrations, and development seeders. Depends on Application and Domain.
- `.github/workflows/` and `cicd/`: GHCR image publishing and Azure deployment/configuration.

## Implementing changes

- Before implementing an approved plan, inspect the current Git branch. If the user has not already created or selected a working branch, start from the repository's default branch and create a task-specific branch using the `codex/` prefix before editing files.
- Never implement an approved plan directly on the default branch. If the user has already created or selected a non-default branch for the work, continue on that branch instead of creating another one.
- Follow the neighboring feature's organization under `Application/Features/<area>/<feature>/`. Controllers should delegate business work to handlers.
- Handlers are injected directly; there is no mediator dispatch layer. Register new handlers in `Application/Application.cs`. Validators are discovered through assembly scanning.
- Put persistence interfaces in `Application/Abstractions/Persistence/` and implementations in `Infrastructure/Repository/`. Register implementations in `Infrastructure/Infrastructure.cs`.
- Commit changes through `IUnitOfWork.SaveChangesAsync()` in the application workflow. Keep EF Core queries and mappings in Infrastructure.
- Preserve entity factory methods and domain mutation methods; enforce business invariants there rather than bypassing private setters or backing collections.
- Use `ICurrentUser` for authenticated identity and enforce ownership when accessing user-owned data. Preserve authorization requirements, routes, request/response shapes, and string enum serialization unless the task changes the API contract.
- Match surrounding C# conventions: four-space indentation, PascalCase types/members, camelCase locals/parameters, nullable annotations, async I/O, and structured logging. Existing namespace styles and handler method names vary; avoid unrelated normalization.
- Trace error handling through `Onefold/Filters/ExceptionFilter.cs`. It currently imports DataAnnotations `ValidationException`, while feature validators use FluentValidation; do not assume every validation or application exception already maps to the intended HTTP status.

## Build and validation

Run from the repository root:

```sh
dotnet --info
dotnet restore Onefold.sln
dotnet build Onefold.sln --no-restore
```

Known setup issue at initialization: `global.json` specifies SDK version `10.0.0`, which the installed .NET SDK reports as invalid (SDK feature bands begin at `10.0.100`). Check the current file before troubleshooting. Report this separately from errors introduced by a task; keep SDK/dependency changes scoped to the work.

There are currently no test projects in the solution. Do not describe a successful build or an empty `dotnet test` run as passing behavioral tests. For behavior changes, add focused regression coverage where practical and report any manual checks and remaining gaps. For documentation-only edits, check paths, commands, and the diff; a rebuild is unnecessary.

The initial build with a corrected SDK selector succeeded but reported nullable initialization warnings, a redundant dependency warning, and NuGet vulnerability warnings. Recheck current output instead of treating these observations as permanent or fixing unrelated warnings automatically.

## Local API and database

- `docker-compose.yml` defines PostgreSQL 18, exposed on port 5432, with a named persistent volume.
- Configuration is read from `ConnectionStrings:OnefoldDb` and `Jwt:Issuer`, `Jwt:Audience`, and `Jwt:SecretKey`. Use local environment overrides for credentials; do not copy secrets into instructions, logs, or commits.
- Development startup runs database seeders and exposes OpenAPI. It does not apply EF migrations; the schema must exist before starting the API.
- Inspect the configured connection target before applying migrations or running seeders. Use a local development database for local verification.

After resolving SDK configuration and configuring the local database/JWT settings:

```sh
docker compose up -d postgres
# Requires a compatible dotnet-ef 10.x tool.
dotnet ef database update --project Infrastructure/Infrastructure.csproj --startup-project Onefold/Onefold.csproj --context AppDbContext
dotnet run --project Onefold/Onefold.csproj --launch-profile http
```

The HTTP launch profile uses `http://localhost:5158`. The HTTPS profile also uses `https://localhost:7223`. Check `Onefold/Properties/launchSettings.json` for changes.

For schema changes, update entity mappings in `Infrastructure/Database/Configurations/`, generate a migration into `Infrastructure/Migrations/`, and review the migration and model snapshot together. Use the Infrastructure project and Onefold startup project for EF tooling; do not rewrite applied migrations to conceal schema changes.

## Pull requests

- Prefix every pull request title with the most applicable bracketed change type: `[Feature]`, `[Hotfix]`, `[Fix]`, `[Refactor]`, `[Chore]`, `[Docs]`, or `[Test]`.
- When an issue or ticket identifier is known, include it after the change type, for example `[Feature] [ONE-123] Add workout schedules` or `[Fix] [#42] Reject invalid refresh tokens`. Use the exact identifier supplied by the user or repository; never invent one.
- Keep the remainder of the title concise and action-oriented. Update the title if the final scope changes enough that the original type or description is no longer accurate.

## Scope and delivery

- Preserve unrelated working-tree changes and local files. Do not include `.DS_Store`, IDE state, build output, or credentials in commits.
- Keep changes focused on the requested behavior. Report what changed, checks performed, and any blockers or unverified behavior.
- Pushing to `master` triggers a deployment workflow, including database migrations and Azure changes. Treat pushes and deployment scripts as deployment actions, not routine local validation.
