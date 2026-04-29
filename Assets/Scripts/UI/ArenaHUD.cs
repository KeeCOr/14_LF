using UnityEngine;
using UnityEngine.UI;
namespace SlotDefense
{
    public class ArenaHUD : MonoBehaviour
    {
        [Header("Village HP")]
        public Slider playerHpSlider;
        public Slider enemyHpSlider;

        [Header("Timer")]
        public Text timerText;

        [Header("Spin")]
        public Text spinChargesText;

        [Header("Record")]
        public Text recordText;
        public Text stageText;

        private ArenaSystem _arenaSystem;

        private void Awake() => _arenaSystem = FindObjectOfType<ArenaSystem>();

        private void Update()
        {
            if (GameManager.Instance == null) return;
            var b = GameManager.Instance.Battle;
            playerHpSlider.value = b.PlayerHp / b.MaxVillageHp;
            enemyHpSlider.value = b.EnemyHp / b.MaxVillageHp;

            var t = Mathf.Max(0f, b.TimeRemaining);
            timerText.text = $"{(int)(t / 60f)}:{(int)(t % 60f):00}";

            var sm = GameManager.Instance.SlotMachine;
            int secs = Mathf.CeilToInt(sm.SecondsToNext);
            spinChargesText.text = sm.SpinCharges > 0
                ? $"SPIN x{sm.SpinCharges}  (+{secs}s)"
                : $"SPIN 충전 중 {secs}s...";
            if (recordText != null) recordText.text = RecordSystem.Summary();
            if (stageText != null && _arenaSystem != null)
                stageText.text = $"STAGE {_arenaSystem.CurrentStage}";
        }
    }
}
