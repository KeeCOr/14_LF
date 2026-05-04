# LotteryFantasy — 개발 작업 로그

> Unity 2022 URP 2D / C# 9 / namespace `SlotDefense`

---

## 프로젝트 개요

슬롯머신으로 카드를 뽑고, 카드로 유닛을 소환해 몬스터 웨이브를 방어하는 덱빌딩 타워디펜스.

| 구분 | 내용 |
|---|---|
| 엔진 | Unity 2022 URP 2D |
| 언어 | C# 9 |
| 네임스페이스 | `SlotDefense` |
| 씬 구성 | `TestSceneBootstrapper` 자동 부트스트랩 (씬에 오브젝트 없이 실행) |

---

## 아키텍처

```
GameManager (싱글턴)
├── BattleManager        — 전투 타이머, 기지 HP 추적
├── SlotMachineSystem    — XP → 스핀 충전 변환
├── HandSystem           — 손패 (최대 4슬롯)
└── DeckSystem           — 카드 드로우, 릴 평가

ArenaSystem              — 웨이브 루프, 유닛/몬스터 스폰
TransferSystem           — 몬스터 처치 시 포탈에서 증원 스폰
AIOpponent               — AI 자동 스핀/유닛 배치

GameEvents (정적 이벤트 버스)
├── OnMonsterKilled
├── OnVillageDamaged
├── OnGlobalBuffApplied
├── OnCardObtained
├── OnSpinCompleted
└── OnBattleEnded
```

### 씬 레이아웃

```
x = -7.5  PlayerVillage (플레이어 기지)
x = -2.5  PlayerSpawn   (적 웨이브 스폰 위치)
x =  0.0  Portal        (적 포탈 — 공격 대상)
x = +2.5  EnemySpawn    (적 진영 웨이브 스폰)
x = +7.5  EnemyVillage  (AI 기지)
```

---

## 구현된 시스템

### 1. 슬롯머신 & 덱

- 몬스터 처치 → XP 획득 → XP 100당 스핀 1회 충전
- 스핀 시 덱에서 릴 3개 드로우 (복원 추출)
- 결과 판정:

| 결과 | 조건 | 보상 |
|---|---|---|
| Triple | 3릴 동일 | 강화 유닛카드 |
| Double | 2릴 동일 | 일반 유닛카드 |
| AllDifferent | 3릴 전부 다름 | 버프카드 (손패에 추가) |

- **덱 구성 (12장 / 4종)**

| 카드 | HP | 피해 | 사거리 | 공속 | 이동 | 시야 | 스핀비용 | 장수 |
|---|---|---|---|---|---|---|---|---|
| 검사 | 80 | 15 | 1.5 | 1.0/s | 2.0 | 5 | 1 | 3 |
| 궁수 | 50 | 10 | 5.0 | 2.0/s | 1.5 | 8 | 1 | 3 |
| 기사 | 120 | 20 | 1.0 | 0.8/s | 1.2 | 4 | 2 | 3 |
| 마법사 | 40 | 28 | 4.5 | 0.6/s | 1.8 | 8 | 2 | 3 |

> AllDifferent 확률: 3종류일 때 22% → 4종류 37%

### 2. 유닛 AI (`UnitController`)

```
매 프레임:
  1. AcquireTarget() — sightRange 이내 가장 가까운 MonsterController 탐색
  2. 적 발견 → 추격 + 사정거리 내 공격
  3. 적 없음 + 포탈 사정거리 내 → 포탈 공격
  4. 적 없음 + 포탈 밖 → 전방(적 진영) 전진
```

- **플레이어 유닛**: 오른쪽(x=0) 방향 전진, x≥0 경계에서 정지
- **AI 유닛**: 왼쪽(x=0) 방향 전진, x≤0 경계에서 정지
- 전역 버프 적용: `static GlobalDamageMultiplier`, `GlobalSpeedMultiplier`

### 3. 포탈 & 증원 메커닉

- 포탈(x=0)은 플레이어 유닛의 공격 목표
- **포탈 피격 시**: 플레이어 진영에 몬스터 즉시 스폰 (포탈 우측 x=1에서 출현)
- **HP < 40%**: 엘리트 몬스터 추가 스폰
- **포탈 색상**: HP에 따라 보라색 → 빨간색 점진 변화

