using UnityEngine;
namespace SlotDefense
{
    [CreateAssetMenu(menuName = "SlotDefense/FixedDeckConfig", fileName = "DefaultDeck")]
    public class FixedDeckConfig : ScriptableObject
    {
        [Tooltip("정확히 12장이어야 합니다")]
        public CardData[] cards = new CardData[12];
    }
}
