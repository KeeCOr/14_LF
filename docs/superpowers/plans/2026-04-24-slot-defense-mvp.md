# Slot Defense Game (14_LT) — MVP Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 플레이어(좌)와 AI(우)가 각자 마을을 몬스터 웨이브로부터 방어하며, 처치한 몬스터가 상대 진영으로 전송되고 슬롯머신으로 얻은 유닛·스킬로 싸우는 MVP를 구축한다.

**Architecture:** 16:9 가로 Unity 2D 씬. 순수 C# 시스템(HandSystem, DeckSystem, SlotMachineSystem, BattleManager)이 모든 게임 로직을 담당하며 Edit Mode 테스트로 검증된다. MonoBehaviour 컴포넌트는 씬 엔티티(Village, Monster, Unit)를 감싸고 GameEvents 정적 이벤트 버스로 통신한다. IOpponent 인터페이스가 AI와 미래 PvP 구현을 격리한다.

**Tech Stack:** Unity 2D (URP), Unity Test Framework (NUnit, Edit Mode), ScriptableObjects, C# interfaces.

---

## 파일 맵

### Config (ScriptableObject & Structs)
| 파일 | 책임 |
|------|------|
| `Assets/Scripts/Config/CardType.cs` | enum: Unit, Skill |
| `Assets/Scripts/Config/CardTier.cs` | enum: Normal, Enhanced |
| `Assets/Scripts/Config/SlotResult.cs` | enum: Triple, Double, AllDifferent |
| `Assets/Scripts/Config/BattleResult.cs` | enum: Ongoing, PlayerWin, PlayerLose, Draw |
| `Assets/Scripts/Config/UnitStats.cs` | struct: hp, damage, moveSpeed, attackRange, attackRate |
| `Assets/Scripts/Config/SkillEffect.cs` | struct: damage, radius, duration |
| `Assets/Scripts/Config/BuffEffect.cs` | struct: attackMultiplier, speedMultiplier, duration |
| `Assets/Scripts/Config/CardData.cs` | SO: cardType, cardName, icon, unitStats, skillEffect |
| `Assets/Scripts/Config/MonsterConfig.cs` | SO: hp, damage, moveSpeed, xpReward |
| `Assets/Scripts/Config/FixedDeckConfig.cs` | SO: CardData[12] |
| `Assets/Scripts/Config/GlobalBuffConfig.cs` | SO: BuffEffect[] possibleBuffs |

### Systems (순수 C#, 테스트 대상)
| 파일 | 책임 |
|------|------|
| `Assets/Scripts/Systems/HandSystem.cs` | 4슬롯 핸드: 추가, 사용, 조회 |
| `Assets/Scripts/Systems/DeckSystem.cs` | 릴 뽑기, SlotResult 판정 |
| `Assets/Scripts/Systems/SlotMachineSystem.cs` | XP → 스핀 기회 적립, 스핀 소비 |
| `Assets/Scripts/Systems/BattleManager.cs` | 마을 HP, 타이머, 승패 판정 |

### Core
| 파일 | 책임 |
|------|------|
| `Assets/Scripts/Core/GameEvents.cs` | 정적 C# 이벤트 버스 |
| `Assets/Scripts/Core/GameManager.cs` | MB: 씬 와이어링, 게임 상태 머신 |

### Entities (MonoBehaviour)
| 파일 | 책임 |
|------|------|
| `Assets/Scripts/Entities/Village.cs` | MB: HP 수신, 파괴 이벤트 발행 |
| `Assets/Scripts/Entities/MonsterController.cs` | MB: 마을 방향 이동, 공격, 사망 |
| `Assets/Scripts/Entities/UnitController.cs` | MB: 최근접 몬스터 탐색, 이동, 공격 |

### Systems (MonoBehaviour)
| 파일 | 책임 |
|------|------|
| `Assets/Scripts/Systems/ArenaSystem.cs` | MB: 웨이브 소환, 유닛 배치 입력 |
| `Assets/Scripts/Systems/TransferSystem.cs` | MB: 처치 몬스터 큐 → 상대 진영 소환 |

### AI
| 파일 | 책임 |
|------|------|
| `Assets/Scripts/AI/IOpponent.cs` | interface: OnUpdate, ReceiveTransferredMonster |
| `Assets/Scripts/AI/AIOpponent.cs` | MB: AI 카드 배치, 자동 스핀 |

### UI
| 파일 | 책임 |
|------|------|
| `Assets/Scripts/UI/ArenaHUD.cs` | MB: 마을 HP바, 타이머, 스핀 횟수 |
| `Assets/Scripts/UI/HandUI.cs` | MB: 4카드 슬롯 버튼, 배치 입력 |
| `Assets/Scripts/UI/SlotMachineUI.cs` | MB: 3릴 표시, 스핀 버튼, 결과 연출 |
| `Assets/Scripts/UI/ResultUI.cs` | MB: 승/패 패널 |

### Tests
| 파일 | 책임 |
|------|------|
| `Assets/Tests/EditMode/HandSystemTests.cs` | HandSystem 단위 테스트 |
| `Assets/Tests/EditMode/DeckSystemTests.cs` | DeckSystem 단위 테스트 |
| `Assets/Tests/EditMode/SlotMachineSystemTests.cs` | SlotMachineSystem 단위 테스트 |
| `Assets/Tests/EditMode/BattleManagerTests.cs` | BattleManager 단위 테스트 |

---

## Task 1: Unity 프로젝트 설정

**Files:**
- Unity Hub에서 신규 프로젝트 생성
- `Assets/Tests/EditMode/SlotDefense.Tests.asmdef`

- [ ] **Step 1: Unity Hub에서 2D (URP) 프로젝트 생성**

  Unity Hub 실행 → New project → 2D (URP) 템플릿 선택  
  Location: `c:\Users\오진우\OneDrive - 스토익엔터테인먼트\바탕 화면\게임개발\14_LT`  
  Project name: `SlotDefense`

- [ ] **Step 2: 폴더 구조 생성**

  Unity Editor의 Project 창에서 `Assets` 하위에 다음 폴더를 생성:
  ```
  Assets/Scripts/Config/
  Assets/Scripts/Core/
  Assets/Scripts/Systems/
  Assets/Scripts/Entities/
  Assets/Scripts/AI/
  Assets/Scripts/UI/
  Assets/Tests/EditMode/
  Assets/ScriptableObjects/Decks/
  Assets/ScriptableObjects/Cards/
  Assets/Prefabs/Monsters/
  Assets/Prefabs/Units/
  ```

- [ ] **Step 3: 화면 해상도 설정**

  Edit → Project Settings → Player  
  - Resolution and Presentation → Default Screen Width: `1920`, Height: `1080`
  - Allow Full Screen Switch: 체크
  - Default Orientation: Landscape Left

- [ ] **Step 4: Edit Mode 테스트 어셈블리 정의 생성**

  `Assets/Tests/EditMode/` 폴더 우클릭 → Create → Testing → Assembly Definition  
  이름: `SlotDefense.Tests`
  
  생성된 `.asmdef` 파일에서 Inspector에서 설정:
  - Platforms: Editor만 체크
  - Assembly Definition References: `UnityEngine.TestRunner`, `UnityEditor.TestRunner`
  - 프로젝트 Scripts Assembly (`Assembly-CSharp`) 도 참조 추가

- [ ] **Step 5: Scripts 어셈블리 정의 생성**

  `Assets/Scripts/` 폴더 우클릭 → Create → Assembly Definition  
  이름: `SlotDefense`  
  이 어셈블리를 `SlotDefense.Tests`의 References에 추가

- [ ] **Step 6: 커밋**

  ```bash
  git init
  git add .
  git commit -m "feat: initialize Unity 2D URP project with folder structure"
  ```