### 4. 전투 이전 시스템 (`TransferSystem`)

- 몬스터 처치 시 1.5초 후 **같은 진영** 포탈 우측(x=1)에서 증원 스폰
- 효과: 더 많이 처치할수록 더 많은 적이 포탈에서 출현

### 5. 버프카드

- AllDifferent 스핀 결과 → "전투 버프" 카드가 손패에 추가
- 손패에서 클릭 시 **즉시 발동** (스핀 비용 없음)
- 효과: 공격력 ×1.5, 이동속도 ×1.2, 지속 10초 후 자동 해제
- 손패 UI에서 **금색** 배경으로 구분 표시

### 6. 물리 기반 유닛 분리

- `UnitController` / `MonsterController` 모두 `Rigidbody2D` + `CircleCollider2D`
- `gravityScale=0`, `drag=6`, `FreezeRotation`, `CollisionDetectionMode2D.Continuous`
- 충돌로 자연스러운 군집 형성 (완전 겹침 방지)
- 이동: `transform.position +=` → `_rb.velocity =` 방식으로 변경

### 7. HP 바 (`HpBar`)

- 모든 게임 오브젝트(유닛/몬스터/포탈/기지)에 월드스페이스 HP 바 표시
- 최대 HP일 때 숨김, 피해 시 표시
- 색상: 빨간색(배경) + HP 비율에 따라 초록→빨강 그라디언트

### 8. 유닛 배치 스핀 비용

- 카드 사용 시 스핀 충전량 소모 (유닛마다 다름)
- 잔여 충전량 부족 시 배치 취소
- 손패 UI에 `[N스핀]` 비용 표시

---

## 파일 목록 및 역할

### Config (ScriptableObject / 데이터 구조체)

| 파일 | 역할 |
|---|---|
| `CardData.cs` | 카드 데이터 (유닛스탯, 버프효과, 배치비용, 카드타입) |
| `CardType.cs` | `Unit`, `Skill`, `Buff` |
| `UnitStats.cs` | HP/피해/이동속도/사거리/공속/시야 |
| `BuffEffect.cs` | 공격배율/속도배율/지속시간 |
| `MonsterConfig.cs` | 몬스터 스탯 + XP보상 |
| `FixedDeckConfig.cs` | 덱 카드 배열 |
| `GlobalBuffConfig.cs` | 버프 풀 |

### Core

| 파일 | 역할 |
|---|---|
| `GameManager.cs` | 싱글턴, 시스템 조율, 스핀 처리, 버프 코루틴 |
| `GameEvents.cs` | 정적 이벤트 버스 |
| `TestSceneBootstrapper.cs` | 런타임 씬 자동 구성 (ScriptableObject/UI/오브젝트 전체 생성) |

### Entities

| 파일 | 역할 |
|---|---|
| `UnitController.cs` | 플레이어/AI 유닛 AI, 물리 이동, 버프 적용 |
| `MonsterController.cs` | 웨이브 몬스터 AI, 물리 이동 |
| `Portal.cs` | 포탈 HP, 피격 시 증원 스폰, 색상 변화 |
| `Village.cs` | 기지 HP 바 |

### Systems

| 파일 | 역할 |
|---|---|
| `ArenaSystem.cs` | 웨이브 루프, 유닛/몬스터 스폰, 유닛 배치 입력 처리, 배치 비용 차감 |
| `TransferSystem.cs` | 몬스터 처치 → 포탈 증원 큐 |
| `SlotMachineSystem.cs` | XP → 스핀 충전 변환, `TrySpin()`, `TryConsume(n)` |
| `DeckSystem.cs` | 릴 드로우, Triple/Double/AllDifferent 판정 |
| `HandSystem.cs` | 손패 슬롯 관리 |
| `BattleManager.cs` | 전투 타이머, 기지 HP 추적, 승패 판정 |

### AI

| 파일 | 역할 |
|---|---|
| `AIOpponent.cs` | 4초마다 자동 스핀, 6초마다 유닛 랜덤 배치 |

