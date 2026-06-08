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
        private RectTransform[] _reelStrips;
        private Image[][] _reelStripImages;
        private float[] _reelScroll;

        private const int StripSymbolCount = 9;
        private const float StripSymbolSize = 76f;
        private static readonly Vector2 ReelViewportSize = new Vector2(112f, 82f);
        private static readonly Vector2 ReelStripSize = new Vector2(108f, StripSymbolCount * StripSymbolSize);
        private static readonly Vector2 ReelSymbolSize = new Vector2(66f, 66f);
        private const float ReelSpinSpeed = 760f;

        private void Start()
        {
            _reelBgs = new Image[3];
            _reelLanding = new bool[3];
            _reelStrips = new RectTransform[3];
            _reelStripImages = new Image[3][];
            _reelScroll = new float[3];

            if (reelIcons == null || reelIcons.Length < 3)
                reelIcons = new Image[3];

            for (int i = 0; i < 3; i++)
            {
                if (reelLabels != null && reelLabels[i] != null)
                    _reelBgs[i] = reelLabels[i].transform.parent.GetComponent<Image>();
                BuildReelStrip(i);
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
            {
                var size = luckGaugeFillRt.sizeDelta;
                size.x = 410f * sm.ChargeRatio;
                luckGaugeFillRt.sizeDelta = size;
            }

            if (luckChargeText != null)
            {
                float next = sm.SecondsToNext;
                luckChargeText.text = charges >= sm.MaxSpinCharges
                    ? $"LUCK GAUGE  {charges} / {sm.MaxSpinCharges}  MAX"
                    : $"LUCK GAUGE  {charges} / {sm.MaxSpinCharges}  {next:F1}s";
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

        private void BuildReelStrip(int index)
        {
            if (reelIcons == null || index >= reelIcons.Length || reelIcons[index] == null) return;

            var icon = reelIcons[index];
            var iconRt = icon.rectTransform;
            icon.enabled = false;

            var viewportGo = new GameObject("ReelViewport", typeof(RectTransform), typeof(Image), typeof(RectMask2D));
            viewportGo.transform.SetParent(icon.transform.parent, false);
            var viewportRt = viewportGo.GetComponent<RectTransform>();
            viewportRt.anchorMin = iconRt.anchorMin;
            viewportRt.anchorMax = iconRt.anchorMax;
            viewportRt.pivot = iconRt.pivot;
            viewportRt.anchoredPosition = iconRt.anchoredPosition;
            viewportRt.sizeDelta = ReelViewportSize;

            var viewportImage = viewportGo.GetComponent<Image>();
            viewportImage.color = new Color(0.02f, 0.03f, 0.07f, 0.62f);
            viewportImage.raycastTarget = false;

            var stripGo = new GameObject("ReelStrip", typeof(RectTransform));
            stripGo.transform.SetParent(viewportGo.transform, false);
            var stripRt = stripGo.GetComponent<RectTransform>();
            stripRt.anchorMin = new Vector2(0.5f, 0.5f);
            stripRt.anchorMax = new Vector2(0.5f, 0.5f);
            stripRt.pivot = new Vector2(0.5f, 0.5f);
            stripRt.sizeDelta = ReelStripSize;

            _reelStrips[index] = stripRt;
            _reelStripImages[index] = new Image[StripSymbolCount];

            for (int i = 0; i < StripSymbolCount; i++)
            {
                var symbol = new GameObject($"Symbol_{i}", typeof(RectTransform), typeof(Image));
                symbol.transform.SetParent(stripGo.transform, false);
                var rt = symbol.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, SymbolOffset(i));
                rt.sizeDelta = ReelSymbolSize;

                var image = symbol.GetComponent<Image>();
                image.preserveAspect = true;
                image.raycastTarget = false;
                _reelStripImages[index][i] = image;
            }

            FillStripSequence(index, index);
            SnapStripTo(index, ElementType.Fire, instant: true);
        }

        private static float SymbolOffset(int stripIndex)
        {
            float center = (StripSymbolCount - 1) * 0.5f;
            return (center - stripIndex) * StripSymbolSize;
        }

        private ElementType StripElement(int reelIndex, int stripIndex)
        {
            return SpinPool[(stripIndex + reelIndex * 2) % SpinPool.Length];
        }

        private void FillStripSequence(int index, int seedOffset)
        {
            if (_reelStripImages == null || index >= _reelStripImages.Length || _reelStripImages[index] == null)
                return;

            for (int i = 0; i < _reelStripImages[index].Length; i++)
            {
                var element = SpinPool[(i + seedOffset) % SpinPool.Length];
                var image = _reelStripImages[index][i];
                image.sprite = ReelSprite(element);
                image.color = ColorOf(element);
            }
        }

        private void ScrollReelStrip(int index, float delta)
        {
            if (_reelStrips == null || index >= _reelStrips.Length || _reelStrips[index] == null)
                return;

            _reelScroll[index] = Mathf.Repeat(_reelScroll[index] + delta, StripSymbolSize * SpinPool.Length);
            int sequenceStep = Mathf.FloorToInt(_reelScroll[index] / StripSymbolSize);
            FillStripSequence(index, index + sequenceStep);
            _reelStrips[index].anchoredPosition = new Vector2(0f, _reelScroll[index] % StripSymbolSize);
        }

        private void SnapStripTo(int index, ElementType element, bool instant)
        {
            if (_reelStrips == null || index >= _reelStrips.Length || _reelStrips[index] == null)
                return;

            FillStripSequence(index, index + Mathf.RoundToInt(Time.time * 10f));

            int center = StripSymbolCount / 2;
            if (_reelStripImages[index] != null && center < _reelStripImages[index].Length)
            {
                _reelStripImages[index][center].sprite = ReelSprite(element);
                _reelStripImages[index][center].color = ColorOf(element);
            }

            _reelScroll[index] = 0f;
            _reelStrips[index].anchoredPosition = Vector2.zero;
            _reelStrips[index].localScale = instant ? Vector3.one : new Vector3(1f, 1.06f, 1f);
        }

        private void SetReelVisual(int index, ElementType element, bool spinning)
        {
            if (reelLabels != null && reelLabels[index] != null)
            {
                reelLabels[index].text = spinning ? "" : SymbolOf(element);
                reelLabels[index].color = ColorOf(element);
            }

            if (spinning)
            {
                ScrollReelStrip(index, (ReelSpinSpeed + index * 90f) * Time.deltaTime);
                if (_reelStrips != null && _reelStrips[index] != null)
                {
                    float stretch = 1.04f + Mathf.Sin(Time.time * 17f + index) * 0.025f;
                    _reelStrips[index].localScale = new Vector3(1f, stretch, 1f);
                }
            }
            else
            {
                SnapStripTo(index, element, instant: false);
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
                        reelIcons[i].enabled = false;
                    }
                    if (_reelStrips != null && _reelStrips[i] != null)
                    {
                        FillStripSequence(i, i);
                        _reelStrips[i].localScale = Vector3.one;
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
                ScrollReelStrip(index, (ReelSpinSpeed * 0.45f) * delay);
                FillStripSequence(index, Random.Range(0, SpinPool.Length));
                yield return new WaitForSeconds(delay);
            }

            SetReelVisual(index, finalElement, spinning: false);

            if (_reelStrips != null && _reelStrips[index] != null)
                yield return ScaleBounce(_reelStrips[index], 1.10f);
            else if (reelIcons != null && reelIcons[index] != null)
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
