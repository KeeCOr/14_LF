using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SlotDefense
{
    public class SlotMachineUI : MonoBehaviour
    {
        public Text[]        reelLabels;
        public Button        spinButton;
        public Text          resultText;
        public RectTransform luckGaugeFillRt;
        public Text          luckChargeText;
        public Button        autoButton;
        public Text          autoButtonLabel;

        private static readonly string[] SpinPool   = { "🔥", "⚔", "💚", "🔥", "⚔", "💚", "🔥" };
        private static readonly string[] StopLabels = { "🔒 릴 1 정지", "🔒 릴 2 정지", "🔒 릴 3 정지" };
        private static readonly Color    GoldColor  = UIStyle.Gold;
        private static readonly Color    WhiteColor = Color.white;
        private static readonly Color    ReelIdleColor = UIStyle.PanelRaised;
        private static readonly Color[]  ReelHighlight = {
            new Color(1f,  0.55f, 0.15f, 1f),  // fire
            new Color(0.5f, 0.75f, 1f,  1f),   // iron
            new Color(0.1f, 1f,   0.45f, 1f),  // life
        };

        private bool    _showingResult;
        private bool    _autoSpin;
        private float   _autoStopTimer;
        private Image[] _reelBgs;
        private bool[]  _reelLanding;

        private void Start()
        {
            _reelBgs     = new Image[3];
            _reelLanding = new bool[3];
            for (int i = 0; i < 3; i++)
                if (reelLabels != null && reelLabels[i] != null)
                    _reelBgs[i] = reelLabels[i].transform.parent.GetComponent<Image>();

            spinButton.onClick.AddListener(OnStopClicked);
            if (autoButton != null) autoButton.onClick.AddListener(OnAutoToggle);
            StartCoroutine(AlwaysSpinLoop());
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;
            var sm      = GameManager.Instance.SlotMachine;
            int charges = sm.SpinCharges;
            int stopped = GameManager.Instance.StoppedReelCount;
            bool canSpin = charges > 0 && stopped == 0 && !_showingResult;

            // 릴 정지 버튼 텍스트 & 활성화
            spinButton.interactable = canSpin;
            var label = spinButton.GetComponentInChildren<Text>();
            if (label != null)
                label.text = (stopped >= 3 || _showingResult)
                    ? "..."
                    : $"STOP  (x{charges})";

            // 버튼 색 — 가능할 때 초록 pulse
            var cb = spinButton.colors;
            if (canSpin)
            {
                float t = (Mathf.Sin(Time.time * 4f) + 1f) * 0.5f;
                cb.normalColor = Color.Lerp(UIStyle.Darken(UIStyle.Green, 0.62f), UIStyle.Green, t);
            }
            else
            {
                cb.normalColor = new Color(0.16f, 0.16f, 0.20f, 0.55f);
            }
            spinButton.colors = cb;

            // 행운 게이지 & 텍스트
            float ratio = sm.ChargeRatio;
            if (luckGaugeFillRt != null)
                luckGaugeFillRt.anchorMax = new Vector2(ratio, 1f);

            if (luckChargeText != null)
            {
                float next = sm.SecondsToNext;
                luckChargeText.text = charges >= sm.MaxSpinCharges
                    ? $"★ {charges} / {sm.MaxSpinCharges}  (최대)"
                    : $"★ {charges} / {sm.MaxSpinCharges}  ({next:F1}s)";
            }

            // 오토 버튼 색상 갱신
            if (autoButton != null)
            {
                var acb = autoButton.colors;
                acb.normalColor = _autoSpin
                    ? UIStyle.Darken(UIStyle.Gold, 0.82f)
                    : UIStyle.PanelRaised;
                autoButton.colors = acb;
                if (autoButtonLabel != null)
                    autoButtonLabel.text = _autoSpin ? "AUTO\nON" : "AUTO\nOFF";
            }

            // 오토 스핀 — 행운 있으면 자동으로 전체 릴 정지
            if (_autoSpin && canSpin && !_showingResult)
            {
                _autoStopTimer -= Time.deltaTime;
                if (_autoStopTimer <= 0f)
                {
                    GameManager.Instance.TryStopAllReels();
                    _autoStopTimer = 0.5f;
                }
            }
        }

        private void OnStopClicked()
        {
            if (GameManager.Instance == null || _showingResult) return;
            GameManager.Instance.TryStopAllReels();
        }

        private void OnAutoToggle()
        {
            _autoSpin = !_autoSpin;
            _autoStopTimer = 0f; // 켜자마자 즉시 첫 정지 시도
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

        private IEnumerator AlwaysSpinLoop()
        {
            while (true)
            {
                while (GameManager.Instance == null || GameManager.Instance.PendingReels == null)
                    yield return null;

                // 스핀 초기화
                for (int i = 0; i < 3; i++)
                {
                    _reelLanding[i] = false;
                    if (reelLabels[i] != null) { reelLabels[i].text = "?"; reelLabels[i].color = WhiteColor; reelLabels[i].transform.localScale = Vector3.one; }
                    if (_reelBgs[i]   != null) _reelBgs[i].color = ReelIdleColor;
                }
                if (resultText != null) resultText.text = "";

                bool[] localStopped = { false, false, false };
                float  elapsed = 0f, lastTick = 0f;
                const float TickInterval = 0.04f;

                while (true)
                {
                    elapsed += Time.deltaTime;

                    if (GameManager.Instance != null)
                    {
                        int sc    = GameManager.Instance.StoppedReelCount;
                        var reels = GameManager.Instance.PendingReels;

                        // 새로 정지된 릴 → 착지 연출 시작
                        for (int i = 0; i < 3; i++)
                        {
                            if (!localStopped[i] && sc > i)
                            {
                                localStopped[i] = true;
                                StartCoroutine(LandReel(i, SymbolOf(reels[i])));
                            }
                        }

                        // 회전 중인 릴 배경 pulse
                        float now = Time.time;
                        for (int i = 0; i < 3; i++)
                        {
                            if (!localStopped[i] && _reelBgs[i] != null)
                            {
                                float p = (Mathf.Sin(now * 7f + i * 2.1f) + 1f) * 0.5f;
                                _reelBgs[i].color = Color.Lerp(ReelIdleColor, new Color(0.14f, 0.27f, 0.50f, 0.97f), p);
                            }
                        }

                        // 회전 중인 릴 심볼 교체
                        if (elapsed - lastTick >= TickInterval)
                        {
                            lastTick = elapsed;
                            for (int i = 0; i < 3; i++)
                                if (!localStopped[i] && !_reelLanding[i] && reelLabels[i] != null)
                                    reelLabels[i].text = SpinPool[Random.Range(0, SpinPool.Length)];
                        }

                        if (sc >= 3) break;
                    }
                    yield return null;
                }

                // 착지 연출 완료 대기
                yield return new WaitUntil(() => !_reelLanding[0] && !_reelLanding[1] && !_reelLanding[2]);

                // 결과 확정 & 에너지 지급
                _showingResult = true;
                var energy = GameManager.Instance.PendingEnergy;
                GameManager.Instance.CommitSpin();

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
                    if (resultText != null)
                    {
                        resultText.text     = "★ JACKPOT! ★\n" + resultText.text;
                        resultText.color    = GoldColor;
                        resultText.fontSize = 36;
                    }
                    ScreenFlash.Instance?.Play(new Color(1f, 0.85f, 0f), 0.8f, 0.12f, 0.68f);
                    StartCoroutine(JackpotPulse());
                }

                yield return new WaitForSeconds(1.5f);

                if (resultText != null) { resultText.text = ""; resultText.color = WhiteColor; resultText.fontSize = 24; }
                _showingResult = false;
            }
        }

        // 릴 착지 연출: 슬로우다운 → 최종 심볼 → 바운스 → 컬러 플래시
        private IEnumerator LandReel(int i, string finalSymbol)
        {
            _reelLanding[i] = true;

            // 점점 느려지는 슬롯 플래시
            float[] delays = { 0.055f, 0.07f, 0.09f, 0.115f, 0.14f };
            foreach (var d in delays)
            {
                if (reelLabels[i] != null)
                    reelLabels[i].text = SpinPool[Random.Range(0, SpinPool.Length)];
                yield return new WaitForSeconds(d);
            }

            // 최종 심볼 확정
            if (reelLabels[i] != null)
                reelLabels[i].text = finalSymbol;

            // 스케일 바운스
            if (reelLabels[i] != null)
                yield return ScaleBounce(reelLabels[i].transform);

            // 배경 컬러 플래시
            if (_reelBgs[i] != null)
            {
                _reelBgs[i].color = ReelHighlight[i];
                yield return new WaitForSeconds(0.22f);
                // 정지 후 어둡게 안정
                float t = 0f;
                var settled = new Color(0.1f, 0.18f, 0.38f, 0.95f);
                while (t < 0.25f)
                {
                    t += Time.deltaTime;
                    _reelBgs[i].color = Color.Lerp(ReelHighlight[i], settled, t / 0.25f);
                    yield return null;
                }
                _reelBgs[i].color = settled;
            }

            _reelLanding[i] = false;
        }

        // 잭팟 연출: 릴 박스 색상을 금색으로 3회 점멸
        private IEnumerator JackpotPulse()
        {
                var goldColor = UIStyle.Gold;
            for (int pulse = 0; pulse < 3; pulse++)
            {
                foreach (var bg in _reelBgs) if (bg != null) bg.color = goldColor;
                yield return new WaitForSeconds(0.12f);
                foreach (var bg in _reelBgs) if (bg != null) bg.color = UIStyle.PanelRaised;
                yield return new WaitForSeconds(0.12f);
            }
        }

        private static IEnumerator ScaleBounce(Transform t)
        {
            float e = 0f;
            while (e < 0.08f) { e += Time.deltaTime; t.localScale = Vector3.one * Mathf.Lerp(1f, 1.4f, e / 0.08f); yield return null; }
            e = 0f;
            while (e < 0.10f) { e += Time.deltaTime; t.localScale = Vector3.one * Mathf.Lerp(1.4f, 1f, e / 0.10f); yield return null; }
            t.localScale = Vector3.one;
        }
    }
}
