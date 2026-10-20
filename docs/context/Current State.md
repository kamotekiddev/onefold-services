---
tags: [onefold, handoff]
updated: 2026-10-20
---

# Current state

## Snapshot

- **Inspected:** 2026-10-20, Asia/Manila.
- **Working branch:** `codex/search-exercises`, created from refreshed `origin/master` for the search feature.
- **Branch base / inspected commit:** `a73f10f353007914bf6df10f85596f550b0ef07a` — Merge pull request #40 from kamotekiddev/codex/obsidian-vault-settings. Cherry-pick search commit `d6d46a7` onto this base; preserve earlier local seed work.
- **Default branch:** local origin HEAD points to `master`.
- **Scope:** The user requested a new branch from the default branch and a cherry-pick of the search changes. This repository has `master`, with no `main`; origin was refreshed. Original search PR #41 is open from `codex/seed-exercises`; publishing the new branch or replacing that PR is not part of this request. Deployment status was not checked.

Relevant local history: `6f34ebc` introduced detailed API exceptions/validation errors; `074694d` added weekly schedules and their regression coverage; `3292bda` and `9200349` documented branch and PR conventions. Read [[Features and API]] and [[Decisions]] for the resulting behavior.

## Current work

The exercise seed catalog is being expanded on `codex/seed-exercises` from `master`. The development seeder targets a curated 24-exercise global starter catalog while preserving the five exercises used by the seeded Push Day template. Its pre-existing disposable PostgreSQL regression coverage was preserved and passed during search verification.

Implemented authenticated exercise name search with case-insensitive literal substring matching, active global/current-user visibility, and pagination with total count. See [[Features and API]] and decision D-005 in [[Decisions]]. Creation still leaves user-owned exercises inactive, so they are excluded from search until activated.

Latest master includes the shareable `.obsidian/app.json`, `.obsidian/appearance.json`, and `.obsidian/core-plugins.json` settings from PR #40. Their contents match the existing local copies and remain unchanged. `.obsidian/workspace.json` and `.DS_Store` files remain local. The earlier seed work stays outside the search commit. No deployment is requested.

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

Search-only delivery verification exported the exact staged code into a temporary directory, excluding the earlier seed catalog and seed-test changes. Restore and a fresh build passed using SDK 10.0.401 from outside the repository (17 existing nullable/dependency/vulnerability warnings, zero errors). The exported executable suite passed **194 regression checks**, including real PostgreSQL 18 and HTTP search/schedule checks against a fresh disposable local database. The earlier 199-check run included five local seed checks that are excluded from the search commit. Branch relocation changes context notes only; application and regression source remain identical to verified search commit `d6d46a7`. No additional behavioral run is required for that relocation.

## Resume checklist

1. Read [AGENTS.md](../../AGENTS.md) and [[Project Home]].
2. Inspect current branch, HEAD, and working tree; this snapshot may be stale.
3. Read the relevant topic notes and confirm their claims against current source.
4. After work, refresh affected notes and this snapshot, then append actual changes, verification, and remaining work to [[Session Log]].
