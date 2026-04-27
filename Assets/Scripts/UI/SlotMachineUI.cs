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
            ShowSpinResult();
        }

        private void ShowSpinResult()
        {
            resultText.text = "SPIN!";
            Invoke(nameof(ClearResult), 1.5f);
        }

        private void ClearResult() => resultText.text = "";
    }
}
