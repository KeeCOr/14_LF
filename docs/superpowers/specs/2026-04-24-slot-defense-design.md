# 14_LT — 슬롯 디펜스 게임 설계 문서

**작성일:** 2026-04-24  
**상태:** 승인됨  
**엔진:** Unity 2D (URP)  
**타겟:** Steam (Windows/Mac) + WebGL  

---

## 1. 게임 컨셉

슬롯머신을 돌려 얻은 유닛·스킬·버프로 마을을 방어하는 경쟁형 디펜스 게임.  
몬스터를 처치하면 XP를 획득하고, XP로 슬롯 기회를 쌓아 전투 중 원할 때 사용한다.  
처치한 몬스터는 상대 진영으로 전송되어 상대를 압박하는 경쟁 메커닉이 된다.  
MVP는 AI 상대, 이후 PvP 확장.

**레퍼런스:** 클래시 로얄 (덱 시스템, 매칭 아트 컨셉)

---

## 2. 화면 레이아웃

**화면 비율:** 16:9 가로 고정

```
┌──────────────────────────────────────────────┐
│  🏰 내 마을    ←몬스터←  →몬스터→  상대 마을 🏰  │
│  (좌측)          중앙 전장          (우측)     │
│                                              │
│              [슬][슬][슬]                    │  ← 슬롯머신 (중앙 하단)
│  [카드1][카드2][카드3][카드4]   XP/스핀 표시  │  ← 핸드 + HUD
└──────────────────────────────────────────────┘
```

- **내 마을 (좌):** 체력 보유, 몬스터가 좌측으로 진입 시 피해
- **상대 마을 (우):** 체력 보유, 내가 전송한 몬스터가 우측으로 진입
- **중앙 전장:** 양쪽 몬스터가 이동하는 공간, 유닛 배치 가능
- **슬롯머신:** 중앙 하단, 스핀 버튼으로 수동 발동
- **핸드 슬롯:** 최대 4장, 초과 획득 시 버려짐

---

## 3. 핵심 게임 루프

```
전투 시작
    ↓
몬스터 웨이브 등장 (중앙 → 내 마을 방향)
    ↓
핸드에서 유닛/스킬 선택 → 원하는 위치에 배치
    ↓
몬스터 처치 → XP 획득 → 슬롯 스핀 기회 스택
    ↓
여유 있을 때 슬롯 스핀 → 덱(12장)에서 릴 결과 결정
    │  [A][A][A]  3개 일치 → 강화 유닛/버프 → 핸드에 추가
    │  [A][A][B]  2개 일치 → 일반 유닛/스킬 → 핸드에 추가
    │  [A][B][C]  모두 다름 → 전체 공용 버프 즉시 적용
    ↓
처치한 몬스터 → 상대 진영으로 자동 전송
    ↓
승패 판정: 상대 마을 HP 0 → 즉시 승리
           시간 종료 → 남은 마을 HP 비교 → 높은 쪽 승리
```

---

## 4. 슬롯머신 시스템

### 4.1 스핀 기회 적립
- 몬스터 처치 시 XP 획득
- XP가 임계값 도달 → 스핀 기회 +1 (스택 무제한)
- 플레이어가 원할 때 수동으로 스핀 버튼 클릭

### 4.2 릴 구성
- 3개 릴, 각 릴에 덱 12장 중 1장이 랜덤 선택
- 릴 결과에 따라 획득 등급 결정:

| 결과 | 효과 |
|------|------|
| 3개 일치 | 해당 카드의 강화(Enhanced) 버전 획득 → 핸드 추가 |
| 2개 일치 | 해당 카드 일반(Normal) 버전 획득 → 핸드 추가 |
| 모두 다름 | 전체 공용 버프 즉시 적용 (예: 공격력 +10%, 이속 +5%) |

### 4.3 핸드 관리
- 핸드 슬롯: 최대 4칸
- 핸드가 가득 찬 상태에서 카드 획득 시 자동으로 버려짐
- 공용 버프는 핸드와 무관하게 즉시 적용

---

## 5. 덱 시스템 (MVP: 고정 덱)

- MVP에서는 덱 빌딩 UI 없음, `FixedDeckConfig` ScriptableObject로 12장 고정
- 에디터에서 카드 구성 변경 가능 (밸런스 작업 용이)
- 카드 종류: 유닛(Unit), 스킬(Skill) — 덱에는 이 두 종류만 포함
- 버프는 덱 카드가 아님. "모두 다름" 슬롯 결과 시 시스템이 자동 생성하는 즉시 효과
- 이후 버전에서 사전 덱 빌딩 UI 추가 예정

---

