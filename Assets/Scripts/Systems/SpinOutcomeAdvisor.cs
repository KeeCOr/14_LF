using System.Collections.Generic;

namespace SlotDefense
{
    public enum SpinOutcomeTone
    {
        Risky,
        Strong,
        Jackpot
    }

    public readonly struct SpinOutcomeAdvice
    {
        public readonly string Headline;
        public readonly string Change;
        public readonly string NextDecision;
        public readonly SpinOutcomeTone Tone;

        public SpinOutcomeAdvice(string headline, string change, string nextDecision, SpinOutcomeTone tone)
        {
            Headline = headline;
            Change = change;
            NextDecision = nextDecision;
            Tone = tone;
        }
    }

    public static class SpinOutcomeAdvisor
    {
        public static SpinOutcomeAdvice Describe(ElementType[] reels, (int fire, int iron, int life) energy)
        {
            var dominant = DominantElement(energy);
            bool isTriple = reels != null && reels.Length >= 3 && reels[0] == reels[1] && reels[1] == reels[2];
            ElementType pairElement = ElementType.Fire;
            bool isPair = !isTriple && HasPair(reels, out pairElement);

            if (isTriple)
            {
                return new SpinOutcomeAdvice(
                    $"JACKPOT - {Label(dominant)} x{EnergyOf(dominant, energy)}",
                    ChangeFor(dominant, jackpot: true),
                    NextDecisionFor(dominant),
                    SpinOutcomeTone.Jackpot);
            }

            if (isPair)
            {
                return new SpinOutcomeAdvice(
                    $"{Label(pairElement)} PAIR - {FormatEnergy(energy)}",
                    ChangeFor(pairElement, jackpot: false),
                    NextDecisionFor(pairElement),
                    SpinOutcomeTone.Strong);
            }

            return new SpinOutcomeAdvice(
                $"MIXED ROLL - {FormatEnergy(energy)}",
                "변환 에너지가 고르게 쌓였지만 즉시 폭발력은 없습니다.",
                "위험 관리: 공격/방어를 크게 바꾸기보다 부족한 속성을 보완하세요.",
                SpinOutcomeTone.Risky);
        }

        private static bool HasPair(ElementType[] reels, out ElementType pairElement)
        {
            pairElement = ElementType.Fire;
            if (reels == null || reels.Length < 3) return false;
            if (reels[0] == reels[1] || reels[0] == reels[2]) { pairElement = reels[0]; return true; }
            if (reels[1] == reels[2]) { pairElement = reels[1]; return true; }
            return false;
        }

        private static ElementType DominantElement((int fire, int iron, int life) energy)
        {
            if (energy.iron >= energy.fire && energy.iron >= energy.life) return ElementType.Iron;
            if (energy.life >= energy.fire && energy.life >= energy.iron) return ElementType.Life;
            return ElementType.Fire;
        }

        private static int EnergyOf(ElementType element, (int fire, int iron, int life) energy)
        {
            return element == ElementType.Fire ? energy.fire : element == ElementType.Iron ? energy.iron : energy.life;
        }

        private static string FormatEnergy((int fire, int iron, int life) energy)
        {
            var parts = new List<string>();
            if (energy.fire > 0) parts.Add($"FIRE x{energy.fire}");
            if (energy.iron > 0) parts.Add($"IRON x{energy.iron}");
            if (energy.life > 0) parts.Add($"LIFE x{energy.life}");
            return parts.Count == 0 ? "NO ENERGY" : string.Join(" / ", parts);
        }

        private static string Label(ElementType element)
        {
            return element == ElementType.Fire ? "FIRE" : element == ElementType.Iron ? "IRON" : "LIFE";
        }

        private static string ChangeFor(ElementType element, bool jackpot)
        {
            switch (element)
            {
                case ElementType.Fire:
                    return jackpot
                        ? "변환 화염 에너지가 크게 올라 광역 공격 선택지가 강해졌습니다."
                        : "변환 화염 에너지가 쌓여 공격 카드의 기대값이 높아졌습니다.";
                case ElementType.Iron:
                    return "변환 철 에너지가 쌓여 방어형 카드 사용이 안정적입니다.";
                default:
                    return "변환 생명 에너지가 쌓여 회복과 유지 선택이 안전해졌습니다.";
            }
        }

        private static string NextDecisionFor(ElementType element)
        {
            switch (element)
            {
                case ElementType.Fire:
                    return "공격 선택: 다음 웨이브에서 몰려드는 적에게 화염 스킬을 먼저 쓰세요.";
                case ElementType.Iron:
                    return "방어 선택: 마을 체력이 흔들리면 방어 카드와 전열 보강을 우선하세요.";
                default:
                    return "회복 선택: 피해 누적 전 회복 카드로 전선을 유지하세요.";
            }
        }
    }
}