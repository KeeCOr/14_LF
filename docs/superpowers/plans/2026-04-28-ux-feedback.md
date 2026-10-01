# UX 피드백 구현 플랜

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 외부 에셋 없이 코드만으로 릴 스핀 애니메이션, Triple 강조 연출, 버프 발동 금색 플래시 이펙트를 구현한다.

**Architecture:** 공유 `ScreenFlash` 싱글턴이 화면 오버레이를 관리하고, `GameManager`에 2단계 스핀(BeginSpin/CommitSpin)을 추가해 `SlotMachineUI`가 애니메이션 완료 후 결과를 확정한다. `HandUI`는 `CardTier.Enhanced` 이벤트를 받아 카드 슬롯을 깜빡인다.

**Tech Stack:** Unity 2022 URP 2D, C# 9, UnityEngine.UI (Text/Image/Button), 코루틴

---

## 파일 맵

| 파일 | 변경 유형 | 역할 |
|---|---|---|
| `Assets/Scripts/UI/ScreenFlash.cs` | **신규** | 전체화면 금색 플래시 싱글턴 |
| `Assets/Scripts/Core/GameManager.cs` | **수정** | `TryBeginSpin()` / `PeekSpin()` / `CommitSpin()` 추가, `TrySpin()` 위임 |
| `Assets/Scripts/UI/SlotMachineUI.cs` | **수정** | 릴 애니메이션 코루틴, Triple 강조 연출 |
| `Assets/Scripts/UI/HandUI.cs` | **수정** | Enhanced 카드 슬롯 금색 깜빡임 |
| `Assets/Scripts/Core/TestSceneBootstrapper.cs` | **수정** | `BuildUI()`에서 ScreenFlash 오브젝트 생성 |

---

## Task 1: ScreenFlash 컴포넌트 신규 생성

**Files:**
- Create: `Assets/Scripts/UI/ScreenFlash.cs`

- [ ] **Step 1: 파일 생성**

`Assets/Scripts/UI/ScreenFlash.cs` 를 아래 내용으로 생성한다.

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SlotDefense
{
    public class ScreenFlash : MonoBehaviour
    {
        public static ScreenFlash Instance { get; private set; }

        private Image _overlay;

        private void Awake()
        {
            Instance = this;
            _overlay = GetComponent<Image>();
            _overlay.raycastTarget = false;
            SetAlpha(0f);
        }

        public void Play(Color color, float maxAlpha, float fadeIn, float fadeOut)
        {
            StopAllCoroutines();
            StartCoroutine(FlashCoroutine(color, maxAlpha, fadeIn, fadeOut));
        }

        private IEnumerator FlashCoroutine(Color color, float maxAlpha, float fadeIn, float fadeOut)
        {
            color.a = 0f;
            _overlay.color = color;
            float t = 0f;
            while (t < fadeIn)
            {
                t += Time.deltaTime;
                SetAlpha(Mathf.Lerp(0f, maxAlpha, t / fadeIn));
                yield return null;
            }
            SetAlpha(maxAlpha);
            t = 0f;
            while (t < fadeOut)
            {
                t += Time.deltaTime;
                SetAlpha(Mathf.Lerp(maxAlpha, 0f, t / fadeOut));
                yield return null;
            }
            SetAlpha(0f);
        }

        private void SetAlpha(float a)
        {
            var c = _overlay.color;
            c.a = a;
            _overlay.color = c;
        }
    }
}
```

- [ ] **Step 2: 커밋**

```bash
git add Assets/Scripts/UI/ScreenFlash.cs
git commit -m "feat: add ScreenFlash singleton for screen overlay effects"
```

---

## Task 2: GameManager — 2단계 스핀 분리

현재 `TrySpin()`이 릴 드로우·이벤트 발사·손패 추가를 한 번에 처리한다.
애니메이션이 끝난 후 결과를 확정하려면 릴 계산과 이벤트 발사를 분리해야 한다.

**Files:**
- Modify: `Assets/Scripts/Core/GameManager.cs`

- [ ] **Step 1: private 필드 추가**

`GameManager` 클래스의 기존 `private System.Random _rng;` 아래에 4개 필드를 추가한다.

```csharp
private CardData[]  _pendingReels;
private SlotResult  _pendingSlotResult;
private CardData    _pendingCard;
private CardTier    _pendingTier;
```

- [ ] **Step 2: TryBeginSpin() 메서드 추가**

`TrySpin()` 메서드 바로 위에 아래 메서드를 추가한다.

```csharp
public bool TryBeginSpin()
{
    if (!SlotMachine.TrySpin()) return false;
    _pendingReels = Deck.DrawReels(_rng);
    _pendingSlotResult = DeckSystem.EvaluateReels(_pendingReels, out var matchedCard);
    if (_pendingSlotResult == SlotResult.AllDifferent)
    {
        var buffEffect = buffConfig.possibleBuffs[_rng.Next(buffConfig.possibleBuffs.Length)];
        var buffCard = ScriptableObject.CreateInstance<CardData>();
        buffCard.cardName      = "전투 버프";
        buffCard.cardType      = CardType.Buff;
        buffCard.buffEffect    = buffEffect;
        buffCard.placementCost = 0;
        _pendingCard = buffCard;
        _pendingTier = CardTier.Normal;
    }
    else
    {
        _pendingCard = matchedCard;
        _pendingTier = _pendingSlotResult == SlotResult.Triple ? CardTier.Enhanced : CardTier.Normal;
    }
    return true;
}

