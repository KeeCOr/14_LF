using UnityEngine;
namespace SlotDefense
{
    public class MonsterController : MonoBehaviour
    {
        [HideInInspector] public MonsterConfig config;
        [HideInInspector] public bool isInPlayerArena;
        [HideInInspector] public Village targetVillage;

        private float _currentHp;
        private float _attackCooldown;
        private const float AttackInterval = 1f;
        private HpBar _hpBar;

        public bool IsDead => _currentHp <= 0f;
        public MonsterConfig Config => config;

        // Init은 Instantiate 직후 (비활성 상태에서) 호출됨.
        // Awake가 아닌 여기서 HpBar를 생성해야 템플릿에 HpBar가 붙지 않음.
        public void Init(MonsterConfig cfg, Village village, bool playerArena)
        {
            config         = cfg;
            targetVillage  = village;
            isInPlayerArena = playerArena;
            _currentHp     = cfg.hp;

            if (_hpBar == null)
            {
                _hpBar = gameObject.AddComponent<HpBar>();
                _hpBar.Setup(yOffset: 0.45f, width: 0.65f);
            }
        }

        private void Update()
        {
            if (IsDead || targetVillage == null) return;
            MoveTowardVillage();
            TryAttackVillage();
        }

        private void MoveTowardVillage()
        {
            var dir = (targetVillage.transform.position - transform.position).normalized;
            transform.position += dir * config.moveSpeed * Time.deltaTime;
        }

        private void TryAttackVillage()
        {
            _attackCooldown -= Time.deltaTime;
            var dist = Vector2.Distance(transform.position, targetVillage.transform.position);
            if (dist > 0.5f || _attackCooldown > 0f) return;
            _attackCooldown = AttackInterval;
            targetVillage.TakeDamage(config.damage);
        }

        public void TakeDamage(float amount)
        {
            if (config == null) return;
            _currentHp -= amount;
            _hpBar?.SetRatio(_currentHp / config.hp);
            if (_currentHp <= 0f) Die();
        }

        private void Die()
        {
            if (config != null)
                GameEvents.MonsterKilled(isInPlayerArena, config);
            Destroy(gameObject);
        }
    }
}
