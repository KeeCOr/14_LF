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
        private HpBar _hpBar;

        public float HpRatio => _currentHp / maxHp;

        private void Awake()
        {
            _currentHp = maxHp;
            _sr = GetComponent<SpriteRenderer>();
            _hpBar = gameObject.AddComponent<HpBar>();
            _hpBar.Setup(yOffset: 1.15f, width: 0.8f);
        }

        private void Update()
        {
            if (_sr == null) return;
            _sr.color = Color.Lerp(Color.red, new Color(0.7f, 0.2f, 1f), HpRatio);
        }

        public void TakeDamage(float amount)
        {
            if (_currentHp <= 0f) return;
            _currentHp = Mathf.Max(0f, _currentHp - amount);
            _hpBar?.SetRatio(HpRatio);

            // 포탈 오른쪽에서 출현 (스프라이트에 가려지지 않도록)
            var spawnPos = transform.position + new Vector3(1.0f, 0, 0);
            arenaSystem?.SpawnMonsterInArena(isPlayerArena: true, atPosition: spawnPos);

            if (HpRatio < eliteThreshold && eliteConfig != null)
                arenaSystem?.SpawnMonsterInArena(isPlayerArena: true, overrideConfig: eliteConfig, atPosition: spawnPos);
        }
    }
}
