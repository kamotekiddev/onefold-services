---
tags: [onefold, handoff]
updated: 2026-10-19
---

# Current state

## Snapshot

- **Inspected:** 2026-10-19, Asia/Manila.
- **Working branch:** `codex/project-context-vault`, created from refreshed `origin/master` for documentation delivery.
- **Branch base:** `84cf53f` — Merge pull request #38 from codex/workout-template-schedules. Its file tree matches the initially inspected revision `9200349`.
- **Default branch:** local origin HEAD points to `master`.
- **Scope:** origin was refreshed during PR preparation. GitHub reports the earlier branch's PR #38 merged. Deployment status was not checked; this documentation branch is separate from that merged work.

Relevant local history: `6f34ebc` introduced detailed API exceptions/validation errors; `074694d` added weekly schedules and their regression coverage; `3292bda` and `9200349` documented branch and PR conventions. Read [[Features and API]] and [[Decisions]] for the resulting behavior.

## Current work

The user configured this repository as an Obsidian vault and asked for persistent project context. The initial context notes now live under `docs/context/`; [AGENTS.md](../../AGENTS.md) points future sessions here and requests ongoing maintenance. Two stale statements in that file were corrected: regression checks now exist, and the exception filter handles FluentValidation as well as DataAnnotations exceptions.

Existing untracked `.obsidian/` settings and `.DS_Store` files were present before this task and must remain separate from documentation changes. No application source or configuration changes are part of this task. The user subsequently requested a separate branch, commit, and PR for these notes.

## Known issues and follow-up candidates

These are observations, not an approved implementation backlog.

| Observation | Evidence | Next useful check |
| --- | --- | --- |
| SDK selector still specifies `10.0.0` | [global.json](../../global.json) | Select a valid installed .NET 10 SDK in a separately scoped change; see [[Development and Validation]] for the existing workaround. |
| Exercise-update route uses `{id:guid}`, while its action parameter is named `templateId` without an explicit binding name | [TemplateController](../../Onefold/Controllers/Workout/TemplateController.cs) | Reproduce route binding through HTTP and add coverage before changing the contract or binding. |
| `WorkoutSession.RemoveExercise` checks membership, then calls `_exercises.Add` | [WorkoutSession](../../Domain/Entities/Workout/Session/WorkoutSession.cs) | Establish a focused domain regression case; verify expected removal behavior and callers. |
| Session saving indexes template exercises by a requested ID directly | [SaveSessionHandler](../../Application/Features/Workout/Session/SaveSession/SaveSessionHandler.cs) | Verify the response for an exercise absent from the template and decide its intended validation/error contract. |

## Verification boundary

This task is documentation-only. Source and command references were inspected; note links and diff whitespace are checked as part of delivery. No build, regression suite, database migration, API startup, or deployment was run. Existing checks are documented without claiming that they currently pass.

## Resume checklist

1. Read [AGENTS.md](../../AGENTS.md) and [[Project Home]].
2. Inspect current branch, HEAD, and working tree; this snapshot may be stale.
3. Read the relevant topic notes and confirm their claims against current source.
4. After work, refresh affected notes and this snapshot, then append actual changes, verification, and remaining work to [[Session Log]].
