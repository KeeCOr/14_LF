using UnityEngine;
namespace SlotDefense
{
    [CreateAssetMenu(menuName = "SlotDefense/GlobalBuffConfig", fileName = "GlobalBuffs")]
    public class GlobalBuffConfig : ScriptableObject
    {
        [Tooltip("슬롯 모두 다름 결과 시 랜덤 적용될 버프 목록")]
        public BuffEffect[] possibleBuffs;
    }
}