### UI

| 파일 | 역할 |
|---|---|
| `ArenaHUD.cs` | 양측 기지 HP 슬라이더, 타이머, 스핀 충전량 |
| `HandUI.cs` | 손패 카드 버튼 (유닛=파랑/선택=초록/버프=금색) |
| `SlotMachineUI.cs` | 릴 결과 표시, SPIN 버튼, 획득 메시지 |
| `ResultUI.cs` | 전투 종료 결과 패널 |
| `HpBar.cs` | 월드스페이스 HP 바 컴포넌트 |

---

## 주요 버그 수정 이력

| 증상 | 원인 | 해결 |
|---|---|---|
| 스핀이 항상 AllDifferent | 카드 인스턴스 12개 별도 생성 → `==` 비교 실패 | 카드 타입당 인스턴스 1개 공유 |
| 템플릿 MonsterController가 FindObjects에 잡힘 | `AddComponent` 전에 `SetActive(true)` | `SetActive(false)` → `AddComponent` 순서 변경 |
| NullReferenceException (MonsterController.Die) | 템플릿의 `config=null`로 HpBar Awake 실행 | HpBar 생성을 `Init()`으로 이동 + null 가드 추가 |
| CS0019: struct null 비교 | `UnitStats`는 struct → `== null` 불가 | `_hpBar == null`로 Init 여부 판단 |
| 포탈 증원이 보이지 않음 | 몬스터가 포탈 스프라이트 뒤(x=0)에 스폰 | 스폰 위치를 x=+1.0 오프셋으로 변경 |
| 버프가 전혀 안 나오는 것 같음 | AllDifferent가 즉시 발동되어 손패에 표시 안 됨 | 버프를 카드로 손패에 추가하는 방식으로 변경 |
| 폰트 깨짐 | TMP Essential Resources 미임포트 | `UnityEngine.UI.Text` + `Font.CreateDynamicFontFromOSFont`로 전환 |

---

## 게임플레이 흐름

```
게임 시작
  └─ 시작 스핀 3회 지급
  └─ 웨이브 5초마다 양 진영 동시 스폰

플레이어 턴
  ├─ SPIN 버튼 → 릴 결과 확인
  │    ├─ 유닛카드 → 손패 추가 (파랑)
  │    └─ 버프카드 → 손패 추가 (금색)
  ├─ 유닛카드 클릭 → 선택(초록) → 좌측 화면 클릭 → 스핀N개 소모 후 유닛 배치
  └─ 버프카드 클릭 → 즉시 발동 (10초 공격력+이동속도 상승)

유닛 AI
  ├─ 시야 내 몬스터 → 추격 & 공격
  ├─ 시야 내 몬스터 없음 + 포탈 사정거리 → 포탈 공격
  └─ 위 모두 아님 → 포탈 방향 전진

몬스터 처치 시
  ├─ XP 획득 → 스핀 충전
  └─ 1.5초 후 포탈 우측에서 증원 스폰

포탈 피격 시
  ├─ 즉시 증원 스폰 (포탈 우측 x=1)
  └─ HP < 40% → 엘리트 몬스터 추가 스폰

전투 종료 조건
  ├─ 내 기지 HP 0 → 패배
  ├─ 적 기지 HP 0 → 승리
  └─ 180초 경과 → 기지 HP 비교 판정
```

---

## 밸런스 수치

| 항목 | 값 |
|---|---|
| 기지 최대 HP | 1000 |
| 전투 제한 시간 | 180초 |
| XP per 스핀 | 100 |
| 시작 스핀 | 3회 (XP 300 지급) |
| 웨이브 간격 | 5초 |
| 일반 몬스터 HP | 40 |
| 일반 몬스터 피해 | 6 |
| 엘리트 몬스터 HP | 150 |
| 엘리트 몬스터 피해 | 20 |
| 포탈 HP | 500 |
| 엘리트 소환 임계값 | 포탈 HP 40% 미만 |
| AI 스핀 주기 | 4초 |
| AI 배치 주기 | 6초 |
| 증원 딜레이 | 1.5초 |
