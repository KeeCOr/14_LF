# Lottery Fantasy 이미지/UI 리소스 목록

> 작성일: 2026-05-28
> 기준 톤: `docs/LotteryFantasy_gameplay_preview.png`의 밝은 캐주얼 판타지, 파랑/빨강 진영 대비, 금장 UI, 큰 실루엣

## 생성 완료 리소스

| 구분 | 파일 | 용도 | 구성 |
|---|---|---|---|
| 카드/유닛 아이콘 | `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-card-unit-icons-sheet.png` | 카드 초상화, 유닛 선택 UI, 덱 보기 | Fire Knight, Iron Golem, Life Cleric, Forest Ranger, Bombardier, Arcane Mage |
| UI 아틀라스 | `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-ui-atlas-sheet.png` | HUD, 슬롯머신, 버튼, 재화/속성 아이콘 | Player/Enemy HP 프레임, 왕관 배지, 릴 프레임, STOP/Draw 버튼 베이스, 행운 오브, Fire/Iron/Life 심볼(2026-06-29 런타임 교체), 코인, 보물상자, 카드 뒷면, 진영 방패 |
| 전장 오브젝트 | `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-battlefield-props-sheet.png` | 기지, 포털, 전장 장식, 배경 소품 | 파랑/빨강 타워, 포털, 석벽, 화로, 울타리, 나무, 덤불, 바위, 길 타일, 왕관 명패 |
| 버튼 시트 | `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-buttons-sheet.png` | STOP, Draw, 확인, 보상, 비활성, 탭, 토글 등 버튼 교체 후보 | 빨강/파랑/초록/금색/비활성 롱 버튼, 사각 아이콘 버튼, 원형 닫기 버튼, ON/OFF 토글, 탭 버튼 |
| 프레임 시트 | `LotteryFantasy/Assets/Resources/GeneratedArt/lotteryfantasy-frames-sheet.png` | HUD, 슬롯머신, 카드, 모달, 툴팁 등 프레임 교체 후보 | 슬롯머신 프레임, 카드 트레이, 타이틀 배너, HP 프레임, 카드 등급 프레임, 모달/툴팁/카운터 프레임 |

## 원본 보관

크로마키 제거 전 원본은 아래 폴더에 보관한다.

| 파일 | 설명 |
|---|---|
| `LotteryFantasy/Assets/Resources/GeneratedArt/Source/lotteryfantasy-card-unit-icons-source.png` | 카드/유닛 아이콘 원본 |
| `LotteryFantasy/Assets/Resources/GeneratedArt/Source/lotteryfantasy-ui-atlas-source.png` | UI 아틀라스 원본 |
| `LotteryFantasy/Assets/Resources/GeneratedArt/Source/lotteryfantasy-battlefield-props-source.png` | 전장 오브젝트 원본 |
| `LotteryFantasy/Assets/Resources/GeneratedArt/Source/lotteryfantasy-buttons-source.png` | 버튼 시트 원본 |
| `LotteryFantasy/Assets/Resources/GeneratedArt/Source/lotteryfantasy-frames-source.png` | 프레임 시트 원본 |

## 권장 슬라이스

| 시트 | 권장 방식 |
|---|---|
| 카드/유닛 아이콘 | 3열 x 2행 기준으로 512 x 512 단위 슬라이스 후 카드 아이콘에 연결 |
| UI 아틀라스 | 수동 Sprite Editor slicing 권장. 컴포넌트 크기가 달라 자동 그리드보다 개별 박스 지정이 안전 |
| 전장 오브젝트 | 수동 Sprite Editor slicing 권장. 타워/포털/길 타일처럼 크기가 다른 오브젝트 혼합 |
| 버튼 시트 | 버튼별 수동 slicing 후 Image Type을 Sliced로 설정해 9-slice 적용 권장 |
| 프레임 시트 | 프레임별 수동 slicing 후 모서리/금장 영역이 늘어나지 않도록 Border 값을 별도 지정 권장 |

## 추가로 만들면 좋은 리소스

| 우선순위 | 리소스 | 이유 |
|---|---|---|
| 높음 | 개별 카드 아이콘 PNG | 런타임에서 카드별 `CardData.icon`에 바로 연결하기 쉬움 |
| 높음 | 버튼/프레임 개별 PNG | 생성된 시트를 확정 디자인별로 분리하면 코드 연결과 9-slice 설정이 쉬움 |
| 완료 | 슬롯 릴 속성 심볼 시인성 개선 | `SlotMachineUI`가 참조하는 UI 아틀라스 Fire/Iron/Life 영역을 같은 경로에서 교체해 릴 회전 중 속성 구분을 강화 |
| 중간 | 스킬 이펙트 시트 | 번개/포털폭격/힐/버프 사용감 강화 |
| 낮음 | 결과 화면 배너 | 승리/패배 화면 완성도 강화 |
