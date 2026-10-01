# LotteryFantasy 업데이트 내역서
> 최신화: 2026-07-01 KST
> 현재 버전: v0.3.2

## v0.3.2 - 슬롯 결과별 다음 전투 선택 추천 문장 명확화

- SpinOutcomeAdvisor의 다음 선택 문장을 Fire는 공격 선택, Iron은 방어 선택, Life는 회복 선택, Mixed는 위험 관리로 구분했다.
- 결과창은 기존 SlotMachineUI 연결을 유지하면서 릴 결과마다 다음 공격/방어/회복 판단이 더 즉시 읽히도록 문장 첫머리에 선택 유형을 표시한다.
- SpinOutcomeAdvisorTests를 4개 케이스로 확장해 Fire Triple, Iron Pair, Life Pair, Mixed 결과의 추천 문장이 서로 다르게 유지되는지 검증했다.
- Unity bundleVersion을 0.3.2로 갱신했다.

### 검증
- RED: Unity EditMode SpinOutcomeAdvisorTests 3개가 기존 다음 선택 문장 때문에 실패함을 확인했다.
- GREEN: Unity EditMode SpinOutcomeAdvisorTests 4개 통과, 실패 0개.

## v0.3.1 - 결과창 피드백 연결 확정

- SlotMachineUI가 SpinOutcomeAdvisor.Describe() 결과를 받아 결과 헤드라인, 에너지 합계, 변화 설명, 다음 선택 안내를 표시하도록 연결했다.
- Jackpot 연출 조건을 SpinOutcomeTone.Jackpot 기준으로 통일했다.
- SlotMachineOutcomeFeedbackTests로 결과창 코드가 어드바이저 문구를 사용하는지 검증했다.

## v0.3.0 - 슬롯 결과 전략 피드백 개선

- SpinOutcomeAdvisor를 추가해 Triple, Pair, Mixed 결과를 전략 문장으로 변환했다.
- 결과창에 결과 헤드라인, 변화 설명, 다음 선택을 함께 표시했다.