---

## Task 2: Config — Enums & Structs

**Files:**
- Create: `Assets/Scripts/Config/CardType.cs`
- Create: `Assets/Scripts/Config/CardTier.cs`
- Create: `Assets/Scripts/Config/SlotResult.cs`
- Create: `Assets/Scripts/Config/BattleResult.cs`
- Create: `Assets/Scripts/Config/UnitStats.cs`
- Create: `Assets/Scripts/Config/SkillEffect.cs`
- Create: `Assets/Scripts/Config/BuffEffect.cs`

- [ ] **Step 1: CardType.cs 생성**

  ```csharp
  namespace SlotDefense
  {
      public enum CardType { Unit, Skill }
  }
  ```

- [ ] **Step 2: CardTier.cs 생성**

  ```csharp
  namespace SlotDefense
  {
      public enum CardTier { Normal, Enhanced }
  }
  ```

- [ ] **Step 3: SlotResult.cs 생성**

  ```csharp
  namespace SlotDefense
  {
      public enum SlotResult { Triple, Double, AllDifferent }
  }
  ```

- [ ] **Step 4: BattleResult.cs 생성**

  ```csharp
  namespace SlotDefense
  {
      public enum BattleResult { Ongoing, PlayerWin, PlayerLose, Draw }
  }
  ```

- [ ] **Step 5: UnitStats.cs 생성**

  ```csharp
  using System;
  namespace SlotDefense
  {
      [Serializable]
      public struct UnitStats
      {
          public float hp;
          public float damage;
          public float moveSpeed;
          public float attackRange;
          public float attackRate; // attacks per second
      }
  }
  ```

- [ ] **Step 6: SkillEffect.cs 생성**

  ```csharp
  using System;
  namespace SlotDefense
  {
      [Serializable]
      public struct SkillEffect
      {
          public float damage;
          public float radius;
          public float duration;
      }
  }
  ```

- [ ] **Step 7: BuffEffect.cs 생성**

  ```csharp
  using System;
  namespace SlotDefense
  {
      [Serializable]
      public struct BuffEffect
      {
          public float attackMultiplier; // 1.0 = no change, 1.1 = +10%
          public float speedMultiplier;
          public float duration;         // 0 = permanent for this battle
      }
  }
  ```

- [ ] **Step 8: 컴파일 확인**

  Unity Editor 하단 Console에 빨간 에러 없음 확인

- [ ] **Step 9: 커밋**

  ```bash
  git add Assets/Scripts/Config/
  git commit -m "feat: add config enums and value structs"
  ```

---

## Task 3: Config — ScriptableObjects

**Files:**
- Create: `Assets/Scripts/Config/CardData.cs`
- Create: `Assets/Scripts/Config/MonsterConfig.cs`
- Create: `Assets/Scripts/Config/FixedDeckConfig.cs`
- Create: `Assets/Scripts/Config/GlobalBuffConfig.cs`

- [ ] **Step 1: CardData.cs 생성**

  ```csharp
  using UnityEngine;
  namespace SlotDefense
  {
      [CreateAssetMenu(menuName = "SlotDefense/CardData", fileName = "NewCard")]
      public class CardData : ScriptableObject
      {
          public CardType cardType;
          public string cardName;
          public Sprite icon;
          public UnitStats unitStats;   // cardType == Unit 일 때 사용
          public SkillEffect skillEffect; // cardType == Skill 일 때 사용
      }
  }
  ```

- [ ] **Step 2: MonsterConfig.cs 생성**

  ```csharp
  using UnityEngine;
  namespace SlotDefense
  {
      [CreateAssetMenu(menuName = "SlotDefense/MonsterConfig", fileName = "NewMonster")]
      public class MonsterConfig : ScriptableObject
      {
          public float hp;
          public float damage;
          public float moveSpeed;
          public float xpReward;
      }
  }
  ```

- [ ] **Step 3: FixedDeckConfig.cs 생성**

  ```csharp
  using UnityEngine;
  namespace SlotDefense
  {
      [CreateAssetMenu(menuName = "SlotDefense/FixedDeckConfig", fileName = "DefaultDeck")]
      public class FixedDeckConfig : ScriptableObject
      {
          [Tooltip("정확히 12장이어야 합니다")]
          public CardData[] cards = new CardData[12];
      }
  }
  ```

- [ ] **Step 4: GlobalBuffConfig.cs 생성**

  ```csharp
  using UnityEngine;
  namespace SlotDefense
  {
      [CreateAssetMenu(menuName = "SlotDefense/GlobalBuffConfig", fileName = "GlobalBuffs")]
      public class GlobalBuffConfig : ScriptableObject
      {
          [Tooltip("슬롯 모두 다름 결과 시 랜덤 적용될 버프 목록")]
          public BuffEffect[] possibleBuffs;
      }
  }
  ```

- [ ] **Step 5: 컴파일 확인**

  Unity Console에 에러 없음 확인

- [ ] **Step 6: 커밋**

  ```bash
  git add Assets/Scripts/Config/
  git commit -m "feat: add ScriptableObject configs (Card, Monster, Deck, Buff)"
  ```

---

## Task 4: HandSystem + Tests

**Files:**
- Create: `Assets/Scripts/Systems/HandSystem.cs`
- Create: `Assets/Tests/EditMode/HandSystemTests.cs`

- [ ] **Step 1: 실패하는 테스트 작성**

  `Assets/Tests/EditMode/HandSystemTests.cs`:

  ```csharp
  using NUnit.Framework;
  using UnityEngine;
  using SlotDefense;

  public class HandSystemTests
  {
      private HandSystem _hand;

      [SetUp]
      public void SetUp() => _hand = new HandSystem(4);

      [Test]
      public void TryAdd_ToEmptyHand_ReturnsTrue()
      {
          var card = ScriptableObject.CreateInstance<CardData>();
          Assert.IsTrue(_hand.TryAdd(card));
      }

      [Test]
      public void TryAdd_ToFullHand_ReturnsFalse()
      {
          var card = ScriptableObject.CreateInstance<CardData>();
          _hand.TryAdd(card);
          _hand.TryAdd(card);
          _hand.TryAdd(card);
          _hand.TryAdd(card);
          Assert.IsFalse(_hand.TryAdd(card));
      }

      [Test]
      public void Use_FilledSlot_ReturnsCardAndClearsSlot()
      {
          var card = ScriptableObject.CreateInstance<CardData>();
          card.cardName = "Test";
          _hand.TryAdd(card);
          var used = _hand.Use(0);
          Assert.AreEqual(card, used);
          Assert.IsNull(_hand.GetSlot(0));
      }

      [Test]
      public void Use_EmptySlot_ReturnsNull()
      {
          Assert.IsNull(_hand.Use(0));
      }

      [Test]
      public void IsFull_WhenAllSlotsFilled_ReturnsTrue()
      {
          var card = ScriptableObject.CreateInstance<CardData>();
          for (int i = 0; i < 4; i++) _hand.TryAdd(card);
          Assert.IsTrue(_hand.IsFull);
      }
  }
  ```

  > `ScriptableObject.CreateInstance<T>()` 는 Edit Mode 테스트에서 사용 가능합니다.

- [ ] **Step 2: 테스트 실행 → 실패 확인**

  Window > General > Test Runner > EditMode > Run All  
  Expected: `HandSystemTests` — 모두 실패 (HandSystem 없음)

