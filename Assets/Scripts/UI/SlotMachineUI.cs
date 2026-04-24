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
            spinButton.interactable = GameManager.Instance.SlotMachine.SpinCharges > 0;
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
