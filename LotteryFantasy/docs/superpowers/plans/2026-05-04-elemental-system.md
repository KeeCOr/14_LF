# Elemental System Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 슬롯머신이 속성 에너지를 생성하고, 덱이 순환하며 손패를 공급하며, 속성 에너지를 소모해 유닛/마법/건물 카드를 배치하는 구조로 전환한다.

**Architecture:** ElementType enum + ElementalCost 구조체로 카드 비용을 정의한다. ReelSystem이 덱 구성을 기반으로 릴 심볼 풀을 구성하고 스핀 결과를 에너지로 변환한다. DeckSystem에 순환 딜링을 추가하고, 손패가 비면 GameManager가 자동으로 다음 4장을 지급한다. 건물/공중 유닛은 새 컴포넌트로 추가한다.

**Tech Stack:** Unity 2022 URP 2D, C#, NUnit (Edit Mode Tests)

---

## 파일 맵

| 상태 | 경로 | 역할 |
|------|------|------|
| 신규 | `Assets/Scripts/Config/ElementType.cs` | 속성 열거형 |
| 신규 | `Assets/Scripts/Config/BuildingData.cs` | 건물 카드 데이터 |
| 수정 | `Assets/Scripts/Config/CardData.cs` | elementalCost 추가, placementCost 제거 |
| 수정 | `Assets/Scripts/Config/CardType.cs` | Building 타입 추가 |
| 수정 | `Assets/Scripts/Config/UnitStats.cs` | canAttackAir, isFlying 추가 |
| 신규 | `Assets/Scripts/Systems/ElementalEnergySystem.cs` | 속성 에너지 카운터 |
| 신규 | `Assets/Scripts/Systems/ReelSystem.cs` | 릴 풀 계산 + 스핀 에너지 변환 |
| 수정 | `Assets/Scripts/Systems/DeckSystem.cs` | 순환 딜링 추가 |
| 수정 | `Assets/Scripts/Systems/HandSystem.cs` | IsEmpty 프로퍼티 추가 |
| 수정 | `Assets/Scripts/Core/GameManager.cs` | ElementalEnergy/ReelSystem 연동, 자동 딜링 |
| 수정 | `Assets/Scripts/Systems/ArenaSystem.cs` | 건물 배치, 엘레멘탈 비용 체크 |
| 수정 | `Assets/Scripts/UI/HandUI.cs` | 속성 비용 표시 |
| 수정 | `Assets/Scripts/UI/SlotMachineUI.cs` | 릴 심볼 표시, 에너지 결과 표시 |
| 신규 | `Assets/Scripts/UI/EnergyHUD.cs` | 화/철/생명 에너지 실시간 표시 |
| 수정 | `Assets/Scripts/Entities/UnitController.cs` | canAttackAir 적용 |
| 수정 | `Assets/Scripts/Entities/MonsterController.cs` | isFlying 플래그 |
| 신규 | `Assets/Scripts/Entities/BuildingController.cs` | 건물 공통 베이스 |
| 신규 | `Assets/Scripts/Entities/BattleBuilding.cs` | 자동 공격 건물 |
| 신규 | `Assets/Scripts/Entities/ProductionBuilding.cs` | 에너지/유닛 생산 건물 |
| 수정 | `Assets/Scripts/Core/TestSceneBootstrapper.cs` | 전체 카드 데이터 재작성 |
| 신규 | `Assets/Tests/EditMode/ElementalEnergySystemTests.cs` | 에너지 시스템 테스트 |
| 신규 | `Assets/Tests/EditMode/ReelSystemTests.cs` | 릴 시스템 테스트 |
| 수정 | `Assets/Tests/EditMode/DeckSystemTests.cs` | 순환 딜링 테스트 추가 |

---

## Task 1: ElementType + 데이터 타입 정의

**Files:**
- Create: `Assets/Scripts/Config/ElementType.cs`
- Create: `Assets/Scripts/Config/BuildingData.cs`
- Modify: `Assets/Scripts/Config/CardType.cs`
- Modify: `Assets/Scripts/Config/CardData.cs`
- Modify: `Assets/Scripts/Config/UnitStats.cs`

- [ ] **Step 1: ElementType 열거형 생성**

`Assets/Scripts/Config/ElementType.cs`:
```csharp
namespace SlotDefense
{
    public enum ElementType { Fire, Iron, Life }
}
```

- [ ] **Step 2: CardType에 Building 추가**

`Assets/Scripts/Config/CardType.cs`:
```csharp
namespace SlotDefense
{
    public enum CardType { Unit, Skill, Buff, Building }
}
```

- [ ] **Step 3: UnitStats에 공중 플래그 추가**

`Assets/Scripts/Config/UnitStats.cs` — 기존 필드 아래에 추가:
```csharp
public bool canAttackAir;
public bool isFlying;
```

- [ ] **Step 4: BuildingData 생성**

`Assets/Scripts/Config/BuildingData.cs`:
```csharp
using UnityEngine;
namespace SlotDefense
{
    public enum BuildingType { BattleTower, ProductionEnergy, ProductionUnit }

    [System.Serializable]
    public class BuildingData
    {
        public BuildingType buildingType;

        // 전투 건물
        public float attackDamage;
        public float attackRate;
        public float attackRange;
        public bool  canAttackAir;

        // 에너지 생산 건물
        public ElementType energyType;
        public float       energyPerSecond;

        // 유닛 생산 건물
        public CardData unitToSpawn;
        public float    spawnInterval;
    }
}
```

- [ ] **Step 5: CardData 수정 — placementCost 제거, elementalCost + buildingData 추가**

`Assets/Scripts/Config/CardData.cs` 전체:
```csharp
using UnityEngine;
namespace SlotDefense
{
    [CreateAssetMenu(menuName = "SlotDefense/CardData", fileName = "NewCard")]
    public class CardData : ScriptableObject
    {
        public CardType    cardType;
        public string      cardName;
        public Sprite      icon;
        public UnitStats   unitStats;
        public SkillEffect skillEffect;
        public BuffEffect  buffEffect;
        public BuildingData buildingData;

        [Tooltip("화/철/생명 에너지 비용")]
        public int fireCost;
        public int ironCost;
        public int lifeCost;

        [Tooltip("유닛 전용 프리팹. 비워두면 기본 템플릿 사용.")]
        public GameObject unitPrefab;

        public ElementalCost ElementalCost => new ElementalCost(fireCost, ironCost, lifeCost);
    }
}
```