- [ ] **Step 3: HandSystem 구현**

  `Assets/Scripts/Systems/HandSystem.cs`:

  ```csharp
  using System;
  namespace SlotDefense
  {
      public class HandSystem
      {
          private readonly CardData[] _slots;
          public int Capacity { get; }

          public HandSystem(int capacity = 4)
          {
              Capacity = capacity;
              _slots = new CardData[capacity];
          }

          public bool TryAdd(CardData card)
          {
              for (int i = 0; i < _slots.Length; i++)
              {
                  if (_slots[i] == null)
                  {
                      _slots[i] = card;
                      return true;
                  }
              }
              return false;
          }

          public CardData Use(int slotIndex)
          {
              if (slotIndex < 0 || slotIndex >= _slots.Length) return null;
              var card = _slots[slotIndex];
              _slots[slotIndex] = null;
              return card;
          }

          public CardData GetSlot(int index) => _slots[index];
          public bool IsFull => Array.TrueForAll(_slots, s => s != null);
      }
  }
  ```

- [ ] **Step 4: 테스트 실행 → 통과 확인**

  Test Runner > Run All  
  Expected: `HandSystemTests` — 5/5 Pass

- [ ] **Step 5: 커밋**

  ```bash
  git add Assets/Scripts/Systems/HandSystem.cs Assets/Tests/EditMode/HandSystemTests.cs
  git commit -m "feat: add HandSystem with unit tests"
  ```

---

## Task 5: DeckSystem + Tests

**Files:**
- Create: `Assets/Scripts/Systems/DeckSystem.cs`
- Create: `Assets/Tests/EditMode/DeckSystemTests.cs`

- [ ] **Step 1: 실패하는 테스트 작성**

  `Assets/Tests/EditMode/DeckSystemTests.cs`:

  ```csharp
  using NUnit.Framework;
  using UnityEngine;
  using SlotDefense;
  using System;

  public class DeckSystemTests
  {
      private CardData[] MakeDeck(int uniqueCount)
      {
          var deck = new CardData[12];
          var cards = new CardData[uniqueCount];
          for (int i = 0; i < uniqueCount; i++)
          {
              cards[i] = ScriptableObject.CreateInstance<CardData>();
              cards[i].cardName = $"Card{i}";
          }
          // 균등 배분
          for (int i = 0; i < 12; i++) deck[i] = cards[i % uniqueCount];
          return deck;
      }

      [Test]
      public void EvaluateReels_AllSame_ReturnsTriple()
      {
          var card = ScriptableObject.CreateInstance<CardData>();
          var reels = new[] { card, card, card };
          var result = DeckSystem.EvaluateReels(reels, out var matched);
          Assert.AreEqual(SlotResult.Triple, result);
          Assert.AreEqual(card, matched);
      }

      [Test]
      public void EvaluateReels_TwoSame_ReturnsDouble()
      {
          var cardA = ScriptableObject.CreateInstance<CardData>();
          var cardB = ScriptableObject.CreateInstance<CardData>();
          var reels = new[] { cardA, cardA, cardB };
          var result = DeckSystem.EvaluateReels(reels, out var matched);
          Assert.AreEqual(SlotResult.Double, result);
          Assert.AreEqual(cardA, matched);
      }

      [Test]
      public void EvaluateReels_AllDifferent_ReturnsAllDifferent()
      {
          var reels = new[]
          {
              ScriptableObject.CreateInstance<CardData>(),
              ScriptableObject.CreateInstance<CardData>(),
              ScriptableObject.CreateInstance<CardData>()
          };
          var result = DeckSystem.EvaluateReels(reels, out var matched);
          Assert.AreEqual(SlotResult.AllDifferent, result);
          Assert.IsNull(matched);
      }

      [Test]
      public void DrawReels_AlwaysReturnsThreeCardsFromDeck()
      {
          var deck = MakeDeck(3);
          var system = new DeckSystem(deck);
          var rng = new Random(42);
          var reels = system.DrawReels(rng);
          Assert.AreEqual(3, reels.Length);
          foreach (var r in reels) Assert.Contains(r, deck);
      }
  }
  ```

- [ ] **Step 2: 테스트 실행 → 실패 확인**

  Test Runner > Run All  
  Expected: `DeckSystemTests` — 모두 실패

- [ ] **Step 3: DeckSystem 구현**

  `Assets/Scripts/Systems/DeckSystem.cs`:

  ```csharp
  using System;
  namespace SlotDefense
  {
      public class DeckSystem
      {
          private readonly CardData[] _deck;

          public DeckSystem(CardData[] deck) => _deck = deck;

          public CardData[] DrawReels(Random rng) => new[]
          {
              _deck[rng.Next(_deck.Length)],
              _deck[rng.Next(_deck.Length)],
              _deck[rng.Next(_deck.Length)]
          };

          public static SlotResult EvaluateReels(CardData[] reels, out CardData matched)
          {
              if (reels[0] == reels[1] && reels[1] == reels[2])
              {
                  matched = reels[0];
                  return SlotResult.Triple;
              }
              if (reels[0] == reels[1]) { matched = reels[0]; return SlotResult.Double; }
              if (reels[1] == reels[2]) { matched = reels[1]; return SlotResult.Double; }
              if (reels[0] == reels[2]) { matched = reels[0]; return SlotResult.Double; }
              matched = null;
              return SlotResult.AllDifferent;
          }
      }
  }
  ```

- [ ] **Step 4: 테스트 실행 → 통과 확인**

  Expected: `DeckSystemTests` — 4/4 Pass

- [ ] **Step 5: 커밋**

  ```bash
  git add Assets/Scripts/Systems/DeckSystem.cs Assets/Tests/EditMode/DeckSystemTests.cs
  git commit -m "feat: add DeckSystem with reel draw and evaluation"
  ```

---

## Task 6: SlotMachineSystem + Tests

**Files:**
- Create: `Assets/Scripts/Systems/SlotMachineSystem.cs`
- Create: `Assets/Tests/EditMode/SlotMachineSystemTests.cs`

- [ ] **Step 1: 실패하는 테스트 작성**

  `Assets/Tests/EditMode/SlotMachineSystemTests.cs`:

  ```csharp
  using NUnit.Framework;
  using SlotDefense;

  public class SlotMachineSystemTests
  {
      [Test]
      public void AddXP_BelowThreshold_DoesNotGrantSpin()
      {
          var sys = new SlotMachineSystem(xpPerSpin: 100f);
          sys.AddXP(99f);
          Assert.AreEqual(0, sys.SpinCharges);
      }

      [Test]
      public void AddXP_ReachesThreshold_GrantsOneSpin()
      {
          var sys = new SlotMachineSystem(xpPerSpin: 100f);
          sys.AddXP(100f);
          Assert.AreEqual(1, sys.SpinCharges);
      }

      [Test]
      public void AddXP_DoubleThreshold_GrantsTwoSpins()
      {
          var sys = new SlotMachineSystem(xpPerSpin: 100f);
          sys.AddXP(200f);
          Assert.AreEqual(2, sys.SpinCharges);
      }

      [Test]
      public void AddXP_Accumulates_AcrossMultipleCalls()
      {
          var sys = new SlotMachineSystem(xpPerSpin: 100f);
          sys.AddXP(60f);
          sys.AddXP(60f);
          Assert.AreEqual(1, sys.SpinCharges);
      }

      [Test]
      public void TrySpin_WithCharge_ReturnsTrueAndDecrementsCharge()
      {
          var sys = new SlotMachineSystem(xpPerSpin: 100f);
          sys.AddXP(100f);
          Assert.IsTrue(sys.TrySpin());
          Assert.AreEqual(0, sys.SpinCharges);
      }

      [Test]
      public void TrySpin_WithoutCharge_ReturnsFalse()
      {
          var sys = new SlotMachineSystem(xpPerSpin: 100f);
          Assert.IsFalse(sys.TrySpin());
      }
  }
  ```

- [ ] **Step 2: 테스트 실행 → 실패 확인**

  Expected: `SlotMachineSystemTests` — 모두 실패

