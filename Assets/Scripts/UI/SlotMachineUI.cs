using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace SlotDefense
{
    public class SlotMachineUI : MonoBehaviour
    {
        public Image[] reelImages;
        public TextMeshProUGUI[] reelNames;
        public Button spinButton;
        public TextMeshProUGUI resultText;

        private void Start() => spinButton.onClick.AddListener(OnSpinClicked);

        private void OnEnable()
        {
            GameEvents.OnCardObtained += OnCardObtained;
            GameEvents.OnGlobalBuffApplied += OnBuffApplied;
        }

        private void OnDisable()
        {
            GameEvents.OnCardObtained -= OnCardObtained;
            GameEvents.OnGlobalBuffApplied -= OnBuffApplied;
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;
            int charges = GameManager.Instance.SlotMachine.SpinCharges;
            spinButton.interactable = charges > 0;
            var label = spinButton.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = $"SPIN (x{charges})";
        }

        private void OnSpinClicked()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.TrySpin();
        }

        private void OnCardObtained(CardData card, CardTier tier)
        {
            string prefix = tier == CardTier.Enhanced ? "[강화] " : "";
            ShowResult($"{prefix}{card.cardName} 획득!");
        }

        private void OnBuffApplied(BuffEffect _) => ShowResult("전체 버프 적용!");

        private void ShowResult(string msg)
        {
            resultText.text = msg;
            CancelInvoke(nameof(ClearResult));
            Invoke(nameof(ClearResult), 2f);
        }

        private void ClearResult() => resultText.text = "";
    }
}
