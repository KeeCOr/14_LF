# Lottery Fantasy 기획서

> 최신 업데이트: 2026-05-28, 생성형 이미지/UI 리소스 시트 추가

## 게임 개요

Lottery Fantasy는 3릴 슬롯 결과로 속성 에너지를 얻고, 그 에너지로 카드형 유닛/스킬/건물을 배치해 마을을 방어하거나 상대 기지를 파괴하는 Unity 2D 전략 게임이다.

## 핵심 루프

1. 슬롯 릴이 자동 회전한다.
2. 행운 충전이 쌓이면 STOP으로 릴 3개를 정지한다.
3. Fire/Iron/Life 결과에 따라 속성 에너지를 획득한다.
4. 손패의 카드 비용을 지불하고 유닛, 스킬, 건물을 사용한다.
5. 유닛과 건물이 포털/기지/몬스터를 상대하며 승패를 결정한다.

## 게임 모드

| 모드 | 설명 |
|---|---|
| 전투 모드 | 3분 동안 AI 기지를 먼저 파괴하거나 제한 시간 후 HP 우위로 승리한다. |
| 생존 모드 | 계속 강화되는 웨이브를 상대로 오래 버틴다. |

## UI 비주얼 방향

2026-05-15 업데이트에서 런타임 생성 UI의 구조는 유지하고, 공통 스타일 유틸을 통해 화면 완성도를 높였다.

| 영역 | 변경 방향 |
|---|---|
| 상단 HUD | 짙은 전투 정보 바, 명확한 HP 색상, 덱 버튼 강조색 적용 |
| 하단 패널 | 카드 영역과 슬롯 영역을 더 또렷하게 분리하고 파란 계열 테두리로 통일 |
| 카드 | 타입/사용 가능/선택/부족 상태별 배경색 통일, 금색 상단 라인과 어두운 하단 라인 추가 |
| 슬롯머신 | 릴 박스 크기 확대, 속성별 상단 accent, STOP/AUTO 버튼 상태색 강화 |
| 결과/덱 화면 | 어두운 오버레이와 금색/청색 accent로 모달 느낌 강화 |

## 오브젝트 및 캐릭터 에셋 방향

2026-05-18 업데이트에서는 새 외부 다운로드보다 프로젝트에 이미 포함된 무료/샘플 에셋을 우선 사용한다. 게임플레이 루트 오브젝트의 `Rigidbody2D`, `Collider2D`, `HpBar`, 컨트롤러 구조는 유지하고, 로우폴리 모델과 자연 소품은 자식 장식 오브젝트로 붙인다.

| 영역 | 사용 에셋 |
|---|---|
| 유닛 | `Polytope Studio` 로우폴리 캐릭터/장비 프리팹 |
| 몬스터 | `Polytope Studio` 캐릭터 파츠를 몬스터/엘리트 구분용 장식으로 사용 |
| 기지 | `SimpleNaturePack` 나무/자연 프리팹으로 마을 실루엣 강화 |
| 포털 | `Polytope Studio` 목재/소품 프리팹을 포털 relic 장식으로 사용 |
| 전장 | `SimpleNaturePack` 나무, 바위, 덤불, 꽃과 `Polytope Studio` 울타리로 주변부 장식 |

## 예시 이미지 기반 전장 연출

2026-05-22 업데이트에서는 `docs/LotteryFantasy_gameplay_preview.png`의 밝은 캐주얼 판타지 방향을 기준으로 런타임 전장 장식을 보강했다. UI와 게임플레이 구조는 유지하고, 게임 시작 시 `GameVisualKit.AddArenaBackdrop`이 전장 바닥, 중앙 흙길, 하단 석재 테라스, 플레이어/적 진영 실루엣, 깃발과 금색 문장을 생성한다.

| 영역 | 변경 방향 |
|---|---|
| 전장 배경 | 밝은 녹색 전장, 먼 언덕, 중앙 흙길을 추가해 예시 이미지처럼 공간 깊이를 만든다. |
| 플레이어/적 기지 | 파랑/빨강 배너와 금색 문장을 배치해 진영 구분을 강화한다. |
| 생존 모드 | 적 기지 대신 보라색 포털 받침대 실루엣을 배치해 모드 목적을 명확히 한다. |
| 구현 방식 | 외부 다운로드 없이 코드 생성 SpriteRenderer와 기존 포함 에셋만 사용한다. |

## 룰렛 밸런스 및 유닛 식별성

2026-05-27 업데이트에서는 룰렛을 덜 자주 돌리도록 행운 충전 밸런스를 낮추고, 예시 이미지처럼 유닛 역할이 한눈에 갈라지도록 런타임 장식을 강화했다.