- [ ] **Step 3: SlotMachineSystem 구현**

  `Assets/Scripts/Systems/SlotMachineSystem.cs`:

  ```csharp
  namespace SlotDefense
  {
      public class SlotMachineSystem
      {
          private readonly float _xpPerSpin;
          private float _xpBuffer;
          private int _spinCharges;

          public int SpinCharges => _spinCharges;
          public float XPBuffer => _xpBuffer;

          public SlotMachineSystem(float xpPerSpin = 100f) => _xpPerSpin = xpPerSpin;

          public void AddXP(float amount)
          {
              _xpBuffer += amount;
              while (_xpBuffer >= _xpPerSpin)
              {
                  _xpBuffer -= _xpPerSpin;
                  _spinCharges++;
              }
          }

          public bool TrySpin()
          {
              if (_spinCharges <= 0) return false;
              _spinCharges--;
              return true;
          }
      }
  }
  ```

- [ ] **Step 4: 테스트 실행 → 통과 확인**

  Expected: `SlotMachineSystemTests` — 6/6 Pass

- [ ] **Step 5: 커밋**

  ```bash
  git add Assets/Scripts/Systems/SlotMachineSystem.cs Assets/Tests/EditMode/SlotMachineSystemTests.cs
  git commit -m "feat: add SlotMachineSystem with XP-to-spin conversion"
  ```

---

## Task 7: BattleManager + Tests

**Files:**
- Create: `Assets/Scripts/Systems/BattleManager.cs`
- Create: `Assets/Tests/EditMode/BattleManagerTests.cs`

- [ ] **Step 1: 실패하는 테스트 작성**

  `Assets/Tests/EditMode/BattleManagerTests.cs`:

  ```csharp
  using NUnit.Framework;
  using SlotDefense;

  public class BattleManagerTests
  {
      [Test]
      public void GetResult_Initially_ReturnsOngoing()
      {
          var bm = new BattleManager(villageHp: 1000f, battleDuration: 180f);
          Assert.AreEqual(BattleResult.Ongoing, bm.GetResult());
      }

      [Test]
      public void DamageEnemyVillage_ReducesTo0_ReturnsPlayerWin()
      {
          var bm = new BattleManager(1000f, 180f);
          bm.DamageEnemyVillage(1000f);
          Assert.AreEqual(BattleResult.PlayerWin, bm.GetResult());
      }

      [Test]
      public void DamagePlayerVillage_ReducesTo0_ReturnsPlayerLose()
      {
          var bm = new BattleManager(1000f, 180f);
          bm.DamagePlayerVillage(1000f);
          Assert.AreEqual(BattleResult.PlayerLose, bm.GetResult());
      }

      [Test]
      public void Tick_ExpiresTimer_PlayerWinWhenHigherHp()
      {
          var bm = new BattleManager(1000f, 1f);
          bm.DamageEnemyVillage(100f); // 상대 HP 900, 내 HP 1000
          bm.Tick(2f);
          Assert.AreEqual(BattleResult.PlayerWin, bm.GetResult());
      }

      [Test]
      public void Tick_ExpiresTimer_DrawWhenEqualHp()
      {
          var bm = new BattleManager(1000f, 1f);
          bm.Tick(2f);
          Assert.AreEqual(BattleResult.Draw, bm.GetResult());
      }

      [Test]
      public void DamagePlayerVillage_CannotGoBelowZero()
      {
          var bm = new BattleManager(1000f, 180f);
          bm.DamagePlayerVillage(9999f);
          Assert.AreEqual(0f, bm.PlayerHp);
      }
  }
  ```

- [ ] **Step 2: 테스트 실행 → 실패 확인**

  Expected: `BattleManagerTests` — 모두 실패

- [ ] **Step 3: BattleManager 구현**

  `Assets/Scripts/Systems/BattleManager.cs`:

  ```csharp
  using UnityEngine;
  namespace SlotDefense
  {
      public class BattleManager
      {
          private float _playerHp;
          private float _enemyHp;
          private float _timeRemaining;

          public float PlayerHp => _playerHp;
          public float EnemyHp => _enemyHp;
          public float TimeRemaining => _timeRemaining;

          public BattleManager(float villageHp, float battleDuration)
          {
              _playerHp = villageHp;
              _enemyHp = villageHp;
              _timeRemaining = battleDuration;
          }

          public void DamagePlayerVillage(float amount) =>
              _playerHp = Mathf.Max(0f, _playerHp - amount);

          public void DamageEnemyVillage(float amount) =>
              _enemyHp = Mathf.Max(0f, _enemyHp - amount);

          public void Tick(float deltaTime) => _timeRemaining -= deltaTime;

          public BattleResult GetResult()
          {
              if (_enemyHp <= 0f) return BattleResult.PlayerWin;
              if (_playerHp <= 0f) return BattleResult.PlayerLose;
              if (_timeRemaining <= 0f)
              {
                  if (_playerHp > _enemyHp) return BattleResult.PlayerWin;
                  if (_enemyHp > _playerHp) return BattleResult.PlayerLose;
                  return BattleResult.Draw;
              }
              return BattleResult.Ongoing;
          }
      }
  }
  ```

- [ ] **Step 4: 테스트 실행 → 통과 확인**

  Expected: `BattleManagerTests` — 6/6 Pass

- [ ] **Step 5: 커밋**

  ```bash
  git add Assets/Scripts/Systems/BattleManager.cs Assets/Tests/EditMode/BattleManagerTests.cs
  git commit -m "feat: add BattleManager with HP, timer, and win/loss logic"
  ```

---

## Task 8: GameEvents + GameManager

**Files:**
- Create: `Assets/Scripts/Core/GameEvents.cs`
- Create: `Assets/Scripts/Core/GameManager.cs`

- [ ] **Step 1: GameEvents.cs 생성**

  ```csharp
  using System;
  using UnityEngine;
  namespace SlotDefense
  {
      public static class GameEvents
      {
          // 몬스터 처치됨 (처치한 진영, 몬스터 config)
          public static event Action<bool, MonsterConfig> OnMonsterKilled;
          // 마을 피해 (플레이어 마을 여부, 피해량)
          public static event Action<bool, float> OnVillageDamaged;
          // 버프 적용 요청
          public static event Action<BuffEffect> OnGlobalBuffApplied;
          // 카드 획득 (카드, 등급)
          public static event Action<CardData, CardTier> OnCardObtained;
          // 전투 종료
          public static event Action<BattleResult> OnBattleEnded;

          public static void MonsterKilled(bool isPlayerArena, MonsterConfig config) =>
              OnMonsterKilled?.Invoke(isPlayerArena, config);
          public static void VillageDamaged(bool isPlayer, float amount) =>
              OnVillageDamaged?.Invoke(isPlayer, amount);
          public static void GlobalBuffApplied(BuffEffect buff) =>
              OnGlobalBuffApplied?.Invoke(buff);
          public static void CardObtained(CardData card, CardTier tier) =>
              OnCardObtained?.Invoke(card, tier);
          public static void BattleEnded(BattleResult result) =>
              OnBattleEnded?.Invoke(result);
      }
  }
  ```