public (CardData[] reels, SlotResult result) PeekSpin() =>
    (_pendingReels, _pendingSlotResult);

public void CommitSpin()
{
    if (_pendingReels == null) return;
    GameEvents.SpinCompleted(_pendingReels, _pendingSlotResult);
    if (Hand.TryAdd(_pendingCard))
        GameEvents.CardObtained(_pendingCard, _pendingTier);
    _pendingReels = null;
}
```

- [ ] **Step 3: TrySpin()을 위임으로 교체**

기존 `TrySpin()` 전체를 아래로 교체한다.

```csharp
public void TrySpin()
{
    if (!TryBeginSpin()) return;
    CommitSpin();
}
```

- [ ] **Step 4: 커밋**

```bash
git add Assets/Scripts/Core/GameManager.cs
git commit -m "refactor: split TrySpin into TryBeginSpin/CommitSpin for deferred result commit"
```

---

## Task 3: SlotMachineUI — 릴 애니메이션 + Triple 연출

**Files:**
- Modify: `Assets/Scripts/UI/SlotMachineUI.cs`

- [ ] **Step 1: 파일 전체 교체**

`Assets/Scripts/UI/SlotMachineUI.cs` 를 아래 내용으로 완전히 교체한다.

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SlotDefense
{
    public class SlotMachineUI : MonoBehaviour
    {
        public Image[] reelImages;
        public Text[]  reelNames;
        public Button  spinButton;
        public Text    resultText;

        private static readonly string[] SpinPool = { "검사", "궁수", "기사", "마법사" };
        private static readonly Color    GoldColor   = new Color(1f, 0.85f, 0.1f);
        private static readonly Color    WhiteColor  = Color.white;

        private bool _animating;

        private void Start() => spinButton.onClick.AddListener(OnSpinClicked);

        private void OnEnable()  => GameEvents.OnGlobalBuffApplied += OnBuffApplied;
        private void OnDisable() => GameEvents.OnGlobalBuffApplied -= OnBuffApplied;

        private void Update()
        {
            if (GameManager.Instance == null) return;
            int charges = GameManager.Instance.SlotMachine.SpinCharges;
            spinButton.interactable = charges > 0 && !_animating;
            var label = spinButton.GetComponentInChildren<Text>();
            if (label != null) label.text = $"SPIN (x{charges})";
        }

        private void OnSpinClicked()
        {
            if (GameManager.Instance == null || _animating) return;
            if (!GameManager.Instance.TryBeginSpin()) return;
            StartCoroutine(SpinAnimation());
        }

        private IEnumerator SpinAnimation()
        {
            _animating = true;
            spinButton.interactable = false;

            var (reels, result) = GameManager.Instance.PeekSpin();

            resultText.text     = "";
            resultText.color    = WhiteColor;
            resultText.fontSize = 24;

            for (int i = 0; i < reelNames.Length; i++)
                if (reelNames[i] != null) reelNames[i].text = "?";

            // 릴별 정지 시각 (초)
            float[] stopAt  = { 0.8f, 1.3f, 1.8f };
            bool[]  stopped = { false, false, false };
            float elapsed   = 0f;
            float lastTick  = 0f;
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
                            if (reelNames[i] != null)
                                reelNames[i].text = reels[i]?.cardName ?? "?";
                        }
                        else
                        {
                            if (reelNames[i] != null)
                                reelNames[i].text = SpinPool[Random.Range(0, SpinPool.Length)];
                        }
                    }
                }
                yield return null;
            }

            // 결과 확정 (이벤트 발사 + 손패 추가)
            GameManager.Instance.CommitSpin();

            // 연출
            if (result == SlotResult.Triple)
                ShowTriple();
            else
                ShowNormal(result);

            _animating = false;
        }

        private void ShowTriple()
        {
            resultText.text     = "★ TRIPLE! ★";
            resultText.color    = GoldColor;
            resultText.fontSize = 32;
            ScreenFlash.Instance?.Play(new Color(1f, 0.8f, 0f), 0.55f, 0.1f, 0.45f);
            CancelInvoke(nameof(ClearResult));
            Invoke(nameof(ClearResult), 3f);
        }

        private void ShowNormal(SlotResult result)
        {
            resultText.text     = result == SlotResult.AllDifferent ? "버프 카드 획득!" : "카드 획득!";
            resultText.color    = WhiteColor;
            resultText.fontSize = 24;
            CancelInvoke(nameof(ClearResult));
            Invoke(nameof(ClearResult), 2f);
        }

        private void OnBuffApplied(BuffEffect _) =>
            ScreenFlash.Instance?.Play(new Color(1f, 0.75f, 0f), 0.35f, 0.1f, 0.3f);

        private void ClearResult()
        {
            resultText.text     = "";
            resultText.color    = WhiteColor;
            resultText.fontSize = 24;
        }
    }
}
```

