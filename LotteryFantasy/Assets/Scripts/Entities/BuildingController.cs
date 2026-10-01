using UnityEngine;
namespace SlotDefense
{
    public class BuildingController : MonoBehaviour
    {
        protected BuildingData _data;
        protected float _currentHp;
        private const float BuildingHp = 300f;
        private HpBar _hpBar;

        public virtual void Init(BuildingData data)
        {
            _data      = data;
            _currentHp = BuildingHp;
            _hpBar     = gameObject.AddComponent<HpBar>();
            _hpBar.Setup(yOffset: 0.6f, width: 0.8f);
        }

        public void TakeDamage(float amount)
        {
            _currentHp -= amount;
            _hpBar?.SetRatio(_currentHp / BuildingHp);
            if (_currentHp <= 0f) Destroy(gameObject);
        }
    }
}
