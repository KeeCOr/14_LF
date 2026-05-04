using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SlotDefense
{
    public class DeckViewerUI : MonoBehaviour
    {
        public GameObject panel;
        public Text       contentText;

        private FixedDeckConfig _config;

        public void Setup(FixedDeckConfig config)
        {
            _config = config;
            if (panel != null) panel.SetActive(false);
        }

        public void Toggle()
        {
            if (panel == null) return;
            bool show = !panel.activeSelf;
            if (show) RefreshContent();
            panel.SetActive(show);
        }

        private void RefreshContent()
        {
            if (_config == null || contentText == null) return;

            var counts = new Dictionary<CardData, int>();
            foreach (var card in _config.cards)
            {
                if (card == null) continue;
                counts.TryGetValue(card, out int n);
                counts[card] = n + 1;
            }

            var sb = new StringBuilder();
            foreach (var kvp in counts)
            {
                var card  = kvp.Key;
                int count = kvp.Value;
                switch (card.cardType)
                {
                    case CardType.Unit:
                        var u = card.unitStats;
                        string action = u.healAmount > 0
                            ? $"회복:{u.healAmount}/공격"
                            : $"피해:{u.damage}";
                        sb.AppendLine($"[유닛]  {card.cardName} ×{count}    HP:{u.hp}  {action}  사거리:{u.attackRange}  비용:{card.ElementalCost.Total}에너지");
                        break;
                    case CardType.Skill:
                        var sk = card.skillEffect;
                        sb.AppendLine($"[스킬]  {card.cardName} ×{count}    피해:{sk.damage}  반경:{sk.radius}");
                        break;
                    case CardType.Buff:
                        var b = card.buffEffect;
                        sb.AppendLine($"[버프]  {card.cardName} ×{count}    공격×{b.attackMultiplier}  속도×{b.speedMultiplier}  {b.duration}초");
                        break;
                }
            }
            contentText.text = sb.ToString();
        }
    }
}
