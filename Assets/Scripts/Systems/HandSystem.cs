using System;
namespace SlotDefense
{
    public class HandSystem
    {
        private readonly CardData[] _slots;
        public int Capacity { get; }

        public HandSystem(int capacity = 4)
        {
            Capacity = capacity;
            _slots = new CardData[capacity];
        }

        public bool TryAdd(CardData card)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == null)
                {
                    _slots[i] = card;
                    return true;
                }
            }
            return false;
        }

        public CardData Use(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Length) return null;
            var card = _slots[slotIndex];
            _slots[slotIndex] = null;
            return card;
        }

        public CardData GetSlot(int index) => _slots[index];
        public bool IsFull => Array.TrueForAll(_slots, s => s != null);

        public bool IsEmpty
        {
            get
            {
                for (int i = 0; i < _slots.Length; i++)
                    if (_slots[i] != null) return false;
                return true;
            }
        }
    }
}
