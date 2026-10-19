---
tags: [onefold, handoff]
updated: 2026-10-19
---

# Current state

## Snapshot

- **Inspected:** 2026-10-19, Asia/Manila.
- **Working branch:** `codex/obsidian-vault-settings`, created from refreshed `origin/master`.
- **Branch base / inspected commit:** `a6beb08` — Merge pull request #39 from kamotekiddev/codex/project-context-vault.
- **Default branch:** `master`.
- **Scope:** Commit the shareable Obsidian settings and prepare a PR. The project-context documentation is present in the refreshed default branch. Deployment status was not checked.

Relevant local history: `6f34ebc` introduced detailed API exceptions/validation errors; `074694d` added weekly schedules and their regression coverage; `3292bda` and `9200349` documented branch and PR conventions. Read [[Features and API]] and [[Decisions]] for the resulting behavior.

## Current work

The user requested a commit, push, and PR for untracked files. The shareable vault settings are `.obsidian/app.json`, `.obsidian/appearance.json`, and `.obsidian/core-plugins.json`. The first two contain empty objects; the third records the existing core-plugin choices. Their contents are preserved unchanged.

Untracked `.obsidian/workspace.json` and `.DS_Store` files remain local in accordance with repository guidance. No application changes are included. This snapshot records preparation; verify publication through Git/GitHub.

## Known issues and follow-up candidates

These are observations, not an approved implementation backlog.

| Observation | Evidence | Next useful check |
| --- | --- | --- |
| SDK selector still specifies `10.0.0` | [global.json](../../global.json) | Select a valid installed .NET 10 SDK in a separately scoped change; see [[Development and Validation]] for the existing workaround. |
| Exercise-update route uses `{id:guid}`, while its action parameter is named `templateId` without an explicit binding name | [TemplateController](../../Onefold/Controllers/Workout/TemplateController.cs) | Reproduce route binding through HTTP and add coverage before changing the contract or binding. |
| `WorkoutSession.RemoveExercise` checks membership, then calls `_exercises.Add` | [WorkoutSession](../../Domain/Entities/Workout/Session/WorkoutSession.cs) | Establish a focused domain regression case; verify expected removal behavior and callers. |
| Session saving indexes template exercises by a requested ID directly | [SaveSessionHandler](../../Application/Features/Workout/Session/SaveSession/SaveSessionHandler.cs) | Verify the response for an exercise absent from the template and decide its intended validation/error contract. |

## Verification boundary

The three settings files passed JSON parsing. This task changes vault settings and handoff notes only; no application build, regression suite, database migration, API startup, or deployment was run. Staged whitespace and scope checks are performed before commit.

## Resume checklist

1. Read [AGENTS.md](../../AGENTS.md) and [[Project Home]].
2. Inspect current branch, HEAD, and working tree; this snapshot may be stale.
3. Read the relevant topic notes and confirm their claims against current source.
4. After work, refresh affected notes and this snapshot, then append actual changes, verification, and remaining work to [[Session Log]].
