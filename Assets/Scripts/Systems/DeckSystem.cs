using System;
namespace SlotDefense
{
    public class DeckSystem
    {
        private readonly CardData[] _deck;
        private int _dealIndex;

        public DeckSystem(CardData[] deck) { _deck = deck; _dealIndex = 0; }

        public CardData[] DrawReels(Random rng) => new[]
        {
            _deck[rng.Next(_deck.Length)],
            _deck[rng.Next(_deck.Length)],
            _deck[rng.Next(_deck.Length)]
        };

        public CardData DealNext()
        {
            if (_deck.Length == 0) return null;
            var card = _deck[_dealIndex % _deck.Length];
            _dealIndex++;
            return card;
        }

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
