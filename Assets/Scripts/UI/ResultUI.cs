using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
namespace SlotDefense
{
    public class ResultUI : MonoBehaviour
    {
        public GameObject panel;
        public TextMeshProUGUI resultText;
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
            resultText.text = result switch
            {
                BattleResult.PlayerWin  => "VICTORY!",
                BattleResult.PlayerLose => "DEFEAT",
                BattleResult.Draw       => "DRAW",
                _                       => ""
            };
        }
    }
}