- [ ] **Step 2: GameManager.cs 생성**

  ```csharp
  using UnityEngine;
  using System;
  namespace SlotDefense
  {
      public class GameManager : MonoBehaviour
      {
          [Header("Config")]
          public FixedDeckConfig deckConfig;
          public GlobalBuffConfig buffConfig;
          [SerializeField] private float villageHp = 1000f;
          [SerializeField] private float battleDuration = 180f;
          [SerializeField] private float xpPerSpin = 100f;

          // 순수 C# 시스템 (씬에서 직접 접근 가능하도록 public)
          public BattleManager Battle { get; private set; }
          public SlotMachineSystem SlotMachine { get; private set; }
          public HandSystem Hand { get; private set; }
          public DeckSystem Deck { get; private set; }

          public static GameManager Instance { get; private set; }

          private Random _rng;
          private bool _battleActive;

          private void Awake()
          {
              if (Instance != null) { Destroy(gameObject); return; }
              Instance = this;
              _rng = new Random();
              Battle = new BattleManager(villageHp, battleDuration);
              SlotMachine = new SlotMachineSystem(xpPerSpin);
              Hand = new HandSystem(4);
              Deck = new DeckSystem(deckConfig.cards);
          }

          private void OnEnable()
          {
              GameEvents.OnMonsterKilled += HandleMonsterKilled;
              GameEvents.OnVillageDamaged += HandleVillageDamaged;
          }

          private void OnDisable()
          {
              GameEvents.OnMonsterKilled -= HandleMonsterKilled;
              GameEvents.OnVillageDamaged -= HandleVillageDamaged;
          }

          private void Update()
          {
              if (!_battleActive) return;
              Battle.Tick(Time.deltaTime);
              var result = Battle.GetResult();
              if (result != BattleResult.Ongoing)
              {
                  _battleActive = false;
                  GameEvents.BattleEnded(result);
              }
          }

          public void StartBattle() => _battleActive = true;

          // 플레이어가 슬롯 스핀 버튼 누를 때 HandUI/SlotMachineUI에서 호출
          public void TrySpin()
          {
              if (!SlotMachine.TrySpin()) return;
              var reels = Deck.DrawReels(_rng);
              var slotResult = DeckSystem.EvaluateReels(reels, out var matchedCard);
              if (slotResult == SlotResult.AllDifferent)
              {
                  var buff = buffConfig.possibleBuffs[_rng.Next(buffConfig.possibleBuffs.Length)];
                  GameEvents.GlobalBuffApplied(buff);
              }
              else
              {
                  var tier = slotResult == SlotResult.Triple ? CardTier.Enhanced : CardTier.Normal;
                  if (!Hand.TryAdd(matchedCard))
                  {
                      // 핸드 가득 참 — 버려짐 (향후 UI 피드백 가능)
                  }
                  else
                  {
                      GameEvents.CardObtained(matchedCard, tier);
                  }
              }
          }

          private void HandleMonsterKilled(bool isPlayerArena, MonsterConfig config)
          {
              if (isPlayerArena) SlotMachine.AddXP(config.xpReward);
          }

          private void HandleVillageDamaged(bool isPlayer, float amount)
          {
              if (isPlayer) Battle.DamagePlayerVillage(amount);
              else Battle.DamageEnemyVillage(amount);
          }
      }
  }
  ```

- [ ] **Step 3: 컴파일 확인**

  Unity Console 에러 없음

- [ ] **Step 4: 커밋**

  ```bash
  git add Assets/Scripts/Core/
  git commit -m "feat: add GameEvents bus and GameManager wiring pure systems"
  ```

---

## Task 9: Village 엔티티

**Files:**
- Create: `Assets/Scripts/Entities/Village.cs`
- Prefab: `Assets/Prefabs/Village.prefab` (수동 생성)

- [ ] **Step 1: Village.cs 생성**

  ```csharp
  using UnityEngine;
  namespace SlotDefense
  {
      public class Village : MonoBehaviour
      {
          [SerializeField] private bool isPlayerVillage;
          [SerializeField] private float maxHp = 1000f;
          private float _currentHp;

          public float HpRatio => _currentHp / maxHp;

          private void Awake() => _currentHp = maxHp;

          public void TakeDamage(float amount)
          {
              _currentHp = Mathf.Max(0f, _currentHp - amount);
              GameEvents.VillageDamaged(isPlayerVillage, amount);
          }
      }
  }
  ```

- [ ] **Step 2: Village Prefab 생성**

  Unity Editor:
  1. Hierarchy > 우클릭 > 2D Object > Sprite → 이름: `PlayerVillage`
  2. Sprite: 임시 사각형 (Window > Package Manager > 2D Sprite 임포트 후 사용)
  3. `Village` 컴포넌트 Add → `Is Player Village` 체크
  4. Prefab으로 드래그: `Assets/Prefabs/PlayerVillage.prefab`
  5. 동일하게 `EnemyVillage.prefab` 생성 (Is Player Village 미체크)

- [ ] **Step 3: 커밋**

  ```bash
  git add Assets/Scripts/Entities/Village.cs Assets/Prefabs/
  git commit -m "feat: add Village entity with damage and event emission"
  ```

---

## Task 10: MonsterController 엔티티

**Files:**
- Create: `Assets/Scripts/Entities/MonsterController.cs`
- Prefab: `Assets/Prefabs/Monsters/BasicMonster.prefab`

- [ ] **Step 1: MonsterController.cs 생성**

  ```csharp
  using UnityEngine;
  namespace SlotDefense
  {
      public class MonsterController : MonoBehaviour
      {
          [HideInInspector] public MonsterConfig config;
          [HideInInspector] public bool isInPlayerArena; // true = 플레이어 아레나
          [HideInInspector] public Village targetVillage;

          private float _currentHp;
          private float _attackCooldown;
          private const float AttackInterval = 1f;

          public bool IsDead => _currentHp <= 0f;
          public MonsterConfig Config => config;

          public void Init(MonsterConfig cfg, Village village, bool playerArena)
          {
              config = cfg;
              targetVillage = village;
              isInPlayerArena = playerArena;
              _currentHp = cfg.hp;
          }

          private void Update()
          {
              if (IsDead || targetVillage == null) return;
              MoveTowardVillage();
              TryAttackVillage();
          }

          private void MoveTowardVillage()
          {
              var dir = (targetVillage.transform.position - transform.position).normalized;
              transform.position += dir * config.moveSpeed * Time.deltaTime;
          }

          private void TryAttackVillage()
          {
              _attackCooldown -= Time.deltaTime;
              var dist = Vector2.Distance(transform.position, targetVillage.transform.position);
              if (dist > 0.5f || _attackCooldown > 0f) return;
              _attackCooldown = AttackInterval;
              targetVillage.TakeDamage(config.damage);
          }

          public void TakeDamage(float amount)
          {
              _currentHp -= amount;
              if (_currentHp <= 0f) Die();
          }

          private void Die()
          {
              GameEvents.MonsterKilled(isInPlayerArena, config);
              Destroy(gameObject);
          }
      }
  }
  ```

- [ ] **Step 2: BasicMonster Prefab 생성**

  Hierarchy > Create Empty → 이름 `BasicMonster`
  - SpriteRenderer 추가 (임시 원형 스프라이트)
  - `MonsterController` 컴포넌트 추가
  - `Assets/Prefabs/Monsters/BasicMonster.prefab`으로 저장

- [ ] **Step 3: BasicMonster ScriptableObject 생성**

  Project > Assets/ScriptableObjects > 우클릭 > SlotDefense > MonsterConfig  
  이름: `BasicMonsterConfig`, 값: hp=100, damage=30, moveSpeed=1, xpReward=50

- [ ] **Step 4: 커밋**

  ```bash
  git add Assets/Scripts/Entities/MonsterController.cs Assets/Prefabs/Monsters/ Assets/ScriptableObjects/
  git commit -m "feat: add MonsterController with movement, attack, and death events"
  ```

---

## Task 11: UnitController 엔티티

**Files:**
- Create: `Assets/Scripts/Entities/UnitController.cs`
- Prefab: `Assets/Prefabs/Units/BasicUnit.prefab`

