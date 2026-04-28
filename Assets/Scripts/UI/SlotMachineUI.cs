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
        private static readonly Color    GoldColor  = new Color(1f, 0.85f, 0.1f);
        private static readonly Color    WhiteColor = Color.white;

        private bool _animating;

        private void Start() => spinButton.onClick.AddListener(OnSpinClicked);

        private void OnEnable()  => GameEvents.OnGlobalBuffApplied += OnBuffApplied;
        private void OnDisable()
        {
            GameEvents.OnGlobalBuffApplied -= OnBuffApplied;
            CancelInvoke(nameof(ClearResult));
        }

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

            float[] stopAt   = { 0.8f, 1.3f, 1.8f };
            bool[]  stopped  = { false, false, false };
            float   elapsed  = 0f;
            float   lastTick = 0f;
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

            GameManager.Instance.CommitSpin();

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