- [ ] **Step 2: 커밋**

```bash
git add Assets/Scripts/UI/SlotMachineUI.cs
git commit -m "feat: reel spin animation with sequential stop and Triple screen flash"
```

---

## Task 4: HandUI — Enhanced 카드 슬롯 금색 깜빡임

**Files:**
- Modify: `Assets/Scripts/UI/HandUI.cs`

- [ ] **Step 1: static 색상 상수 추가**

`HandUI` 클래스 내 기존 `private static readonly Color ColorBuff` 줄 아래에 추가한다.

```csharp
private static readonly Color ColorEnhanced = new Color(1f, 0.85f, 0.1f, 0.9f);
```

- [ ] **Step 2: RefreshHand 메서드 교체**

기존 `private void RefreshHand(CardData card, CardTier tier) => RefreshDisplay();` 를 아래로 교체한다.

```csharp
private void RefreshHand(CardData card, CardTier tier)
{
    RefreshDisplay();
    if (tier != CardTier.Enhanced) return;
    for (int i = 0; i < cardButtons.Length; i++)
    {
        if (GameManager.Instance?.Hand.GetSlot(i) == card)
        {
            StartCoroutine(BlinkSlot(i));
            break;
        }
    }
}

private IEnumerator BlinkSlot(int index)
{
    for (int i = 0; i < 3; i++)
    {
        if (cardButtons[index].targetGraphic is Image bg)
            bg.color = ColorEnhanced;
        yield return new WaitForSeconds(0.15f);
        RefreshDisplay();
        yield return new WaitForSeconds(0.15f);
    }
}
```

- [ ] **Step 3: 커밋**

```bash
git add Assets/Scripts/UI/HandUI.cs
git commit -m "feat: blink gold on hand slot when Triple Enhanced card is obtained"
```

---

## Task 5: TestSceneBootstrapper — ScreenFlash 오브젝트 생성

**Files:**
- Modify: `Assets/Scripts/Core/TestSceneBootstrapper.cs`

- [ ] **Step 1: BuildUI() 끝에 ScreenFlash 오브젝트 추가**

`BuildUI()` 메서드 내 `// ResultUI` 블록이 끝나는 마지막 줄(`resultUI.retryButton = ...`) 바로 아래에 추가한다.

```csharp
// ScreenFlash overlay — 반드시 Canvas의 마지막 자식으로 추가해야 최상위에 렌더링됨
var flashGo  = Child(canvasGo.transform, "ScreenFlash");
var flashRt  = (RectTransform)flashGo.transform;
flashRt.anchorMin = Vector2.zero;
flashRt.anchorMax = Vector2.one;
flashRt.sizeDelta = Vector2.zero;
var flashImg = flashGo.AddComponent<Image>();
flashImg.color         = new Color(1f, 1f, 1f, 0f);
flashImg.raycastTarget = false;
flashGo.AddComponent<ScreenFlash>();
```

- [ ] **Step 2: 커밋**

```bash
git add Assets/Scripts/Core/TestSceneBootstrapper.cs
git commit -m "feat: add ScreenFlash overlay object to canvas in bootstrapper"
```

---

## 수동 테스트 체크리스트

Unity Editor에서 Play 모드 진입 후 확인:

- [ ] **릴 애니메이션**: SPIN 클릭 시 세 릴이 빠르게 이름을 바꾸다가 릴1(0.8s) → 릴2(1.3s) → 릴3(1.8s) 순으로 결과 고정
- [ ] **버튼 잠금**: 애니메이션 중 SPIN 버튼 비활성화, 완료 후 재활성화
- [ ] **Double 결과**: "카드 획득!" 흰색 텍스트 2초 후 사라짐
- [ ] **AllDifferent 결과**: "버프 카드 획득!" 텍스트 표시
- [ ] **Triple 결과**: "★ TRIPLE! ★" 금색 큰 텍스트 + 화면 금색 플래시, 해당 카드 슬롯 금색 3회 깜빡임
- [ ] **버프카드 사용**: 손패에서 버프카드 클릭 시 화면 금색 플래시 (Triple보다 약하게)
- [ ] **연속 스핀**: 한 스핀 애니메이션 완료 후 다음 스핀 정상 작동
