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

        private static readonly string[] SpinPool   = { "🔥", "⚔", "💚", "🔥", "⚔" };
        private static readonly string[] StopLabels = { "🔒 릴 1 정지", "🔒 릴 2 정지", "🔒 릴 3 정지" };
        private static readonly Color    GoldColor  = new Color(1f, 0.85f, 0.1f);
        private static readonly Color    WhiteColor = Color.white;
        private bool _showingResult;

        private void Start()
        {
            spinButton.onClick.AddListener(OnStopClicked);
            StartCoroutine(AlwaysSpinLoop());
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;
            int charges = GameManager.Instance.SlotMachine.SpinCharges;
            int stopped = GameManager.Instance.StoppedReelCount;
            var label   = spinButton.GetComponentInChildren<Text>();
            spinButton.interactable = charges > 0 && stopped < 3 && !_showingResult;
            if (label != null)
                label.text = (stopped >= 3 || _showingResult)
                    ? "..."
                    : $"{StopLabels[stopped]}  (x{charges})";
        }

        private void OnStopClicked()
        {
            if (GameManager.Instance == null || _showingResult) return;
            GameManager.Instance.TryStopReel(); // 행운 1 소모, 릴 1개 정지
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

        // 항상 돌아가는 릴 루프 — 전체 게임 동안 중단 없이 순환
        private IEnumerator AlwaysSpinLoop()
        {
            while (true)
            {
                // 게임 매니저 & 첫 스핀 준비 대기
                while (GameManager.Instance == null || GameManager.Instance.PendingReels == null)
                    yield return null;

                if (resultText != null) resultText.text = "";
                for (int i = 0; i < reelLabels.Length; i++)
                    if (reelLabels[i] != null) reelLabels[i].text = "?";

                bool[] localStopped = { false, false, false };
                float  lastTick = 0f, elapsed = 0f;
                const float TickInterval = 0.07f;

                // 3개 모두 정지할 때까지 계속 회전
                while (true)
                {
                    elapsed += Time.deltaTime;

                    if (GameManager.Instance != null)
                    {
                        int sc    = GameManager.Instance.StoppedReelCount;
                        var reels = GameManager.Instance.PendingReels;

                        // 정지된 릴 고정
                        for (int i = 0; i < 3; i++)
                        {
                            if (!localStopped[i] && sc > i)
                            {
                                localStopped[i] = true;
                                if (reelLabels[i] != null && reels != null)
                                    reelLabels[i].text = SymbolOf(reels[i]);
                            }
                        }

                        // 회전 중인 릴 랜덤 심볼
                        if (elapsed - lastTick >= TickInterval)
                        {
                            lastTick = elapsed;
                            for (int i = 0; i < 3; i++)
                                if (!localStopped[i] && reelLabels[i] != null)
                                    reelLabels[i].text = SpinPool[Random.Range(0, SpinPool.Length)];
                        }

                        if (sc >= 3) break;
                    }
                    yield return null;
                }

                // 결과 확정 & 에너지 지급
                _showingResult = true;
                var energy = GameManager.Instance.PendingEnergy;
                GameManager.Instance.CommitSpin(); // 내부에서 BeginNewSpin() → 즉시 다음 스핀 준비

                var parts = new List<string>();
                if (energy.fire > 0) parts.Add($"🔥x{energy.fire}");
                if (energy.iron > 0) parts.Add($"⚔x{energy.iron}");
                if (energy.life > 0) parts.Add($"💚x{energy.life}");
                if (resultText != null)
                {
                    resultText.text     = string.Join("  ", parts);
                    resultText.color    = WhiteColor;
                    resultText.fontSize = 24;
                }

                bool isTriple = energy.fire >= 6 || energy.iron >= 6 || energy.life >= 6;
                if (isTriple)
                {
                    if (resultText != null) { resultText.color = GoldColor; resultText.fontSize = 32; }
                    ScreenFlash.Instance?.Play(new Color(1f, 0.8f, 0f), 0.55f, 0.1f, 0.45f);
                }

                yield return new WaitForSeconds(1.5f);

                if (resultText != null)
                {
                    resultText.text     = "";
                    resultText.color    = WhiteColor;
                    resultText.fontSize = 24;
                }
                _showingResult = false;
                // CommitSpin이 이미 BeginNewSpin() 호출 → 루프 재시작 시 즉시 새 스핀
            }
        }
    }
}
