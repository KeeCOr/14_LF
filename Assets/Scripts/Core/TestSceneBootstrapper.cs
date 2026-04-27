using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace SlotDefense
{
    [DefaultExecutionOrder(-100)]
    public class TestSceneBootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install()
        {
            if (FindObjectOfType<GameManager>() != null) return;
            new GameObject("[TestBootstrap]").AddComponent<TestSceneBootstrapper>();
        }

        private void Awake()
        {
            // --- ScriptableObjects (in-memory) ---
            var monsterCfg = Inst<MonsterConfig>(m => { m.hp = 40f; m.damage = 6f; m.moveSpeed = 1.3f; m.xpReward = 50f; });

            var deckCfg = Inst<FixedDeckConfig>(d =>
            {
                // 카드 타입당 하나의 인스턴스를 공유해야 EvaluateReels의 참조 비교(==)가 작동함
                var swordsman = MakeCard("검사",  hp: 80,  dmg: 15, speed: 2f,   range: 1.5f, rate: 1f);
                var archer    = MakeCard("궁수",  hp: 50,  dmg: 10, speed: 1.5f, range: 5f,   rate: 2f);
                var knight    = MakeCard("기사",  hp: 120, dmg: 20, speed: 1.2f, range: 1f,   rate: 0.8f);
                d.cards = new CardData[12];
                for (int i = 0; i < 4; i++)  d.cards[i]    = swordsman;
                for (int i = 4; i < 8; i++)  d.cards[i]    = archer;
                for (int i = 8; i < 12; i++) d.cards[i]    = knight;
            });

            var buffCfg = Inst<GlobalBuffConfig>(b =>
                b.possibleBuffs = new[] { new BuffEffect { attackMultiplier = 1.5f, speedMultiplier = 1.2f, duration = 10f } });

            // --- Camera ---
            if (Camera.main == null)
            {
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                var cam = camGo.AddComponent<Camera>();
                cam.orthographic = true;
                cam.orthographicSize = 7f;
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.08f, 0.1f, 0.18f);
                cam.transform.position = new Vector3(0, 0, -10);
            }

            // --- EventSystem ---
            if (FindObjectOfType<EventSystem>() == null)
            {
                var esGo = new GameObject("EventSystem");
                esGo.AddComponent<EventSystem>();
                esGo.AddComponent<StandaloneInputModule>();
            }

            // --- Arenas ---
            var pVillage = MakeVillage("PlayerVillage", new Vector3(-7.5f, 0, 0), true);
            var eVillage = MakeVillage("EnemyVillage",  new Vector3( 7.5f, 0, 0), false);
            var pSpawn   = Spawn("PlayerSpawn", new Vector3(-2.5f, 0, 0));
            var eSpawn   = Spawn("EnemySpawn",  new Vector3( 2.5f, 0, 0));

            // --- Entity templates (inactive — instantiated clones are activated after Init) ---
            var mTemplate = MakeTemplate("MonsterTemplate", Color.red,  new Vector3(-999, -999, 0));
            mTemplate.AddComponent<MonsterController>();
            mTemplate.SetActive(false);

            var uTemplate = MakeTemplate("UnitTemplate", Color.cyan, new Vector3(-999, -999, 0));
            uTemplate.AddComponent<UnitController>();
            uTemplate.SetActive(false);

            // --- GameManager (inactive until deckConfig assigned) ---
            var gmGo = new GameObject("GameManager");
            gmGo.SetActive(false);
            var gm = gmGo.AddComponent<GameManager>();
            gm.deckConfig = deckCfg;
            gm.buffConfig = buffCfg;
            gmGo.SetActive(true); // Awake runs here; Instance is now set

            // Give player 3 starter spins so testing can start right away
            GameManager.Instance.SlotMachine.AddXP(300f);

            // --- ArenaSystem (Start deferred; GameManager.Instance already set) ---
            var arenaGo = new GameObject("ArenaSystem");
            var arena   = arenaGo.AddComponent<ArenaSystem>();
            arena.playerSpawnPoint = pSpawn;
            arena.enemySpawnPoint  = eSpawn;
            arena.playerVillage    = pVillage;
            arena.enemyVillage     = eVillage;
            arena.monsterPrefab    = mTemplate;
            arena.monsterConfig    = monsterCfg;
            arena.unitPrefab       = uTemplate;

            // --- TransferSystem ---
            var tsGo = new GameObject("TransferSystem");
            var ts   = tsGo.AddComponent<TransferSystem>();
            ts.arenaSystem = arena;

            // --- AIOpponent (inactive until deckConfig assigned) ---
            var aiGo = new GameObject("AIOpponent");
            aiGo.SetActive(false);
            var ai = aiGo.AddComponent<AIOpponent>();
            ai.deckConfig  = deckCfg;
            ai.buffConfig  = buffCfg;
            ai.arenaSystem = arena;
            ai.unitPrefab  = uTemplate;
            aiGo.SetActive(true);
            ai.AddStarterXP(240f); // 3 spins at 80 XP each

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
            scaler.uiScaleMode        = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            // ArenaHUD
            var hudGo = Child(canvasGo.transform, "ArenaHUD");
            var hud   = hudGo.AddComponent<ArenaHUD>();
            LabelTMP(hudGo.transform, "LabelPlayer", "내 기지 HP",  new Vector2(-340, 523));
            LabelTMP(hudGo.transform, "LabelEnemy",  "적 기지 HP",  new Vector2( 340, 523));
            hud.playerHpSlider  = MakeSlider(hudGo.transform, "PlayerHP",  new Vector2(-340, 490), Color.green);
            hud.enemyHpSlider   = MakeSlider(hudGo.transform, "EnemyHP",   new Vector2( 340, 490), Color.red);
            hud.timerText       = MakeTMP(hudGo.transform,    "Timer",     "3:00", new Vector2(0, 490), 36);
            hud.spinChargesText = MakeTMP(hudGo.transform,    "Spins",     "x0",   new Vector2(0, 450), 28);

            // SlotMachineUI
            var slotGo = Child(canvasGo.transform, "SlotMachineUI");
            var slotUI = slotGo.AddComponent<SlotMachineUI>();
            slotUI.reelImages = new Image[0];
            slotUI.reelNames  = new TextMeshProUGUI[0];
            LabelTMP(slotGo.transform, "SpinDesc", "몬스터 처치 -> XP 충전 -> 버튼 클릭 -> 카드 획득", new Vector2(0, -360));
            slotUI.spinButton = MakeButton(slotGo.transform, "SpinBtn", "SPIN", new Vector2(0, -410), new Vector2(220, 60));
            slotUI.resultText = MakeTMP(slotGo.transform,   "Result",  "",     new Vector2(0, -460), 22);

            // HandUI — 4 card slots across the bottom
            var handGo = Child(canvasGo.transform, "HandUI");
            var handUI = handGo.AddComponent<HandUI>();
            handUI.arenaSystem = arena;
            handUI.cardButtons = new Button[4];
            handUI.cardIcons   = new Image[4];
            handUI.cardNames   = new TextMeshProUGUI[4];
            LabelTMP(handGo.transform, "HandDesc", "카드 클릭으로 선택 (초록색) -> 게임 화면 클릭으로 유닛 배치", new Vector2(0, -488));

            for (int i = 0; i < 4; i++)
            {
                float xPos = -285f + i * 190f;
                var cardGo = Child(handGo.transform, $"CardSlot{i}");
                var rt     = (RectTransform)cardGo.transform;
                rt.anchoredPosition = new Vector2(xPos, -520f);
                rt.sizeDelta        = new Vector2(175f, 60f);

                var bg  = cardGo.AddComponent<Image>();
                bg.color = new Color(0.2f, 0.25f, 0.45f, 0.9f);
                var btn = cardGo.AddComponent<Button>();
                btn.targetGraphic = bg;
                handUI.cardButtons[i] = btn;

                var iconGo = Child(cardGo.transform, "Icon");
                var iconRt = (RectTransform)iconGo.transform;
                iconRt.anchoredPosition = new Vector2(-55, 0);
                iconRt.sizeDelta        = new Vector2(44, 44);
                handUI.cardIcons[i] = iconGo.AddComponent<Image>();
                iconGo.SetActive(false);

                handUI.cardNames[i] = MakeTMP(cardGo.transform, "Name", "---", new Vector2(20, 0), 18);
            }

            // ResultUI — wrapping panel so the ResultUI component stays active
            var resultUIGo = Child(canvasGo.transform, "ResultUI");
            StretchFull((RectTransform)resultUIGo.transform);
            var resultUI = resultUIGo.AddComponent<ResultUI>();

            var panelGo = Child(resultUIGo.transform, "Panel");
            StretchFull((RectTransform)panelGo.transform);
            var panelImg = panelGo.AddComponent<Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.82f);

            resultUI.panel       = panelGo;
            resultUI.resultText  = MakeTMP(panelGo.transform,    "ResultText", "",      Vector2.zero,        80);
            resultUI.retryButton = MakeButton(panelGo.transform,  "RetryBtn",  "RETRY", new Vector2(0, -110), new Vector2(220, 65));
        }

        // ============================================================
        //  Factories
        // ============================================================

        static T Inst<T>(System.Action<T> init) where T : ScriptableObject
        {
            var obj = ScriptableObject.CreateInstance<T>(); init(obj); return obj;
        }

        static CardData MakeCard(string name, float hp, float dmg, float speed, float range, float rate)
        {
            var card = ScriptableObject.CreateInstance<CardData>();
            card.cardName  = name;
            card.cardType  = CardType.Unit;
            card.unitStats = new UnitStats { hp = hp, damage = dmg, moveSpeed = speed, attackRange = range, attackRate = rate };
            return card;
        }

        static Village MakeVillage(string name, Vector3 pos, bool isPlayer)
        {
            var go = new GameObject(name);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeSprite(isPlayer ? Color.cyan : new Color(1f, 0.6f, 0f), 1f, 2.5f);
            var v = go.AddComponent<Village>();
            // isPlayerVillage is [SerializeField] private — set via reflection
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
            fillRt.anchorMin = Vector2.zero; fillRt.anchorMax = new Vector2(1, 1); fillRt.sizeDelta = Vector2.zero;
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = fillColor;

            slider.fillRect = fillRt;
            return slider;
        }

        static TextMeshProUGUI LabelTMP(Transform parent, string name, string text, Vector2 pos)
        {
            var tmp = MakeTMP(parent, name, text, pos, 17);
            tmp.color = new Color(0.75f, 0.85f, 1f);
            ((RectTransform)tmp.transform).sizeDelta = new Vector2(800, 36);
            return tmp;
        }

        static TextMeshProUGUI MakeTMP(Transform parent, string name, string text, Vector2 pos, int fontSize = 24)
        {
            var go = Child(parent, name);
            var rt = (RectTransform)go.transform;
            rt.anchoredPosition = pos;
            rt.sizeDelta        = new Vector2(400, 60);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text      = text;
            tmp.fontSize  = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color     = Color.white;
            return tmp;
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
            MakeTMP(go.transform, "Label", label, Vector2.zero, 26);
            return btn;
        }
    }
}
