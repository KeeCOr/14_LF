namespace SlotDefense
{
    public enum ElementType { Fire, Iron, Life }

    [System.Serializable]
    public struct ElementalCost
    {
        public int fire;
        public int iron;
        public int life;

        public ElementalCost(int fire, int iron, int life)
        { this.fire = fire; this.iron = iron; this.life = life; }

        public int Total => fire + iron + life;
        public bool IsZero => fire == 0 && iron == 0 && life == 0;
    }
}
