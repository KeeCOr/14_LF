# UI Visual Polish Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Upgrade the in-game UI presentation while preserving the current runtime-generated Unity UI flow.

**Architecture:** Add a small `UIStyle` utility for shared colors and button state blocks, then apply it from `TestSceneBootstrapper`, `HandUI`, and `SlotMachineUI`. Keep the existing Canvas and controller wiring intact so gameplay behavior does not change.

**Tech Stack:** Unity 2022.3 URP, C#, Unity UI, NUnit EditMode tests.

---

### Task 1: Shared UI Style

**Files:**
- Create: `LotteryFantasy/Assets/Scripts/UI/UIStyle.cs`
- Create: `LotteryFantasy/Assets/Tests/EditMode/UIStyleTests.cs`

- [ ] Add tests proving button colors include distinct normal/highlight/pressed/disabled states.
- [ ] Add tests proving card background colors distinguish selected, affordable, unaffordable, skill, and buff states.
- [ ] Implement `UIStyle` with shared palette, card colors, and button color blocks.

### Task 2: Runtime UI Skin Application

**Files:**
- Modify: `LotteryFantasy/Assets/Scripts/Core/TestSceneBootstrapper.cs`
- Modify: `LotteryFantasy/Assets/Scripts/UI/HandUI.cs`
- Modify: `LotteryFantasy/Assets/Scripts/UI/SlotMachineUI.cs`

- [ ] Replace flat panel colors with layered dark panels, accent bars, and stronger section labels.
- [ ] Upgrade HP sliders, deck button, result panel, energy panel, card slots, and slot reels using shared colors.
- [ ] Keep all existing gameplay references and event wiring unchanged.

### Task 3: Documentation, Verification, Build

**Files:**
- Create or update: `docs/LotteryFantasy_기획서.md`
- Update: `docs/LotteryFantasy_기획서.html`

- [ ] Document the UI polish direction in both planning documents.
- [ ] Run Unity EditMode tests.
- [ ] Build `C:/Development/14_LT/release/LotteryFantasy.exe` with Unity CLI when available.
- [ ] Copy or produce `C:/Development/14_LT/LotteryFantasy_v{version}_portable.exe`.