- [ ] **Step 6: ElementalCost 구조체 생성**

`Assets/Scripts/Config/ElementType.cs` 아래에 추가 (같은 파일):
```csharp
namespace SlotDefense
{
    public enum ElementType { Fire, Iron, Life }

    [System.Serializable]
    public struct ElementalCost
    {
        public int fire;
        public int iron;
        public int life;

        public ElementalCost(int fire, int iron, int life)
        { this.fire = fire; this.iron = iron; this.life = life; }

        public int Total => fire + iron + life;
        public bool IsZero => fire == 0 && iron == 0 && life == 0;
    }
}
```

- [ ] **Step 7: 컴파일 오류 수정**

`placementCost` 참조 파일들에서 임시로 `card.placementCost` → `card.ElementalCost.Total`로 교체 (Task 6에서 올바르게 수정):
- `Assets/Scripts/UI/HandUI.cs` line 67: `$"[{card.placementCost}행운]"` → `$"[{card.ElementalCost.Total}에너지]"`
- `Assets/Scripts/UI/DeckViewerUI.cs` line 53: `비용:{card.placementCost}행운` → `비용:{card.ElementalCost.Total}`
- `Assets/Scripts/UI/CardDragHandler.cs` line 39: `[{card.placementCost}행운]` → `[{card.ElementalCost.Total}에너지]`
- `Assets/Scripts/Core/GameManager.cs` lines 85, 96: `buffCard.placementCost = 0;` 삭제, `enhanced.placementCost = ...` 삭제
- `Assets/Scripts/Core/TestSceneBootstrapper.cs`: `card.placementCost = N;` 라인들 삭제

- [ ] **Step 8: 컴파일 확인 후 커밋**

```bash
git add Assets/Scripts/Config/
git commit -m "feat: add ElementType, ElementalCost, BuildingData; replace placementCost"
```

---

## Task 2: ElementalEnergySystem

**Files:**
- Create: `Assets/Scripts/Systems/ElementalEnergySystem.cs`
- Create: `Assets/Tests/EditMode/ElementalEnergySystemTests.cs`

- [ ] **Step 1: 테스트 파일 작성**

`Assets/Tests/EditMode/ElementalEnergySystemTests.cs`:
```csharp
using NUnit.Framework;
using SlotDefense;

public class ElementalEnergySystemTests
{
    [Test]
    public void Add_IncreasesEachType()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(2, 1, 3);
        Assert.AreEqual(2, sys.Fire);
        Assert.AreEqual(1, sys.Iron);
        Assert.AreEqual(3, sys.Life);
    }

    [Test]
    public void Add_CapsAtMax()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(99, 99, 99);
        Assert.AreEqual(10, sys.Fire);
        Assert.AreEqual(10, sys.Iron);
        Assert.AreEqual(10, sys.Life);
    }

    [Test]
    public void CanAfford_ReturnsTrueWhenSufficient()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(3, 2, 1);
        Assert.IsTrue(sys.CanAfford(new ElementalCost(3, 2, 1)));
    }

    [Test]
    public void CanAfford_ReturnsFalseWhenInsufficient()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(2, 2, 2);
        Assert.IsFalse(sys.CanAfford(new ElementalCost(3, 0, 0)));
    }

    [Test]
    public void TryConsume_DeductsOnSuccess()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(5, 5, 5);
        Assert.IsTrue(sys.TryConsume(new ElementalCost(2, 1, 3)));
        Assert.AreEqual(3, sys.Fire);
        Assert.AreEqual(4, sys.Iron);
        Assert.AreEqual(2, sys.Life);
    }

    [Test]
    public void TryConsume_ReturnsFalseAndDoesNotDeductOnFailure()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(1, 1, 1);
        Assert.IsFalse(sys.TryConsume(new ElementalCost(2, 0, 0)));
        Assert.AreEqual(1, sys.Fire);
    }
}
```

- [ ] **Step 2: 테스트 실행 — 실패 확인**

Unity Editor → Window > General > Test Runner → Edit Mode → Run All
예상: `ElementalEnergySystem` 클래스 없음 오류

- [ ] **Step 3: ElementalEnergySystem 구현**

`Assets/Scripts/Systems/ElementalEnergySystem.cs`:
```csharp
using UnityEngine;
namespace SlotDefense
{
    public class ElementalEnergySystem
    {
        private int _fire, _iron, _life;
        public const int Max = 10;

        public int Fire => _fire;
        public int Iron => _iron;
        public int Life => _life;

        public void Add(int fire, int iron, int life)
        {
            _fire = Mathf.Min(_fire + fire, Max);
            _iron = Mathf.Min(_iron + iron, Max);
            _life = Mathf.Min(_life + life, Max);
        }

        public void AddByType(ElementType type, int amount)
        {
            switch (type)
            {
                case ElementType.Fire: _fire = Mathf.Min(_fire + amount, Max); break;
                case ElementType.Iron: _iron = Mathf.Min(_iron + amount, Max); break;
                case ElementType.Life: _life = Mathf.Min(_life + amount, Max); break;
            }
        }

        public bool CanAfford(ElementalCost cost) =>
            _fire >= cost.fire && _iron >= cost.iron && _life >= cost.life;

        public bool TryConsume(ElementalCost cost)
        {
            if (!CanAfford(cost)) return false;
            _fire -= cost.fire;
            _iron -= cost.iron;
            _life -= cost.life;
            return true;
        }
    }
}
```

- [ ] **Step 4: 테스트 실행 — 통과 확인**

Unity Test Runner → Edit Mode → Run All
예상: ElementalEnergySystemTests 6개 모두 PASS

- [ ] **Step 5: 커밋**

```bash
git add Assets/Scripts/Systems/ElementalEnergySystem.cs Assets/Tests/EditMode/ElementalEnergySystemTests.cs
git commit -m "feat: add ElementalEnergySystem with tests"
```

---

## Task 3: ReelSystem

**Files:**
- Create: `Assets/Scripts/Systems/ReelSystem.cs`
- Create: `Assets/Tests/EditMode/ReelSystemTests.cs`

- [ ] **Step 1: 테스트 파일 작성**

