using UnityEngine;
namespace SlotDefense
{
    public class UnitController : MonoBehaviour
    {
        private UnitStats _stats;
        private float _currentHp;
        private float _attackCooldown;
        private MonsterController _target;
        private float _maxX = float.MaxValue;
        private Portal _portal;
        private HpBar _hpBar;

        private void Awake()
        {
            _hpBar = gameObject.AddComponent<HpBar>();
            _hpBar.Setup(yOffset: 0.45f, width: 0.65f);
        }

        public void Init(UnitStats stats, bool isPlayerUnit = false, Portal portal = null)
        {
            _stats = stats;
            _currentHp = stats.hp;
            _maxX = isPlayerUnit ? 0f : float.MaxValue;
            _portal = portal;
        }

        private void Update()
        {
            if (_currentHp <= 0f) return;
            _attackCooldown -= Time.deltaTime;

            AcquireTarget();

            if (_target != null)
            {
                MoveToward(_target.transform.position);
                if (InRange(_target.transform.position) && _attackCooldown <= 0f)
                {
                    _attackCooldown = 1f / _stats.attackRate;
                    _target.TakeDamage(_stats.damage);
                }
            }
            else if (_portal != null)
            {
                MoveToward(_portal.transform.position);
                if (InRange(_portal.transform.position) && _attackCooldown <= 0f)
                {
                    _attackCooldown = 1f / _stats.attackRate;
                    _portal.TakeDamage(_stats.damage);
                }
            }
        }

        private void MoveToward(Vector3 target)
        {
            if (InRange(target)) return;
            var dir = (target - transform.position).normalized;
            var next = transform.position + dir * _stats.moveSpeed * Time.deltaTime;
            if (next.x > _maxX) next.x = _maxX;
            transform.position = next;
        }

        private bool InRange(Vector3 target) =>
            Vector2.Distance(transform.position, target) <= _stats.attackRange;

        private void AcquireTarget()
        {
            if (_target != null && !_target.IsDead) return;
            _target = null;
            float nearest = float.MaxValue;
            foreach (var m in FindObjectsOfType<MonsterController>())
            {
                if (m.IsDead) continue;
                var dist = Vector2.Distance(transform.position, m.transform.position);
                if (dist < nearest) { nearest = dist; _target = m; }
            }
        }

        public void TakeDamage(float amount)
        {
            _currentHp -= amount;
            _hpBar?.SetRatio(_currentHp / _stats.hp);
            if (_currentHp <= 0f) Destroy(gameObject);
        }
    }
}
