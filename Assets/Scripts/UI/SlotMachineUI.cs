using UnityEngine;
using UnityEngine.UI;
namespace SlotDefense
{
    public class SlotMachineUI : MonoBehaviour
    {
        public Image[] reelImages;
        public Text[] reelNames;   // 3개: 각 릴에 뽑힌 카드 이름 표시
        public Button spinButton;
        public Text resultText;

        private void Start() => spinButton.onClick.AddListener(OnSpinClicked);

        private void OnEnable()
        {
            GameEvents.OnSpinCompleted  += OnSpinCompleted;
            GameEvents.OnCardObtained   += OnCardObtained;
            GameEvents.OnGlobalBuffApplied += OnBuffApplied;
        }

        private void OnDisable()
        {
            GameEvents.OnSpinCompleted  -= OnSpinCompleted;
            GameEvents.OnCardObtained   -= OnCardObtained;
            GameEvents.OnGlobalBuffApplied -= OnBuffApplied;
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;
            int charges = GameManager.Instance.SlotMachine.SpinCharges;
            spinButton.interactable = charges > 0;
            var label = spinButton.GetComponentInChildren<Text>();
            if (label != null) label.text = $"SPIN (x{charges})";
        }

        private void OnSpinClicked()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.TrySpin();
        }

        private void OnSpinCompleted(CardData[] reels, SlotResult result)
        {
            for (int i = 0; i < reelNames.Length && i < reels.Length; i++)
                if (reelNames[i] != null) reelNames[i].text = reels[i]?.cardName ?? "?";
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
