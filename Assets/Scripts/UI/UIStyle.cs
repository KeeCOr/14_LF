using UnityEngine;
using UnityEngine.UI;

namespace SlotDefense
{
    public static class UIStyle
    {
        public static readonly Color Panel = new Color(0.035f, 0.045f, 0.075f, 0.96f);
        public static readonly Color PanelDeep = new Color(0.018f, 0.023f, 0.045f, 0.98f);
        public static readonly Color PanelRaised = new Color(0.075f, 0.095f, 0.155f, 0.96f);
        public static readonly Color Stroke = new Color(0.25f, 0.48f, 0.95f, 0.55f);
        public static readonly Color StrokeSoft = new Color(0.22f, 0.34f, 0.58f, 0.38f);
        public static readonly Color TextMuted = new Color(0.62f, 0.72f, 0.90f, 0.92f);
        public static readonly Color Gold = new Color(1f, 0.74f, 0.18f, 1f);
        public static readonly Color Cyan = new Color(0.32f, 0.78f, 1f, 1f);
        public static readonly Color Green = new Color(0.22f, 0.95f, 0.52f, 1f);
        public static readonly Color Red = new Color(1f, 0.28f, 0.24f, 1f);

        public static readonly Color CardUnit = new Color(0.13f, 0.18f, 0.32f, 0.96f);
        public static readonly Color CardSelected = new Color(0.11f, 0.50f, 0.32f, 0.98f);
        public static readonly Color CardUnavailable = new Color(0.085f, 0.075f, 0.115f, 0.92f);
        public static readonly Color CardSkill = new Color(0.30f, 0.12f, 0.46f, 0.97f);
        public static readonly Color CardBuff = new Color(0.46f, 0.34f, 0.10f, 0.97f);

        public static Color CardBackground(CardType type, bool canAfford, bool selected)
        {
            if (selected) return CardSelected;
            if (!canAfford) return CardUnavailable;

            switch (type)
            {
                case CardType.Skill:
                    return CardSkill;
                case CardType.Buff:
                    return CardBuff;
                default:
                    return CardUnit;
            }
        }

        public static ColorBlock AccentButtonColors(Color baseColor)
        {
            return new ColorBlock
            {
                normalColor = Darken(baseColor, 0.68f),
                highlightedColor = Color.Lerp(baseColor, Color.white, 0.18f),
                pressedColor = Darken(baseColor, 0.48f),
                selectedColor = Color.Lerp(baseColor, Color.white, 0.10f),
                disabledColor = new Color(0.16f, 0.16f, 0.20f, 0.36f),
                colorMultiplier = 1f,
                fadeDuration = 0.08f
            };
        }

        public static Color Darken(Color color, float amount)
        {
            return new Color(color.r * amount, color.g * amount, color.b * amount, color.a);
        }
    }
}
