# Regression checks

This project is an executable regression suite. Run it with `dotnet run`, rather than `dotnet test`.

From the repository root:

```sh
dotnet run --project Tests/Tests.csproj
```

The existing `global.json` selects the invalid SDK version `10.0.0`. Until that is corrected separately, use the installed .NET 10 SDK by running from outside the repository with an absolute project path:

```sh
onefold_repo="$PWD"
(cd /private/tmp && dotnet run --project "$onefold_repo/Tests/Tests.csproj")
```

## PostgreSQL and HTTP schedule and exercise search checks

Without `ONEFOLD_TEST_CONNECTION_STRING`, the suite runs its in-process checks and explicitly reports that integration checks were skipped.

Integration checks require a fresh, disposable local PostgreSQL database named `onefold_schedule_test`. They apply migrations and create test data. They also start a temporary HTTP server on a random loopback port to exercise real controllers, JWT authentication, validation, handlers, and repositories.

For example, start PostgreSQL 18 with temporary storage and no published non-loopback ports:

```sh
docker run --detach --rm --name onefold-schedule-tests \
  --publish 127.0.0.1::5432 --tmpfs /var/lib/postgresql \
  --env POSTGRES_HOST_AUTH_METHOD=trust \
  --env POSTGRES_USER=onefold_schedule_test \
  --env POSTGRES_DB=onefold_schedule_test postgres:18
docker port onefold-schedule-tests 5432/tcp
docker exec onefold-schedule-tests pg_isready -U onefold_schedule_test -d onefold_schedule_test
```

Once PostgreSQL is ready, replace `<port>` with the port printed above and run from the repository root:

```sh
onefold_repo="$PWD"
(cd /private/tmp && \
  ONEFOLD_TEST_CONNECTION_STRING='Host=127.0.0.1;Port=<port>;Database=onefold_schedule_test;Username=onefold_schedule_test' \
  dotnet run --project "$onefold_repo/Tests/Tests.csproj")
```

The migration check inserts a template into the original schema, applies the new migrations, and verifies that the existing template is unscheduled. A separate prerequisite migration supplies the already-modeled `WorkoutExercises.WeightIncrement` column that was missing from the original migration history.

The same disposable database and HTTP host also run exercise search checks: authenticated visibility, active status, case-insensitive substring matching, literal wildcard characters, pagination and counts, validation problem responses, and the existing exercise detail route. In-process checks cover search validation, offset overflow, and handler mapping even when PostgreSQL checks are skipped.

Remove the disposable database after the run, including after a failure:

```sh
docker stop onefold-schedule-tests
```

Start a fresh container for another integration run; the suite refuses databases with previously applied migrations. Do not use the application's development or production database.
