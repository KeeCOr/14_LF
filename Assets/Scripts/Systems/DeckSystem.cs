using System;
namespace SlotDefense
{
    public class DeckSystem
    {
        private readonly CardData[] _deck;

        public DeckSystem(CardData[] deck) => _deck = deck;

        public CardData[] DrawReels(Random rng) => new[]
        {
            _deck[rng.Next(_deck.Length)],
            _deck[rng.Next(_deck.Length)],
            _deck[rng.Next(_deck.Length)]
        };

        public static SlotResult EvaluateReels(CardData[] reels, out CardData matched)
        {
            if (reels[0] == reels[1] && reels[1] == reels[2])
            {
                matched = reels[0];
                return SlotResult.Triple;
            }
            if (reels[0] == reels[1]) { matched = reels[0]; return SlotResult.Double; }
            if (reels[1] == reels[2]) { matched = reels[1]; return SlotResult.Double; }
            if (reels[0] == reels[2]) { matched = reels[0]; return SlotResult.Double; }
            matched = null;
            return SlotResult.AllDifferent;
        }
    }
}
