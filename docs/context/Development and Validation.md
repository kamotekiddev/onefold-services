---
tags: [onefold, development, validation]
updated: 2026-10-19
---

# Development and validation

See [AGENTS.md](../../AGENTS.md) for repository rules and [[Current State]] for the last recorded verification. Commands below are repository-root commands unless noted. They are instructions, not evidence that they ran successfully in this session.

## SDK and build

[global.json](../../global.json) currently selects `10.0.0`, an invalid SDK feature-band version. Keep fixes to SDK/dependencies scoped to the task. Once a valid installed .NET 10 SDK selector is available, the normal build sequence is:

```sh
dotnet --info
dotnet restore Onefold.sln
dotnet build Onefold.sln --no-restore
```

[Tests/README.md](../../Tests/README.md) documents running from outside the repository to bypass this selector with an installed .NET 10 SDK. Do not turn an SDK-selector failure into an application regression report. Old nullable, dependency, and vulnerability warnings are historical observations; recheck actual output when building.

## Regression checks

`Tests/Tests.csproj` is an executable, not a conventional test-runner project:

```sh
dotnet run --project Tests/Tests.csproj
```

The documented SDK workaround, starting at the repository root, is:

```sh
onefold_repo="$PWD"
(cd /private/tmp && dotnet run --project "$onefold_repo/Tests/Tests.csproj")
```

[Program.cs](../../Tests/Program.cs) checks error responses and validation; [schedule checks](../../Tests/WorkoutTemplateScheduleChecks.cs) cover domain/handler behavior. [Integration checks](../../Tests/WorkoutTemplateScheduleIntegrationChecks.cs) use PostgreSQL and a temporary loopback HTTP server to exercise migrations, controllers, JWT, validation, and repositories.

Without `ONEFOLD_TEST_CONNECTION_STRING`, PostgreSQL/HTTP checks are skipped. For those checks, follow the disposable database setup and cleanup in [Tests/README.md](../../Tests/README.md). The suite requires a fresh local database named `onefold_schedule_test`, applies migrations, writes data, and rejects a database with already-applied migrations. Never point it at a development or production database. Report in-process and integration results separately.

## Local API

[docker-compose.yml](../../docker-compose.yml) defines PostgreSQL 18 on port 5432 with persistent storage. Before migrations or startup, inspect the effective connection target and configure a local development database using local environment overrides. Do not put credential values in notes or commits.

| Configuration key | Environment override |
| --- | --- |
| `ConnectionStrings:OnefoldDb` | `ConnectionStrings__OnefoldDb` |
| `Jwt:Issuer` | `Jwt__Issuer` |
| `Jwt:Audience` | `Jwt__Audience` |
| `Jwt:SecretKey` | `Jwt__SecretKey` |
| `Jwt:AccessTokenExpirationMinutes` | `Jwt__AccessTokenExpirationMinutes` |

After SDK and local configuration are ready:

```sh
docker compose up -d postgres
# Requires a compatible dotnet-ef 10.x tool.
dotnet ef database update --project Infrastructure/Infrastructure.csproj --startup-project Onefold/Onefold.csproj --context AppDbContext
dotnet run --project Onefold/Onefold.csproj --launch-profile http
```

Development startup seeds data and exposes OpenAPI; it does not apply migrations. [Launch profiles](../../Onefold/Properties/launchSettings.json) currently specify HTTP `http://localhost:5158` and HTTPS `https://localhost:7223`. Inspect the profiles again if these change.

For schema changes, update [mappings](../../Infrastructure/Database/Configurations/), generate an Infrastructure migration using the Onefold startup project, and review it with [the model snapshot](../../Infrastructure/Migrations/AppDbContextModelSnapshot.cs). Preserve applied migration history.

## Deployment and delivery

[deploy.yml](../../.github/workflows/deploy.yml) triggers a development deployment on pushes to `master`, and supports manual dev/prod selection. It publishes a GHCR image, configures Azure App Service, applies migrations, and deploys/restarts the app. Production selection requires a `release-X.Y.Z` input. These are deployment actions, not local verification.

Follow branch and PR title rules in [AGENTS.md](../../AGENTS.md). For documentation-only work, verify source paths, note links, commands, and the diff; no rebuild is required. Never claim an empty `dotnet test` invocation proves behavior.
