using UnityEngine;
namespace SlotDefense
{
    [CreateAssetMenu(menuName = "SlotDefense/MonsterConfig", fileName = "NewMonster")]
    public class MonsterConfig : ScriptableObject
    {
        public float hp;
        public float damage;
        public float moveSpeed;
        public float xpReward;
    }
}
