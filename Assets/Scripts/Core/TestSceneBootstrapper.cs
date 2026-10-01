using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlotDefense
{
    [DefaultExecutionOrder(-100)]
    public class TestSceneBootstrapper : MonoBehaviour
    {
        [Header("?좊떅 ?꾨━??(鍮꾩썙?먮㈃ Resources ?먮룞 濡쒕뱶)")]
        public GameObject swordsmanPrefab;
        public GameObject archerPrefab;
        public GameObject knightPrefab;
        public GameObject magePrefab;
        public GameObject healerPrefab;
        public GameObject luckGenPrefab;

        [Header("紐ъ뒪???꾨━??(鍮꾩썙?먮㈃ 湲곕낯 諛뺤뒪 ?ъ슜)")]
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
                // ?좊떅 14醫?
                var swordsman   = MakeCard("Swordsman",   hp:80,  dmg:15, speed:2f,   range:1.5f, rate:1f,   sight:5f,  iron:1);
                var archer      = MakeCard("Archer",      hp:50,  dmg:10, speed:1.5f, range:5f,   rate:2f,   sight:8f,  canAttackAir:true, fire:1, iron:1);
                var knight      = MakeCard("Knight",      hp:120, dmg:20, speed:1.2f, range:1f,   rate:0.8f, sight:4f,  iron:2);
                var mage        = MakeCard("Mage",        hp:40,  dmg:28, speed:1.8f, range:4.5f, rate:0.6f, sight:8f,  canAttackAir:true, fire:2);
                var healer      = MakeCard("Healer",      hp:70,  dmg:0,  speed:1.6f, range:2f,   rate:0.8f, sight:6f,  heal:8f, life:2);
                var luckGen     = MakeCard("Luck Sage",   hp:60,  dmg:0,  speed:0.8f, range:0f,   rate:0f,   sight:0f,  luckPerSec:0.5f, iron:1, life:1);
                var paladin     = MakeCard("Paladin",     hp:200, dmg:18, speed:0.9f, range:1.2f, rate:0.7f, sight:4f,  iron:2, life:2);
                var pyromancer  = MakeCard("Pyromancer",  hp:40,  dmg:35, speed:1.7f, range:4f,   rate:0.5f, sight:8f,  canAttackAir:true, fire:3);
                var crusader    = MakeCard("Crusader",    hp:100, dmg:15, speed:1.4f, range:1.5f, rate:1f,   sight:5f,  heal:3f, fire:1, iron:2, life:1);
                var stormArcher = MakeCard("Storm Archer", hp:60, dmg:12, speed:1.6f, range:6f,   rate:2.5f, sight:10f, canAttackAir:true, fire:2, iron:2);
                // ?좉퇋 4醫?
                var giant      = MakeCard("Giant",       hp:350, dmg:35, speed:0.7f, range:1.2f, rate:0.55f, sight:3f,  iron:3);
                var dragon     = MakeCard("Dragon",      hp:200, dmg:45, speed:2.2f, range:3.5f, rate:0.5f,  sight:9f,  canAttackAir:true, isFlying:true, fire:3, life:1);
                var skeleton   = MakeCard("Skeleton",    hp:35,  dmg:12, speed:3.2f, range:1.0f, rate:1.8f,  sight:5f,  iron:1, life:1);
                var dwarf      = MakeCard("Dwarf",       hp:110, dmg:25, speed:1.0f, range:1.0f, rate:1.1f,  sight:4f,  fire:1, iron:2);

                // 留덈쾿 2醫?
                var lightning   = MakeSkillCard("Lightning Arrow", SkillType.LightningArrow, damage:80f,  radius:2.0f, fire:2);
                var portalBomb  = MakeSkillCard("Portal Bomb",     SkillType.PortalBomb,     damage:120f, radius:3.0f, fire:2, iron:1);

                // 嫄대Ъ ???꾪닾
                var fireTower   = MakeBuildingCard("Fire Tower", new BuildingData
                    { buildingType = BuildingType.BattleTower, attackDamage = 20f, attackRate = 1f, attackRange = 5f, canAttackAir = true }, fire:2);
                var sniperTower = MakeBuildingCard("Sniper Tower", new BuildingData
                    { buildingType = BuildingType.BattleTower, attackDamage = 50f, attackRate = 0.4f, attackRange = 8f, canAttackAir = true }, fire:1, iron:1);

                // 嫄대Ъ ???먮꼫吏 ?앹궛
                var furnace     = MakeBuildingCard("Furnace",   new BuildingData
                    { buildingType = BuildingType.ProductionEnergy, energyType = ElementType.Fire, energyPerSecond = 1f }, fire:1);
                var forge       = MakeBuildingCard("Forge",   new BuildingData
                    { buildingType = BuildingType.ProductionEnergy, energyType = ElementType.Iron, energyPerSecond = 1f }, iron:1);
                var lifespring  = MakeBuildingCard("Life Spring", new BuildingData
                    { buildingType = BuildingType.ProductionEnergy, energyType = ElementType.Life, energyPerSecond = 1f }, life:1);

                // 嫄대Ъ ???좊떅 ?앹궛
                var barracks    = MakeBuildingCard("Barracks", new BuildingData
                    { buildingType = BuildingType.ProductionUnit, unitToSpawn = swordsman, spawnInterval = 10f }, iron:2);
                var magicCircle = MakeBuildingCard("Magic Circle", new BuildingData
                    { buildingType = BuildingType.ProductionUnit, unitToSpawn = mage, spawnInterval = 15f }, fire:2, life:1);

                // 湲곕낯 ??12??
                d.cards = new CardData[]
                {
                    swordsman, archer, knight, mage,
                    healer, lightning, portalBomb, fireTower,
                    giant, dragon, skeleton, dwarf
                };

                // Inspector ?꾨뱶 ?곗꽑, ?놁쑝硫?Resources/Prefabs/Units/{移대뱶紐?.prefab ?먮룞 濡쒕뱶
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
                    new BuffEffect { displayName = "怨듦꺽 踰꾪봽", attackMultiplier = 2.0f, speedMultiplier = 1.0f, duration = 8f  },
                    new BuffEffect { displayName = "?띾룄 踰꾪봽", attackMultiplier = 1.0f, speedMultiplier = 1.8f, duration = 12f },
                    new BuffEffect { displayName = "洹좏삎 踰꾪봽", attackMultiplier = 1.5f, speedMultiplier = 1.2f, duration = 10f }
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

            var titleTxt = MakeText(_startMenuGo.transform, "Title", "Lottery Fantasy", new Vector2(0, 180f), 64);
            titleTxt.color = new Color(1f, 0.9f, 0.3f);

            var subTxt = MakeText(_startMenuGo.transform, "Sub", "Choose a game mode", new Vector2(0, 90f), 28);
            subTxt.color = new Color(0.7f, 0.78f, 1f);

            var battleBtn = MakeButton(_startMenuGo.transform, "BattleBtn",
                "BATTLE MODE\nDestroy the enemy base",
                new Vector2(0, -20f), new Vector2(380f, 80f));
            UIArtKit.Apply(battleBtn, UIArtSprite.BlueButton);
            battleBtn.onClick.AddListener(() => Launch(GameMode.Battle));

            var survBtn = MakeButton(_startMenuGo.transform, "SurvBtn",
                "SURVIVAL MODE\nEndure the waves",
                new Vector2(0, -130f), new Vector2(380f, 80f));
            UIArtKit.Apply(survBtn, UIArtSprite.GoldButton);
            survBtn.onClick.AddListener(() => Launch(GameMode.Survival));
        }

        // ============================================================
        //  Game Launch
        // ============================================================

        private void Launch(GameMode mode)
        {
            if (_startMenuGo != null) { Destroy(_startMenuGo); _startMenuGo = null; }
            bool isSurvival = mode == GameMode.Survival;

            // --- 紐⑤뱶蹂?諛곌꼍 ?곸슜 ---
            if (Camera.main != null && _bgCfg != null)
                Camera.main.backgroundColor = isSurvival ? _bgCfg.survivalSkyColor : _bgCfg.battleSkyColor;
            else if (Camera.main != null)
                Camera.main.backgroundColor = isSurvival ? new Color(0.10f, 0.08f, 0.18f) : new Color(0.48f, 0.72f, 0.88f);
            GameVisualKit.AddArenaBackdrop(isSurvival);

            // --- Arenas ---
            var pVillage = MakeVillage("PlayerVillage", new Vector3(-7.5f, 0, 0), true);
            var eVillage = isSurvival ? null : MakeVillage("EnemyVillage", new Vector3(7.5f, 0, 0), false);
            var pSpawn   = Spawn("PlayerSpawn", new Vector3(-2.5f, 0, 0));
            var eSpawn   = Spawn("EnemySpawn",  isSurvival ? new Vector3(5.5f, 0, 0) : new Vector3(2.5f, 0, 0));
            var sceneryRoot = new GameObject("ArenaScenery");
            GameVisualKit.AddArenaScenery(sceneryRoot.transform, isSurvival);

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
                GameVisualKit.AttachPortalVisual(portalGo);
            }

            // 鍮꾪솢?깊솕瑜?AddComponent ?꾩뿉 ?댁빞 Awake()媛 ?ㅽ뻾?섏? ?딆쓬.
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

            // ?쒖옉 移대뱶 1???쒕뜡 吏湲?
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

            // ?? ?곷떒 諛?諛곌꼍 (sibling 0, 媛???ㅼ뿉 ?뚮뜑) ?????????????????
            {
                var go = Child(canvasGo.transform, "TopBar");
                var rt = (RectTransform)go.transform;
                rt.anchorMin = new Vector2(0f, 1f); rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.sizeDelta = new Vector2(0f, 170f); rt.anchoredPosition = Vector2.zero;
                var img = go.AddComponent<Image>();
                img.color = new Color(0f, 0f, 0f, 0f); img.raycastTarget = false;
                go.transform.SetSiblingIndex(0);
                var bgo = Child(go.transform, "Border");
                var brt = (RectTransform)bgo.transform;
                brt.anchorMin = new Vector2(0f, 0f); brt.anchorMax = new Vector2(1f, 0f);
                brt.pivot = new Vector2(0.5f, 0f); brt.sizeDelta = new Vector2(0f, 3f); brt.anchoredPosition = Vector2.zero;
                var bi = bgo.AddComponent<Image>(); bi.color = new Color(0f, 0f, 0f, 0f); bi.raycastTarget = false;
            }

            // ?? ?섎떒 ?⑤꼸 諛곌꼍 (sibling 1) ????????????????????????????????
            {
                var go = Child(canvasGo.transform, "BottomPanel");
                var rt = (RectTransform)go.transform;
                rt.anchorMin = new Vector2(0f, 0f); rt.anchorMax = new Vector2(1f, 0f);
                rt.pivot = new Vector2(0.5f, 0f);
                rt.sizeDelta = new Vector2(0f, 280f); rt.anchoredPosition = Vector2.zero;
                var img = go.AddComponent<Image>();
                img.color = UIStyle.PanelDeep; img.raycastTarget = false;
                go.transform.SetSiblingIndex(1);
                var bgo = Child(go.transform, "Border");
                var brt = (RectTransform)bgo.transform;
                brt.anchorMin = new Vector2(0f, 1f); brt.anchorMax = new Vector2(1f, 1f);
                brt.pivot = new Vector2(0.5f, 1f); brt.sizeDelta = new Vector2(0f, 3f); brt.anchoredPosition = Vector2.zero;
                var bi = bgo.AddComponent<Image>(); bi.color = UIStyle.Stroke; bi.raycastTarget = false;
            }

            // ?? ArenaHUD ??????????????????????????????????????????????????
            var hudGo = Child(canvasGo.transform, "ArenaHUD");
            var hud   = hudGo.AddComponent<ArenaHUD>();
            MakeArt(hudGo.transform, "PlayerHpFrame", UIArtSprite.PlayerHpFrame, new Vector2(-610, 486), new Vector2(390, 80));
            MakeArt(hudGo.transform, "EnemyHpFrame", UIArtSprite.EnemyHpFrame, new Vector2(610, 486), new Vector2(390, 80));
            MakeArt(hudGo.transform, "TimerBanner", UIArtSprite.TitleBannerFrame, new Vector2(0, 500), new Vector2(430, 104));
            LabelText(hudGo.transform, "LabelPlayer", "??湲곗? HP",  new Vector2(-610, 508));
            LabelText(hudGo.transform, "LabelEnemy",  "??湲곗? HP",  new Vector2( 610, 508));
            hud.playerHpSlider  = MakeSlider(hudGo.transform, "PlayerHP", new Vector2(-610, 484), UIStyle.Green);
            hud.enemyHpSlider   = MakeSlider(hudGo.transform, "EnemyHP",  new Vector2( 610, 484), UIStyle.Red);
            hud.timerText       = MakeText(hudGo.transform, "Timer",  "3:00", new Vector2(0, 516), 36);
            hud.spinChargesText = MakeText(hudGo.transform, "Spins",  "x0",   new Vector2(0, 486), 21);
            hud.recordText = MakeText(hudGo.transform, "Record", RecordSystem.Summary(), new Vector2(0, 382), 16);
            hud.stageText  = MakeText(hudGo.transform, "Stage",  "STAGE 1",              new Vector2(0, 360), 18);
            // ??蹂닿린 踰꾪듉 ??EnergyHUD? 寃뱀튂吏 ?딅룄濡??쇱そ??諛곗튂
            var deckBtn = MakeButton(hudGo.transform, "DeckViewBtn", "??蹂닿린", new Vector2(610f, 484f), new Vector2(118f, 38f));
            UIArtKit.Apply(deckBtn, UIArtSprite.BlueTab);
            SetButtonAccent(deckBtn, UIStyle.Cyan);

            // ?? 諛곗튂 援ъ뿭 ?ㅻ쾭?덉씠 (湲곕낯 ?щ챸 ??HandUI媛 移대뱶 ?좏깮 ?쒖뿉留??쒖떆) ??
            var zoneGo = Child(canvasGo.transform, "DeployZone");
            var zoneRt = (RectTransform)zoneGo.transform;
            zoneRt.anchorMin = new Vector2(0f, 0f); zoneRt.anchorMax = new Vector2(0.48f, 1f);
            zoneRt.offsetMin = new Vector2(0f, 280f); zoneRt.offsetMax = new Vector2(0f, -170f);
            var zoneImg = zoneGo.AddComponent<Image>();
            zoneImg.color = new Color(0.3f, 0.75f, 1f, 0f); // 湲곕낯 ?꾩쟾 ?щ챸
            zoneImg.raycastTarget = false;
            {
                var bgo = Child(zoneGo.transform, "Border");
                var brt = (RectTransform)bgo.transform;
                brt.anchorMin = new Vector2(1f, 0f); brt.anchorMax = new Vector2(1f, 1f);
                brt.pivot = new Vector2(1f, 0.5f); brt.sizeDelta = new Vector2(4f, 0f); brt.anchoredPosition = Vector2.zero;
                var bi = bgo.AddComponent<Image>(); bi.color = new Color(0.3f, 0.85f, 1f, 0f); bi.raycastTarget = false;
            }

            // ?? SlotMachineUI (?섎떒 ?ㅻⅨ履??덈컲) ?????????????????????????
            var slotGo = Child(canvasGo.transform, "SlotMachineUI");
            var slotUI = slotGo.AddComponent<SlotMachineUI>();

            // ?щ’ ?대? ?⑤꼸 ???ㅻⅨ履??덈컲(x=0~960) ?꾩껜
            {
                var go = Child(slotGo.transform, "SlotPanel");
                var rt = (RectTransform)go.transform;
                rt.anchoredPosition = new Vector2(480f, -400f); rt.sizeDelta = new Vector2(918f, 258f);
                var img = go.AddComponent<Image>();
                UIArtKit.Apply(img, UIArtSprite.SlotMachineFrame);
                img.raycastTarget = false;
                var bgo = Child(go.transform, "TopBorder");
                var brt = (RectTransform)bgo.transform;
                brt.anchorMin = new Vector2(0f, 1f); brt.anchorMax = new Vector2(1f, 1f);
                brt.pivot = new Vector2(0.5f, 1f); brt.sizeDelta = new Vector2(0f, 2f); brt.anchoredPosition = Vector2.zero;
                var bi = bgo.AddComponent<Image>(); bi.color = UIStyle.Stroke; bi.raycastTarget = false;
                AddEdgeLine(go.transform, "BottomBorder", RectEdge.Bottom, 2f, UIStyle.StrokeSoft);
            }

            var slotHeader = MakeText(slotGo.transform, "SlotHeader", "[ SLOT MACHINE ]", new Vector2(480f, -281f), 14);
            slotHeader.color = UIStyle.TextMuted;

            var reelNameTexts = new Text[3];
            var reelIconImages = new Image[3];
            float[] reelX     = { 292f, 480f, 668f };
            string[] reelLbls = { "由?1", "由?2", "由?3" };
            for (int i = 0; i < 3; i++)
            {
                var reelBox = Child(slotGo.transform, $"Reel{i}");
                var reelRt  = (RectTransform)reelBox.transform;
                reelRt.anchoredPosition = new Vector2(reelX[i], -333f);
                reelRt.sizeDelta        = new Vector2(172f, 112f);
                var reelImg = reelBox.AddComponent<Image>();
                UIArtKit.Apply(reelImg, UIArtSprite.SquareIconFrame);
                AddEdgeLine(reelBox.transform, "TopAccent", RectEdge.Top, 3f, i == 0 ? new Color(1f, 0.48f, 0.18f, 0.95f) : i == 1 ? new Color(0.55f, 0.80f, 1f, 0.95f) : new Color(0.25f, 1f, 0.50f, 0.95f));
                AddEdgeLine(reelBox.transform, "BottomShade", RectEdge.Bottom, 2f, UIStyle.StrokeSoft);
                MakeText(reelBox.transform, "Label", reelLbls[i], new Vector2(0, 43), 11).color = UIStyle.TextMuted;
                var iconGo = Child(reelBox.transform, "Icon");
                var iconRt = (RectTransform)iconGo.transform;
                iconRt.anchoredPosition = new Vector2(0f, -3f);
                iconRt.sizeDelta = new Vector2(112f, 82f);
                reelIconImages[i] = iconGo.AddComponent<Image>();
                reelIconImages[i].sprite = UIArtKit.ElementIcon(i == 0 ? ElementType.Fire : i == 1 ? ElementType.Iron : ElementType.Life);
                reelIconImages[i].preserveAspect = true;
                reelNameTexts[i] = MakeText(reelBox.transform, "Value", "", new Vector2(0, -48), 13);
                reelNameTexts[i].fontStyle = FontStyle.Bold;
            }
            slotUI.reelLabels = reelNameTexts;
            slotUI.reelIcons  = reelIconImages;

            slotUI.resultText = MakeText(slotGo.transform, "Result", "", new Vector2(480f, -424f), 20);
            slotUI.spinButton = MakeButton(slotGo.transform, "SpinBtn", "STOP", new Vector2(392f, -462f), new Vector2(175f, 48f));
            UIArtKit.Apply(slotUI.spinButton, UIArtSprite.RedButton);
            SetButtonAccent(slotUI.spinButton, UIStyle.Green);

            var autoBtn = MakeButton(slotGo.transform, "AutoBtn", "AUTO\nOFF", new Vector2(578f, -462f), new Vector2(74f, 48f));
            UIArtKit.Apply(autoBtn, UIArtSprite.DarkSquareButton);
            autoBtn.GetComponentInChildren<Text>().fontSize = 13;
            slotUI.autoButton      = autoBtn;
            slotUI.autoButtonLabel = autoBtn.GetComponentInChildren<Text>();

            // ?됱슫 寃뚯씠吏
            {
                var ggo = Child(slotGo.transform, "LuckGauge");
                var grt = (RectTransform)ggo.transform;
                grt.anchoredPosition = new Vector2(480f, -397f); grt.sizeDelta = new Vector2(430f, 26f);
                var bgImg = ggo.AddComponent<Image>();
                UIArtKit.Apply(bgImg, UIArtSprite.ResourceCounterFrame);
                bgImg.color = new Color(0.75f, 0.82f, 1f, 0.95f);
                bgImg.raycastTarget = false;
                var fgo = Child(ggo.transform, "Fill");
                var frt = (RectTransform)fgo.transform;
                frt.anchorMin = new Vector2(0f, 0.5f); frt.anchorMax = new Vector2(0f, 0.5f);
                frt.pivot = new Vector2(0f, 0.5f); frt.sizeDelta = new Vector2(410f, 14f); frt.anchoredPosition = new Vector2(10f, -1f);
                var fi = fgo.AddComponent<Image>(); fi.color = new Color(1f, 0.78f, 0.16f, 0.95f); fi.raycastTarget = false;
                AddEdgeLine(ggo.transform, "LuckTopGlow", RectEdge.Top, 2f, new Color(1f, 0.88f, 0.22f, 0.85f));
                slotUI.luckGaugeFillRt = frt;
            }
            slotUI.luckChargeText = MakeText(slotGo.transform, "LuckCount", "", new Vector2(480f, -397f), 14);
            slotUI.luckChargeText.color = new Color(1f, 0.90f, 0.36f);
            slotUI.luckChargeText.fontStyle = FontStyle.Bold;

            // ?? HandUI (?섎떒 ?쇱そ ?덈컲) ???????????????????????????????????
            var handGo = Child(canvasGo.transform, "HandUI");
            var handUI = handGo.AddComponent<HandUI>();
            handUI.arenaSystem       = arena;
            handUI.deployZoneOverlay = zoneImg;
            handUI.cardButtons = new Button[4];
            handUI.cardIcons   = new Image[4];
            handUI.cardNames   = new Text[4];

            MakeText(handGo.transform, "HandHeader", "", new Vector2(-480f, -274f), 14)
                .color = new Color(1f, 1f, 1f, 0f);

            // 移대뱶: ?쇱そ ?덈컲(-960~0)??4?? 140px 媛꾧꺽
            float[] cardXPos = { -735f, -555f, -375f, -195f };
            for (int i = 0; i < 4; i++)
            {
                var cardGo = Child(handGo.transform, $"CardSlot{i}");
                var rt     = (RectTransform)cardGo.transform;
                rt.anchoredPosition = new Vector2(cardXPos[i], -400f);
                rt.sizeDelta        = new Vector2(158f, 232f);

                var bg  = cardGo.AddComponent<Image>();
                UIArtKit.Apply(bg, i == 0 ? UIArtSprite.CardFrameBronze :
                    i == 1 ? UIArtSprite.CardFrameSilver :
                    i == 2 ? UIArtSprite.CardFrameGold : UIArtSprite.CardFramePurple);
                var btn = cardGo.AddComponent<Button>();
                btn.targetGraphic = bg;
                btn.colors = UIStyle.AccentButtonColors(UIStyle.Cyan);
                handUI.cardButtons[i] = btn;
                AddEdgeLine(cardGo.transform, "CardTopAccent", RectEdge.Top, 4f, UIStyle.Gold);
                AddEdgeLine(cardGo.transform, "CardBottomShade", RectEdge.Bottom, 2f, UIStyle.StrokeSoft);

                var drag = cardGo.AddComponent<CardDragHandler>();
                drag.slotIndex   = i;
                drag.arenaSystem = arena;
                drag.font        = SharedFont();

                var iconGo = Child(cardGo.transform, "Icon");
                var iconRt = (RectTransform)iconGo.transform;
                iconRt.anchoredPosition = new Vector2(0, 58);
                iconRt.sizeDelta        = new Vector2(108, 108);
                handUI.cardIcons[i] = iconGo.AddComponent<Image>();
                iconGo.SetActive(false);

                var nameText = MakeText(cardGo.transform, "Name", "---", new Vector2(0, -38), 17);
                ((RectTransform)nameText.transform).sizeDelta = new Vector2(148f, 112f);
                nameText.supportRichText = true;
                nameText.lineSpacing     = 1.2f;
                handUI.cardNames[i] = nameText;
            }

            // 醫뚯슦 援щ텇??(HAND | SLOT)
            {
                var go = Child(canvasGo.transform, "BtmDivider");
                var rt = (RectTransform)go.transform;
                rt.anchorMin = new Vector2(0.5f, 0f); rt.anchorMax = new Vector2(0.5f, 0f);
                rt.pivot = new Vector2(0.5f, 0f); rt.sizeDelta = new Vector2(2f, 278f); rt.anchoredPosition = new Vector2(0f, 0f);
                var img = go.AddComponent<Image>(); img.color = new Color(0f, 0f, 0f, 0f); img.raycastTarget = false;
            }

            // ?? ResultUI ?????????????????????????????????????????????????
            var resultUIGo = Child(canvasGo.transform, "ResultUI");
            StretchFull((RectTransform)resultUIGo.transform);
            var resultUI = resultUIGo.AddComponent<ResultUI>();
            var panelGo = Child(resultUIGo.transform, "Panel");
            StretchFull((RectTransform)panelGo.transform);
            panelGo.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.86f);
            MakeArt(panelGo.transform, "ResultFrame", UIArtSprite.ModalPanelFrame, Vector2.zero, new Vector2(560f, 330f));
            resultUI.panel       = panelGo;
            resultUI.resultText  = MakeText(panelGo.transform, "ResultText", "", Vector2.zero, 70);
            resultUI.retryButton = MakeButton(panelGo.transform, "RetryBtn", "RETRY", new Vector2(0, -110), new Vector2(220, 65));
            UIArtKit.Apply(resultUI.retryButton, UIArtSprite.GoldButton);
            SetButtonAccent(resultUI.retryButton, UIStyle.Gold);

            // ?? DeckViewer ????????????????????????????????????????????????
            var dvGo = Child(canvasGo.transform, "DeckViewer");
            var dv   = dvGo.AddComponent<DeckViewerUI>();
            var dvPanel   = Child(dvGo.transform, "Panel");
            var dvPanelRt = (RectTransform)dvPanel.transform;
            dvPanelRt.anchoredPosition = Vector2.zero; dvPanelRt.sizeDelta = new Vector2(960f, 540f);
            var dvPanelImage = dvPanel.AddComponent<Image>();
            UIArtKit.Apply(dvPanelImage, UIArtSprite.ModalPanelFrame);
            AddEdgeLine(dvPanel.transform, "TopAccent", RectEdge.Top, 4f, UIStyle.Cyan);
            MakeText(dvPanel.transform, "Title", "?щ’ 移대뱶 紐⑸줉", new Vector2(0, 225f), 26).color = UIStyle.Cyan;
            var dvContent = MakeText(dvPanel.transform, "Content", "", new Vector2(-20f, -20f), 18);
            dvContent.alignment = TextAnchor.UpperLeft;
            ((RectTransform)dvContent.transform).sizeDelta = new Vector2(900f, 420f);
            dvContent.horizontalOverflow = HorizontalWrapMode.Wrap;
            MakeButton(dvPanel.transform, "CloseBtn", "?リ린", new Vector2(420f, 220f), new Vector2(80f, 38f))
                .onClick.AddListener(() => dv.Toggle());
            dv.panel = dvPanel; dv.contentText = dvContent;
            dv.Setup(GameManager.Instance.deckConfig);
            deckBtn.onClick.AddListener(() => dv.Toggle());

            // ?? ScreenFlash ???????????????????????????????????????????????
            var flashGo = Child(canvasGo.transform, "ScreenFlash");
            var flashRt = (RectTransform)flashGo.transform;
            flashRt.anchorMin = Vector2.zero; flashRt.anchorMax = Vector2.one; flashRt.sizeDelta = Vector2.zero;
            var flashImg = flashGo.AddComponent<Image>();
            flashImg.color = new Color(1f, 1f, 1f, 0f); flashImg.raycastTarget = false;
            flashGo.AddComponent<ScreenFlash>();

            // ?? EnergyHUD (?곗긽???낅┰ ?⑤꼸 ???곷떒 諛??꾩뿉 ?뚮뜑) ?????????
            var energyGo = Child(canvasGo.transform, "EnergyHUD");
            var energyRt = (RectTransform)energyGo.transform;
            energyRt.anchorMin        = new Vector2(0.5f, 0.5f);
            energyRt.anchorMax        = new Vector2(0.5f, 0.5f);
            energyRt.pivot            = new Vector2(0.5f, 0.5f);
            energyRt.anchoredPosition = new Vector2(0f, 432f);
            energyRt.sizeDelta        = new Vector2(360f, 78f);
            var energyBg = energyGo.AddComponent<Image>();
            UIArtKit.Apply(energyBg, UIArtSprite.TooltipFrame);
            energyBg.raycastTarget = false;
            // 湲덉깋 ?뚮몢由?(?곷떒)
            {
                var bgo = Child(energyGo.transform, "TopBorder");
                var brt = (RectTransform)bgo.transform;
                brt.anchorMin = new Vector2(0f, 1f); brt.anchorMax = new Vector2(1f, 1f);
                brt.pivot = new Vector2(0.5f, 1f); brt.sizeDelta = new Vector2(0f, 3f); brt.anchoredPosition = Vector2.zero;
                var bi = bgo.AddComponent<Image>(); bi.color = new Color(1f, 0.72f, 0.08f, 1f); bi.raycastTarget = false;
            }
            // 湲덉깋 ?뚮몢由?(?섎떒)
            {
                var bgo = Child(energyGo.transform, "BotBorder");
                var brt = (RectTransform)bgo.transform;
                brt.anchorMin = new Vector2(0f, 0f); brt.anchorMax = new Vector2(1f, 0f);
                brt.pivot = new Vector2(0.5f, 0f); brt.sizeDelta = new Vector2(0f, 2f); brt.anchoredPosition = Vector2.zero;
                var bi = bgo.AddComponent<Image>(); bi.color = new Color(1f, 0.72f, 0.08f, 0.7f); bi.raycastTarget = false;
            }
            // 湲덉깋 ?뚮몢由?(醫뚯륫)
            {
                var bgo = Child(energyGo.transform, "LeftBorder");
                var brt = (RectTransform)bgo.transform;
                brt.anchorMin = new Vector2(0f, 0f); brt.anchorMax = new Vector2(0f, 1f);
                brt.pivot = new Vector2(0f, 0.5f); brt.sizeDelta = new Vector2(2f, 0f); brt.anchoredPosition = Vector2.zero;
                var bi = bgo.AddComponent<Image>(); bi.color = new Color(1f, 0.72f, 0.08f, 0.7f); bi.raycastTarget = false;
            }
            // ?먮꼫吏 ?ㅻ뜑 ?쇰꺼
            var energyHeader = MakeEnergyLabel(energyGo.transform, "EnergyHeader", new Vector2(180f, 58f));
            energyHeader.text = "ELEMENT ENERGY"; energyHeader.fontSize = 14;
            energyHeader.color = new Color(1f, 0.82f, 0.18f);

            var energyHud = energyGo.AddComponent<EnergyHUD>();
            energyHud.fireText = MakeEnergyLabel(energyGo.transform, "FireText", new Vector2(80f, 25f), new Color(1f,  0.45f, 0.1f));
            energyHud.ironText = MakeEnergyLabel(energyGo.transform, "IronText", new Vector2(180f, 25f), new Color(0.6f, 0.8f, 1f));
            energyHud.lifeText = MakeEnergyLabel(energyGo.transform, "LifeText", new Vector2(280f, 25f), new Color(0.2f, 1f,  0.45f));
            ((RectTransform)energyHud.fireText.transform).sizeDelta = new Vector2(96f, 32f);
            ((RectTransform)energyHud.ironText.transform).sizeDelta = new Vector2(96f, 32f);
            ((RectTransform)energyHud.lifeText.transform).sizeDelta = new Vector2(96f, 32f);
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
            card.icon        = UIArtKit.CardIcon(card);
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
            card.icon         = UIArtKit.CardIcon(card);
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
            card.icon      = UIArtKit.CardIcon(card);
            return card;
        }

        static Village MakeVillage(string name, Vector3 pos, bool isPlayer)
        {
            var go = new GameObject(name);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeSprite(isPlayer ? Color.cyan : new Color(1f, 0.6f, 0f), 1f, 2.5f);
            var v = go.AddComponent<Village>();
            GameVisualKit.AttachVillageVisual(go, isPlayer);
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
            t.color = UIStyle.TextMuted;
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
            txt.fontSize           = 24;
            txt.color              = color == default ? Color.white : color;
            txt.alignment          = TextAnchor.MiddleCenter;
            txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            txt.verticalOverflow   = VerticalWrapMode.Overflow;
            txt.supportRichText    = true;
            return txt;
        }

        static Image MakeArt(Transform parent, string name, UIArtSprite spriteId, Vector2 pos, Vector2 size)
        {
            var go = Child(parent, name);
            var rt = (RectTransform)go.transform;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var img = go.AddComponent<Image>();
            UIArtKit.Apply(img, spriteId);
            img.raycastTarget = false;
            return img;
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
            bgImg.color = UIStyle.PanelDeep;
            var slider = go.AddComponent<Slider>();
            slider.minValue = 0f; slider.maxValue = 1f; slider.value = 1f;

            var fillArea = Child(go.transform, "Fill Area");
            var faRt     = (RectTransform)fillArea.transform;
            faRt.anchorMin = Vector2.zero; faRt.anchorMax = Vector2.one; faRt.sizeDelta = Vector2.zero;

            var fill    = Child(fillArea.transform, "Fill");
            var fillRt  = (RectTransform)fill.transform;
            fillRt.anchorMin = Vector2.zero; fillRt.anchorMax = Vector2.one; fillRt.sizeDelta = Vector2.zero;
            fill.AddComponent<Image>().color = fillColor;
            AddEdgeLine(go.transform, "SliderTopLight", RectEdge.Top, 2f, UIStyle.StrokeSoft);

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
            img.color = UIStyle.Darken(UIStyle.Green, 0.68f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.colors = UIStyle.AccentButtonColors(UIStyle.Green);
            UIArtKit.Apply(btn, UIArtSprite.BlueButton);
            var labelText = MakeText(go.transform, "Label", label, Vector2.zero, 22);
            labelText.color = Color.white;
            labelText.fontStyle = FontStyle.Bold;
            return btn;
        }

        enum RectEdge { Top, Bottom, Left, Right }

        static void SetButtonAccent(Button button, Color color)
        {
            if (button == null) return;
            button.colors = UIStyle.AccentButtonColors(color);
            if (button.targetGraphic is Image img)
                img.color = img.sprite != null ? Color.white : UIStyle.Darken(color, 0.68f);
        }

        static void AddEdgeLine(Transform parent, string name, RectEdge edge, float thickness, Color color)
        {
            var go = Child(parent, name);
            var rt = (RectTransform)go.transform;

            switch (edge)
            {
                case RectEdge.Top:
                    rt.anchorMin = new Vector2(0f, 1f);
                    rt.anchorMax = new Vector2(1f, 1f);
                    rt.pivot = new Vector2(0.5f, 1f);
                    rt.sizeDelta = new Vector2(0f, thickness);
                    break;
                case RectEdge.Bottom:
                    rt.anchorMin = new Vector2(0f, 0f);
                    rt.anchorMax = new Vector2(1f, 0f);
                    rt.pivot = new Vector2(0.5f, 0f);
                    rt.sizeDelta = new Vector2(0f, thickness);
                    break;
                case RectEdge.Left:
                    rt.anchorMin = new Vector2(0f, 0f);
                    rt.anchorMax = new Vector2(0f, 1f);
                    rt.pivot = new Vector2(0f, 0.5f);
                    rt.sizeDelta = new Vector2(thickness, 0f);
                    break;
                case RectEdge.Right:
                    rt.anchorMin = new Vector2(1f, 0f);
                    rt.anchorMax = new Vector2(1f, 1f);
                    rt.pivot = new Vector2(1f, 0.5f);
                    rt.sizeDelta = new Vector2(thickness, 0f);
                    break;
            }

            rt.anchoredPosition = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;
        }
    }
}

