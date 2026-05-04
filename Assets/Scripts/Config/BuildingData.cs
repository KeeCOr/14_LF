namespace SlotDefense
{
    public enum BuildingType { BattleTower, ProductionEnergy, ProductionUnit }

    [System.Serializable]
    public class BuildingData
    {
        public BuildingType buildingType;

        // 전투 건물
        public float attackDamage;
        public float attackRate;
        public float attackRange;
        public bool  canAttackAir;

        // 에너지 생산 건물
        public ElementType energyType;
        public float       energyPerSecond;

        // 유닛 생산 건물
        public CardData unitToSpawn;
        public float    spawnInterval;
    }
}