`Assets/Tests/EditMode/ReelSystemTests.cs`:
```csharp
using NUnit.Framework;
using SlotDefense;
using System;

public class ReelSystemTests
{
    private CardData MakeCard(int fire, int iron, int life)
    {
        var c = UnityEngine.ScriptableObject.CreateInstance<CardData>();
        c.fireCost = fire; c.ironCost = iron; c.lifeCost = life;
        return c;
    }

    [Test]
    public void BuildPool_ReflectsDeckCosts()
    {
        var deck = new[] { MakeCard(2, 0, 0), MakeCard(0, 1, 0) };
        var sys = new ReelSystem(new Random(0));
        sys.BuildPool(deck);
        // pool: Fire×2, Iron×1 → 화 66.7%, 철 33.3%, 생명 0%
        Assert.AreEqual(3, sys.PoolSize);
    }

    [Test]
    public void BuildPool_EmptyDeck_FallsBackToOneEach()
    {
        var sys = new ReelSystem(new Random(0));
        sys.BuildPool(new CardData[0]);
        Assert.AreEqual(3, sys.PoolSize);
    }

    [Test]
    public void CalcEnergy_AllDifferent_GivesOneEach()
    {
        var reels = new[] { ElementType.Fire, ElementType.Iron, ElementType.Life };
        var (f, i, l) = ReelSystem.CalcEnergy(reels);
        Assert.AreEqual(1, f);
        Assert.AreEqual(1, i);
        Assert.AreEqual(1, l);
    }

    [Test]
    public void CalcEnergy_TwoSame_GivesBonus()
    {
        var reels = new[] { ElementType.Fire, ElementType.Fire, ElementType.Iron };
        var (f, i, l) = ReelSystem.CalcEnergy(reels);
        Assert.AreEqual(3, f); // 2 base + 1 bonus
        Assert.AreEqual(1, i);
        Assert.AreEqual(0, l);
    }

    [Test]
    public void CalcEnergy_ThreeSame_GivesBigBonus()
    {
        var reels = new[] { ElementType.Fire, ElementType.Fire, ElementType.Fire };
        var (f, i, l) = ReelSystem.CalcEnergy(reels);
        Assert.AreEqual(6, f); // 3 base + 3 bonus
        Assert.AreEqual(0, i);
        Assert.AreEqual(0, l);
    }
}
```

- [ ] **Step 2: 테스트 실행 — 실패 확인**

Unity Test Runner → Edit Mode → Run All

- [ ] **Step 3: ReelSystem 구현**

`Assets/Scripts/Systems/ReelSystem.cs`:
```csharp
using System;
using System.Collections.Generic;
namespace SlotDefense
{
    public class ReelSystem
    {
        private readonly List<ElementType> _pool = new List<ElementType>();
        private readonly Random _rng;

        public int PoolSize => _pool.Count;

        public ReelSystem(Random rng) => _rng = rng;

        public void BuildPool(CardData[] deck)
        {
            _pool.Clear();
            foreach (var card in deck)
            {
                if (card == null) continue;
                for (int i = 0; i < card.fireCost; i++) _pool.Add(ElementType.Fire);
                for (int i = 0; i < card.ironCost; i++) _pool.Add(ElementType.Iron);
                for (int i = 0; i < card.lifeCost; i++) _pool.Add(ElementType.Life);
            }
            if (_pool.Count == 0)
            {
                _pool.Add(ElementType.Fire);
                _pool.Add(ElementType.Iron);
                _pool.Add(ElementType.Life);
            }
        }

        public ElementType[] Spin() => new[]
        {
            _pool[_rng.Next(_pool.Count)],
            _pool[_rng.Next(_pool.Count)],
            _pool[_rng.Next(_pool.Count)]
        };

        public static (int fire, int iron, int life) CalcEnergy(ElementType[] reels)
        {
            int fc = 0, ic = 0, lc = 0;
            foreach (var r in reels)
            {
                if (r == ElementType.Fire)      fc++;
                else if (r == ElementType.Iron) ic++;
                else                            lc++;
            }
            int fire = fc + (fc == 2 ? 1 : fc == 3 ? 3 : 0);
            int iron = ic + (ic == 2 ? 1 : ic == 3 ? 3 : 0);
            int life = lc + (lc == 2 ? 1 : lc == 3 ? 3 : 0);
            return (fire, iron, life);
        }
    }
}
```

- [ ] **Step 4: 테스트 통과 확인 후 커밋**

```bash
git add Assets/Scripts/Systems/ReelSystem.cs Assets/Tests/EditMode/ReelSystemTests.cs
git commit -m "feat: add ReelSystem — deck-driven reel pool and energy calculation"
```

---

## Task 4: DeckSystem 순환 딜링 + HandSystem.IsEmpty

**Files:**
- Modify: `Assets/Scripts/Systems/DeckSystem.cs`
- Modify: `Assets/Scripts/Systems/HandSystem.cs`
- Modify: `Assets/Tests/EditMode/DeckSystemTests.cs`

- [ ] **Step 1: DeckSystemTests에 순환 딜링 테스트 추가**

`Assets/Tests/EditMode/DeckSystemTests.cs` 파일을 열고 기존 테스트 아래에 추가:
```csharp
[Test]
public void DealNext_ReturnsCardsInOrder()
{
    var c1 = ScriptableObject.CreateInstance<CardData>();
    var c2 = ScriptableObject.CreateInstance<CardData>();
    var deck = new DeckSystem(new[] { c1, c2 });
    Assert.AreEqual(c1, deck.DealNext());
    Assert.AreEqual(c2, deck.DealNext());
}

[Test]
public void DealNext_CyclesWhenExhausted()
{
    var c1 = ScriptableObject.CreateInstance<CardData>();
    var deck = new DeckSystem(new[] { c1 });
    deck.DealNext();
    Assert.AreEqual(c1, deck.DealNext()); // 다시 처음으로
}
```

- [ ] **Step 2: DeckSystem에 DealNext 추가**

`Assets/Scripts/Systems/DeckSystem.cs`:
```csharp
using System;
namespace SlotDefense
{
    public class DeckSystem
    {
        private readonly CardData[] _deck;
        private int _dealIndex;

        public DeckSystem(CardData[] deck) { _deck = deck; _dealIndex = 0; }

        public CardData DealNext()
        {
            if (_deck.Length == 0) return null;
            var card = _deck[_dealIndex % _deck.Length];
            _dealIndex++;
            return card;
        }

        public CardData[] DrawReels(Random rng) => new[]
        {
            _deck[rng.Next(_deck.Length)],
            _deck[rng.Next(_deck.Length)],
            _deck[rng.Next(_deck.Length)]
        };

        public static SlotResult EvaluateReels(CardData[] reels, out CardData matched)
        {
            if (reels[0] == reels[1] && reels[1] == reels[2])
            { matched = reels[0]; return SlotResult.Triple; }
            if (reels[0] == reels[1]) { matched = reels[0]; return SlotResult.Double; }
            if (reels[1] == reels[2]) { matched = reels[1]; return SlotResult.Double; }
            if (reels[0] == reels[2]) { matched = reels[0]; return SlotResult.Double; }
            matched = null;
            return SlotResult.AllDifferent;
        }
    }
}
```