- [ ] **Step 1: UnitController.cs 생성**

  ```csharp
  using UnityEngine;
  namespace SlotDefense
  {
      public class UnitController : MonoBehaviour
      {
          private UnitStats _stats;
          private float _currentHp;
          private float _attackCooldown;
          private MonsterController _target;

          public void Init(UnitStats stats)
          {
              _stats = stats;
              _currentHp = stats.hp;
          }

          private void Update()
          {
              if (_currentHp <= 0f) return;
              AcquireTarget();
              if (_target == null) return;
              ChaseTarget();
              TryAttack();
          }

          private void AcquireTarget()
          {
              if (_target != null && !_target.IsDead) return;
              _target = null;
              float nearest = float.MaxValue;
              foreach (var m in FindObjectsOfType<MonsterController>())
              {
                  var dist = Vector2.Distance(transform.position, m.transform.position);
                  if (dist < nearest) { nearest = dist; _target = m; }
              }
          }

          private void ChaseTarget()
          {
              var dist = Vector2.Distance(transform.position, _target.transform.position);
              if (dist <= _stats.attackRange) return;
              var dir = (_target.transform.position - transform.position).normalized;
              transform.position += dir * _stats.moveSpeed * Time.deltaTime;
          }

          private void TryAttack()
          {
              _attackCooldown -= Time.deltaTime;
              var dist = Vector2.Distance(transform.position, _target.transform.position);
              if (dist > _stats.attackRange || _attackCooldown > 0f) return;
              _attackCooldown = 1f / _stats.attackRate;
              _target.TakeDamage(_stats.damage);
          }

          public void TakeDamage(float amount)
          {
              _currentHp -= amount;
              if (_currentHp <= 0f) Destroy(gameObject);
          }
      }
  }
  ```

  > `FindObjectsOfType` 는 MVP 수준에서 충분합니다. 유닛 수가 많아지면 ArenaSystem이 리스트를 관리하는 방식으로 최적화합니다.

- [ ] **Step 2: BasicUnit Prefab 생성**

  Hierarchy > Create Empty → `BasicUnit`
  - SpriteRenderer (임시 삼각형 스프라이트)
  - `UnitController` 추가
  - `Assets/Prefabs/Units/BasicUnit.prefab` 저장

- [ ] **Step 3: 커밋**

  ```bash
  git add Assets/Scripts/Entities/UnitController.cs Assets/Prefabs/Units/
  git commit -m "feat: add UnitController with target acquisition and attack"
  ```

---

## Task 12: ArenaSystem — 웨이브 소환 + 유닛 배치

**Files:**
- Create: `Assets/Scripts/Systems/ArenaSystem.cs`

- [ ] **Step 1: ArenaSystem.cs 생성**

  ```csharp
  using UnityEngine;
  using System.Collections;
  namespace SlotDefense
  {
      public class ArenaSystem : MonoBehaviour
      {
          [Header("Player Arena")]
          public Transform playerSpawnPoint;   // 몬스터 소환 위치 (플레이어 진영)
          public Village playerVillage;
          public GameObject monsterPrefab;

          [Header("Enemy Arena")]
          public Transform enemySpawnPoint;    // 몬스터 소환 위치 (AI 진영)
          public Village enemyVillage;

          [Header("Config")]
          public MonsterConfig monsterConfig;
          [SerializeField] private float waveInterval = 5f;

          // 유닛 배치용 — HandUI가 설정
          private int _selectedHandSlot = -1;
          public GameObject unitPrefab;

          private void Start() => StartCoroutine(WaveLoop());

          private IEnumerator WaveLoop()
          {
              while (true)
              {
                  yield return new WaitForSeconds(waveInterval);
                  SpawnMonsterInArena(isPlayerArena: true);
                  SpawnMonsterInArena(isPlayerArena: false);
              }
          }

          public void SpawnMonsterInArena(bool isPlayerArena, MonsterConfig overrideConfig = null)
          {
              var cfg = overrideConfig ?? monsterConfig;
              var spawnPos = isPlayerArena ? playerSpawnPoint.position : enemySpawnPoint.position;
              var village = isPlayerArena ? playerVillage : enemyVillage;
              var go = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
              go.GetComponent<MonsterController>().Init(cfg, village, isPlayerArena);
          }

          // HandUI에서 슬롯 선택 시 호출
          public void SelectHandSlot(int slotIndex) => _selectedHandSlot = slotIndex;

          // 플레이어가 아레나 클릭 시 유닛 소환 (Camera.main.ScreenToWorldPoint 사용)
          private void Update()
          {
              if (_selectedHandSlot < 0) return;
              if (!Input.GetMouseButtonDown(0)) return;

              var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
              worldPos.z = 0f;

              var card = GameManager.Instance.Hand.Use(_selectedHandSlot);
              _selectedHandSlot = -1;

              if (card == null || card.cardType != CardType.Unit) return;

              var go = Instantiate(unitPrefab, worldPos, Quaternion.identity);
              go.GetComponent<UnitController>().Init(card.unitStats);
          }
      }
  }
  ```

- [ ] **Step 2: 커밋**

  ```bash
  git add Assets/Scripts/Systems/ArenaSystem.cs
  git commit -m "feat: add ArenaSystem with wave spawning and unit placement"
  ```

---

## Task 13: TransferSystem — 처치 몬스터 전송

**Files:**
- Create: `Assets/Scripts/Systems/TransferSystem.cs`

- [ ] **Step 1: TransferSystem.cs 생성**

  ```csharp
  using UnityEngine;
  using System.Collections.Generic;
  namespace SlotDefense
  {
      public class TransferSystem : MonoBehaviour
      {
          public ArenaSystem arenaSystem;
          [SerializeField] private float transferDelay = 1.5f;

          private Queue<(MonsterConfig config, bool toPlayerArena)> _queue = new();
          private float _timer;

          private void OnEnable() => GameEvents.OnMonsterKilled += EnqueueTransfer;
          private void OnDisable() => GameEvents.OnMonsterKilled -= EnqueueTransfer;

          private void EnqueueTransfer(bool killedInPlayerArena, MonsterConfig config)
          {
              // 플레이어 진영에서 처치 → 상대(AI) 진영으로 전송, 반대도 동일
              _queue.Enqueue((config, toPlayerArena: !killedInPlayerArena));
          }

          private void Update()
          {
              if (_queue.Count == 0) return;
              _timer -= Time.deltaTime;
              if (_timer > 0f) return;
              _timer = transferDelay;
              var (config, toPlayer) = _queue.Dequeue();
              arenaSystem.SpawnMonsterInArena(toPlayer, config);
          }
      }
  }
  ```

- [ ] **Step 2: 커밋**

  ```bash
  git add Assets/Scripts/Systems/TransferSystem.cs
  git commit -m "feat: add TransferSystem routing killed monsters to opponent arena"
  ```

---

## Task 14: IOpponent + AIOpponent

**Files:**
- Create: `Assets/Scripts/AI/IOpponent.cs`
- Create: `Assets/Scripts/AI/AIOpponent.cs`

- [ ] **Step 1: IOpponent.cs 생성**

  ```csharp
  namespace SlotDefense
  {
      public interface IOpponent
      {
          void OnUpdate(float deltaTime);
          void ReceiveTransferredMonster(MonsterConfig monster);
      }
  }
  ```

