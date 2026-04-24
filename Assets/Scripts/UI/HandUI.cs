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

        private void RefreshDisplay()
        {
            if (GameManager.Instance == null) return;
            for (int i = 0; i < cardButtons.Length; i++)
            {
                var card = GameManager.Instance.Hand.GetSlot(i);
                cardButtons[i].interactable = card != null;
                cardIcons[i].sprite = card?.icon;
                cardNames[i].text = card?.cardName ?? "";
                cardIcons[i].gameObject.SetActive(card != null);
            }
        }

        private void SelectSlot(int index) => arenaSystem.SelectHandSlot(index);

        private void RefreshHand(CardData card, CardTier tier) => RefreshDisplay();
    }
}
