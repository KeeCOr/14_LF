using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace SlotDefense
{
    public class HandUI : MonoBehaviour
    {
        public Button[] cardButtons;
        public Image[] cardIcons;
        public TextMeshProUGUI[] cardNames;
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
                cardNames[i].text = card != null
                    ? (isSelected ? $"{card.cardName}\n▶ 배치 클릭" : card.cardName)
                    : "---";
                cardIcons[i].gameObject.SetActive(card != null);
                if (cardButtons[i].targetGraphic is Image bg)
                    bg.color = isSelected ? ColorSelected : ColorNormal;
            }
        }

        private void SelectSlot(int index) => arenaSystem.SelectHandSlot(index);

        private void RefreshHand(CardData card, CardTier tier) => RefreshDisplay();
    }
}
