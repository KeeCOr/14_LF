using UnityEngine;
namespace SlotDefense
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private float maxHp = 500f;
        [SerializeField] private float eliteThreshold = 0.4f;

        public ArenaSystem arenaSystem;
        public MonsterConfig eliteConfig;

        private float _currentHp;
        private SpriteRenderer _sr;

        public float HpRatio => _currentHp / maxHp;

        private void Awake()
        {
            _currentHp = maxHp;
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_sr == null) return;
            // 색상으로 HP 시각화: 보라 → 빨강
            _sr.color = Color.Lerp(Color.red, new Color(0.7f, 0.2f, 1f), HpRatio);
        }

        public void TakeDamage(float amount)
        {
            if (_currentHp <= 0f) return;
            _currentHp = Mathf.Max(0f, _currentHp - amount);

            // 피격마다 일반 몬스터 소환
            arenaSystem?.SpawnMonsterInArena(isPlayerArena: true);

            // HP 40% 미만이면 정예 몬스터도 추가 소환
            if (HpRatio < eliteThreshold && eliteConfig != null)
                arenaSystem?.SpawnMonsterInArena(isPlayerArena: true, overrideConfig: eliteConfig);
        }
    }
}
