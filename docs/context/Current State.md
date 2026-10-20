---
tags: [onefold, handoff]
updated: 2026-10-20
---

# Current state

## Snapshot

- **Inspected:** 2026-10-20, Asia/Manila.
- **Working branch:** `codex/seed-exercises`; existing seed work preserved while implementing exercise search.
- **Inspected HEAD:** `a6beb08c49fca1e6cf00482c2de399794a52ec19`. Search delivery is being prepared from this revision; earlier seed changes remain local and uncommitted.
- **Default branch:** local origin HEAD points to `master`.
- **Scope:** No remote refresh, publication, or deployment occurred during search implementation. The prior documentation session reported schedule PR #38 merged; current deployment status was not checked.

Relevant local history: `6f34ebc` introduced detailed API exceptions/validation errors; `074694d` added weekly schedules and their regression coverage; `3292bda` and `9200349` documented branch and PR conventions. Read [[Features and API]] and [[Decisions]] for the resulting behavior.

## Current work

The exercise seed catalog is being expanded on `codex/seed-exercises` from `master`. The development seeder targets a curated 24-exercise global starter catalog while preserving the five exercises used by the seeded Push Day template. Its pre-existing disposable PostgreSQL regression coverage was preserved and passed during search verification.

Implemented authenticated exercise name search with case-insensitive literal substring matching, active global/current-user visibility, and pagination with total count. See [[Features and API]] and decision D-005 in [[Decisions]]. Creation still leaves user-owned exercises inactive, so they are excluded from search until activated.

Existing untracked `.obsidian/` settings and `.DS_Store` files were present before this task and must remain separate from implementation changes. The user requested commit, push, and PR delivery for search; the earlier seed work remains outside the staged change. No deployment is requested.

## Known issues and follow-up candidates

These are observations, not an approved implementation backlog.

| Observation | Evidence | Next useful check |
| --- | --- | --- |
| SDK selector still specifies `10.0.0` | [global.json](../../global.json) | Select a valid installed .NET 10 SDK in a separately scoped change; see [[Development and Validation]] for the existing workaround. |
| Exercise-update route uses `{id:guid}`, while its action parameter is named `templateId` without an explicit binding name | [TemplateController](../../Onefold/Controllers/Workout/TemplateController.cs) | Reproduce route binding through HTTP and add coverage before changing the contract or binding. |
| `WorkoutSession.RemoveExercise` checks membership, then calls `_exercises.Add` | [WorkoutSession](../../Domain/Entities/Workout/Session/WorkoutSession.cs) | Establish a focused domain regression case; verify expected removal behavior and callers. |
| Session saving indexes template exercises by a requested ID directly | [SaveSessionHandler](../../Application/Features/Workout/Session/SaveSession/SaveSessionHandler.cs) | Verify the response for an exercise absent from the template and decide its intended validation/error contract. |

## Verification boundary

`dotnet --info` confirms the known invalid `global.json` selector (`10.0.0`) with SDK `10.0.401` installed. Restore and build succeeded using absolute solution paths from `/private/tmp`; build reported five existing NuGet vulnerability/redundant dependency warnings and zero errors. The executable suite passed **199 regression checks**, including PostgreSQL 18 migrations, existing seed/schedule checks, and exercise search HTTP/database checks against a fresh disposable loopback-only database. No development/production database migration or deployment was run. `git diff --check` passes.

Search-only delivery verification exported the exact staged code into a temporary directory, excluding the earlier seed catalog and seed-test changes. Restore and a fresh build passed using SDK 10.0.401 from outside the repository (17 existing nullable/dependency/vulnerability warnings, zero errors). The exported executable suite passed **194 regression checks**, including real PostgreSQL 18 and HTTP search/schedule checks against a fresh disposable local database. The earlier 199-check run included five local seed checks that are excluded from this PR.

## Resume checklist

1. Read [AGENTS.md](../../AGENTS.md) and [[Project Home]].
2. Inspect current branch, HEAD, and working tree; this snapshot may be stale.
3. Read the relevant topic notes and confirm their claims against current source.
4. After work, refresh affected notes and this snapshot, then append actual changes, verification, and remaining work to [[Session Log]].
