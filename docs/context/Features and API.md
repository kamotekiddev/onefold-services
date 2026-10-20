---
tags: [onefold, api, workouts]
updated: 2026-10-20
---

# Features and API

This is a navigation and behavior summary, not a replacement for request types or generated OpenAPI. See [[Architecture]], [[Decisions]], and [[Current State]].

## Endpoint map

Routes below follow the controller names in the inspected checkout. All listed workout actions require authorization; authentication actions do not declare `[Authorize]`.

| Controller | Routes | Source |
| --- | --- | --- |
| Authentication | `POST /api/Auth/sign-up`, `/sign-in`, `/refresh` | [AuthController](../../Onefold/Controllers/Authentication/AuthController.cs) |
| Exercises | `POST /api/workout/Exercise`, `GET /api/workout/Exercise/{id}`, `GET /api/workout/Exercise/search` | [ExerciseController](../../Onefold/Controllers/Workout/ExerciseController.cs) |
| Templates | `POST /api/workout/Template`, `GET /api/workout/Template`, `GET /api/workout/Template/{id}`, `PUT /api/workout/Template/{id}/schedule`, `PUT /api/workout/Template/{id}/exercises` | [TemplateController](../../Onefold/Controllers/Workout/TemplateController.cs) |
| Sessions | `POST /api/workout/Session`, `GET /api/workout/Session`, `GET /api/workout/Session/{id}` | [SessionController](../../Onefold/Controllers/Workout/SessionController.cs) |

Signup returns 201. The other controller actions above return 200 on success. Inspect the feature request/response records before changing contracts.

## Authentication

[TokenService](../../Infrastructure/Authentication/TokenService.cs) issues signed JWT access tokens with user ID and email claims. Access-token lifetime is read from `Jwt:AccessTokenExpirationMinutes`; generated refresh tokens expire after one day in the inspected implementation.

[RefreshAccessTokenHandler](../../Application/Features/Authentication/RefreshAccessToken/RefreshAccessTokenHandler.cs) rejects missing, expired, or revoked tokens, revokes the accepted token, and persists a newly issued refresh token. Do not assume this provides concurrency or replay guarantees beyond what the code and tests establish.

## Exercise search

- `GET /api/workout/Exercise/search?name=press&page=1&pageSize=20` requires authentication and searches active global exercises plus active exercises belonging to the current user.
- `name` is required, trimmed, and must contain 1–255 characters after trimming. Matching is case-insensitive substring matching; `%`, `_`, and `\` are literal characters.
- `page` defaults to 1 and must be positive. `pageSize` defaults to 20 and accepts 1–100. Requests whose calculated offset exceeds `Int32.MaxValue` are rejected.
- The response is `{ items: [{ id, name }], page, pageSize, totalCount }`. Both items and total count use the same visibility and name filters. Results use database name order followed by ID; different exercises with the same name are retained.
- No matches return empty items and zero total count. Pages beyond the results return empty items with the matching total count. Invalid queries use the existing 400 validation problem format.
- User-created exercises currently remain inactive on creation, so search excludes them until activated. Search does not change creation or existing detail authorization behavior.

Sources: [search handler](../../Application/Features/Workout/Exercise/SearchExercises/SearchExercisesHandler.cs), [validator](../../Application/Features/Workout/Exercise/SearchExercises/SearchExercisesRequestValidator.cs), [repository](../../Infrastructure/Repository/ExerciseRepository.cs), [HTTP/PostgreSQL checks](../../Tests/ExerciseSearchIntegrationChecks.cs). See [[Decisions]] and [[Current State]] for the decision and delivery status.

## Weekly template schedules

- Creation accepts optional `scheduledDays`. Omission or null produces an unscheduled template.
- `PUT /api/workout/Template/{id}/schedule` replaces the entire schedule. `{"scheduledDays":[]}` clears it. Missing/null replacement collections are invalid.
- Valid weekdays use `DayOfWeek`; duplicates and undefined enum values are rejected. The domain returns the schedule in Monday-to-Sunday order.
- `GET /api/workout/Template` returns the current user's templates, including unscheduled templates. `?day=Monday` selects templates containing that weekday. Results are ordered by name, then ID.
- Schedule replacement returns 404 for a missing template and 403 for another user's template. Listing is scoped to the current user in the repository query.
- Template detail and list DTOs expose `scheduledDays`. JSON output uses weekday names. Do not infer that numeric enum input is universally rejected: serializer options and validation determine accepted input.
- These schedules are weekday membership on a template. They do not represent dated appointments, time zones, reminders, or automatic session creation.

Sources: [domain schedule](../../Domain/Entities/Workout/Template/WorkoutTemplate.cs), [create request](../../Application/Features/Workout/Template/CreateWorkoutTemplate/CreateWorkoutTemplateRequest.cs), [replacement validator](../../Application/Features/Workout/Template/UpdateWorkoutTemplateSchedule/UpdateWorkoutTemplateScheduleRequestValidator.cs), [replacement handler](../../Application/Features/Workout/Template/UpdateWorkoutTemplateSchedule/UpdateWorkoutTemplateScheduleHandler.cs), [repository filtering](../../Infrastructure/Repository/WorkoutTemplateRepository.cs).

## Errors

FluentValidation and model-binding failures return 400 with an `errors` dictionary that preserves field paths and multiple messages. Known exceptions map to 400, 401, 403, 404, or 409. Unexpected controller exceptions are logged and return a generic 500 detail. Problem responses include `traceId` and use `application/problem+json`.

Sources: [exception mapping](../../Onefold/Filters/ExceptionFilter.cs), [response construction](../../Onefold/Filters/ErrorResponses.cs), [regression checks](../../Tests/Program.cs). See [[Current State]] for code observations that still need investigation.