## 6. 유닛 / 스킬 / 버프 정의

| 종류 | 획득 방법 | 사용 방법 |
|------|-----------|-----------|
| 유닛 | 슬롯 2개·3개 일치 | 핸드에서 선택 → 원하는 위치에 소환 |
| 스킬 | 슬롯 2개·3개 일치 | 핸드에서 선택 → 원하는 위치에 발동 |
| 버프 | 슬롯 모두 다름 | 즉시 전체 적용 (핸드 불필요) |

---

## 7. 경쟁 메커닉 — 몬스터 전송

- 플레이어가 몬스터를 처치하면 해당 몬스터 복사본이 상대 진영으로 전송
- 전송된 몬스터는 상대 마을을 향해 진행
- AI도 동일 규칙 적용 (AI가 처치한 몬스터 → 내 진영으로 전송)

---

## 8. 승패 판정

- **즉시 승리:** 상대 마을 HP ≤ 0
- **시간 종료:** 타이머 만료 시 마을 HP 비교, 높은 쪽 승리
- **무승부 처리:** HP 동일 시 무승부 (향후 연장전 고려)

---

## 9. AI 시스템 (MVP)

```csharp
interface IOpponent {
    void OnUpdate(float deltaTime);
    void ReceiveTransferredMonster(MonsterData monster);
}

class AIOpponent : IOpponent {
    // 고정 덱 동일 사용
    // 일정 간격으로 랜덤 카드 배치
    // 슬롯: XP 쌓이면 자동 스핀
    // 난이도: Easy / Normal / Hard (배치 빈도 + 반응속도)
}
```

- PvP 전환 시 `NetworkOpponent : IOpponent`로 교체
- AI는 내부 구현만 다르고 외부 인터페이스 동일

---

## 10. 시스템 구조

| 시스템 | 역할 |
|--------|------|
| `DeckSystem` | 고정 덱 12장 관리, 슬롯 릴 결과 → 카드 결정 |
| `SlotMachineSystem` | XP → 스핀 기회 적립, 스핀 실행, 결과 판정 |
| `HandSystem` | 핸드 4슬롯 관리, 카드 배치 입력 처리 |
| `ArenaSystem` | 양측 아레나 몬스터 웨이브, 유닛 이동/전투 |
| `TransferSystem` | 처치 몬스터 → 상대 진영 전송 |
| `BattleManager` | 승패 판정, 타이머, 마을 HP 관리 |
| `AIOpponent` | AI 덱 운용 + 유닛 배치 결정 |

---

## 11. 데이터 모델

```csharp
// 카드 데이터 (덱에 포함되는 카드: Unit / Skill만)
CardData : ScriptableObject
  - CardType cardType       // Unit / Skill
  - string cardName
  - Sprite icon
  - CardTier tier           // Normal / Enhanced (슬롯 결과로 결정)
  - UnitStats unitStats     // 유닛일 경우
  - SkillEffect skillEffect // 스킬일 경우

// 공용 버프 정의 (덱과 무관, SlotMachineSystem이 관리)
GlobalBuffConfig : ScriptableObject
  - BuffEffect[] possibleBuffs  // 모두 다름 결과 시 랜덤 적용

// 슬롯 판정
enum SlotResult { Triple, Double, AllDifferent }

// 런타임 게임 상태
class GameState
  - float myVillageHp
  - float enemyVillageHp
  - float timer
  - int spinCharges          // 스핀 기회 스택
  - CardData[4] hand         // 핸드 슬롯

// MVP 고정 덱
FixedDeckConfig : ScriptableObject
  - CardData[12] cards
```

---

## 12. 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── Core/         GameManager, GameEvents, BattleManager
│   ├── Systems/      SlotMachineSystem, HandSystem, ArenaSystem
│   │                 TransferSystem, DeckSystem
│   ├── Entities/     Unit, Projectile, Monster, Village
│   ├── AI/           IOpponent, AIOpponent
│   ├── UI/           SlotMachineUI, HandUI, ArenaHUD, ResultUI
│   └── Config/       FixedDeckConfig, CardData, MonsterConfig
├── Scenes/
│   ├── Battle.unity       메인 전투 씬
│   └── DeckSetup.unity    (MVP 이후)
├── ScriptableObjects/
│   ├── Decks/
│   └── Cards/
└── Art/
    └── UI/
```

---

## 13. MVP 범위 (Out of Scope)

- 덱 빌딩 UI (카드 선택 화면)
- PvP 네트워크 매칭
- 카드 잠금 해제 / 컬렉션 시스템
- 연장전
- 모바일 빌드 (Unity WebGL + Steam 우선)
