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

        public bool IsDead => _currentHp <= 0f;
        public MonsterConfig Config => config;

        public void Init(MonsterConfig cfg, Village village, bool playerArena)
        {
            config = cfg;
            targetVillage = village;
            isInPlayerArena = playerArena;
            _currentHp = cfg.hp;
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
            _currentHp -= amount;
            if (_currentHp <= 0f) Die();
        }

        private void Die()
        {
            GameEvents.MonsterKilled(isInPlayerArena, config);
            Destroy(gameObject);
        }
    }
}
