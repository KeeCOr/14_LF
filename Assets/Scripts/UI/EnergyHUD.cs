using UnityEngine;
using UnityEngine.UI;
namespace SlotDefense
{
    public class EnergyHUD : MonoBehaviour
    {
        public Text fireText;
        public Text ironText;
        public Text lifeText;

        private void Update()
        {
            if (GameManager.Instance == null) return;
            var e = GameManager.Instance.ElementalEnergy;
            if (fireText != null) fireText.text = $"🔥 {e.Fire}";
            if (ironText != null) ironText.text = $"⚔ {e.Iron}";
            if (lifeText != null) lifeText.text = $"💚 {e.Life}";
        }
    }
}
