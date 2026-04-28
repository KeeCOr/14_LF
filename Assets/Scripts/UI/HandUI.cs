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

        private void OnEnable() => GameEvents.OnCardObtained += RefreshHand;
        private void OnDisable() => GameEvents.OnCardObtained -= RefreshHand;

        private void Start()
        {
            for (int i = 0; i < cardButtons.Length; i++)
            {
                int index = i;
                cardButtons[i].onClick.AddListener(() => SelectSlot(index));
            }
        }

        private void Update() => RefreshDisplay();

        private static readonly Color ColorNormal   = new Color(0.2f, 0.25f, 0.45f, 0.9f);
        private static readonly Color ColorSelected = new Color(0.2f, 0.65f, 0.25f, 0.9f);
        private static readonly Color ColorBuff     = new Color(0.5f, 0.38f, 0.05f, 0.9f);
        private static readonly Color ColorEnhanced = new Color(1f, 0.85f, 0.1f, 0.9f);

        private void RefreshDisplay()
        {
            if (GameManager.Instance == null) return;
            int selected = arenaSystem != null ? arenaSystem.SelectedSlot : -1;
            for (int i = 0; i < cardButtons.Length; i++)
            {
                var card = GameManager.Instance.Hand.GetSlot(i);
                bool isSelected = selected == i && card != null;
                cardButtons[i].interactable = card != null;
                cardIcons[i].sprite = card?.icon;
                cardIcons[i].gameObject.SetActive(card != null);

                Color bgColor;
                if (card == null)
                {
                    cardNames[i].text = "---";
                    bgColor = ColorNormal;
                }
                else if (card.cardType == CardType.Buff)
                {
                    cardNames[i].text = $"{card.cardName}\n[클릭 → 즉시 발동]";
                    bgColor = ColorBuff;
                }
                else
                {
                    string costStr = $"[{card.placementCost}스핀]";
                    cardNames[i].text = isSelected
                        ? $"{card.cardName} {costStr}\n> 배치 클릭"
                        : $"{card.cardName} {costStr}";
                    bgColor = isSelected ? ColorSelected : ColorNormal;
                }

                if (cardButtons[i].targetGraphic is Image bg)
                    bg.color = bgColor;
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

            arenaSystem.SelectHandSlot(index);
        }

        private void RefreshHand(CardData card, CardTier tier)
        {
            RefreshDisplay();
            if (tier != CardTier.Enhanced) return;
            for (int i = 0; i < cardButtons.Length; i++)
            {
                if (GameManager.Instance?.Hand.GetSlot(i) == card)
                {
                    StartCoroutine(BlinkSlot(i));
                    break;
                }
            }
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