| 영역 | 변경 방향 |
|---|---|
| 룰렛 충전 | 기본 최대 충전량을 10개에서 6개로 낮춘다. |
| 시작 충전 | 플레이어와 AI 모두 시작 충전을 3개에서 1개로 줄인다. |
| 충전 간격 | 플레이어 자동 충전은 6초에서 10초, AI 충전은 3초에서 6초로 늘린다. |
| AI 사용 빈도 | AI 슬롯 사용 간격을 4초에서 7초로 늘려 초반 압박을 완화한다. |
| 유닛 식별성 | 검사/기사/궁수/마법/힐러/거인 계열에 서로 다른 accent 색, 스케일, 무기 위치를 적용한다. |

## 생성형 이미지/UI 리소스

2026-05-28 업데이트에서는 예시 이미지의 밝은 캐주얼 판타지 톤을 기준으로 카드/유닛 아이콘, UI 아틀라스, 전장 오브젝트 시트를 생성했다. 실제 사용 목록과 슬라이스 가이드는 `docs/LotteryFantasy_이미지_UI_리소스_목록.md`에 정리한다.

| 파일 | 용도 |
|---|---|
| `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-card-unit-icons-sheet.png` | 카드 초상화와 유닛 아이콘 후보 |
| `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-ui-atlas-sheet.png` | HUD, 슬롯머신, 버튼, 속성 심볼, 배지 후보 |
| `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-battlefield-props-sheet.png` | 기지, 포털, 전장 장식 후보 |
| `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-buttons-sheet.png` | STOP, Draw, 확인, 보상, 비활성, 탭, 토글 버튼 후보 |
| `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-frames-sheet.png` | HUD, 슬롯머신, 카드, 모달, 툴팁 프레임 후보 |

## 빌드 및 배포

- Unity 프로젝트 경로: `C:/Development/14_LT/LotteryFantasy`
- Windows 빌드 출력: `C:/Development/14_LT/release/LotteryFantasy.exe`
- 포터블 실행파일 배치: `C:/Development/14_LT/LotteryFantasy_v{버전}_portable.exe`
## 생성 리소스 런타임 적용

2026-05-29 업데이트에서 생성된 버튼, 프레임, 카드 아이콘, 전장 오브젝트 시트를 `Resources/GeneratedArt`에서 런타임 로드해 실제 UI와 전장에 적용했다. `UIArtKit`이 시트별 스프라이트 좌표와 9-slice border를 관리하며, 시작 메뉴/덱 보기/결과 모달/에너지 패널/룰렛 패널/손패 카드 프레임에 버튼 및 프레임 리소스를 사용한다.

| 영역 | 적용 내용 |
|---|---|
| 카드 | 생성 카드 아이콘 시트를 `CardData.icon`과 `HandUI` fallback 아이콘에 연결하고, 손패 슬롯에 브론즈/실버/골드/퍼플 카드 프레임을 적용 |
| 룰렛/버튼 | STOP, AUTO, 덱 보기, 시작 메뉴, 재시도 버튼에 생성 버튼 시트와 프레임 시트 적용 |
| HUD/패널 | HP 프레임, 타이머 배너, 에너지 패널, 덱 보기 모달, 결과 모달에 생성 프레임 적용 |
| 전장 | 플레이어/적 성, 생존 포탈, 중앙 길 패치, 나무/울타리/바위/횃불/성벽 장식을 생성 전장 오브젝트 시트에서 SpriteRenderer로 추가 |
## Asset Store 대체 로우폴리 바람 연출

2026-05-29 업데이트에서 `Low Poly Wind` 패키지를 직접 다운로드하지 못하는 상황을 대비해 프로젝트 내 런타임 바람 연출을 추가했다. `LowPolyWindAnimator`는 Tree/Grass/Banner/Torch/Portal/AmbientProp 프로필별로 위치, 회전, 스케일을 미세하게 흔들어 로우폴리 맵의 정적인 느낌을 줄인다.

| 영역 | 적용 내용 |
|---|---|
| 전장 풀 | 포함된 `PT_Grass_02` 프리팹을 전장 하단에 추가하고 Grass 프로필의 빠른 흔들림 적용 |
| 나무/덤불/울타리 | 기존 자연 프리팹과 생성 오브젝트에 Tree/Banner/Grass 프로필을 연결 |
| 성/포탈 장식 | 생성 성, 포탈, 건물 장식에 Banner/Portal 프로필을 적용해 깃발과 마법 장식이 은근히 움직이도록 구성 |
| 검증 | `LowPolyWindAnimatorTests`와 `GameVisualKitTests`에 프로필/전장 연결 확인 테스트 추가 |
