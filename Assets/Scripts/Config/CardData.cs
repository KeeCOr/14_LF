using UnityEngine;
namespace SlotDefense
{
    [CreateAssetMenu(menuName = "SlotDefense/CardData", fileName = "NewCard")]
    public class CardData : ScriptableObject
    {
        public CardType    cardType;
        public string      cardName;
        public Sprite      icon;
        public UnitStats   unitStats;
        public SkillEffect skillEffect;
        public BuffEffect  buffEffect;
        public BuildingData buildingData;

        [Tooltip("화/철/생명 에너지 비용")]
        public int fireCost;
        public int ironCost;
        public int lifeCost;

        [Tooltip("유닛 전용 프리팹. 비워두면 기본 템플릿 사용.")]
        public GameObject unitPrefab;

        public ElementalCost ElementalCost => new ElementalCost(fireCost, ironCost, lifeCost);
    }
}
