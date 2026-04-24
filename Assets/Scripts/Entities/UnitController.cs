using UnityEngine;
namespace SlotDefense
{
    public class UnitController : MonoBehaviour
    {
        private UnitStats _stats;
        private float _currentHp;
        private float _attackCooldown;
        private MonsterController _target;

        public void Init(UnitStats stats)
        {
            _stats = stats;
            _currentHp = stats.hp;
        }

        private void Update()
        {
            if (_currentHp <= 0f) return;
            AcquireTarget();
            if (_target == null) return;
            ChaseTarget();
            TryAttack();
        }

        private void AcquireTarget()
        {
            if (_target != null && !_target.IsDead) return;
            _target = null;
            float nearest = float.MaxValue;
            foreach (var m in FindObjectsOfType<MonsterController>())
            {
                var dist = Vector2.Distance(transform.position, m.transform.position);
                if (dist < nearest) { nearest = dist; _target = m; }
            }
        }

        private void ChaseTarget()
        {
            var dist = Vector2.Distance(transform.position, _target.transform.position);
            if (dist <= _stats.attackRange) return;
            var dir = (_target.transform.position - transform.position).normalized;
            transform.position += dir * _stats.moveSpeed * Time.deltaTime;
        }

        private void TryAttack()
        {
            if (_target == null || _target.IsDead) return;
            _attackCooldown -= Time.deltaTime;
            var dist = Vector2.Distance(transform.position, _target.transform.position);
            if (dist > _stats.attackRange || _attackCooldown > 0f) return;
            _attackCooldown = 1f / _stats.attackRate;
            _target.TakeDamage(_stats.damage);
        }

        public void TakeDamage(float amount)
        {
            _currentHp -= amount;
            if (_currentHp <= 0f) Destroy(gameObject);
        }
    }
}