- [ ] **Step 2: AIOpponent.cs 생성**

  ```csharp
  using UnityEngine;
  using System;
  namespace SlotDefense
  {
      public class AIOpponent : MonoBehaviour, IOpponent
      {
          public FixedDeckConfig deckConfig;
          public GlobalBuffConfig buffConfig;
          public ArenaSystem arenaSystem;
          public GameObject unitPrefab;

          [Header("AI Difficulty")]
          [SerializeField] private float spinInterval = 4f;   // Normal 기준
          [SerializeField] private float placeInterval = 6f;

          private HandSystem _hand;
          private DeckSystem _deck;
          private SlotMachineSystem _slotMachine;
          private float _spinTimer;
          private float _placeTimer;
          private Random _rng;

          private void Awake()
          {
              _hand = new HandSystem(4);
              _deck = new DeckSystem(deckConfig.cards);
              _slotMachine = new SlotMachineSystem(xpPerSpin: 80f); // AI는 약간 더 빠르게
              _rng = new Random();
          }

          private void OnEnable() => GameEvents.OnMonsterKilled += OnMonsterKilled;
          private void OnDisable() => GameEvents.OnMonsterKilled -= OnMonsterKilled;

          private void Update() => OnUpdate(Time.deltaTime);

          public void OnUpdate(float deltaTime)
          {
              _spinTimer -= deltaTime;
              _placeTimer -= deltaTime;

              if (_spinTimer <= 0f && _slotMachine.TrySpin())
              {
                  _spinTimer = spinInterval;
                  ExecuteSpin();
              }

              if (_placeTimer <= 0f)
              {
                  _placeTimer = placeInterval;
                  PlaceRandomUnit();
              }
          }

          public void ReceiveTransferredMonster(MonsterConfig monster)
          {
              // AI 진영 몬스터는 ArenaSystem이 직접 처리하므로 여기서는 XP 부여
              _slotMachine.AddXP(monster.xpReward);
          }

          private void OnMonsterKilled(bool isPlayerArena, MonsterConfig config)
          {
              if (!isPlayerArena) _slotMachine.AddXP(config.xpReward);
          }

          private void ExecuteSpin()
          {
              var reels = _deck.DrawReels(_rng);
              var result = DeckSystem.EvaluateReels(reels, out var matched);
              if (result != SlotResult.AllDifferent && matched != null)
                  _hand.TryAdd(matched);
          }

          private void PlaceRandomUnit()
          {
              for (int i = 0; i < 4; i++)
              {
                  var card = _hand.GetSlot(i);
                  if (card == null || card.cardType != CardType.Unit) continue;
                  _hand.Use(i);
                  var spawnPos = arenaSystem.enemySpawnPoint.position
                      + new UnityEngine.Vector3(_rng.Next(-3, 3), _rng.Next(-2, 2), 0);
                  var go = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
                  go.GetComponent<UnitController>().Init(card.unitStats);
                  break;
              }
          }
      }
  }
  ```

- [ ] **Step 3: 커밋**

  ```bash
  git add Assets/Scripts/AI/
  git commit -m "feat: add IOpponent interface and AIOpponent with auto-spin and placement"
  ```

---

## Task 15: UI — ArenaHUD

**Files:**
- Create: `Assets/Scripts/UI/ArenaHUD.cs`

- [ ] **Step 1: ArenaHUD.cs 생성**

  ```csharp
  using UnityEngine;
  using UnityEngine.UI;
  using TMPro;
  namespace SlotDefense
  {
      public class ArenaHUD : MonoBehaviour
      {
          [Header("Village HP")]
          public Slider playerHpSlider;
          public Slider enemyHpSlider;

          [Header("Timer")]
          public TextMeshProUGUI timerText;

          [Header("Spin")]
          public TextMeshProUGUI spinChargesText;

          private void Update()
          {
              if (GameManager.Instance == null) return;
              var b = GameManager.Instance.Battle;
              playerHpSlider.value = b.PlayerHp / 1000f;
              enemyHpSlider.value = b.EnemyHp / 1000f;

              var t = Mathf.Max(0f, b.TimeRemaining);
              timerText.text = $"{(int)(t / 60f)}:{(int)(t % 60f):00}";

              spinChargesText.text = $"x{GameManager.Instance.SlotMachine.SpinCharges}";
          }
      }
  }
  ```

- [ ] **Step 2: Canvas + HUD 오브젝트 설정**

  Hierarchy > UI > Canvas (Screen Space - Overlay)  
  Canvas 하위에:
  - `PlayerHpSlider` (Slider UI)
  - `EnemyHpSlider` (Slider UI)
  - `TimerText` (TextMeshProUGUI)
  - `SpinChargesText` (TextMeshProUGUI)
  
  `ArenaHUD` 컴포넌트를 Canvas에 추가하고 각 필드 연결

- [ ] **Step 3: 커밋**

  ```bash
  git add Assets/Scripts/UI/ArenaHUD.cs
  git commit -m "feat: add ArenaHUD with HP sliders, timer, and spin count display"
  ```

---

## Task 16: UI — HandUI

**Files:**
- Create: `Assets/Scripts/UI/HandUI.cs`

- [ ] **Step 1: HandUI.cs 생성**

  ```csharp
  using UnityEngine;
  using UnityEngine.UI;
  using TMPro;
  namespace SlotDefense
  {
      public class HandUI : MonoBehaviour
      {
          public Button[] cardButtons;    // 4개
          public Image[] cardIcons;
          public TextMeshProUGUI[] cardNames;
          public ArenaSystem arenaSystem;

          private int _selectedSlot = -1;

          private void OnEnable() => GameEvents.OnCardObtained += RefreshHand;
          private void OnDisable() => GameEvents.OnCardObtained -= RefreshHand;

          private void Start()
          {
              for (int i = 0; i < cardButtons.Length; i++)
              {
                  int index = i;
                  cardButtons[i].onClick.AddListener(() => SelectSlot(index));
              }
          }

          private void Update() => RefreshDisplay();

          private void RefreshDisplay()
          {
              if (GameManager.Instance == null) return;
              for (int i = 0; i < cardButtons.Length; i++)
              {
                  var card = GameManager.Instance.Hand.GetSlot(i);
                  cardButtons[i].interactable = card != null;
                  cardIcons[i].sprite = card?.icon;
                  cardNames[i].text = card?.cardName ?? "";
                  cardIcons[i].gameObject.SetActive(card != null);
              }
          }

          private void SelectSlot(int index)
          {
              _selectedSlot = index;
              arenaSystem.SelectHandSlot(index);
          }

          private void RefreshHand(CardData card, CardTier tier) => RefreshDisplay();
      }
  }
  ```

- [ ] **Step 2: Canvas에 HandUI 패널 추가**

  Canvas 하위 > Create Empty → `HandPanel`
  - 4개의 Button (각각 Image + TextMeshProUGUI 자식)
  - `HandUI` 컴포넌트 → 각 필드 연결
  - `ArenaSystem` 레퍼런스 연결

- [ ] **Step 3: 커밋**

  ```bash
  git add Assets/Scripts/UI/HandUI.cs
  git commit -m "feat: add HandUI with slot buttons and card display"
  ```

---

## Task 17: UI — SlotMachineUI

**Files:**
- Create: `Assets/Scripts/UI/SlotMachineUI.cs`

- [ ] **Step 1: SlotMachineUI.cs 생성**

  ```csharp
  using UnityEngine;
  using UnityEngine.UI;
  using TMPro;
  namespace SlotDefense
  {
      public class SlotMachineUI : MonoBehaviour
      {
          public Image[] reelImages;         // 3개 릴 아이콘
          public TextMeshProUGUI[] reelNames; // 3개 릴 카드명
          public Button spinButton;
          public TextMeshProUGUI resultText;

          private void Start() => spinButton.onClick.AddListener(OnSpinClicked);

          private void Update()
          {
              if (GameManager.Instance == null) return;
              spinButton.interactable = GameManager.Instance.SlotMachine.SpinCharges > 0;
          }

          private void OnSpinClicked()
          {
              GameManager.Instance.TrySpin();
              // TrySpin 내부에서 릴 뽑기가 일어남 — 결과는 GameEvents로 수신
              // 간단한 MVP: 릴 UI는 결과 후 즉시 업데이트 (애니메이션 없음)
              ShowLastResult();
          }

          private void ShowLastResult()
          {
              // MVP에서는 결과 텍스트만 표시
              resultText.text = "SPIN!";
              Invoke(nameof(ClearResult), 1.5f);
          }

          private void ClearResult() => resultText.text = "";
      }
  }
  ```

