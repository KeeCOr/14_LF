using UnityEngine;
namespace SlotDefense
{
    public class ElementalEnergySystem
    {
        private int _fire, _iron, _life;
        public const int Max = 10;

        public int Fire => _fire;
        public int Iron => _iron;
        public int Life => _life;

        public void Add(int fire, int iron, int life)
        {
            _fire = Mathf.Min(_fire + fire, Max);
            _iron = Mathf.Min(_iron + iron, Max);
            _life = Mathf.Min(_life + life, Max);
        }

        public void AddByType(ElementType type, int amount)
        {
            switch (type)
            {
                case ElementType.Fire: _fire = Mathf.Min(_fire + amount, Max); break;
                case ElementType.Iron: _iron = Mathf.Min(_iron + amount, Max); break;
                case ElementType.Life: _life = Mathf.Min(_life + amount, Max); break;
            }
        }

        public bool CanAfford(ElementalCost cost) =>
            _fire >= cost.fire && _iron >= cost.iron && _life >= cost.life;

        public bool TryConsume(ElementalCost cost)
        {
            if (!CanAfford(cost)) return false;
            _fire -= cost.fire;
            _iron -= cost.iron;
            _life -= cost.life;
            return true;
        }
    }
}
