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
        private int  _stopCount; // 플레이어가 멈춘 릴 수 (0~3)

        private static readonly string[] StopLabels = { "▌ 릴 1 정지", "▌ 릴 2 정지", "▌ 릴 3 정지" };

        private void Start() => spinButton.onClick.AddListener(OnButtonClicked);

        private void Update()
        {
            if (GameManager.Instance == null) return;
            int charges = GameManager.Instance.SlotMachine.SpinCharges;
            var label = spinButton.GetComponentInChildren<Text>();
            if (!_animating)
            {
                spinButton.interactable = charges > 0;
                if (label != null) label.text = $"SPIN  (행운 x{charges})";
            }
            else
            {
                spinButton.interactable = _stopCount < 3;
                if (label != null)
                    label.text = _stopCount < 3 ? StopLabels[_stopCount] : "...";
            }
        }

        private void OnButtonClicked()
        {
            if (!_animating)
            {
                if (GameManager.Instance == null) return;
                if (!GameManager.Instance.TryBeginSpin()) return;
                _stopCount = 0;
                StartCoroutine(SpinAnimation());
            }
            else if (_stopCount < 3)
            {
                _stopCount++;
            }
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

            var (reels, energy) = GameManager.Instance.PeekSpin();
            if (resultText != null) resultText.text = "";

            for (int i = 0; i < reelLabels.Length; i++)
                if (reelLabels[i] != null) reelLabels[i].text = "?";

            // 버튼을 3번 눌러 릴을 하나씩 수동으로 정지
            bool[] stopped = { false, false, false };
            float lastTick = 0f;
            const float TickInterval = 0.07f;
            float elapsed = 0f;

            while (!stopped[2])
            {
                elapsed += Time.deltaTime;

                // _stopCount가 증가할 때마다 해당 릴 고정
                for (int i = 0; i < 3; i++)
                {
                    if (!stopped[i] && _stopCount > i)
                    {
                        stopped[i] = true;
                        if (reelLabels[i] != null) reelLabels[i].text = SymbolOf(reels[i]);
                    }
                }

                if (elapsed - lastTick >= TickInterval)
                {
                    lastTick = elapsed;
                    for (int i = 0; i < 3; i++)
                    {
                        if (!stopped[i] && reelLabels[i] != null)
                            reelLabels[i].text = SpinPool[Random.Range(0, SpinPool.Length)];
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
