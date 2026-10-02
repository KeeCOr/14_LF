# Next Improvement Instruction

## Scope
LotteryFantasy roulette result feedback in the authoritative Unity project `C:\Development\50_LT\LotteryFantasy`.

## Goal
Show a one-round flow line that changes per roulette outcome, e.g. FIRE result recommends next attack, IRON recommends defense, LIFE recommends recovery.

## Safe implementation path
1. Add an EditMode test in `Assets/Tests/EditMode/SpinOutcomeAdvisorTests.cs` for a `RoundFlow` or equivalent line on FIRE and IRON outcomes.
2. Extend `Assets/Scripts/Systems/SpinOutcomeAdvisor.cs` without touching scenes or meta files.
3. Append the flow line in `Assets/Scripts/UI/SlotMachineUI.cs` result text after `advice.NextDecision`.
4. Verify with Unity EditMode tests.

## Blocker this batch
Unity batchmode could not run because no valid Unity Editor license was available.

## 2026-09-18 프로젝트별 고유 개선 3개
> Unity 제외 조건: 이번 반영은 설계·씬·검증 명세이며 런타임 구현 완료를 뜻하지 않는다.

1. 룰렛 결과가 다음 공격·방어 선택을 실제로 변경
2. 운 보정과 전략 개입 수단을 확률 정보와 함께 공개
3. 한 라운드 안에 추첨·판단·전투·보상 루프 완결

## 2026-09-21 진행 상태

- [부분 완료] 결과별 `RoundFlow`를 구현해 추첨·공격/방어/회복/보완 판단·전투·보상 순서를 결과창에 표시한다.
- [검증 완료] Unity 비의존 .NET 실행에서 FIRE·IRON·LIFE·Mixed 4/4 흐름 통과.
- [미검증] Unity EditMode 테스트, 씬의 텍스트 오버플로, Game View 좁은 화면, 빌드.
- [선결 조건 유지] 확률형 추첨을 고정 패턴/사전 공개 선택으로 바꾸는 사행성 리스크 재설계 방향을 먼저 확정해야 한다.
- 다음 후보: 선택된 재설계 방향에 따라 `SpinOutcomeAdvisor` 입력을 확률 결과가 아닌 플레이어 선택/사전 공개 시퀀스로 교체한다.
