# Included Asset Visuals Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace flat placeholder-looking runtime objects with included free low-poly/nature assets without changing gameplay behavior.

**Architecture:** Add a small `GameVisualKit` runtime decorator that loads existing `Resources/Asset` prefabs and attaches them as visual-only children. Keep `UnitController`, `MonsterController`, `Village`, `Portal`, colliders, rigidbodies, and HP bars on the existing root objects.

**Tech Stack:** Unity 2022.3 URP, C#, Unity `Resources.Load`, NUnit EditMode tests.

---

### Task 1: Visual Mapping

**Files:**
- Create: `LotteryFantasy/Assets/Scripts/Visuals/GameVisualKit.cs`
- Create: `LotteryFantasy/Assets/Tests/EditMode/GameVisualKitTests.cs`

- [ ] Add tests for unit, monster, elite monster, village, portal, and scenery asset path mappings.
- [ ] Implement pure mapping methods returning existing `Resources/Asset/...` prefab paths.

### Task 2: Runtime Decoration

**Files:**
- Modify: `LotteryFantasy/Assets/Scripts/Core/TestSceneBootstrapper.cs`
- Modify: `LotteryFantasy/Assets/Scripts/Visuals/GameVisualKit.cs`

- [ ] Decorate unit and monster templates with child visuals while keeping root sprites available as fallback silhouettes.
- [ ] Decorate villages and portal with included props/character/nature assets.
- [ ] Add lightweight arena scenery from `SimpleNaturePack` and `Polytope Studio` prefabs outside the main combat lane.

### Task 3: Docs, Tests, Build

**Files:**
- Modify: `docs/LotteryFantasy_기획서.md`
- Modify: `docs/LotteryFantasy_기획서.html`

- [ ] Document that included free/sample assets are used first.
- [ ] Run Unity EditMode tests and Windows build.
- [ ] Copy the built executable to `C:/Development/14_LT/LotteryFantasy_v0.1.0_portable.exe`.
