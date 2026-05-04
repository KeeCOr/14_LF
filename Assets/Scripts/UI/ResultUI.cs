using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace SlotDefense
{
    public class ResultUI : MonoBehaviour
    {
        public GameObject panel;
        public Text resultText;
        public Button retryButton;

        private void OnEnable() => GameEvents.OnBattleEnded += ShowResult;
        private void OnDisable() => GameEvents.OnBattleEnded -= ShowResult;

        private void Start()
        {
            panel.SetActive(false);
            retryButton.onClick.AddListener(() =>
                SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        }

        private void ShowResult(BattleResult result)
        {
            panel.SetActive(true);

            if (GameManager.Instance?.IsSurvivalMode == true)
            {
                RecordSystem.RecordLoss();
                var arena = FindObjectOfType<ArenaSystem>();
                int wave  = arena != null ? arena.SurvivalWave : 0;
                resultText.text = $"WAVE {wave} 생존!\n{RecordSystem.Summary()}";
                return;
            }

            switch (result)
            {
                case BattleResult.PlayerWin:  RecordSystem.RecordWin();  break;
                case BattleResult.PlayerLose: RecordSystem.RecordLoss(); break;
                case BattleResult.Draw:       RecordSystem.RecordDraw(); break;
            }
            string outcome = result switch
            {
                BattleResult.PlayerWin  => "VICTORY!",
                BattleResult.PlayerLose => "DEFEAT",
                BattleResult.Draw       => "DRAW",
                _                       => ""
            };
            resultText.text = $"{outcome}\n{RecordSystem.Summary()}";
        }
    }
}
