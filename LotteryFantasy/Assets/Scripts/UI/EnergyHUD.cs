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

        private static readonly Color FireColor = new Color(1f, 0.45f, 0.1f);
        private static readonly Color IronColor = new Color(0.6f, 0.8f, 1f);
        private static readonly Color LifeColor = new Color(0.2f, 1f, 0.45f);
        private static readonly Color DimColor = new Color(0.55f, 0.55f, 0.55f);

        private void Awake() { Instance = this; }
        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void Update()
        {
            if (GameManager.Instance == null) return;
            var e = GameManager.Instance.ElementalEnergy;
            SetLabel(fireText, "F", e.Fire, FireColor);
            SetLabel(ironText, "I", e.Iron, IronColor);
            SetLabel(lifeText, "L", e.Life, LifeColor);
        }

        private static void SetLabel(Text text, string icon, int amount, Color color)
        {
            if (text == null) return;
            text.color = amount > 0 ? color : DimColor;
            text.text = $"{icon} <b>{amount}</b>";
        }

        public void FlashInsufficient(ElementalCost needed)
        {
            if (GameManager.Instance == null) return;
            var e = GameManager.Instance.ElementalEnergy;
            if (e.Fire < needed.fire && fireText != null) StartCoroutine(Flash(fireText, FireColor));
            if (e.Iron < needed.iron && ironText != null) StartCoroutine(Flash(ironText, IronColor));
            if (e.Life < needed.life && lifeText != null) StartCoroutine(Flash(lifeText, LifeColor));
        }

        private static IEnumerator Flash(Text text, Color baseColor)
        {
            for (int i = 0; i < 4; i++)
            {
                text.color = Color.red;
                yield return new WaitForSeconds(0.08f);
                text.color = baseColor;
                yield return new WaitForSeconds(0.08f);
            }
        }
    }
}
