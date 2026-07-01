# 다음 개선 지시서 - LotteryFantasy

최신화: 2026-07-01 KST

## 현재 반영 완료

- 스핀 결과를 전투 선택과 바로 연결하는 `SpinOutcomeAdvisor`를 추가했다.
- Fire 결과는 **공격 선택**, Iron 결과는 **방어 선택**, Life 결과는 **회복 선택**, Mixed 결과는 **위험 관리**로 안내한다.
- `SpinOutcomeAdvisorTests`로 Jackpot, Iron pair, Life pair, Mixed roll 4개 케이스를 검증했다.
- Unity EditMode 검증: `TestResults-SpinOutcomeAdvisor-commit-fixed.xml` 기준 4 passed / 0 failed.

## 다음 우선순위

1. 스핀 결과 안내를 실제 슬롯 결과 UI에 더 강하게 연결한다.
2. 공격/방어/회복/위험 관리 문구 옆에 아이콘 또는 색상 상태를 붙여 3초 안에 읽히게 만든다.
3. 선택 실행 후 실제 전투 로그에 "왜 이 선택이 유리했는지"를 한 줄로 남긴다.
4. Jackpot 결과에는 짧은 시각/사운드 피드백을 추가한다.

## 완료 기준

- 스핀 직후 플레이어가 다음 선택을 바꿔야 하는 이유를 3초 안에 이해할 수 있다.
- 결과 안내, 버튼 선택, 전투 로그가 같은 용어를 사용한다.
- EditMode 테스트가 계속 통과하고, 빌드 산출물 버전과 기획서가 같은 버전을 가리킨다.