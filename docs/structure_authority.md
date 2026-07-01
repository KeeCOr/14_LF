# LotteryFantasy Structure Authority Note

Date: 2026-06-24

## What Changed In This Batch

- Fixed the authoritative Unity development root as C:\Development\14_LT\LotteryFantasy.
- Clarified that C:\Development\14_LT is the distribution and shared-documentation root.
- Rewrote 14_LT/AGENTS.md so future work does not accidentally edit the wrong Unity root.
- Did not delete root-level duplicate Unity folders because both the root and nested project contain Git-tracked Unity files.

## Current Folder Roles

| Path | Role | Current action |
|---|---|---|
| 14_LT/LotteryFantasy | Authoritative Unity development project | Keep and use for source edits |
| 14_LT/docs | Shared planning docs and update logs | Keep |
| 14_LT/release | Distribution output folder | Keep |
| 14_LT/Assets, 14_LT/Packages, 14_LT/ProjectSettings | Duplicate/root Unity source candidate | Do not delete until verified |
| 14_LT/LotteryFantasy_v*_portable.exe | Root portable executable | Keep latest only |

## Conditions Before Removing Root Duplicate Unity Folders

1. git -C C:\Development\14_LT\LotteryFantasy status --short must be clean.
2. The nested project pending code, tests, and ProjectSettings changes must be reviewed or merged.
3. Unity must open C:\Development\14_LT\LotteryFantasy successfully.
4. Scenes, prefabs, tests, and the Windows build must work from the nested project path.
5. Only after those checks should root-level duplicate Unity folders be archived or deleted.

## Why Deletion Was Deferred

14_LT/LotteryFantasy currently has pending changes in scripts, tests, ProjectSettings, and generated test-runner files. Removing or moving the nested project while it is dirty would risk losing active work or confusing Unity metadata.
