using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotDefense
{
    public class HandUI : MonoBehaviour
    {
        public Button[] cardButtons;
        public Image[] cardIcons;
        public Text[] cardNames;
        public ArenaSystem arenaSystem;
        public Image deployZoneOverlay;

        private Vector2[] _basePos;
        private float[] _liftY;
        private CostBadge[][] _costBadges;

        private static readonly Color ColorEnhanced = new Color(1f, 0.85f, 0.1f, 0.9f);

        private void OnEnable() => GameEvents.OnCardObtained += RefreshHand;
        private void OnDisable() => GameEvents.OnCardObtained -= RefreshHand;

        private void Start()
        {
            _basePos = new Vector2[cardButtons.Length];
            _liftY = new float[cardButtons.Length];

            for (int i = 0; i < cardButtons.Length; i++)
            {
                _basePos[i] = ((RectTransform)cardButtons[i].transform).anchoredPosition;
                CreateCostBadges(i);
                int index = i;
                cardButtons[i].onClick.AddListener(() => SelectSlot(index));
            }
        }

        private void Update() => RefreshDisplay();

        private void RefreshDisplay()
        {
            if (GameManager.Instance == null) return;
            if (_basePos == null) return;

            int selected = arenaSystem != null ? arenaSystem.SelectedSlot : -1;
            for (int i = 0; i < cardButtons.Length; i++)
            {
                var card = GameManager.Instance.Hand.GetSlot(i);
                bool isSelected = selected == i && card != null;
                cardButtons[i].interactable = card != null;

                cardIcons[i].sprite = card != null ? card.icon ?? UIArtKit.CardIcon(card) : null;
                cardIcons[i].preserveAspect = true;
                cardIcons[i].gameObject.SetActive(card != null);
                UpdateCostBadges(i, card, GameManager.Instance.ElementalEnergy);

                bool canUse = false;
                Color bgColor;
                if (card == null)
                {
                    cardNames[i].text = "---";
                    bgColor = UIStyle.CardBackground(CardType.Unit, canAfford: false, selected: false);
                }
                else if (card.cardType == CardType.Buff)
                {
                    cardNames[i].text = $"<color=#FFD66B><b>BUFF</b></color>\n<b>{card.cardName}</b>\n<color=#FFE9A6>[INSTANT]</color>";
                    bgColor = UIStyle.CardBackground(CardType.Buff, canAfford: true, selected: false);
                    canUse = true;
                }
                else if (card.cardType == CardType.Skill)
                {
                    var ec = card.ElementalCost;
                    var energy = GameManager.Instance.ElementalEnergy;
                    bool canAfford = energy.CanAfford(ec);
                    canUse = canAfford;
                    bool skillSelected = arenaSystem != null && arenaSystem.SelectedSkillSlot == i;
                    string skillAction = skillSelected
                        ? "\n<color=#FFD66B>> CAST TARGET</color>"
                        : "\n<color=#B98CFF>[CLICK CAST]</color>";
                    if (!canAfford)
                        skillAction = "\n" + MissingCostLine(energy, ec);
                    cardNames[i].text = $"<color=#C377FF><b>MAGIC</b></color>\n<b>{card.cardName}</b>{skillAction}";
                    bgColor = UIStyle.CardBackground(CardType.Skill, canAfford, skillSelected);
                }
                else
                {
                    var ec = card.ElementalCost;
                    var energy = GameManager.Instance.ElementalEnergy;
                    bool canAfford = energy.CanAfford(ec);
                    canUse = canAfford;

                    string action = canAfford
                        ? isSelected ? "\n<color=#88FFCC>> DEPLOY</color>" : ""
                        : "\n" + MissingCostLine(energy, ec);
                    string typeLabel = card.cardType == CardType.Building
                        ? "<color=#D8E8FF><b>BUILD</b></color>"
                        : "<color=#6BD5FF><b>UNIT</b></color>";
                    cardNames[i].text = $"{typeLabel}\n<b>{card.cardName}</b>{action}";
                    bgColor = UIStyle.CardBackground(card.cardType, canAfford, isSelected);
                }

                if (cardButtons[i].targetGraphic is Image bg)
                {
                    if (card != null)
                        bg.sprite = UIArtKit.CardFrame(card.cardType, i);

                    if (bg.sprite != null)
                    {
                        var tint = card == null
                            ? new Color(0.55f, 0.55f, 0.65f, 0.78f)
                            : Color.Lerp(Color.white, bgColor, isSelected ? 0.30f : 0.16f);
                        bg.color = tint;
                    }
                    else
                    {
                        bg.color = bgColor;
                    }
                }

                float targetY = card == null || isSelected ? 0f : canUse ? 14f : 0f;
                _liftY[i] = Mathf.Lerp(_liftY[i], targetY, Time.deltaTime * 9f);
                ((RectTransform)cardButtons[i].transform).anchoredPosition = _basePos[i] + new Vector2(0f, _liftY[i]);
            }

            if (deployZoneOverlay != null)
            {
                int sel = arenaSystem != null ? arenaSystem.SelectedSlot : -1;
                bool unitPending = sel >= 0 && GameManager.Instance?.Hand.GetSlot(sel)?.cardType == CardType.Unit;
                deployZoneOverlay.color = new Color(0.3f, 0.75f, 1f, unitPending ? 0.28f : 0f);
            }
        }

        private void CreateCostBadges(int index)
        {
            if (_costBadges == null)
                _costBadges = new CostBadge[cardButtons.Length][];

            _costBadges[index] = new CostBadge[3];

            var row = new GameObject("EnergyCostIcons", typeof(RectTransform));
            row.transform.SetParent(cardButtons[index].transform, false);
            var rowRt = row.GetComponent<RectTransform>();
            rowRt.anchorMin = new Vector2(0.5f, 0f);
            rowRt.anchorMax = new Vector2(0.5f, 0f);
            rowRt.pivot = new Vector2(0.5f, 0f);
            rowRt.anchoredPosition = new Vector2(0f, 18f);
            rowRt.sizeDelta = new Vector2(136f, 28f);

            _costBadges[index][0] = CreateCostBadge(row.transform, 0, ElementType.Fire, -46f);
            _costBadges[index][1] = CreateCostBadge(row.transform, 1, ElementType.Iron, 0f);
            _costBadges[index][2] = CreateCostBadge(row.transform, 2, ElementType.Life, 46f);
        }

        private static CostBadge CreateCostBadge(Transform parent, int index, ElementType element, float x)
        {
            var root = new GameObject(element + "Cost", typeof(RectTransform), typeof(Image));
            root.transform.SetParent(parent, false);
            var rt = root.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x, 0f);
            rt.sizeDelta = new Vector2(40f, 24f);

            var bg = root.GetComponent<Image>();
            bg.sprite = UIArtKit.Sprite(UIArtSprite.ResourceCounterFrame);
            bg.type = Image.Type.Sliced;
            bg.color = new Color(0.04f, 0.06f, 0.10f, 0.78f);
            bg.raycastTarget = false;

            var icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            icon.transform.SetParent(root.transform, false);
            var iconRt = icon.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0f, 0.5f);
            iconRt.anchorMax = new Vector2(0f, 0.5f);
            iconRt.pivot = new Vector2(0f, 0.5f);
            iconRt.anchoredPosition = new Vector2(4f, 0f);
            iconRt.sizeDelta = new Vector2(18f, 18f);
            var iconImage = icon.GetComponent<Image>();
            iconImage.sprite = UIArtKit.ElementIcon(element);
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;

            var value = new GameObject("Value", typeof(RectTransform), typeof(Text));
            value.transform.SetParent(root.transform, false);
            var valueRt = value.GetComponent<RectTransform>();
            valueRt.anchorMin = new Vector2(1f, 0.5f);
            valueRt.anchorMax = new Vector2(1f, 0.5f);
            valueRt.pivot = new Vector2(1f, 0.5f);
            valueRt.anchoredPosition = new Vector2(-4f, 0f);
            valueRt.sizeDelta = new Vector2(17f, 20f);
            var valueText = value.GetComponent<Text>();
            valueText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            valueText.fontSize = 14;
            valueText.fontStyle = FontStyle.Bold;
            valueText.alignment = TextAnchor.MiddleRight;
            valueText.raycastTarget = false;

            return new CostBadge(root, iconImage, valueText, element);
        }

        private static string MissingCostLine(ElementalEnergySystem energy, ElementalCost cost)
        {
            var missing = ElementalEnergySystem.MissingCost(energy, cost);
            if (missing.IsZero) return "<color=#88FFCC>READY</color>";

            var parts = new List<string>(3);
            if (missing.fire > 0) parts.Add("F" + missing.fire);
            if (missing.iron > 0) parts.Add("I" + missing.iron);
            if (missing.life > 0) parts.Add("L" + missing.life);
            return "<color=#FF6F66><b>NEED " + string.Join(" ", parts) + "</b></color>";
        }

        private void UpdateCostBadges(int index, CardData card, ElementalEnergySystem energy)
        {
            if (_costBadges == null || index < 0 || index >= _costBadges.Length || _costBadges[index] == null)
                return;

            if (card == null || card.cardType == CardType.Buff || energy == null)
            {
                SetCostBadge(_costBadges[index][0], 0, 0, true);
                SetCostBadge(_costBadges[index][1], 0, 0, true);
                SetCostBadge(_costBadges[index][2], 0, 0, true);
                return;
            }

            var ec = card.ElementalCost;
            SetCostBadge(_costBadges[index][0], ec.fire, energy.Fire, false);
            SetCostBadge(_costBadges[index][1], ec.iron, energy.Iron, false);
            SetCostBadge(_costBadges[index][2], ec.life, energy.Life, false);
        }

        private static void SetCostBadge(CostBadge badge, int cost, int current, bool forceHidden)
        {
            if (badge == null || badge.root == null) return;

            bool visible = !forceHidden && cost > 0;
            badge.root.SetActive(visible);
            if (!visible) return;

            bool affordable = current >= cost;
            badge.value.text = cost.ToString();
            badge.value.color = affordable ? CostTextColor(badge.element) : new Color(1f, 0.25f, 0.22f, 1f);
            badge.icon.color = affordable ? Color.white : new Color(1f, 0.38f, 0.34f, 0.72f);
        }

        private static Color CostTextColor(ElementType element)
        {
            switch (element)
            {
                case ElementType.Fire:
                    return new Color(1f, 0.58f, 0.24f, 1f);
                case ElementType.Iron:
                    return new Color(0.62f, 0.82f, 1f, 1f);
                case ElementType.Life:
                    return new Color(0.32f, 1f, 0.52f, 1f);
                default:
                    return Color.white;
            }
        }

        private sealed class CostBadge
        {
            public readonly GameObject root;
            public readonly Image icon;
            public readonly Text value;
            public readonly ElementType element;

            public CostBadge(GameObject root, Image icon, Text value, ElementType element)
            {
                this.root = root;
                this.icon = icon;
                this.value = value;
                this.element = element;
            }
        }

        private void SelectSlot(int index)
        {
            if (GameManager.Instance == null) return;
            var card = GameManager.Instance.Hand.GetSlot(index);
            if (card == null) return;

            if (card.cardType == CardType.Buff)
            {
                GameManager.Instance.Hand.Use(index);
                GameEvents.GlobalBuffApplied(card.buffEffect);
                return;
            }

            if (card.cardType == CardType.Skill)
            {
                arenaSystem.SelectSkillSlot(index);
                return;
            }

            arenaSystem.SelectHandSlot(index);
        }

        private void RefreshHand(CardData card, CardTier tier)
        {
            RefreshDisplay();
            for (int i = 0; i < cardButtons.Length; i++)
            {
                if (GameManager.Instance?.Hand.GetSlot(i) == card)
                {
                    StartCoroutine(PopInSlot(i));
                    if (tier == CardTier.Enhanced) StartCoroutine(BlinkSlot(i));
                    break;
                }
            }
        }

        private IEnumerator PopInSlot(int index)
        {
            var t = cardButtons[index].transform;
            float elapsed = 0f;
            while (elapsed < 0.15f)
            {
                elapsed += Time.deltaTime;
                float s = Mathf.Lerp(0f, 1.2f, elapsed / 0.15f);
                t.localScale = new Vector3(s, s, 1f);
                yield return null;
            }
            elapsed = 0f;
            while (elapsed < 0.07f)
            {
                elapsed += Time.deltaTime;
                float s = Mathf.Lerp(1.2f, 1f, elapsed / 0.07f);
                t.localScale = new Vector3(s, s, 1f);
                yield return null;
            }
            t.localScale = Vector3.one;
        }

        private IEnumerator BlinkSlot(int index)
        {
            for (int i = 0; i < 3; i++)
            {
                if (cardButtons[index].targetGraphic is Image bg)
                    bg.color = ColorEnhanced;
                yield return new WaitForSeconds(0.15f);
                RefreshDisplay();
                yield return new WaitForSeconds(0.15f);
            }
        }
    }
}
