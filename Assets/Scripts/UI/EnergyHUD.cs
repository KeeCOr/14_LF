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

        // 각 속성의 기본 색상
        private static readonly Color FireColor = new Color(1f,  0.45f, 0.1f);
        private static readonly Color IronColor = new Color(0.6f, 0.8f, 1f);
        private static readonly Color LifeColor = new Color(0.2f, 1f,  0.45f);
        private static readonly Color DimColor  = new Color(0.55f, 0.55f, 0.55f);

        private void Awake()   { Instance = this; }
        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void Update()
        {
            if (GameManager.Instance == null) return;
            var e = GameManager.Instance.ElementalEnergy;
            SetLabel(fireText, "🔥", e.Fire,  FireColor);
            SetLabel(ironText, "⚔", e.Iron,  IronColor);
            SetLabel(lifeText, "💚", e.Life, LifeColor);
        }

        private static void SetLabel(Text t, string icon, int amount, Color col)
        {
            if (t == null) return;
            t.color = amount > 0 ? col : DimColor;
            t.text = $"{icon} <b>{amount}</b>";
        }

        // 부족한 속성 라벨을 빨간색으로 3회 점멸
        public void FlashInsufficient(ElementalCost needed)
        {
            if (GameManager.Instance == null) return;
            var e = GameManager.Instance.ElementalEnergy;
            if (e.Fire < needed.fire && fireText != null) StartCoroutine(Flash(fireText, FireColor));
            if (e.Iron < needed.iron && ironText != null) StartCoroutine(Flash(ironText, IronColor));
            if (e.Life < needed.life && lifeText != null) StartCoroutine(Flash(lifeText, LifeColor));
        }

        private static IEnumerator Flash(Text t, Color baseColor)
        {
            for (int i = 0; i < 4; i++)
            {
                t.color = Color.red;
                yield return new WaitForSeconds(0.08f);
                t.color = baseColor;
                yield return new WaitForSeconds(0.08f);
            }
        }
    }
}
