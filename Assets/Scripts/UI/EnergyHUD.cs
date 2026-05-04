using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace SlotDefense
{
    public class EnergyHUD : MonoBehaviour
    {
        public Text fireText;
        public Text ironText;
        public Text lifeText;

        public static EnergyHUD Instance { get; private set; }

        private void Awake()  { Instance = this; }
        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void Update()
        {
            if (GameManager.Instance == null) return;
            var e = GameManager.Instance.ElementalEnergy;
            if (fireText != null) fireText.text = $"🔥 {e.Fire}";
            if (ironText != null) ironText.text = $"⚔ {e.Iron}";
            if (lifeText != null) lifeText.text = $"💚 {e.Life}";
        }

        // 부족한 속성 라벨을 빨간색으로 3회 점멸
        public void FlashInsufficient(ElementalCost needed)
        {
            if (GameManager.Instance == null) return;
            var e = GameManager.Instance.ElementalEnergy;
            if (e.Fire < needed.fire && fireText != null) StartCoroutine(Flash(fireText));
            if (e.Iron < needed.iron && ironText != null) StartCoroutine(Flash(ironText));
            if (e.Life < needed.life && lifeText != null) StartCoroutine(Flash(lifeText));
        }

        private static IEnumerator Flash(Text t)
        {
            for (int i = 0; i < 3; i++)
            {
                t.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                t.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