- [ ] **Step 2: Canvas에 SlotMachineUI 패널 추가**

  Canvas 하위 > `SlotPanel`
  - 3개 Image (릴 아이콘)
  - 3개 TextMeshProUGUI (릴 카드명)
  - Spin Button
  - Result TextMeshProUGUI
  - `SlotMachineUI` 컴포넌트 → 필드 연결

- [ ] **Step 3: 커밋**

  ```bash
  git add Assets/Scripts/UI/SlotMachineUI.cs
  git commit -m "feat: add SlotMachineUI with spin button and result display"
  ```

---

## Task 18: UI — ResultUI

**Files:**
- Create: `Assets/Scripts/UI/ResultUI.cs`

- [ ] **Step 1: ResultUI.cs 생성**

  ```csharp
  using UnityEngine;
  using UnityEngine.UI;
  using UnityEngine.SceneManagement;
  using TMPro;
  namespace SlotDefense
  {
      public class ResultUI : MonoBehaviour
      {
          public GameObject panel;
          public TextMeshProUGUI resultText;
          public Button retryButton;

          private void OnEnable() => GameEvents.OnBattleEnded += ShowResult;
          private void OnDisable() => GameEvents.OnBattleEnded -= ShowResult;

          private void Start()
          {
              panel.SetActive(false);
              retryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
          }

          private void ShowResult(BattleResult result)
          {
              panel.SetActive(true);
              resultText.text = result switch
              {
                  BattleResult.PlayerWin  => "VICTORY!",
                  BattleResult.PlayerLose => "DEFEAT",
                  BattleResult.Draw       => "DRAW",
                  _                       => ""
              };
          }
      }
  }
  ```

- [ ] **Step 2: Canvas에 ResultUI 패널 추가**

  Canvas 하위 > `ResultPanel` (처음에 비활성화)
  - TextMeshProUGUI (resultText)
  - Button (retryButton)
  - `ResultUI` 컴포넌트 → 필드 연결

- [ ] **Step 3: 커밋**

  ```bash
  git add Assets/Scripts/UI/ResultUI.cs
  git commit -m "feat: add ResultUI showing win/lose/draw with retry"
  ```

---

## Task 19: 씬 조립 + ScriptableObject 에셋 생성

**Files:**
- Scene: `Assets/Scenes/Battle.unity`
- Assets: `Assets/ScriptableObjects/Decks/DefaultDeck.asset`
- Assets: `Assets/ScriptableObjects/GlobalBuffs.asset`

- [ ] **Step 1: Battle 씬 세팅**

  File > New Scene → 이름 `Battle`으로 저장 (`Assets/Scenes/Battle.unity`)

- [ ] **Step 2: GameManager 오브젝트 배치**

  Hierarchy > Create Empty → `GameManager`
  - `GameManager` 컴포넌트 추가
  - Deck Config, Buff Config 필드: 아래에서 생성할 SO 연결 (Step 4 후 연결)

- [ ] **Step 3: 씬 오브젝트 배치**

  Hierarchy에 다음 배치:
  ```
  GameManager         (GameManager.cs)
  TransferSystem      (TransferSystem.cs)
  ArenaSystem         (ArenaSystem.cs)
  AIOpponent          (AIOpponent.cs)
  PlayerVillage       (Village.cs, isPlayerVillage=true, 좌측 배치)
  EnemyVillage        (Village.cs, isPlayerVillage=false, 우측 배치)
  Canvas
    ArenaHUD
    HandPanel
    SlotPanel
    ResultPanel
  ```

- [ ] **Step 4: ScriptableObject 에셋 생성**

  Project > Assets/ScriptableObjects > 우클릭:
  
  **DefaultDeck.asset** (FixedDeckConfig):
  - 12장의 CardData SO를 먼저 생성 (예시: Warrior×4, Archer×4, Mage×4)
  - FixedDeckConfig의 cards 배열에 12장 연결

  **GlobalBuffs.asset** (GlobalBuffConfig):
  - possibleBuffs에 2-3개 BuffEffect 추가:
    - `{ attackMultiplier: 1.1, speedMultiplier: 1.0, duration: 0 }` (공격력 +10%)
    - `{ attackMultiplier: 1.0, speedMultiplier: 1.2, duration: 0 }` (이속 +20%)

- [ ] **Step 5: 컴포넌트 레퍼런스 연결**

  Inspector에서:
  - `ArenaSystem`: playerSpawnPoint, enemySpawnPoint, playerVillage, enemyVillage, monsterPrefab, unitPrefab 연결
  - `TransferSystem`: arenaSystem 연결
  - `AIOpponent`: deckConfig, buffConfig, arenaSystem, unitPrefab 연결
  - `HandUI`: arenaSystem, cardButtons, cardIcons, cardNames 연결
  - 모든 UI 컴포넌트 레퍼런스 연결

- [ ] **Step 6: GameManager.StartBattle() 호출 설정**

  GameManager의 `Start()` 메서드에 아래 한 줄 추가:
  ```csharp
  private void Start() => StartBattle();
  ```

- [ ] **Step 7: 커밋**

  ```bash
  git add Assets/Scenes/ Assets/ScriptableObjects/
  git commit -m "feat: assemble Battle scene with all systems and ScriptableObjects"
  ```

---

## Task 20: 첫 플레이어블 빌드 & 검증

- [ ] **Step 1: 전체 테스트 실행**

  Window > General > Test Runner > EditMode > Run All  
  Expected: 모든 테스트 통과 (HandSystem 5, DeckSystem 4, SlotMachine 6, BattleManager 6 = 21개)

- [ ] **Step 2: Play Mode 기본 동작 확인**

  Unity Editor에서 Play 버튼:
  - 몬스터가 중앙에서 좌/우 마을을 향해 이동하는지 확인
  - 타이머가 감소하는지 확인
  - 슬롯 스핀 버튼이 비활성화 상태로 시작하는지 확인

- [ ] **Step 3: 몬스터 처치 → XP → 스핀 플로우 확인**

  Play Mode에서:
  - 유닛을 배치해 몬스터 처치
  - SpinCharges 카운터가 증가하는지 확인
  - Spin 버튼 클릭 → 핸드에 카드 추가되는지 확인

- [ ] **Step 4: WebGL 빌드 테스트**

  File > Build Settings → WebGL 선택 → Switch Platform  
  Build And Run  
  브라우저에서 기본 동작 확인 (입력, 렌더링)

- [ ] **Step 5: 최종 커밋**

  ```bash
  git add .
  git commit -m "feat: first playable MVP build — slot defense game"
  ```

---

## 구현 완료 체크리스트

| 시스템 | 테스트 | 씬 배치 |
|--------|--------|---------|
| HandSystem | Edit Mode 5개 | GameManager 내부 |
| DeckSystem | Edit Mode 4개 | GameManager 내부 |
| SlotMachineSystem | Edit Mode 6개 | GameManager 내부 |
| BattleManager | Edit Mode 6개 | GameManager 내부 |
| Village | - | ✅ |
| MonsterController | - | Prefab |
| UnitController | - | Prefab |
| ArenaSystem | - | ✅ |
| TransferSystem | - | ✅ |
| AIOpponent | - | ✅ |
| ArenaHUD | - | ✅ |
| HandUI | - | ✅ |
| SlotMachineUI | - | ✅ |
| ResultUI | - | ✅ |