- [ ] **Step 3: HandSystem에 IsEmpty 프로퍼티 추가**

`Assets/Scripts/Systems/HandSystem.cs`를 열고 클래스 내부에 추가:
```csharp
public bool IsEmpty
{
    get
    {
        for (int i = 0; i < _slots.Length; i++)
            if (_slots[i] != null) return false;
        return true;
    }
}
```

- [ ] **Step 4: 테스트 통과 확인 후 커밋**

```bash
git add Assets/Scripts/Systems/DeckSystem.cs Assets/Scripts/Systems/HandSystem.cs Assets/Tests/EditMode/DeckSystemTests.cs
git commit -m "feat: deck rotation dealing and HandSystem.IsEmpty"
```

---

## Task 5: GameManager 재연결

**Files:**
- Modify: `Assets/Scripts/Core/GameManager.cs`

- [ ] **Step 1: GameManager 전체 재작성**

`Assets/Scripts/Core/GameManager.cs`:
```csharp
using System.Collections;
using UnityEngine;
namespace SlotDefense
{
    public class GameManager : MonoBehaviour
    {
        [Header("Config")]
        public FixedDeckConfig deckConfig;
        public GlobalBuffConfig buffConfig;
        [SerializeField] private float villageHp      = 1000f;
        [SerializeField] private float battleDuration = 180f;

        public bool isSurvivalMode;
        public bool IsSurvivalMode => isSurvivalMode;

        public BattleManager          Battle         { get; private set; }
        public SlotMachineSystem      SlotMachine    { get; private set; }
        public HandSystem             Hand           { get; private set; }
        public DeckSystem             Deck           { get; private set; }
        public ElementalEnergySystem  ElementalEnergy { get; private set; }
        public ReelSystem             Reels          { get; private set; }

        public static GameManager Instance { get; private set; }

        private System.Random    _rng;
        private ElementType[]             _pendingReels;
        private (int fire, int iron, int life) _pendingEnergy;
        private bool             _hasPendingSpin;
        private bool             _battleActive;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            _rng = new System.Random();
            if (isSurvivalMode) battleDuration = 99999f;
            Battle          = new BattleManager(villageHp, battleDuration);
            SlotMachine     = new SlotMachineSystem(chargeInterval: 2f, initialCharges: 3);
            Hand            = new HandSystem(4);
            Deck            = new DeckSystem(deckConfig.cards);
            ElementalEnergy = new ElementalEnergySystem();
            Reels           = new ReelSystem(_rng);
            Reels.BuildPool(deckConfig.cards);
        }

        private void OnEnable()
        {
            GameEvents.OnVillageDamaged    += HandleVillageDamaged;
            GameEvents.OnGlobalBuffApplied += HandleGlobalBuff;
        }

        private void OnDisable()
        {
            GameEvents.OnVillageDamaged    -= HandleVillageDamaged;
            GameEvents.OnGlobalBuffApplied -= HandleGlobalBuff;
        }

        private void Update()
        {
            if (!_battleActive) return;
            Battle.Tick(Time.deltaTime);
            SlotMachine.Tick(Time.deltaTime);

            // 손패가 비면 덱에서 다음 4장 자동 지급
            if (Hand.IsEmpty)
            {
                for (int i = 0; i < 4; i++)
                {
                    var card = Deck.DealNext();
                    if (card != null) Hand.TryAdd(card);
                }
            }

            var result = Battle.GetResult();
            if (result != BattleResult.Ongoing)
            {
                _battleActive = false;
                GameEvents.BattleEnded(result);
            }
        }

        public void StartBattle() => _battleActive = true;

        // 스핀: 행운 1 소모 → 릴 결과 계산 (카드 지급 없음)
        public bool TryBeginSpin()
        {
            if (_hasPendingSpin) return false;
            if (!SlotMachine.TrySpin()) return false;
            _pendingReels  = Reels.Spin();
            _pendingEnergy = ReelSystem.CalcEnergy(_pendingReels);
            _hasPendingSpin = true;
            return true;
        }

        public (ElementType[] reels, (int fire, int iron, int life) energy) PeekSpin() =>
            (_pendingReels, _pendingEnergy);

        public void CommitSpin()
        {
            if (_pendingReels == null) return;
            ElementalEnergy.Add(_pendingEnergy.fire, _pendingEnergy.iron, _pendingEnergy.life);
            _pendingReels   = null;
            _hasPendingSpin = false;
        }

        public void UseSkill(SkillEffect effect, Vector3 worldPos)
        {
            float r = effect.radius;
            foreach (var m in FindObjectsOfType<MonsterController>())
                if (!m.IsDead && Vector2.Distance(worldPos, m.transform.position) <= r)
                    m.TakeDamage(effect.damage);

            // 적 기지에도 피해 적용
            foreach (var v in FindObjectsOfType<Village>())
                if (!v.IsPlayerVillage && Vector2.Distance(worldPos, v.transform.position) <= r)
                    GameEvents.VillageDamaged(false, effect.damage);
        }

        private void HandleVillageDamaged(bool isPlayer, float amount)
        {
            if (isPlayer) Battle.DamagePlayerVillage(amount);
            else          Battle.DamageEnemyVillage(amount);
        }

        private void HandleGlobalBuff(BuffEffect buff) => StartCoroutine(BuffCoroutine(buff));

        private IEnumerator BuffCoroutine(BuffEffect buff)
        {
            UnitController.GlobalDamageMultiplier = buff.attackMultiplier;
            UnitController.GlobalSpeedMultiplier  = buff.speedMultiplier;
            yield return new WaitForSeconds(buff.duration);
            UnitController.GlobalDamageMultiplier = 1f;
            UnitController.GlobalSpeedMultiplier  = 1f;
        }
    }
}
```

- [ ] **Step 2: Village에 IsPlayerVillage 프로퍼티 추가**

