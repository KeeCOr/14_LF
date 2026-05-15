using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace SlotDefense
{
    public class HandUI : MonoBehaviour
    {
        public Button[]    cardButtons;
        public Image[]     cardIcons;
        public Text[]      cardNames;
        public ArenaSystem arenaSystem;
        public Image       deployZoneOverlay;

        private Vector2[] _basePos;
        private float[]   _liftY;

        private void OnEnable() => GameEvents.OnCardObtained += RefreshHand;
        private void OnDisable() => GameEvents.OnCardObtained -= RefreshHand;

        private void Start()
        {
            _basePos = new Vector2[cardButtons.Length];
            _liftY   = new float[cardButtons.Length];
            for (int i = 0; i < cardButtons.Length; i++)
                _basePos[i] = ((RectTransform)cardButtons[i].transform).anchoredPosition;

            for (int i = 0; i < cardButtons.Length; i++)
            {
                int index = i;
                cardButtons[i].onClick.AddListener(() => SelectSlot(index));
            }
        }

        private void Update() => RefreshDisplay();

        private static readonly Color ColorEnhanced = new Color(1f, 0.85f, 0.1f, 0.9f);

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
                cardIcons[i].sprite = card?.icon;
                cardIcons[i].gameObject.SetActive(card != null);

                bool canUse = false;
                Color bgColor;
                if (card == null)
                {
                    cardNames[i].text = "---";
                    bgColor = UIStyle.CardBackground(CardType.Unit, canAfford: false, selected: false);
                }
                else if (card.cardType == CardType.Buff)
                {
                    cardNames[i].text = $"{card.cardName}\n[클릭 → 즉시 발동]";
                    bgColor = UIStyle.CardBackground(CardType.Buff, canAfford: true, selected: false);
                    canUse  = true;
                }
                else if (card.cardType == CardType.Skill)
                {
                    var ec         = card.ElementalCost;
                    var energy     = GameManager.Instance.ElementalEnergy;
                    bool canAfford = energy.CanAfford(ec);
                    canUse = canAfford;
                    bool skillSelected = arenaSystem != null && arenaSystem.SelectedSkillSlot == i;
                    cardNames[i].text = skillSelected
                        ? $"{card.cardName}\n> 지점 클릭"
                        : $"{card.cardName}\n[클릭 → 지점 선택]";
                    bgColor = UIStyle.CardBackground(CardType.Skill, canAfford, skillSelected);
                }
                else
                {
                    var ec = card.ElementalCost;
                    var energy = GameManager.Instance.ElementalEnergy;
                    bool canAfford = energy.CanAfford(ec);
                    canUse = canAfford;

                    var costParts = new System.Collections.Generic.List<string>();
                    if (ec.fire > 0)
                    {
                        string col = energy.Fire < ec.fire ? "#FF4444" : "#FF8833";
                        costParts.Add($"<color={col}><b>🔥{ec.fire}</b></color>");
                    }
                    if (ec.iron > 0)
                    {
                        string col = energy.Iron < ec.iron ? "#FF4444" : "#99CCFF";
                        costParts.Add($"<color={col}><b>⚔{ec.iron}</b></color>");
                    }
                    if (ec.life > 0)
                    {
                        string col = energy.Life < ec.life ? "#FF4444" : "#33FF77";
                        costParts.Add($"<color={col}><b>💚{ec.life}</b></color>");
                    }
                    string costStr = costParts.Count > 0 ? string.Join("  ", costParts) : "<color=#AAAAAA>무료</color>";
                    string action  = isSelected ? "\n<color=#88FFCC>> 배치 클릭</color>" : "";
                    cardNames[i].text = $"<b>{card.cardName}</b>\n{costStr}{action}";
                    bgColor = UIStyle.CardBackground(card.cardType, canAfford, isSelected);
                }

                if (cardButtons[i].targetGraphic is Image bg)
                    bg.color = bgColor;

                // 카드가 있으면 항상 살짝 위로 (여유 있으면 더 높게)
                float targetY = card == null || isSelected ? 0f : canUse ? 14f : 0f;
                _liftY[i] = Mathf.Lerp(_liftY[i], targetY, Time.deltaTime * 9f);
                ((RectTransform)cardButtons[i].transform).anchoredPosition = _basePos[i] + new Vector2(0f, _liftY[i]);
            }

            if (deployZoneOverlay != null)
            {
                int sel = arenaSystem != null ? arenaSystem.SelectedSlot : -1;
                bool unitPending = sel >= 0 && GameManager.Instance?.Hand.GetSlot(sel)?.cardType == CardType.Unit;
                float a = unitPending ? 0.28f : 0f;
                deployZoneOverlay.color = new Color(0.3f, 0.75f, 1f, a);
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
