using System;
namespace SlotDefense
{
    [Serializable]
    public struct UnitStats
    {
        public float hp;
        public float damage;
        public float moveSpeed;
        public float attackRange;
        public float attackRate;
        public float sightRange;
        public float healAmount;
        public float luckGenRate;  // 행운/초 (0이면 비활성)
        public bool canAttackAir;
        public bool isFlying;
    }
}