`Assets/Scripts/Entities/Village.cs`를 열고 확인한다. `isPlayerVillage` bool 필드 또는 프로퍼티가 없으면 추가:
```csharp
public bool IsPlayerVillage; // Inspector에서 플레이어 기지면 true 설정
```

- [ ] **Step 3: 컴파일 확인 후 커밋**

```bash
git add Assets/Scripts/Core/GameManager.cs Assets/Scripts/Entities/Village.cs
git commit -m "feat: GameManager uses ReelSystem for energy; deck auto-refills hand"
```

---

## Task 6: 배치 게이트 — 속성 에너지 소모

**Files:**
- Modify: `Assets/Scripts/Systems/ArenaSystem.cs`
- Modify: `Assets/Scripts/UI/CardDragHandler.cs`

- [ ] **Step 1: ArenaSystem.Update — 유닛/건물 배치 시 ElementalEnergy 소모**

`Assets/Scripts/Systems/ArenaSystem.cs`의 `Update()` 메서드에서 SlotMachine.TryConsume 부분을 교체:
```csharp
// 기존:
// if (!GameManager.Instance.SlotMachine.TryConsume(unitCard.placementCost))

// 변경 후:
if (!GameManager.Instance.ElementalEnergy.TryConsume(unitCard.ElementalCost))
{
    _selectedHandSlot = -1;
    return;
}
```

스킬 카드 사용도 에너지 소모 추가 — skill 사용 블록에서 `Hand.Use(slot)` 직전에:
```csharp
if (!GameManager.Instance.ElementalEnergy.TryConsume(card.ElementalCost)) return;
GameManager.Instance.Hand.Use(slot);
GameManager.Instance.UseSkill(card.skillEffect, worldPos);
```

- [ ] **Step 2: CardDragHandler.OnEndDrag — ElementalEnergy 소모로 변경**

`Assets/Scripts/UI/CardDragHandler.cs` line 65:
```csharp
// 기존:
// if (!GameManager.Instance.SlotMachine.TryConsume(card.placementCost)) return;

// 변경 후:
if (!GameManager.Instance.ElementalEnergy.TryConsume(card.ElementalCost)) return;
```

- [ ] **Step 3: 커밋**

```bash
git add Assets/Scripts/Systems/ArenaSystem.cs Assets/Scripts/UI/CardDragHandler.cs
git commit -m "feat: unit/skill placement consumes elemental energy"
```

---

## Task 7: UI 업데이트

**Files:**
- Modify: `Assets/Scripts/UI/SlotMachineUI.cs`
- Modify: `Assets/Scripts/UI/HandUI.cs`
- Create: `Assets/Scripts/UI/EnergyHUD.cs`

- [ ] **Step 1: SlotMachineUI 업데이트 — 릴에 속성 심볼 표시**

`Assets/Scripts/UI/SlotMachineUI.cs` 전체 교체:
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace SlotDefense
{
    public class SlotMachineUI : MonoBehaviour
    {
        public Text[]  reelLabels;   // 릴 3개 텍스트
        public Button  spinButton;
        public Text    resultText;

        private static readonly string[] SpinPool = { "🔥", "⚔", "💚", "🔥", "⚔" };
        private static readonly Color    GoldColor  = new Color(1f, 0.85f, 0.1f);
        private static readonly Color    WhiteColor = Color.white;
        private bool _animating;

        private void Start() => spinButton.onClick.AddListener(OnSpinClicked);

        private void Update()
        {
            if (GameManager.Instance == null) return;
            int charges = GameManager.Instance.SlotMachine.SpinCharges;
            spinButton.interactable = charges > 0 && !_animating;
            var label = spinButton.GetComponentInChildren<Text>();
            if (label != null) label.text = $"행운 소모 (x{charges})";
        }

        private void OnSpinClicked()
        {
            if (GameManager.Instance == null || _animating) return;
            if (!GameManager.Instance.TryBeginSpin()) return;
            StartCoroutine(SpinAnimation());
        }

        private static string SymbolOf(ElementType t) => t switch
        {
            ElementType.Fire => "🔥화",
            ElementType.Iron => "⚔철",
            ElementType.Life => "💚생",
            _                => "?"
        };

        private IEnumerator SpinAnimation()
        {
            _animating = true;
            spinButton.interactable = false;

            var (reels, energy) = GameManager.Instance.PeekSpin();
            resultText.text = "";

            for (int i = 0; i < reelLabels.Length; i++)
                if (reelLabels[i] != null) reelLabels[i].text = "?";

            float[] stopAt  = { 0.8f, 1.3f, 1.8f };
            bool[]  stopped = { false, false, false };
            float elapsed = 0f, lastTick = 0f;
            const float TickInterval = 0.07f;

            while (!stopped[2])
            {
                elapsed += Time.deltaTime;
                if (elapsed - lastTick >= TickInterval)
                {
                    lastTick = elapsed;
                    for (int i = 0; i < 3; i++)
                    {
                        if (stopped[i]) continue;
                        if (elapsed >= stopAt[i])
                        {
                            stopped[i] = true;
                            if (reelLabels[i] != null) reelLabels[i].text = SymbolOf(reels[i]);
                        }
                        else
                        {
                            if (reelLabels[i] != null)
                                reelLabels[i].text = SpinPool[Random.Range(0, SpinPool.Length)];
                        }
                    }
                }
                yield return null;
            }

            GameManager.Instance.CommitSpin();

            // 결과 표시
            var parts = new System.Collections.Generic.List<string>();
            if (energy.fire > 0) parts.Add($"🔥×{energy.fire}");
            if (energy.iron > 0) parts.Add($"⚔×{energy.iron}");
            if (energy.life > 0) parts.Add($"💚×{energy.life}");
            resultText.text  = string.Join("  ", parts);
            resultText.color = WhiteColor;

            // 트리플이면 골드 강조
            if (energy.fire >= 6 || energy.iron >= 6 || energy.life >= 6)
            {
                resultText.color    = GoldColor;
                resultText.fontSize = 32;
                ScreenFlash.Instance?.Play(new Color(1f, 0.8f, 0f), 0.55f, 0.1f, 0.45f);
            }

            _animating = false;
            Invoke(nameof(ClearResult), 2.5f);
        }

        private void ClearResult()
        {
            resultText.text     = "";
            resultText.color    = WhiteColor;
            resultText.fontSize = 24;
        }
    }
}
```

- [ ] **Step 2: HandUI — 속성 비용 표시로 변경**

`Assets/Scripts/UI/HandUI.cs` 67번줄 교체:
```csharp
// 기존:
// string costStr = $"[{card.placementCost}행운]";

