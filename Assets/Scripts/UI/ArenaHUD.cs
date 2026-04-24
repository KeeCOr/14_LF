using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace SlotDefense
{
    public class ArenaHUD : MonoBehaviour
    {
        [Header("Village HP")]
        public Slider playerHpSlider;
        public Slider enemyHpSlider;

        [Header("Timer")]
        public TextMeshProUGUI timerText;

        [Header("Spin")]
        public TextMeshProUGUI spinChargesText;

        private void Update()
        {
            if (GameManager.Instance == null) return;
            var b = GameManager.Instance.Battle;
            playerHpSlider.value = b.PlayerHp / b.MaxVillageHp;
            enemyHpSlider.value = b.EnemyHp / b.MaxVillageHp;

            var t = Mathf.Max(0f, b.TimeRemaining);
            timerText.text = $"{(int)(t / 60f)}:{(int)(t % 60f):00}";

            spinChargesText.text = $"x{GameManager.Instance.SlotMachine.SpinCharges}";
        }
    }
}
