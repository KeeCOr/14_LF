# Next Improvement Instruction

## Scope
LotteryFantasy roulette result feedback in the authoritative Unity project `C:\Development\14_LT\LotteryFantasy`.

## Goal
Show a one-round flow line that changes per roulette outcome, e.g. FIRE result recommends next attack, IRON recommends defense, LIFE recommends recovery.

## Safe implementation path
1. Add an EditMode test in `Assets/Tests/EditMode/SpinOutcomeAdvisorTests.cs` for a `RoundFlow` or equivalent line on FIRE and IRON outcomes.
2. Extend `Assets/Scripts/Systems/SpinOutcomeAdvisor.cs` without touching scenes or meta files.
3. Append the flow line in `Assets/Scripts/UI/SlotMachineUI.cs` result text after `advice.NextDecision`.
4. Verify with Unity EditMode tests.

## Blocker this batch
Unity batchmode could not run because no valid Unity Editor license was available.
