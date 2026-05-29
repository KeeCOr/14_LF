using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotDefense
{
    public class SlotMachineUI : MonoBehaviour
    {
        public Text[] reelLabels;
        public Image[] reelIcons;
        public Button spinButton;
        public Text resultText;
        public RectTransform luckGaugeFillRt;
        public Text luckChargeText;
        public Button autoButton;
        public Text autoButtonLabel;

        private static readonly ElementType[] SpinPool =
        {
            ElementType.Fire, ElementType.Iron, ElementType.Life,
            ElementType.Fire, ElementType.Iron, ElementType.Life,
            ElementType.Fire
        };

        private static readonly Color GoldColor = UIStyle.Gold;
        private static readonly Color WhiteColor = Color.white;
        private static readonly Color ReelIdleColor = UIStyle.PanelRaised;

        private static readonly Color[] ReelHighlight =
        {
            new Color(1f, 0.55f, 0.15f, 1f),
            new Color(0.5f, 0.75f, 1f, 1f),
            new Color(0.1f, 1f, 0.45f, 1f)
        };

        private bool _showingResult;
        private bool _autoSpin;
        private float _autoStopTimer;
        private Image[] _reelBgs;
        private bool[] _reelLanding;

        private void Start()
        {
            _reelBgs = new Image[3];
            _reelLanding = new bool[3];

            if (reelIcons == null || reelIcons.Length < 3)
                reelIcons = new Image[3];

            for (int i = 0; i < 3; i++)
            {
                if (reelLabels != null && reelLabels[i] != null)
                    _reelBgs[i] = reelLabels[i].transform.parent.GetComponent<Image>();
            }

            spinButton.onClick.AddListener(OnStopClicked);
            if (autoButton != null) autoButton.onClick.AddListener(OnAutoToggle);
            StartCoroutine(AlwaysSpinLoop());
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;
            var sm = GameManager.Instance.SlotMachine;
            int charges = sm.SpinCharges;
            int stopped = GameManager.Instance.StoppedReelCount;
            bool canSpin = charges > 0 && stopped == 0 && !_showingResult;

            spinButton.interactable = canSpin;
            var label = spinButton.GetComponentInChildren<Text>();
            if (label != null)
                label.text = stopped >= 3 || _showingResult ? "..." : $"STOP  (x{charges})";

            var cb = spinButton.colors;
            cb.normalColor = canSpin
                ? Color.Lerp(UIStyle.Darken(UIStyle.Green, 0.62f), UIStyle.Green, (Mathf.Sin(Time.time * 4f) + 1f) * 0.5f)
                : new Color(0.16f, 0.16f, 0.20f, 0.55f);
            spinButton.colors = cb;

            if (luckGaugeFillRt != null)
                luckGaugeFillRt.anchorMax = new Vector2(sm.ChargeRatio, 1f);

            if (luckChargeText != null)
            {
                float next = sm.SecondsToNext;
                luckChargeText.text = charges >= sm.MaxSpinCharges
                    ? $"LUCK {charges} / {sm.MaxSpinCharges}  MAX"
                    : $"LUCK {charges} / {sm.MaxSpinCharges}  {next:F1}s";
            }

            if (autoButton != null)
            {
                var acb = autoButton.colors;
                acb.normalColor = _autoSpin ? UIStyle.Darken(UIStyle.Gold, 0.82f) : UIStyle.PanelRaised;
                autoButton.colors = acb;
                if (autoButtonLabel != null)
                    autoButtonLabel.text = _autoSpin ? "AUTO\nON" : "AUTO\nOFF";
            }

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

        public static Sprite ReelSprite(ElementType element) => UIArtKit.ElementIcon(element);

        private void OnStopClicked()
        {
            if (GameManager.Instance == null || _showingResult) return;
            GameManager.Instance.TryStopAllReels();
        }

        private void OnAutoToggle()
        {
            _autoSpin = !_autoSpin;
            _autoStopTimer = 0f;
        }

        private static string SymbolOf(ElementType element)
        {
            switch (element)
            {
                case ElementType.Fire:
                    return "FIRE";
                case ElementType.Iron:
                    return "IRON";
                case ElementType.Life:
                    return "LIFE";
                default:
                    return "?";
            }
        }

        private static Color ColorOf(ElementType element)
        {
            switch (element)
            {
                case ElementType.Fire:
                    return new Color(1f, 0.58f, 0.22f, 1f);
                case ElementType.Iron:
                    return new Color(0.64f, 0.82f, 1f, 1f);
                case ElementType.Life:
                    return new Color(0.28f, 1f, 0.55f, 1f);
                default:
                    return WhiteColor;
            }
        }

        private void SetReelVisual(int index, ElementType element, bool spinning)
        {
            if (reelLabels != null && reelLabels[index] != null)
            {
                reelLabels[index].text = spinning ? "" : SymbolOf(element);
                reelLabels[index].color = ColorOf(element);
            }

            if (reelIcons != null && reelIcons[index] != null)
            {
                reelIcons[index].sprite = ReelSprite(element);
                reelIcons[index].color = ColorOf(element);
                reelIcons[index].preserveAspect = true;
                reelIcons[index].gameObject.SetActive(true);

                if (spinning)
                {
                    float angle = (Time.time * 720f + index * 87f) % 360f;
                    reelIcons[index].transform.localEulerAngles = new Vector3(0f, 0f, angle);
                    float pulse = 0.86f + Mathf.Sin(Time.time * 12f + index) * 0.08f;
                    reelIcons[index].transform.localScale = Vector3.one * pulse;
                }
                else
                {
                    reelIcons[index].transform.localEulerAngles = Vector3.zero;
                    reelIcons[index].transform.localScale = Vector3.one;
                }
            }
        }

        private IEnumerator AlwaysSpinLoop()
        {
            while (true)
            {
                while (GameManager.Instance == null || GameManager.Instance.PendingReels == null)
                    yield return null;

                for (int i = 0; i < 3; i++)
                {
                    _reelLanding[i] = false;
                    if (reelLabels[i] != null)
                    {
                        reelLabels[i].text = "";
                        reelLabels[i].color = WhiteColor;
                        reelLabels[i].transform.localScale = Vector3.one;
                    }
                    if (reelIcons != null && reelIcons[i] != null)
                    {
                        reelIcons[i].transform.localScale = Vector3.one;
                        reelIcons[i].transform.localEulerAngles = Vector3.zero;
                    }
                    if (_reelBgs[i] != null) _reelBgs[i].color = ReelIdleColor;
                }
                if (resultText != null) resultText.text = "";

                bool[] localStopped = { false, false, false };
                float elapsed = 0f;
                float lastTick = 0f;
                const float TickInterval = 0.035f;

                while (true)
                {
                    elapsed += Time.deltaTime;

                    if (GameManager.Instance != null)
                    {
                        int sc = GameManager.Instance.StoppedReelCount;
                        var reels = GameManager.Instance.PendingReels;

                        for (int i = 0; i < 3; i++)
                        {
                            if (!localStopped[i] && sc > i)
                            {
                                localStopped[i] = true;
                                StartCoroutine(LandReel(i, reels[i]));
                            }
                        }

                        float now = Time.time;
                        for (int i = 0; i < 3; i++)
                        {
                            if (!localStopped[i] && _reelBgs[i] != null)
                            {
                                float p = (Mathf.Sin(now * 7f + i * 2.1f) + 1f) * 0.5f;
                                _reelBgs[i].color = Color.Lerp(ReelIdleColor, new Color(0.14f, 0.27f, 0.50f, 0.97f), p);
                            }
                        }

                        if (elapsed - lastTick >= TickInterval)
                        {
                            lastTick = elapsed;
                            for (int i = 0; i < 3; i++)
                            {
                                if (!localStopped[i] && !_reelLanding[i])
                                    SetReelVisual(i, SpinPool[Random.Range(0, SpinPool.Length)], spinning: true);
                            }
                        }

                        if (sc >= 3) break;
                    }
                    yield return null;
                }

                yield return new WaitUntil(() => !_reelLanding[0] && !_reelLanding[1] && !_reelLanding[2]);

                _showingResult = true;
                var energy = GameManager.Instance.PendingEnergy;
                GameManager.Instance.CommitSpin();

                var parts = new List<string>();
                if (energy.fire > 0) parts.Add($"FIRE x{energy.fire}");
                if (energy.iron > 0) parts.Add($"IRON x{energy.iron}");
                if (energy.life > 0) parts.Add($"LIFE x{energy.life}");
                if (resultText != null)
                {
                    resultText.text = string.Join("  ", parts);
                    resultText.color = WhiteColor;
                    resultText.fontSize = 24;
                }

                bool isTriple = energy.fire >= 6 || energy.iron >= 6 || energy.life >= 6;
                if (isTriple)
                {
                    if (resultText != null)
                    {
                        resultText.text = "JACKPOT!\n" + resultText.text;
                        resultText.color = GoldColor;
                        resultText.fontSize = 36;
                    }
                    ScreenFlash.Instance?.Play(new Color(1f, 0.85f, 0f), 0.8f, 0.12f, 0.68f);
                    StartCoroutine(JackpotPulse());
                }

                yield return new WaitForSeconds(1.5f);

                if (resultText != null)
                {
                    resultText.text = "";
                    resultText.color = WhiteColor;
                    resultText.fontSize = 24;
                }
                _showingResult = false;
            }
        }

        private IEnumerator LandReel(int index, ElementType finalElement)
        {
            _reelLanding[index] = true;

            float[] delays = { 0.045f, 0.065f, 0.09f, 0.12f, 0.15f };
            foreach (var delay in delays)
            {
                SetReelVisual(index, SpinPool[Random.Range(0, SpinPool.Length)], spinning: true);
                yield return new WaitForSeconds(delay);
            }

            SetReelVisual(index, finalElement, spinning: false);

            if (reelIcons != null && reelIcons[index] != null)
                yield return ScaleBounce(reelIcons[index].transform, 1.28f);
            else if (reelLabels[index] != null)
                yield return ScaleBounce(reelLabels[index].transform, 1.28f);

            if (_reelBgs[index] != null)
            {
                _reelBgs[index].color = ReelHighlight[index];
                yield return new WaitForSeconds(0.22f);
                float t = 0f;
                var settled = new Color(0.1f, 0.18f, 0.38f, 0.95f);
                while (t < 0.25f)
                {
                    t += Time.deltaTime;
                    _reelBgs[index].color = Color.Lerp(ReelHighlight[index], settled, t / 0.25f);
                    yield return null;
                }
                _reelBgs[index].color = settled;
            }

            _reelLanding[index] = false;
        }

        private IEnumerator JackpotPulse()
        {
            for (int pulse = 0; pulse < 3; pulse++)
            {
                foreach (var bg in _reelBgs)
                    if (bg != null) bg.color = GoldColor;
                yield return new WaitForSeconds(0.12f);
                foreach (var bg in _reelBgs)
                    if (bg != null) bg.color = UIStyle.PanelRaised;
                yield return new WaitForSeconds(0.12f);
            }
        }

        private static IEnumerator ScaleBounce(Transform target, float peak)
        {
            float elapsed = 0f;
            while (elapsed < 0.08f)
            {
                elapsed += Time.deltaTime;
                target.localScale = Vector3.one * Mathf.Lerp(1f, peak, elapsed / 0.08f);
                yield return null;
            }
            elapsed = 0f;
            while (elapsed < 0.10f)
            {
                elapsed += Time.deltaTime;
                target.localScale = Vector3.one * Mathf.Lerp(peak, 1f, elapsed / 0.10f);
                yield return null;
            }
            target.localScale = Vector3.one;
        }
    }
}