// 변경 후:
var ec = card.ElementalCost;
var parts = new System.Collections.Generic.List<string>();
if (ec.fire > 0) parts.Add($"🔥{ec.fire}");
if (ec.iron > 0) parts.Add($"⚔{ec.iron}");
if (ec.life > 0) parts.Add($"💚{ec.life}");
string costStr = parts.Count > 0 ? $"[{string.Join(" ", parts)}]" : "[무료]";
```

- [ ] **Step 3: EnergyHUD 생성**

`Assets/Scripts/UI/EnergyHUD.cs`:
```csharp
using UnityEngine;
using UnityEngine.UI;
namespace SlotDefense
{
    public class EnergyHUD : MonoBehaviour
    {
        public Text fireText;
        public Text ironText;
        public Text lifeText;

        private void Update()
        {
            if (GameManager.Instance == null) return;
            var e = GameManager.Instance.ElementalEnergy;
            if (fireText != null) fireText.text = $"🔥 {e.Fire}";
            if (ironText != null) ironText.text = $"⚔ {e.Iron}";
            if (lifeText != null) lifeText.text = $"💚 {e.Life}";
        }
    }
}
```

- [ ] **Step 4: 커밋**

```bash
git add Assets/Scripts/UI/SlotMachineUI.cs Assets/Scripts/UI/HandUI.cs Assets/Scripts/UI/EnergyHUD.cs
git commit -m "feat: UI shows elemental reel symbols, energy costs, and EnergyHUD"
```

---

## Task 8: 건물 시스템

**Files:**
- Create: `Assets/Scripts/Entities/BuildingController.cs`
- Create: `Assets/Scripts/Entities/BattleBuilding.cs`
- Create: `Assets/Scripts/Entities/ProductionBuilding.cs`
- Modify: `Assets/Scripts/Systems/ArenaSystem.cs`

- [ ] **Step 1: BuildingController 베이스 생성**

`Assets/Scripts/Entities/BuildingController.cs`:
```csharp
using UnityEngine;
namespace SlotDefense
{
    public class BuildingController : MonoBehaviour
    {
        protected BuildingData _data;
        protected float _currentHp;
        private const float BuildingHp = 300f;
        private HpBar _hpBar;

        public virtual void Init(BuildingData data)
        {
            _data      = data;
            _currentHp = BuildingHp;
            _hpBar     = gameObject.AddComponent<HpBar>();
            _hpBar.Setup(yOffset: 0.6f, width: 0.8f);
        }

        public void TakeDamage(float amount)
        {
            _currentHp -= amount;
            _hpBar?.SetRatio(_currentHp / BuildingHp);
            if (_currentHp <= 0f) Destroy(gameObject);
        }
    }
}
```

- [ ] **Step 2: BattleBuilding 구현**

`Assets/Scripts/Entities/BattleBuilding.cs`:
```csharp
using UnityEngine;
namespace SlotDefense
{
    public class BattleBuilding : BuildingController
    {
        private float _attackCooldown;

        private void Update()
        {
            if (_data == null) return;
            _attackCooldown -= Time.deltaTime;
            if (_attackCooldown > 0f) return;

            // 가장 가까운 적 탐색 (몬스터 우선)
            float nearest = _data.attackRange;
            MonsterController target = null;
            foreach (var m in MonsterController.AllMonsters)
            {
                if (m.IsDead) continue;
                if (m.isFlying && !_data.canAttackAir) continue;
                float d = Vector2.Distance(transform.position, m.transform.position);
                if (d < nearest) { nearest = d; target = m; }
            }

            if (target != null)
            {
                _attackCooldown = 1f / _data.attackRate;
                target.TakeDamage(_data.attackDamage);
            }
        }
    }
}
```

- [ ] **Step 3: ProductionBuilding 구현**

`Assets/Scripts/Entities/ProductionBuilding.cs`:
```csharp
using UnityEngine;
namespace SlotDefense
{
    public class ProductionBuilding : BuildingController
    {
        private float _timer;

        private void Update()
        {
            if (_data == null || GameManager.Instance == null) return;
            _timer += Time.deltaTime;

            if (_data.buildingType == BuildingType.ProductionEnergy)
            {
                if (_timer >= 1f)
                {
                    _timer -= 1f;
                    int amt = Mathf.RoundToInt(_data.energyPerSecond);
                    GameManager.Instance.ElementalEnergy.AddByType(_data.energyType, amt);
                }
            }
            else if (_data.buildingType == BuildingType.ProductionUnit)
            {
                if (_timer >= _data.spawnInterval && _data.unitToSpawn != null)
                {
                    _timer = 0f;
                    SpawnUnit();
                }
            }
        }

