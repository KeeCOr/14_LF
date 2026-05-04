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

        public bool isFlying;

        [Tooltip("소환에 사용할 프리팹. 비워두면 기본 템플릿 사용.")]
        public GameObject prefab;
    }
}
