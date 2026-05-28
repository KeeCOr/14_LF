# Lottery Fantasy 이미지/UI 리소스 목록

> 작성일: 2026-05-28
> 기준 톤: `docs/LotteryFantasy_gameplay_preview.png`의 밝은 캐주얼 판타지, 파랑/빨강 진영 대비, 금장 UI, 큰 실루엣

## 생성 완료 리소스

| 구분 | 파일 | 용도 | 구성 |
|---|---|---|---|
| 카드/유닛 아이콘 | `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-card-unit-icons-sheet.png` | 카드 초상화, 유닛 선택 UI, 덱 보기 | Fire Knight, Iron Golem, Life Cleric, Forest Ranger, Bombardier, Arcane Mage |
| UI 아틀라스 | `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-ui-atlas-sheet.png` | HUD, 슬롯머신, 버튼, 재화/속성 아이콘 | Player/Enemy HP 프레임, 왕관 배지, 릴 프레임, STOP/Draw 버튼 베이스, 행운 오브, Fire/Iron/Life 심볼, 코인, 보물상자, 카드 뒷면, 진영 방패 |
| 전장 오브젝트 | `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-battlefield-props-sheet.png` | 기지, 포털, 전장 장식, 배경 소품 | 파랑/빨강 타워, 포털, 석벽, 화로, 울타리, 나무, 덤불, 바위, 길 타일, 왕관 명패 |

## 원본 보관

크로마키 제거 전 원본은 아래 폴더에 보관한다.

| 파일 | 설명 |
|---|---|
| `LotteryFantasy/Assets/Resources/GeneratedArt/Source/lotteryfantasy-card-unit-icons-source.png` | 카드/유닛 아이콘 원본 |
| `LotteryFantasy/Assets/Resources/GeneratedArt/Source/lotteryfantasy-ui-atlas-source.png` | UI 아틀라스 원본 |
| `LotteryFantasy/Assets/Resources/GeneratedArt/Source/lotteryfantasy-battlefield-props-source.png` | 전장 오브젝트 원본 |

## 권장 슬라이스

| 시트 | 권장 방식 |
|---|---|
| 카드/유닛 아이콘 | 3열 x 2행 기준으로 512 x 512 단위 슬라이스 후 카드 아이콘에 연결 |
| UI 아틀라스 | 수동 Sprite Editor slicing 권장. 컴포넌트 크기가 달라 자동 그리드보다 개별 박스 지정이 안전 |
| 전장 오브젝트 | 수동 Sprite Editor slicing 권장. 타워/포털/길 타일처럼 크기가 다른 오브젝트 혼합 |

## 추가로 만들면 좋은 리소스

| 우선순위 | 리소스 | 이유 |
|---|---|---|
| 높음 | 개별 카드 아이콘 PNG | 런타임에서 카드별 `CardData.icon`에 바로 연결하기 쉬움 |
| 높음 | 버튼 9-slice PNG | 현재 코드 생성 UI를 더 고급스럽게 교체할 수 있음 |
| 중간 | 슬롯 릴 개별 심볼 PNG | 릴 회전 중 이모지 대신 실제 심볼 이미지로 표시 가능 |
| 중간 | 스킬 이펙트 시트 | 번개/포털폭격/힐/버프 사용감 강화 |
| 낮음 | 결과 화면 배너 | 승리/패배 화면 완성도 강화 |
