using System.Collections.Generic;
using UnityEngine;
namespace SlotDefense
{
    public class Village : MonoBehaviour
    {
        public static readonly List<Village> AllVillages = new List<Village>();

        [SerializeField] private bool isPlayerVillage;
        public bool IsPlayerVillage => isPlayerVillage;
        [SerializeField] private float maxHp = 1000f;
        private float _currentHp;
        private HpBar _hpBar;

        public float HpRatio => _currentHp / maxHp;

        private void Awake()
        {
            _currentHp = maxHp;
            AllVillages.Add(this);
            _hpBar = gameObject.AddComponent<HpBar>();
            _hpBar.Setup(yOffset: 1.45f, width: 0.85f);
        }

        private void OnDestroy() => AllVillages.Remove(this);

        public void TakeDamage(float amount)
        {
            _currentHp = Mathf.Max(0f, _currentHp - amount);
            _hpBar?.SetRatio(HpRatio);
            GameEvents.VillageDamaged(isPlayerVillage, amount);
        }
    }
}
