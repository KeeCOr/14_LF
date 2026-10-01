using System;
using System.Collections.Generic;
namespace SlotDefense
{
    public class ReelSystem
    {
        private readonly List<ElementType> _pool = new List<ElementType>();
        private readonly Random _rng;

        public int PoolSize => _pool.Count;

        public ReelSystem(Random rng) => _rng = rng;

        public void BuildPool(CardData[] deck)
        {
            _pool.Clear();
            foreach (var card in deck)
            {
                if (card == null) continue;
                for (int i = 0; i < card.fireCost; i++) _pool.Add(ElementType.Fire);
                for (int i = 0; i < card.ironCost; i++) _pool.Add(ElementType.Iron);
                for (int i = 0; i < card.lifeCost; i++) _pool.Add(ElementType.Life);
            }
            if (_pool.Count == 0)
            {
                _pool.Add(ElementType.Fire);
                _pool.Add(ElementType.Iron);
                _pool.Add(ElementType.Life);
            }
        }

        public ElementType[] Spin() => new[]
        {
            _pool[_rng.Next(_pool.Count)],
            _pool[_rng.Next(_pool.Count)],
            _pool[_rng.Next(_pool.Count)]
        };

        public static (int fire, int iron, int life) CalcEnergy(ElementType[] reels)
        {
            int fc = 0, ic = 0, lc = 0;
            foreach (var r in reels)
            {
                if (r == ElementType.Fire)      fc++;
                else if (r == ElementType.Iron) ic++;
                else                            lc++;
            }
            int fire = fc + (fc == 2 ? 1 : fc == 3 ? 3 : 0);
            int iron = ic + (ic == 2 ? 1 : ic == 3 ? 3 : 0);
            int life = lc + (lc == 2 ? 1 : lc == 3 ? 3 : 0);
            return (fire, iron, life);
        }
    }
}
