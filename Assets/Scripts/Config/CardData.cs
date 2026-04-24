using UnityEngine;
namespace SlotDefense
{
    [CreateAssetMenu(menuName = "SlotDefense/CardData", fileName = "NewCard")]
    public class CardData : ScriptableObject
    {
        public CardType cardType;
        public string cardName;
        public Sprite icon;
        public UnitStats unitStats;
        public SkillEffect skillEffect;
    }
}
