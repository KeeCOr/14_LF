# UX 피드백 시스템 설계 — 릴 애니메이션 / Triple 연출 / 버프 이펙트

> 작성일: 2026-04-28

---

## 목표

외부 에셋(사운드, 스프라이트) 없이 코드만으로 세 가지 UX 피드백을 구현한다.

1. 슬롯 릴 스핀 애니메이션 — 릴이 순서대로 하나씩 멈추는 실제 슬롯머신 느낌
2. Triple 강조 연출 — 강렬한 화면 플래시 + 텍스트 강조 + 카드 깜빡임
3. 버프 발동 이펙트 — 금색 화면 플래시

---

## 공통 인프라: ScreenFlash

### 역할
Canvas에 전체화면 반투명 Image를 올려두고 코루틴으로 alpha를 제어하는 싱글턴 컴포넌트.
Triple 연출과 버프 이펙트가 동일 컴포넌트를 재사용하며 강도(maxAlpha)만 다르게 호출한다.

### 인터페이스
```csharp
ScreenFlash.Instance.Play(color, maxAlpha, fadeInDuration, fadeOutDuration);
```

### 구현 위치
- 신규 파일: `Assets/Scripts/UI/ScreenFlash.cs`
- `TestSceneBootstrapper.BuildUI()` 에서 Canvas 하위에 전체화면 Image + ScreenFlash 컴포넌트 생성

---

## 1. 슬롯 릴 스핀 애니메이션

### 현재 상태
`SlotMachineUI` 의 SPIN 버튼이 `GameManager.TrySpin()` 을 즉시 호출하고 결과를 바로 표시한다.
릴 텍스트 3개(`reelNames`)가 이미 존재하므로 텍스트 교체 애니메이션만 추가하면 된다.

### 변경 방식
- SPIN 버튼 클릭 시 `GameManager.TrySpin()` 호출을 **지연**시키고, 먼저 릴 애니메이션 코루틴을 실행한다.
- 애니메이션 중 SPIN 버튼 비활성화.

### 타이밍
| 릴 | 정지 시점 |
|---|---|
| 릴 1 | 0.8초 |
| 릴 2 | 1.3초 |
| 릴 3 | 1.8초 |
| 결과 처리 | 1.8초 (릴 3 정지 직후) |

### 릴 애니메이션 상세
- 정지 전까지: 0.07초 간격으로 덱 카드 이름 중 랜덤 하나를 표시
- 정지 시: 해당 릴의 실제 결과값으로 고정
- 릴 3 정지 후: `GameManager.TrySpin()` 호출 → 결과 텍스트 + 카드 획득 처리

### 릴 결과값 사전 결정
`TrySpin()` 에서 결과를 계산하는 로직을 UI에서 분리하기 위해,
`GameManager` 에 `PeekReels()` 메서드를 추가해 릴 결과를 미리 반환하고 `TrySpin()` 은 XP 차감만 담당하도록 분리한다.

> **대안**: SlotMachineUI가 덱 카드 이름 목록만 참조해 랜덤 표시하다가,
> 1.8초 후 실제 TrySpin() 결과와 맞춰 표시. 이 방식이 더 단순하므로 채택.

---

## 2. Triple 강조 연출

### 트리거
`SlotMachineUI` 에서 `SlotResult.Triple` 판정 시 발동.

### 연출 내용
1. **화면 플래시**: `ScreenFlash.Instance.Play(gold, maxAlpha: 0.55f, fadeIn: 0.1s, fadeOut: 0.45s)`
2. **결과 텍스트**: `"★ TRIPLE! ★"` 금색, 폰트 크기 32 (기본 24 → 32)
3. **카드 슬롯 깜빡임**: 획득한 카드가 들어간 HandUI 슬롯의 배경색을 금색 ↔ 원래색 3회 깜빡임 (0.15초 간격)

### 카드 슬롯 깜빡임 연동
`GameEvents.OnCardObtained` 에 `CardTier` 가 이미 전달되므로,
`HandUI` 에서 `CardTier.Enhanced` 수신 시 해당 슬롯 인덱스에 깜빡임 코루틴 실행.

---

## 3. 버프 발동 이펙트

### 트리거
`HandUI.SelectSlot()` 에서 버프카드 사용 시 발동.

### 연출 내용
- **화면 플래시**: `ScreenFlash.Instance.Play(gold, maxAlpha: 0.35f, fadeIn: 0.1s, fadeOut: 0.3s)`
- Triple보다 약한 강도 (maxAlpha 0.35 vs 0.55)

---

## 변경 파일 목록

| 파일 | 변경 유형 | 내용 |
|---|---|---|
| `Assets/Scripts/UI/ScreenFlash.cs` | **신규** | 화면 오버레이 플래시 싱글턴 |
| `Assets/Scripts/UI/SlotMachineUI.cs` | 수정 | 릴 애니메이션 코루틴, Triple 연출 |
| `Assets/Scripts/UI/HandUI.cs` | 수정 | Enhanced 카드 슬롯 깜빡임 |
| `Assets/Scripts/Core/GameManager.cs` | 수정 없음 | TrySpin() 로직 그대로 유지 |
| `Assets/Scripts/Core/TestSceneBootstrapper.cs` | 수정 | BuildUI() 에서 ScreenFlash 오브젝트 생성 |

---

## 비고

- `GameManager.TrySpin()` 내부 로직(XP 차감, 릴 드로우, 카드 획득)은 변경하지 않는다.
- 릴 애니메이션 중 표시되는 카드 이름은 실제 덱 카드 이름 목록에서 무작위 선택한다 (결과와 무관).
- 애니메이션 완료 전 연속 SPIN 방지를 위해 버튼 비활성화를 유지한다.
