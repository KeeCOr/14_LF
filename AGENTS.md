# LotteryFantasy Agent Rules

## Authoritative Project Root

The authoritative Unity project root is:

    C:\Development\14_LT\LotteryFantasy

Do not treat the root-level C:\Development\14_LT\Assets, Packages, or ProjectSettings folders as the active development source unless the user explicitly asks for a root-level Unity cleanup. They currently appear to duplicate the nested Unity project and contain Git-tracked files, so deleting or moving them requires a separate verification pass.

## Root Folder Role

C:\Development\14_LT is the distribution and shared-documentation root.

- Root portable executable: C:\Development\14_LT\LotteryFantasy_v{version}_portable.exe
- Release folder: C:\Development\14_LT\release\
- Main planning document: C:\Development\14_LT\docs\LotteryFantasy planning document (Korean filename: LotteryFantasy_kihoekseo.md equivalent)
- Update log: C:\Development\14_LT\docs\LotteryFantasy update log (Korean filename: LotteryFantasy_update_history.md equivalent)

## Build Path

Open or build this Unity project path:

    C:\Development\14_LT\LotteryFantasy

Example:

    "C:\Program Files\Unity\Hub\Editor\{version}\Editor\Unity.exe" `
      -batchmode -quit `
      -projectPath "C:\Development\14_LT\LotteryFantasy" `
      -buildWindows64Player "C:\Development\14_LT\release\LotteryFantasy.exe"

If Unity is open or file locks block the build, report the lock instead of claiming the executable is fresh.

## Cleanup Rules

1. Do not move or delete LotteryFantasy/ while its Git status is dirty.
2. Before touching root duplicate Unity folders, check Git status and Unity references.
3. Runtime resource changes belong under LotteryFantasy/Assets/.
4. Root docs/ is for shared planning documents and update logs.
5. After gameplay, UX, resource, or build behavior changes, update both planning and update documents.
