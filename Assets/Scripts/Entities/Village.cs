using UnityEngine;
namespace SlotDefense
{
    public class Village : MonoBehaviour
    {
        [SerializeField] private bool isPlayerVillage;
        [SerializeField] private float maxHp = 1000f;
        private float _currentHp;

        public float HpRatio => _currentHp / maxHp;

        private void Awake() => _currentHp = maxHp;

        public void TakeDamage(float amount)
        {
            _currentHp = Mathf.Max(0f, _currentHp - amount);
            GameEvents.VillageDamaged(isPlayerVillage, amount);
        }
    }
}
