---
tags: [onefold, architecture]
updated: 2026-10-19
---

# Architecture

See [[Project Home]], [[Features and API]], and [[Decisions]]. Verified against the checkout recorded in [[Current State]].

## Responsibilities

| Location | Responsibility | Entry point |
| --- | --- | --- |
| `Onefold/` | HTTP endpoints, startup, JSON serialization, exception responses, OpenAPI | [Program.cs](../../Onefold/Program.cs) |
| `Application/` | Feature workflows, request validation, DTOs, persistence/authentication interfaces | [Application.cs](../../Application/Application.cs) |
| `Domain/` | Entities, factories, mutation methods, business invariants | [WorkoutTemplate.cs](../../Domain/Entities/Workout/Template/WorkoutTemplate.cs) |
| `Infrastructure/` | EF Core/Npgsql, repositories, JWT, current user, migrations, seeders | [Infrastructure.cs](../../Infrastructure/Infrastructure.cs) |
| `Tests/` | Executable regression checks, optional PostgreSQL/HTTP checks | [test guide](../../Tests/README.md) |
| `.github/workflows/`, `cicd/` | Image publishing, Azure configuration, migrations, deployment | [deploy.yml](../../.github/workflows/deploy.yml) |

Application depends on Domain. Infrastructure implements Application abstractions and uses Domain. Onefold composes these layers. Keep Domain independent of HTTP and EF Core.

## Request and persistence flow

Controllers receive requests and call directly injected feature handlers. There is no mediator dispatch. Handlers validate requests, obtain identity through `ICurrentUser`, perform ownership checks, call repositories, mutate domain entities, and commit through `IUnitOfWork.SaveChangesAsync()`.

Handlers require registration in [Application.cs](../../Application/Application.cs); validators are discovered by assembly scanning. Persistence interfaces live in [Application abstractions](../../Application/Abstractions/Persistence/), and their implementations are registered in [Infrastructure.cs](../../Infrastructure/Infrastructure.cs). `AppDbContext` supplies `IUnitOfWork`.

EF queries, relationship mapping, and database-specific behavior belong in Infrastructure. Domain factories and mutation methods protect invariants; do not bypass them through setters or backing collections. Ownership must be checked explicitly in the workflow; an entity lookup by ID alone does not prove access.

## Data model landmarks

- Users have credentials, refresh tokens, profiles, templates, and sessions. Authentication uses JWT bearer tokens and password hashing through `IPasswordHasher<User>`.
- A workout template contains prescribed exercises and a weekly schedule. Template names have a unique composite database index with `UserId`.
- A template exercise holds prescription details such as target sets/reps, rest, and weight-related settings.
- A recorded session references a template and user, records start/completion times, and contains exercises and performed sets. [SaveSessionHandler](../../Application/Features/Workout/Session/SaveSession/SaveSessionHandler.cs) copies target sets/reps/rest from the template exercise when saving.

## Cross-cutting behavior

[Program.cs](../../Onefold/Program.cs) configures string enum JSON serialization, authentication, authorization, controllers, the exception filter, and model-binding validation responses. Development startup runs seeders and exposes OpenAPI, but does not apply migrations.

[ExceptionFilter.cs](../../Onefold/Filters/ExceptionFilter.cs) maps application/domain errors, including FluentValidation failures. [ErrorResponses.cs](../../Onefold/Filters/ErrorResponses.cs) produces problem responses with trace IDs. This is an MVC filter, so do not assume that middleware failures or bearer authentication challenges use the same response envelope.
