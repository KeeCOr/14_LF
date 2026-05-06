using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlotDefense
{
    [DefaultExecutionOrder(-100)]
    public class TestSceneBootstrapper : MonoBehaviour
    {
        [Header("유닛 프리팹 (비워두면 Resources 자동 로드)")]
        public GameObject swordsmanPrefab;
        public GameObject archerPrefab;
        public GameObject knightPrefab;
        public GameObject magePrefab;
        public GameObject healerPrefab;
        public GameObject luckGenPrefab;

        [Header("몬스터 프리팹 (비워두면 기본 박스 사용)")]
        public GameObject monsterPrefabOverride;
        public GameObject elitePrefabOverride;

        private MonsterConfig    _monsterCfg;
        private MonsterConfig    _eliteCfg;
        private MonsterConfig    _goblinCfg;
        private MonsterConfig    _trollCfg;
        private MonsterConfig    _batCfg;
        private MonsterConfig    _dragonCfg;
        private FixedDeckConfig  _deckCfg;
        private GlobalBuffConfig _buffCfg;
        private BackgroundConfig _bgCfg;
        private GameObject       _startMenuGo;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install()
        {
            if (FindObjectOfType<GameManager>() != null) return;
            if (FindObjectOfType<TestSceneBootstrapper>() != null) return;
            new GameObject("[TestBootstrap]").AddComponent<TestSceneBootstrapper>();
        }

        private void Awake()
        {
            // --- ScriptableObjects ---
            _monsterCfg = Inst<MonsterConfig>(m =>
                { m.hp = 40f; m.damage = 6f; m.moveSpeed = 1.3f; m.xpReward = 50f;
                  m.prefab = monsterPrefabOverride; });

            _eliteCfg = Inst<MonsterConfig>(m =>
                { m.hp = 150f; m.damage = 20f; m.moveSpeed = 1.8f; m.xpReward = 150f;
                  m.prefab = elitePrefabOverride; });

            _goblinCfg = Inst<MonsterConfig>(m =>
                { m.hp = 20f; m.damage = 4f; m.moveSpeed = 2.5f; m.xpReward = 20f; m.prefab = monsterPrefabOverride; });
            _trollCfg  = Inst<MonsterConfig>(m =>
                { m.hp = 200f; m.damage = 15f; m.moveSpeed = 0.8f; m.xpReward = 120f; m.prefab = monsterPrefabOverride; });
            _batCfg    = Inst<MonsterConfig>(m =>
                { m.hp = 15f; m.damage = 5f; m.moveSpeed = 3f; m.xpReward = 30f; m.isFlying = true; });
            _dragonCfg = Inst<MonsterConfig>(m =>
                { m.hp = 400f; m.damage = 40f; m.moveSpeed = 1.5f; m.xpReward = 300f; m.isFlying = true; });

            _deckCfg = Inst<FixedDeckConfig>(d =>
            {
                // 유닛 10종
                var swordsman   = MakeCard("검사",     hp:80,  dmg:15, speed:2f,   range:1.5f, rate:1f,   sight:5f,  iron:1);
                var archer      = MakeCard("궁수",     hp:50,  dmg:10, speed:1.5f, range:5f,   rate:2f,   sight:8f,  canAttackAir:true, fire:1, iron:1);
                var knight      = MakeCard("기사",     hp:120, dmg:20, speed:1.2f, range:1f,   rate:0.8f, sight:4f,  iron:2);
                var mage        = MakeCard("마법사",   hp:40,  dmg:28, speed:1.8f, range:4.5f, rate:0.6f, sight:8f,  canAttackAir:true, fire:2);
                var healer      = MakeCard("힐러",     hp:70,  dmg:0,  speed:1.6f, range:2f,   rate:0.8f, sight:6f,  heal:8f, life:2);
                var luckGen     = MakeCard("행운술사", hp:60,  dmg:0,  speed:0.8f, range:0f,   rate:0f,   sight:0f,  luckPerSec:0.5f, iron:1, life:1);
                var paladin     = MakeCard("팔라딘",   hp:200, dmg:18, speed:0.9f, range:1.2f, rate:0.7f, sight:4f,  iron:2, life:2);
                var pyromancer  = MakeCard("화염술사", hp:40,  dmg:35, speed:1.7f, range:4f,   rate:0.5f, sight:8f,  canAttackAir:true, fire:3);
                var crusader    = MakeCard("성기사",   hp:100, dmg:15, speed:1.4f, range:1.5f, rate:1f,   sight:5f,  heal:3f, fire:1, iron:2, life:1);
                var stormArcher = MakeCard("폭풍궁수", hp:60,  dmg:12, speed:1.6f, range:6f,   rate:2.5f, sight:10f, canAttackAir:true, fire:2, iron:2);

                // 마법 2종
                var lightning   = MakeSkillCard("번개화살", SkillType.LightningArrow, damage:80f,  radius:2.0f, fire:2);
                var portalBomb  = MakeSkillCard("포탈폭격", SkillType.PortalBomb,     damage:120f, radius:3.0f, fire:2, iron:1);

                // 건물 — 전투
                var fireTower   = MakeBuildingCard("화염탑", new BuildingData
                    { buildingType = BuildingType.BattleTower, attackDamage = 20f, attackRate = 1f, attackRange = 5f, canAttackAir = true }, fire:2);
                var sniperTower = MakeBuildingCard("저격탑", new BuildingData
                    { buildingType = BuildingType.BattleTower, attackDamage = 50f, attackRate = 0.4f, attackRange = 8f, canAttackAir = true }, fire:1, iron:1);

                // 건물 — 에너지 생산
                var furnace     = MakeBuildingCard("화염로",   new BuildingData
                    { buildingType = BuildingType.ProductionEnergy, energyType = ElementType.Fire, energyPerSecond = 1f }, fire:1);
                var forge       = MakeBuildingCard("제철소",   new BuildingData
                    { buildingType = BuildingType.ProductionEnergy, energyType = ElementType.Iron, energyPerSecond = 1f }, iron:1);
                var lifespring  = MakeBuildingCard("생명의샘", new BuildingData
                    { buildingType = BuildingType.ProductionEnergy, energyType = ElementType.Life, energyPerSecond = 1f }, life:1);

                // 건물 — 유닛 생산
                var barracks    = MakeBuildingCard("병영", new BuildingData
                    { buildingType = BuildingType.ProductionUnit, unitToSpawn = swordsman, spawnInterval = 10f }, iron:2);
                var magicCircle = MakeBuildingCard("마법진", new BuildingData
                    { buildingType = BuildingType.ProductionUnit, unitToSpawn = mage, spawnInterval = 15f }, fire:2, life:1);

                // 기본 덱 8장
                d.cards = new CardData[]
                {
                    swordsman, archer, knight, mage,
                    healer, lightning, portalBomb, fireTower
                };

                // Inspector 필드 우선, 없으면 Resources/Prefabs/Units/{카드명}.prefab 자동 로드
                TrySetUnitPrefab(swordsman,   swordsmanPrefab);
                TrySetUnitPrefab(archer,      archerPrefab);
                TrySetUnitPrefab(knight,      knightPrefab);
                TrySetUnitPrefab(mage,        magePrefab);
                TrySetUnitPrefab(healer,      healerPrefab);
                TrySetUnitPrefab(luckGen,     luckGenPrefab);
            });

            _buffCfg = Inst<GlobalBuffConfig>(b =>
                b.possibleBuffs = new BuffEffect[]
                {
                    new BuffEffect { displayName = "공격 버프", attackMultiplier = 2.0f, speedMultiplier = 1.0f, duration = 8f  },
                    new BuffEffect { displayName = "속도 버프", attackMultiplier = 1.0f, speedMultiplier = 1.8f, duration = 12f },
                    new BuffEffect { displayName = "균형 버프", attackMultiplier = 1.5f, speedMultiplier = 1.2f, duration = 10f }
                });

            // --- BackgroundConfig (Resources/BackgroundConfig.asset) ---
            _bgCfg = Resources.Load<BackgroundConfig>("BackgroundConfig");

            // --- Camera ---
            Camera cam;
            if (Camera.main == null)
            {
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<Camera>();
            }
            else
            {
                cam = Camera.main;
            }
            cam.orthographic     = true;
            cam.orthographicSize = _bgCfg != null ? _bgCfg.orthographicSize : 5f;
            cam.clearFlags       = CameraClearFlags.SolidColor;
            cam.transform.position = new Vector3(0, _bgCfg != null ? _bgCfg.cameraY : -0.5f, -10);

            // --- EventSystem ---
            if (FindObjectOfType<EventSystem>() == null)
            {
                var esGo = new GameObject("EventSystem");
                esGo.AddComponent<EventSystem>();
                esGo.AddComponent<StandaloneInputModule>();
            }

            BuildStartMenu();
        }

        // ============================================================
        //  Start Menu
        // ============================================================

        private void BuildStartMenu()
        {
            _startMenuGo = new GameObject("StartMenuCanvas");
            var canvas   = _startMenuGo.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = _startMenuGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            _startMenuGo.AddComponent<GraphicRaycaster>();

            var bgGo = Child(_startMenuGo.transform, "BG");
            StretchFull((RectTransform)bgGo.transform);
            bgGo.AddComponent<Image>().color = new Color(0.07f, 0.09f, 0.18f, 1f);

            var titleTxt = MakeText(_startMenuGo.transform, "Title", "슬롯 디펜스", new Vector2(0, 180f), 64);
            titleTxt.color = new Color(1f, 0.9f, 0.3f);

            var subTxt = MakeText(_startMenuGo.transform, "Sub", "게임 모드를 선택하세요", new Vector2(0, 90f), 28);
            subTxt.color = new Color(0.7f, 0.78f, 1f);

            var battleBtn = MakeButton(_startMenuGo.transform, "BattleBtn",
                "⚔  전투 모드\n상대 기지를 먼저 파괴하세요",
                new Vector2(0, -20f), new Vector2(380f, 80f));
            battleBtn.onClick.AddListener(() => Launch(GameMode.Battle));

            var survBtn = MakeButton(_startMenuGo.transform, "SurvBtn",
                "🌊  생존 모드\n웨이브를 최대한 버티세요",
                new Vector2(0, -130f), new Vector2(380f, 80f));
            survBtn.onClick.AddListener(() => Launch(GameMode.Survival));
            if (survBtn.targetGraphic is Image si) si.color = new Color(0.5f, 0.25f, 0.05f);
        }

        // ============================================================
        //  Game Launch
        // ============================================================

        private void Launch(GameMode mode)
        {
            if (_startMenuGo != null) { Destroy(_startMenuGo); _startMenuGo = null; }
            bool isSurvival = mode == GameMode.Survival;

            // --- 모드별 배경 적용 ---
            if (Camera.main != null && _bgCfg != null)
                Camera.main.backgroundColor = isSurvival ? _bgCfg.survivalSkyColor : _bgCfg.battleSkyColor;
            else if (Camera.main != null)
                Camera.main.backgroundColor = isSurvival ? new Color(0.10f, 0.08f, 0.18f) : new Color(0.48f, 0.72f, 0.88f);

            // --- Arenas ---
            var pVillage = MakeVillage("PlayerVillage", new Vector3(-7.5f, 0, 0), true);
            var eVillage = isSurvival ? null : MakeVillage("EnemyVillage", new Vector3(7.5f, 0, 0), false);
            var pSpawn   = Spawn("PlayerSpawn", new Vector3(-2.5f, 0, 0));
            var eSpawn   = Spawn("EnemySpawn",  isSurvival ? new Vector3(5.5f, 0, 0) : new Vector3(2.5f, 0, 0));

            // --- Portal ---
            Portal portalComp = null;
            {
                var portalGo = new GameObject("Portal");
                portalGo.transform.position = isSurvival ? new Vector3(7.5f, 0, 0) : Vector3.zero;
                var portalSr = portalGo.AddComponent<SpriteRenderer>();
                portalSr.sprite = MakeSprite(new Color(0.7f, 0.2f, 1f), 0.9f, 2f);
                portalSr.sortingOrder = 1;
                portalComp = portalGo.AddComponent<Portal>();
                portalComp.eliteConfig = _eliteCfg;
            }

            // 비활성화를 AddComponent 전에 해야 Awake()가 실행되지 않음.
            var mTemplate = MakeTemplate("MonsterTemplate", Color.red,  new Vector3(-999, -999, 0));
            mTemplate.SetActive(false);
            mTemplate.AddComponent<MonsterController>();

            var uTemplate = MakeTemplate("UnitTemplate", Color.cyan, new Vector3(-999, -999, 0));
            uTemplate.SetActive(false);
            uTemplate.AddComponent<UnitController>();

            // --- GameManager ---
            var gmGo = new GameObject("GameManager");
            gmGo.SetActive(false);
            var gm = gmGo.AddComponent<GameManager>();
            gm.deckConfig     = _deckCfg;
            gm.buffConfig     = _buffCfg;
            gm.isSurvivalMode = isSurvival;
            gmGo.SetActive(true);

            // 시작 카드 1장 랜덤 지급
            var rng = new System.Random();
            GameManager.Instance.Hand.TryAdd(_deckCfg.cards[rng.Next(_deckCfg.cards.Length)]);

            // --- ArenaSystem ---
            var arenaGo = new GameObject("ArenaSystem");
            var arena   = arenaGo.AddComponent<ArenaSystem>();
            arena.playerSpawnPoint   = pSpawn;
            arena.enemySpawnPoint    = eSpawn;
            arena.playerVillage      = pVillage;
            arena.enemyVillage       = eVillage;
            arena.monsterPrefab      = mTemplate;
            arena.monsterConfig      = _monsterCfg;
            arena.unitPrefab         = uTemplate;
            arena.portal             = portalComp;
            arena.eliteMonsterConfig = _eliteCfg;
            arena.survivalMode       = isSurvival;
            if (portalComp != null) portalComp.arenaSystem = arena;

            // --- TransferSystem (battle only) ---
            if (!isSurvival)
            {
                var tsGo = new GameObject("TransferSystem");
                var ts   = tsGo.AddComponent<TransferSystem>();
                ts.arenaSystem = arena;
                ts.portal      = portalComp;
            }

            // --- AIOpponent (battle only) ---
            if (!isSurvival)
            {
                var aiGo = new GameObject("AIOpponent");
                aiGo.SetActive(false);
                var ai = aiGo.AddComponent<AIOpponent>();
                ai.deckConfig  = _deckCfg;
                ai.buffConfig  = _buffCfg;
                ai.arenaSystem = arena;
                ai.portal      = portalComp;
                ai.unitPrefab  = uTemplate;
                aiGo.SetActive(true);
            }

            // --- UI ---
            BuildUI(arena);
        }

        // ============================================================
        //  UI
        // ============================================================

        static void BuildUI(ArenaSystem arena)
        {
            var canvasGo = new GameObject("Canvas");
            var canvas   = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            // ArenaHUD
            var hudGo = Child(canvasGo.transform, "ArenaHUD");
            var hud   = hudGo.AddComponent<ArenaHUD>();
            LabelText(hudGo.transform, "LabelPlayer", "내 기지 HP",  new Vector2(-340, 523));
            LabelText(hudGo.transform, "LabelEnemy",  "적 기지 HP",  new Vector2( 340, 523));
            hud.playerHpSlider  = MakeSlider(hudGo.transform, "PlayerHP", new Vector2(-340, 490), Color.green);
            hud.enemyHpSlider   = MakeSlider(hudGo.transform, "EnemyHP",  new Vector2( 340, 490), Color.red);
            hud.timerText       = MakeText(hudGo.transform, "Timer",  "3:00", new Vector2(0, 490), 36);
            hud.spinChargesText = MakeText(hudGo.transform, "Spins",  "x0",   new Vector2(0, 450), 28);
            hud.recordText = MakeText(hudGo.transform, "Record", RecordSystem.Summary(), new Vector2(0, 410), 20);
            hud.stageText = MakeText(hudGo.transform, "Stage", "STAGE 1", new Vector2(0, 370), 22);

            // SlotMachineUI
            var slotGo = Child(canvasGo.transform, "SlotMachineUI");
            var slotUI = slotGo.AddComponent<SlotMachineUI>();

            LabelText(slotGo.transform, "SlotDesc", "행운 소모 → SPIN → 카드 획득  (2초마다 행운 +1)", new Vector2(0, -400));

            // 3개 릴 박스
            var reelNameTexts = new Text[3];
            float[] reelX = { -200f, 0f, 200f };
            string[] reelLabels = { "릴 1", "릴 2", "릴 3" };
            for (int i = 0; i < 3; i++)
            {
                var reelBox = Child(slotGo.transform, $"Reel{i}");
                var reelRt  = (RectTransform)reelBox.transform;
                reelRt.anchoredPosition = new Vector2(reelX[i], -445f);
                reelRt.sizeDelta        = new Vector2(170f, 60f);
                var reelBg = reelBox.AddComponent<Image>();
                reelBg.color = new Color(0.1f, 0.15f, 0.3f, 0.95f);
                MakeText(reelBox.transform, "Label", reelLabels[i], new Vector2(0, 20), 16).color = new Color(0.6f, 0.7f, 1f);
                reelNameTexts[i] = MakeText(reelBox.transform, "Value", "?", new Vector2(0, -10), 28);
            }
            slotUI.reelLabels = reelNameTexts;

            slotUI.resultText = MakeText(slotGo.transform, "Result", "", new Vector2(0, -490), 24);
            slotUI.spinButton = MakeButton(slotGo.transform, "SpinBtn", "SPIN (행운 1 소모)", new Vector2(0, -530), new Vector2(260, 58));

            // 배치 가능 구역 오버레이
            var zoneGo  = Child(canvasGo.transform, "DeployZone");
            var zoneRt  = (RectTransform)zoneGo.transform;
            zoneRt.anchorMin  = Vector2.zero;
            zoneRt.anchorMax  = new Vector2(0.48f, 1f);
            zoneRt.sizeDelta  = Vector2.zero;
            var zoneImg = zoneGo.AddComponent<Image>();
            zoneImg.color = new Color(0.3f, 0.75f, 1f, 0.10f);
            zoneImg.raycastTarget = false;
            zoneGo.transform.SetSiblingIndex(0);

            // 구역 경계선 (오른쪽 끝 세로선)
            var borderGo = Child(zoneGo.transform, "Border");
            var borderRt = (RectTransform)borderGo.transform;
            borderRt.anchorMin = new Vector2(1f, 0f);
            borderRt.anchorMax = new Vector2(1f, 1f);
            borderRt.pivot     = new Vector2(1f, 0.5f);
            borderRt.sizeDelta = new Vector2(5f, 0f);
            var borderImg = borderGo.AddComponent<Image>();
            borderImg.color = new Color(0.3f, 0.85f, 1f, 0.65f);
            borderImg.raycastTarget = false;

            // 구역 상단 라벨
            var zoneLbl = MakeText(zoneGo.transform, "ZoneLabel", "▼ 배치 구역 ▼", new Vector2(0, 40), 22);
            zoneLbl.color = new Color(0.4f, 0.9f, 1f, 0.75f);

            // 구역 하단 설명
            var zoneDesc = MakeText(zoneGo.transform, "ZoneDesc", "카드 드래그하여 유닛 배치", new Vector2(0, -20), 16);
            zoneDesc.color = new Color(0.6f, 0.9f, 1f, 0.55f);

            // HandUI
            var handGo = Child(canvasGo.transform, "HandUI");
            var handUI = handGo.AddComponent<HandUI>();
            handUI.arenaSystem       = arena;
            handUI.deployZoneOverlay = zoneImg;
            handUI.cardButtons = new Button[4];
            handUI.cardIcons   = new Image[4];
            handUI.cardNames   = new Text[4];

            LabelText(handGo.transform, "HandDesc", "카드 클릭 선택 → 배치 구역 클릭으로 유닛 배치", new Vector2(0, -255));

            for (int i = 0; i < 4; i++)
            {
                float xPos = -345f + i * 230f;
                var cardGo = Child(handGo.transform, $"CardSlot{i}");
                var rt     = (RectTransform)cardGo.transform;
                rt.anchoredPosition = new Vector2(xPos, -320f);
                rt.sizeDelta        = new Vector2(220f, 130f);

                var bg  = cardGo.AddComponent<Image>();
                bg.color = new Color(0.2f, 0.25f, 0.45f, 0.9f);
                var btn = cardGo.AddComponent<Button>();
                btn.targetGraphic = bg;
                handUI.cardButtons[i] = btn;

                var drag = cardGo.AddComponent<CardDragHandler>();
                drag.slotIndex   = i;
                drag.arenaSystem = arena;
                drag.font        = SharedFont();

                var iconGo = Child(cardGo.transform, "Icon");
                var iconRt = (RectTransform)iconGo.transform;
                iconRt.anchoredPosition = new Vector2(-70, 0);
                iconRt.sizeDelta        = new Vector2(48, 48);
                handUI.cardIcons[i] = iconGo.AddComponent<Image>();
                iconGo.SetActive(false);

                var nameText = MakeText(cardGo.transform, "Name", "---", new Vector2(0, 0), 26);
                nameText.supportRichText = true;
                nameText.lineSpacing     = 1.2f;
                handUI.cardNames[i] = nameText;
            }

            // ResultUI
            var resultUIGo = Child(canvasGo.transform, "ResultUI");
            StretchFull((RectTransform)resultUIGo.transform);
            var resultUI = resultUIGo.AddComponent<ResultUI>();

            var panelGo = Child(resultUIGo.transform, "Panel");
            StretchFull((RectTransform)panelGo.transform);
            var panelImg = panelGo.AddComponent<Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.82f);

            resultUI.panel       = panelGo;
            resultUI.resultText  = MakeText(panelGo.transform, "ResultText", "", Vector2.zero, 70);
            resultUI.retryButton = MakeButton(panelGo.transform, "RetryBtn", "RETRY", new Vector2(0, -110), new Vector2(220, 65));

            // DeckViewerUI — 덱 카드 목록 팝업
            var dvGo = Child(canvasGo.transform, "DeckViewer");
            var dv   = dvGo.AddComponent<DeckViewerUI>();

            var dvPanel   = Child(dvGo.transform, "Panel");
            var dvPanelRt = (RectTransform)dvPanel.transform;
            dvPanelRt.anchoredPosition = Vector2.zero;
            dvPanelRt.sizeDelta        = new Vector2(960f, 540f);
            dvPanel.AddComponent<Image>().color = new Color(0.04f, 0.07f, 0.14f, 0.97f);

            var dvTitle = MakeText(dvPanel.transform, "Title", "슬롯 카드 목록", new Vector2(0, 225f), 26);
            dvTitle.color = new Color(0.55f, 0.88f, 1f);

            var dvContent = MakeText(dvPanel.transform, "Content", "", new Vector2(-20f, -20f), 18);
            dvContent.alignment = TextAnchor.UpperLeft;
            ((RectTransform)dvContent.transform).sizeDelta = new Vector2(900f, 420f);
            dvContent.horizontalOverflow = HorizontalWrapMode.Wrap;

            var dvClose = MakeButton(dvPanel.transform, "CloseBtn", "닫기", new Vector2(420f, 220f), new Vector2(80f, 38f));
            dvClose.onClick.AddListener(() => dv.Toggle());

            dv.panel       = dvPanel;
            dv.contentText = dvContent;
            dv.Setup(GameManager.Instance.deckConfig);

            // "덱 보기" 버튼 — HUD 우상단
            var deckBtn = MakeButton(hudGo.transform, "DeckViewBtn", "덱 보기", new Vector2(840f, 490f), new Vector2(110f, 36f));
            deckBtn.onClick.AddListener(() => dv.Toggle());

            // ScreenFlash overlay
            var flashGo  = Child(canvasGo.transform, "ScreenFlash");
            var flashRt  = (RectTransform)flashGo.transform;
            flashRt.anchorMin = Vector2.zero;
            flashRt.anchorMax = Vector2.one;
            flashRt.sizeDelta = Vector2.zero;
            var flashImg = flashGo.AddComponent<Image>();
            flashImg.color         = new Color(1f, 1f, 1f, 0f);
            flashImg.raycastTarget = false;
            flashGo.AddComponent<ScreenFlash>();

            // --- EnergyHUD — 화면 우상단 패널 (속성별 색상 행) ---
            var energyGo  = Child(canvasGo.transform, "EnergyHUD");
            var energyRt  = (RectTransform)energyGo.transform;
            energyRt.anchorMin        = new Vector2(1f, 1f);
            energyRt.anchorMax        = new Vector2(1f, 1f);
            energyRt.pivot            = new Vector2(1f, 1f);
            energyRt.anchoredPosition = new Vector2(-12f, -12f);
            energyRt.sizeDelta        = new Vector2(180f, 130f);

            var energyBg = energyGo.AddComponent<Image>();
            energyBg.color        = new Color(0.04f, 0.06f, 0.15f, 0.92f);
            energyBg.raycastTarget = false;

            var energyHud = energyGo.AddComponent<EnergyHUD>();
            energyHud.fireText = MakeEnergyLabel(energyGo.transform, "FireText", new Vector2(90f, 96f),  new Color(1f,  0.45f, 0.1f));
            energyHud.ironText = MakeEnergyLabel(energyGo.transform, "IronText", new Vector2(90f, 60f),  new Color(0.6f, 0.8f, 1f));
            energyHud.lifeText = MakeEnergyLabel(energyGo.transform, "LifeText", new Vector2(90f, 24f),  new Color(0.2f, 1f,  0.45f));
        }

        // ============================================================
        //  Factories
        // ============================================================

        static T Inst<T>(System.Action<T> init) where T : ScriptableObject
        {
            var obj = ScriptableObject.CreateInstance<T>(); init(obj); return obj;
        }

        static CardData MakeSkillCard(string name, SkillType type, float damage, float radius,
            int fire = 0, int iron = 0, int life = 0)
        {
            var card = ScriptableObject.CreateInstance<CardData>();
            card.cardName    = name;
            card.cardType    = CardType.Skill;
            card.fireCost    = fire;
            card.ironCost    = iron;
            card.lifeCost    = life;
            card.skillEffect = new SkillEffect { type = type, damage = damage, radius = radius };
            return card;
        }

        static CardData MakeBuildingCard(string name, BuildingData bdata,
            int fire = 0, int iron = 0, int life = 0)
        {
            var card = ScriptableObject.CreateInstance<CardData>();
            card.cardName     = name;
            card.cardType     = CardType.Building;
            card.fireCost     = fire;
            card.ironCost     = iron;
            card.lifeCost     = life;
            card.buildingData = bdata;
            return card;
        }

        void TrySetUnitPrefab(CardData card, GameObject overridePrefab)
        {
            if (overridePrefab != null) { card.unitPrefab = overridePrefab; return; }
            var loaded = Resources.Load<GameObject>($"Prefabs/Units/{card.cardName}");
            if (loaded != null) card.unitPrefab = loaded;
        }

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

        static Village MakeVillage(string name, Vector3 pos, bool isPlayer)
        {
            var go = new GameObject(name);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeSprite(isPlayer ? Color.cyan : new Color(1f, 0.6f, 0f), 1f, 2.5f);
            var v = go.AddComponent<Village>();
            typeof(Village)
                .GetField("isPlayerVillage", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(v, isPlayer);
            return v;
        }

        static Transform Spawn(string name, Vector3 pos)
        {
            var go = new GameObject(name);
            go.transform.position = pos;
            return go.transform;
        }

        static GameObject MakeTemplate(string name, Color color, Vector3 pos)
        {
            var go = new GameObject(name);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeSprite(color, 0.6f, 0.6f);
            return go;
        }

        static Sprite MakeSprite(Color color, float w, float h)
        {
            int pw = Mathf.Max(1, Mathf.RoundToInt(w * 32));
            int ph = Mathf.Max(1, Mathf.RoundToInt(h * 32));
            var tex    = new Texture2D(pw, ph);
            var pixels = new Color[pw * ph];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels); tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, pw, ph), new Vector2(0.5f, 0.5f), 32f);
        }

        // ============================================================
        //  UI helpers
        // ============================================================

        static Font _sharedFont;
        static Font SharedFont()
        {
            if (_sharedFont != null) return _sharedFont;
            _sharedFont = Font.CreateDynamicFontFromOSFont(
                new[] { "Malgun Gothic", "NanumGothic", "Arial Unicode MS", "Arial" }, 14);
            if (_sharedFont == null)
                _sharedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return _sharedFont;
        }

        static GameObject Child(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
        }

        static Text MakeText(Transform parent, string name, string content, Vector2 pos, int fontSize = 24)
        {
            var go = Child(parent, name);
            var rt = (RectTransform)go.transform;
            rt.anchoredPosition = pos;
            rt.sizeDelta        = new Vector2(500, 60);
            var t = go.AddComponent<Text>();
            t.font              = SharedFont();
            t.text              = content;
            t.fontSize          = fontSize;
            t.alignment         = TextAnchor.MiddleCenter;
            t.color             = Color.white;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow   = VerticalWrapMode.Overflow;
            return t;
        }

        static Text LabelText(Transform parent, string name, string content, Vector2 pos)
        {
            var t = MakeText(parent, name, content, pos, 17);
            t.color = new Color(0.75f, 0.85f, 1f);
            ((RectTransform)t.transform).sizeDelta = new Vector2(800, 36);
            return t;
        }

        static Text MakeEnergyLabel(Transform parent, string name, Vector2 offset, Color color = default)
        {
            var go   = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin        = new Vector2(0f, 0f);
            rect.anchorMax        = new Vector2(0f, 0f);
            rect.pivot            = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = offset;
            rect.sizeDelta        = new Vector2(170f, 36f);
            var txt  = go.AddComponent<Text>();
            txt.font               = SharedFont();
            txt.fontSize           = 34;
            txt.color              = color == default ? Color.white : color;
            txt.alignment          = TextAnchor.MiddleCenter;
            txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            txt.supportRichText    = true;
            return txt;
        }

        static Text MakeLabel(Transform parent, string name, Vector2 offset)
        {
            var go   = Child(parent, name);
            var rect = (RectTransform)go.transform;
            rect.anchoredPosition = offset;
            rect.sizeDelta        = new Vector2(120f, 30f);
            var txt  = go.AddComponent<Text>();
            txt.font             = SharedFont();
            txt.fontSize         = 18;
            txt.color            = Color.white;
            txt.alignment        = TextAnchor.MiddleCenter;
            txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            return txt;
        }

        static Slider MakeSlider(Transform parent, string name, Vector2 pos, Color fillColor)
        {
            var go = Child(parent, name);
            var rt = (RectTransform)go.transform;
            rt.anchoredPosition = pos;
            rt.sizeDelta        = new Vector2(320, 28);
            var bgImg = go.AddComponent<Image>();
            bgImg.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);
            var slider = go.AddComponent<Slider>();
            slider.minValue = 0f; slider.maxValue = 1f; slider.value = 1f;

            var fillArea = Child(go.transform, "Fill Area");
            var faRt     = (RectTransform)fillArea.transform;
            faRt.anchorMin = Vector2.zero; faRt.anchorMax = Vector2.one; faRt.sizeDelta = Vector2.zero;

            var fill    = Child(fillArea.transform, "Fill");
            var fillRt  = (RectTransform)fill.transform;
            fillRt.anchorMin = Vector2.zero; fillRt.anchorMax = Vector2.one; fillRt.sizeDelta = Vector2.zero;
            fill.AddComponent<Image>().color = fillColor;

            slider.fillRect = fillRt;
            return slider;
        }

        static Button MakeButton(Transform parent, string name, string label, Vector2 pos, Vector2 size)
        {
            var go  = Child(parent, name);
            var rt  = (RectTransform)go.transform;
            rt.anchoredPosition = pos;
            rt.sizeDelta        = size;
            var img = go.AddComponent<Image>();
            img.color = new Color(0.25f, 0.55f, 0.25f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            MakeText(go.transform, "Label", label, Vector2.zero, 24);
            return btn;
        }
    }
}