        private void SpawnUnit()
        {
            var arena = FindObjectOfType<ArenaSystem>();
            if (arena == null) return;
            var prefab = _data.unitToSpawn.unitPrefab != null
                ? _data.unitToSpawn.unitPrefab
                : arena.unitPrefab;
            var go = Instantiate(prefab, transform.position + Vector3.left * 0.5f, Quaternion.identity);
            go.GetComponent<UnitController>().Init(_data.unitToSpawn.unitStats, isPlayerUnit: true, portal: arena.portal);
            go.SetActive(true);
        }
    }
}
```

- [ ] **Step 4: ArenaSystem에 건물 배치 처리 추가**

`Assets/Scripts/Systems/ArenaSystem.cs`의 `Update()` 메서드에서 유닛 배치 블록 이후에 추가:

```csharp
// 건물 배치
if (unitCard.cardType == CardType.Building && unitCard.buildingData != null)
{
    worldPos.x = Mathf.Min(worldPos.x, -0.5f);
    var bgo = new GameObject(unitCard.cardName);
    bgo.transform.position = worldPos;
    var sr = bgo.AddComponent<SpriteRenderer>();
    sr.sprite       = unitCard.icon;
    sr.sortingOrder = 1;

    BuildingController bc;
    if (unitCard.buildingData.buildingType == BuildingType.BattleTower)
        bc = bgo.AddComponent<BattleBuilding>();
    else
        bc = bgo.AddComponent<ProductionBuilding>();
    bc.Init(unitCard.buildingData);

    GameManager.Instance.Hand.Use(_selectedHandSlot);
    _selectedHandSlot = -1;
    return;
}
```

- [ ] **Step 5: 커밋**

```bash
git add Assets/Scripts/Entities/BuildingController.cs Assets/Scripts/Entities/BattleBuilding.cs Assets/Scripts/Entities/ProductionBuilding.cs Assets/Scripts/Systems/ArenaSystem.cs
git commit -m "feat: building system — battle towers and production buildings"
```

---

## Task 9: 공중 유닛 + 몬스터 다양화

**Files:**
- Modify: `Assets/Scripts/Entities/UnitController.cs`
- Modify: `Assets/Scripts/Entities/MonsterController.cs`

- [ ] **Step 1: MonsterController에 isFlying 추가**

`Assets/Scripts/Entities/MonsterController.cs` — 기존 필드 아래에 추가:
```csharp
public bool isFlying;
```

`Init()` 메서드에서 config에 따라 설정하려면 MonsterConfig에도 추가:
```csharp
// MonsterConfig.cs에 추가
public bool isFlying;
```

`Init()` 내부에:
```csharp
isFlying = cfg.isFlying;
```

- [ ] **Step 2: UnitController — 공중 타겟 필터링**

`AcquireTarget()` 내 foreach 루프에서 공중 몬스터 필터 추가:
```csharp
foreach (var m in FindObjectsOfType<MonsterController>())
{
    if (m.IsDead) continue;
    if (m.isFlying && !_stats.canAttackAir) continue; // 공중 공격 불가 시 스킵
    var dist = Vector2.Distance(transform.position, m.transform.position);
    if (dist > sight) continue;
    if (dist < nearest) { nearest = dist; _target = m; }
}
```

- [ ] **Step 3: 커밋**

```bash
git add Assets/Scripts/Entities/UnitController.cs Assets/Scripts/Entities/MonsterController.cs Assets/Scripts/Config/MonsterConfig.cs
git commit -m "feat: flying unit flag and anti-air targeting filter"
```

---

## Task 10: TestSceneBootstrapper 전체 재작성

**Files:**
- Modify: `Assets/Scripts/Core/TestSceneBootstrapper.cs`

- [ ] **Step 1: MakeCard 시그니처 변경**

`TestSceneBootstrapper.cs`에서 `MakeCard` 메서드를 찾아 교체:
```csharp
static CardData MakeCard(string name,
    float hp, float dmg, float speed, float range, float rate,
    float sight = 5f, float heal = 0f, float luckPerSec = 0f,
    bool canAttackAir = false, bool isFlying = false,
    int fire = 0, int iron = 0, int life = 0)
{
    var card = ScriptableObject.CreateInstance<CardData>();
    card.cardName  = name;
    card.cardType  = CardType.Unit;
    card.fireCost  = fire;
    card.ironCost  = iron;
    card.lifeCost  = life;
    card.unitStats = new UnitStats
    {
        hp = hp, damage = dmg, moveSpeed = speed,
        attackRange = range, attackRate = rate,
        sightRange = sight, healAmount = heal,
        luckGenRate = luckPerSec,
        canAttackAir = canAttackAir, isFlying = isFlying
    };
    return card;
}
```

- [ ] **Step 2: MakeSkillCard 시그니처 변경**

```csharp
static CardData MakeSkillCard(string name, SkillType type, float damage, float radius,
    int fire = 0, int iron = 0, int life = 0)
{
    var card = ScriptableObject.CreateInstance<CardData>();
    card.cardName   = name;
    card.cardType   = CardType.Skill;
    card.fireCost   = fire;
    card.ironCost   = iron;
    card.lifeCost   = life;
    card.skillEffect = new SkillEffect { type = type, damage = damage, radius = radius };
    return card;
}
```

- [ ] **Step 3: MakeBuildingCard 헬퍼 추가**

```csharp
static CardData MakeBuildingCard(string name, BuildingData bdata,
    int fire = 0, int iron = 0, int life = 0)
{
    var card = ScriptableObject.CreateInstance<CardData>();
    card.cardName    = name;
    card.cardType    = CardType.Building;
    card.fireCost    = fire;
    card.ironCost    = iron;
    card.lifeCost    = life;
    card.buildingData = bdata;
    return card;
}
```

- [ ] **Step 4: Awake의 _deckCfg 카드 정의 재작성**

`_deckCfg = Inst<FixedDeckConfig>(d => { ... })` 블록 전체 교체:
```csharp
_deckCfg = Inst<FixedDeckConfig>(d =>
{
    // 유닛 10종
    var swordsman  = MakeCard("검사",     hp:80,  dmg:15, speed:2f,   range:1.5f, rate:1f,   sight:5f,  iron:1);
    var archer     = MakeCard("궁수",     hp:50,  dmg:10, speed:1.5f, range:5f,   rate:2f,   sight:8f,  canAttackAir:true, fire:1, iron:1);
    var knight     = MakeCard("기사",     hp:120, dmg:20, speed:1.2f, range:1f,   rate:0.8f, sight:4f,  iron:2);
    var mage       = MakeCard("마법사",   hp:40,  dmg:28, speed:1.8f, range:4.5f, rate:0.6f, sight:8f,  canAttackAir:true, fire:2);
    var healer     = MakeCard("힐러",     hp:70,  dmg:0,  speed:1.6f, range:2f,   rate:0.8f, sight:6f,  heal:8f, life:2);
    var luckGen    = MakeCard("행운술사", hp:60,  dmg:0,  speed:0.8f, range:0f,   rate:0f,   sight:0f,  luckPerSec:0.5f, iron:1, life:1);
    var paladin    = MakeCard("팔라딘",   hp:200, dmg:18, speed:0.9f, range:1.2f, rate:0.7f, sight:4f,  iron:2, life:2);
    var pyromancer = MakeCard("화염술사", hp:40,  dmg:35, speed:1.7f, range:4f,   rate:0.5f, sight:8f,  canAttackAir:true, fire:3);
    var crusader   = MakeCard("성기사",   hp:100, dmg:15, speed:1.4f, range:1.5f, rate:1f,   sight:5f,  heal:3f, fire:1, iron:2, life:1);
    var stormArcher= MakeCard("폭풍궁수", hp:60,  dmg:12, speed:1.6f, range:6f,   rate:2.5f, sight:10f, canAttackAir:true, fire:2, iron:2);

    // 마법 4종
    var lightning  = MakeSkillCard("번개화살", SkillType.LightningArrow, damage:80f,  radius:2.0f, fire:2);
    var portalBomb = MakeSkillCard("포탈폭격", SkillType.PortalBomb,     damage:120f, radius:3.0f, fire:2, iron:1);
    // 주의: 생명의비/철벽강화는 신규 SkillType 추가가 필요 — 현재 스펙 범위 외
    // 기본 덱에는 포함하지 않음

    // 건물 — 전투
    var fireTower = MakeBuildingCard("화염탑", new BuildingData
    {
        buildingType = BuildingType.BattleTower,
        attackDamage = 20f, attackRate = 1f, attackRange = 5f, canAttackAir = true
    }, fire:2);
    var sniperTower = MakeBuildingCard("저격탑", new BuildingData
    {
        buildingType = BuildingType.BattleTower,
        attackDamage = 50f, attackRate = 0.4f, attackRange = 8f, canAttackAir = true
    }, fire:1, iron:1);

    // 건물 — 에너지 생산
    var furnace = MakeBuildingCard("화염로", new BuildingData
    {
        buildingType = BuildingType.ProductionEnergy, energyType = ElementType.Fire, energyPerSecond = 1f
    }, fire:1);
    var forge = MakeBuildingCard("제철소", new BuildingData
    {
        buildingType = BuildingType.ProductionEnergy, energyType = ElementType.Iron, energyPerSecond = 1f
    }, iron:1);
    var lifespring = MakeBuildingCard("생명의샘", new BuildingData
    {
        buildingType = BuildingType.ProductionEnergy, energyType = ElementType.Life, energyPerSecond = 1f
    }, life:1);

    // 건물 — 유닛 생산
    var barracks = MakeBuildingCard("병영", new BuildingData
    {
        buildingType = BuildingType.ProductionUnit, unitToSpawn = swordsman, spawnInterval = 10f
    }, iron:2);
    var magicCircle = MakeBuildingCard("마법진", new BuildingData
    {
        buildingType = BuildingType.ProductionUnit, unitToSpawn = mage, spawnInterval = 15f
    }, fire:2, life:1);

    // 덱: 8장 (기본 구성 — 플레이어가 Inspector에서 교체 가능)
    d.cards = new CardData[]
    {
        swordsman, archer, knight, mage,
        healer, lightning, portalBomb, fireTower
    };

    // Resources/Prefabs/Units/ 에 있으면 연결
    foreach (var card in new[] { swordsman, archer, knight, mage, healer, luckGen,
                                  paladin, pyromancer, crusader, stormArcher })
    {
        TrySetUnitPrefab(card, null);
    }
});
```

- [ ] **Step 5: 몬스터 설정 추가 — 지상 4종 + 공중 3종**

`_monsterCfg`, `_eliteCfg` 외에 신규 설정 추가. `TestSceneBootstrapper` 상단 필드에:
```csharp
private MonsterConfig _goblinCfg;
private MonsterConfig _trollCfg;
private MonsterConfig _batCfg;
private MonsterConfig _dragonCfg;
```

`Awake()` 에서:
```csharp
_goblinCfg = Inst<MonsterConfig>(m =>
    { m.hp = 20f; m.damage = 4f; m.moveSpeed = 2.5f; m.xpReward = 20f; });
