using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SlotDefense
{
    public class SlotMachineUI : MonoBehaviour
    {
        public Text[]  reelLabels;
        public Button  spinButton;
        public Text    resultText;

        private static readonly string[] SpinPool = { "🔥", "⚔", "💚", "🔥", "⚔" };
        private static readonly Color    GoldColor  = new Color(1f, 0.85f, 0.1f);
        private static readonly Color    WhiteColor = Color.white;
        private bool _animating;

        private void Start() => spinButton.onClick.AddListener(OnSpinClicked);

        private void Update()
        {
            if (GameManager.Instance == null) return;
            int charges = GameManager.Instance.SlotMachine.SpinCharges;
            spinButton.interactable = charges > 0 && !_animating;
            var label = spinButton.GetComponentInChildren<Text>();
            if (label != null) label.text = $"행운 소모 (x{charges})";
        }

        private void OnSpinClicked()
        {
            if (GameManager.Instance == null || _animating) return;
            if (!GameManager.Instance.TryBeginSpin()) return;
            StartCoroutine(SpinAnimation());
        }

        private static string SymbolOf(ElementType t)
        {
            switch (t)
            {
                case ElementType.Fire: return "🔥화";
                case ElementType.Iron: return "⚔철";
                case ElementType.Life: return "💚생";
                default:               return "?";
            }
        }

        private IEnumerator SpinAnimation()
        {
            _animating = true;
            spinButton.interactable = false;

            var (reels, energy) = GameManager.Instance.PeekSpin();
            if (resultText != null) resultText.text = "";

            for (int i = 0; i < reelLabels.Length; i++)
                if (reelLabels[i] != null) reelLabels[i].text = "?";

            float[] stopAt  = { 0.8f, 1.3f, 1.8f };
            bool[]  stopped = { false, false, false };
            float elapsed = 0f, lastTick = 0f;
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
                            if (reelLabels[i] != null) reelLabels[i].text = SymbolOf(reels[i]);
                        }
                        else
                        {
                            if (reelLabels[i] != null)
                                reelLabels[i].text = SpinPool[Random.Range(0, SpinPool.Length)];
                        }
                    }
                }
                yield return null;
            }

            GameManager.Instance.CommitSpin();

            var parts = new List<string>();
            if (energy.fire > 0) parts.Add($"🔥x{energy.fire}");
            if (energy.iron > 0) parts.Add($"⚔x{energy.iron}");
            if (energy.life > 0) parts.Add($"💚x{energy.life}");
            if (resultText != null)
            {
                resultText.text  = string.Join("  ", parts);
                resultText.color = WhiteColor;
                resultText.fontSize = 24;
            }

            bool isTriple = energy.fire >= 6 || energy.iron >= 6 || energy.life >= 6;
            if (isTriple && resultText != null)
            {
                resultText.color    = GoldColor;
                resultText.fontSize = 32;
                ScreenFlash.Instance?.Play(new Color(1f, 0.8f, 0f), 0.55f, 0.1f, 0.45f);
            }

            _animating = false;
            CancelInvoke(nameof(ClearResult));
            Invoke(nameof(ClearResult), 2.5f);
        }

        private void ClearResult()
        {
            if (resultText == null) return;
            resultText.text     = "";
            resultText.color    = WhiteColor;
            resultText.fontSize = 24;
        }
    }
}
