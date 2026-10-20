---
tags: [onefold, decisions]
updated: 2026-10-20
---

# Decisions

See [[Project Home]], [[Architecture]], and [[Features and API]]. These entries preserve established constraints and observed choices. Reconstructed entries do not claim a historical discussion or approval that is absent from the available evidence.

## D-001 — Keep project memory in this repository

- **Status:** Explicit user direction, 2026-10-19.
- **Decision:** Use the repository's Obsidian vault to retain project details, decisions, and context useful to a new AI session.
- **Implementation:** Linked topic notes under `docs/context/`, with an entry point and maintenance instructions in [AGENTS.md](../../AGENTS.md).
- **Consequence:** Update knowledge alongside meaningful changes. Keep durable facts separate from dated session status. Verify against current code and avoid secrets and workspace state.
- **Evidence:** User request in the session recorded in [[Session Log]]. The folder organization is an implementation choice made during this documentation task.

## D-002 — Direct handlers and explicit persistence boundaries

- **Status:** Established repository guidance and observed implementation; original adoption rationale not recorded here.
- **Decision:** Controllers invoke directly injected handlers. Application owns persistence abstractions and commits workflows through `IUnitOfWork`; Infrastructure owns EF Core queries and mappings; Domain owns entity invariants.
- **Consequence:** New handlers and repositories need explicit dependency-injection registration. Do not add mediator dispatch or bypass aggregate methods as incidental cleanup.
- **Evidence:** [AGENTS.md](../../AGENTS.md), [Application registrations](../../Application/Application.cs), [Infrastructure registrations](../../Infrastructure/Infrastructure.cs).

## D-003 — Store weekly schedules on templates

- **Status:** Reconstructed from implemented behavior in commit `074694d`; original product discussion not available.
- **Decision:** Represent a weekly schedule as unique `DayOfWeek` values on `WorkoutTemplate`, ordered Monday first. Replace the collection as a whole and use an empty collection for unscheduled templates.
- **Persistence:** Map the private array to a required PostgreSQL `integer[]` column with an empty-array default. Existing templates remain unscheduled after migration. Filter weekday membership in the database.
- **Consequence:** The model supports weekly membership without dates or time zones. More complex recurrence would require a new product/design decision. Keep the public collection read-only and mutate through `ReplaceSchedule`.
- **Migration context:** A separate prerequisite migration adds the previously modeled `WorkoutExercises.WeightIncrement` column; preserve both migrations and the snapshot rather than rewriting the initial migration.
- **Evidence:** [WorkoutTemplate](../../Domain/Entities/Workout/Template/WorkoutTemplate.cs), [mapping](../../Infrastructure/Database/Configurations/WorkoutTemplateConfiguration.cs), [weight migration](../../Infrastructure/Migrations/20261016081250_AddWorkoutTemplateExerciseWeightIncrement.cs), [schedule migration](../../Infrastructure/Migrations/20261016081251_AddWorkoutTemplateScheduledDays.cs), [integration checks](../../Tests/WorkoutTemplateScheduleIntegrationChecks.cs).

## D-004 — Return structured validation and exception details

- **Status:** Reconstructed from implementation in commit `6f34ebc`; original rationale not recorded here.
- **Decision:** Use problem responses with trace IDs, preserve field-level validation messages, map known domain/application exceptions, and keep unexpected error details generic in responses.
- **Consequence:** New exception types need a deliberate mapping. Model-binding and handler validation should remain consistent. MVC exception handling does not imply equivalent formatting for all middleware responses.
- **Evidence:** [ExceptionFilter](../../Onefold/Filters/ExceptionFilter.cs), [ErrorResponses](../../Onefold/Filters/ErrorResponses.cs), [Program](../../Onefold/Program.cs), [checks](../../Tests/Program.cs).

## D-005 — Search active available exercises by name

- **Status:** Explicit user selections and approved implementation plan, 2026-10-20.
- **Decision:** Provide authenticated, case-insensitive contains search over active global and active current-user exercises, with paged results and a total count. Interpret wildcard characters literally.
- **Consequence:** Other users' exercises and inactive exercises are excluded from items and counts. Existing creation leaves personal exercises inactive; changing that lifecycle is outside this feature. No schema change or deployment is required.
- **Contract defaults:** Required trimmed name of 1–255 characters; page 1, page size 20 (maximum 100); order by name then ID. Return existing ID/name DTOs in a pagination envelope.
- **Evidence:** User selected contains matching, active global plus active personal visibility, and pagination, then requested implementation of the proposed plan. See [handler](../../Application/Features/Workout/Exercise/SearchExercises/SearchExercisesHandler.cs), [repository](../../Infrastructure/Repository/ExerciseRepository.cs), and [integration checks](../../Tests/ExerciseSearchIntegrationChecks.cs).

## Adding or replacing decisions

Assign the next stable `D-###` identifier. Record date, status (explicit direction, observed implementation, proposed, or superseded), context, chosen behavior, consequences, and source evidence. Record alternatives only if they were actually considered. If a decision changes, mark the old entry superseded and link its replacement instead of erasing the history.