_trollCfg  = Inst<MonsterConfig>(m =>
    { m.hp = 200f; m.damage = 15f; m.moveSpeed = 0.8f; m.xpReward = 120f; });
_batCfg    = Inst<MonsterConfig>(m =>
    { m.hp = 15f; m.damage = 5f; m.moveSpeed = 3f; m.xpReward = 30f; m.isFlying = true; });
_dragonCfg = Inst<MonsterConfig>(m =>
    { m.hp = 400f; m.damage = 40f; m.moveSpeed = 1.5f; m.xpReward = 300f; m.isFlying = true; });
```

- [ ] **Step 6: 전체 컴파일 확인 + 커밋**

```bash
git add Assets/Scripts/Core/TestSceneBootstrapper.cs
git commit -m "feat: full card/monster data rewrite for elemental system"
```

---

## Task 11: EnergyHUD를 TestSceneBootstrapper에서 빌드

**Files:**
- Modify: `Assets/Scripts/Core/TestSceneBootstrapper.cs`

- [ ] **Step 1: BuildUI 메서드 내부에 EnergyHUD 추가**

`BuildUI()` 메서드에서 기존 ArenaHUD / SlotMachineUI 빌드 이후에 추가:
```csharp
// --- EnergyHUD ---
var energyGo   = new GameObject("EnergyHUD");
energyGo.transform.SetParent(canvas.transform, false);
var energyHud  = energyGo.AddComponent<EnergyHUD>();
var energyRect = energyGo.GetComponent<RectTransform>();
energyRect.anchorMin = new Vector2(0.5f, 1f);
energyRect.anchorMax = new Vector2(0.5f, 1f);
energyRect.pivot     = new Vector2(0.5f, 1f);
energyRect.anchoredPosition = new Vector2(0f, -10f);
energyRect.sizeDelta = new Vector2(300f, 40f);

energyHud.fireText = MakeLabel(energyGo, "FireText", new Vector2(-90f, 0f));
energyHud.ironText = MakeLabel(energyGo, "IronText", new Vector2(0f, 0f));
energyHud.lifeText = MakeLabel(energyGo, "LifeText", new Vector2(90f, 0f));
```

`MakeLabel` 헬퍼 (없으면 추가):
```csharp
static Text MakeLabel(GameObject parent, string name, Vector2 offset)
{
    var go   = new GameObject(name);
    go.transform.SetParent(parent.transform, false);
    var rect = go.AddComponent<RectTransform>();
    rect.anchoredPosition = offset;
    rect.sizeDelta        = new Vector2(90f, 30f);
    var txt  = go.AddComponent<Text>();
    txt.font      = Resources.GetBuiltinResource<Font>("Arial.ttf");
    txt.fontSize  = 18;
    txt.color     = Color.white;
    txt.alignment = TextAnchor.MiddleCenter;
    return txt;
}
```

- [ ] **Step 2: 최종 컴파일 + 테스트 전체 실행**

Unity Test Runner → Edit Mode → Run All
예상: 모든 기존 테스트 + 신규 테스트 PASS

- [ ] **Step 3: 최종 커밋**

```bash
git add .
git commit -m "feat: EnergyHUD integrated into scene bootstrap"
```
