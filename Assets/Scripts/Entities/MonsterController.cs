using System.Collections.Generic;
using UnityEngine;
namespace SlotDefense
{
    public class MonsterController : MonoBehaviour
    {
        public static readonly List<MonsterController> AllMonsters = new List<MonsterController>();

        [HideInInspector] public MonsterConfig config;
        [HideInInspector] public bool isInPlayerArena;
        [HideInInspector] public Village targetVillage;

        private float _currentHp;
        private float _attackCooldown;
        private const float AttackInterval = 1f;
        private const float AttackRange = 0.5f;
        private HpBar _hpBar;
        private Rigidbody2D _rb;

        public bool IsDead => _currentHp <= 0f;
        public MonsterConfig Config => config;

        public void Init(MonsterConfig cfg, Village village, bool playerArena)
        {
            config          = cfg;
            targetVillage   = village;
            isInPlayerArena = playerArena;
            _currentHp      = cfg.hp;
            AllMonsters.Add(this);

            if (_hpBar == null)
            {
                _rb              = gameObject.AddComponent<Rigidbody2D>();
                _rb.gravityScale = 0f;
                _rb.drag         = 6f;
                _rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
                _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

                var col    = gameObject.AddComponent<CircleCollider2D>();
                col.radius = 0.22f;

                _hpBar = gameObject.AddComponent<HpBar>();
                _hpBar.Setup(yOffset: 0.45f, width: 0.65f);
            }
        }

        private void Update()
        {
            if (IsDead || targetVillage == null) return;
            _attackCooldown -= Time.deltaTime;

            var sep  = CalcSeparation();
            var dist = Vector2.Distance(transform.position, targetVillage.transform.position);
            if (dist > AttackRange)
            {
                var dir = ((Vector2)targetVillage.transform.position - (Vector2)transform.position).normalized;
                _rb.velocity = dir * config.moveSpeed + sep;
            }
            else
            {
                _rb.velocity = sep;
                if (_attackCooldown <= 0f)
                {
                    _attackCooldown = AttackInterval;
                    targetVillage.TakeDamage(config.damage);
                }
            }
        }

        private Vector2 CalcSeparation()
        {
            const float sepRadius = 0.65f;
            const float sepForce  = 2.0f;
            var sep = Vector2.zero;
            foreach (var m in AllMonsters)
            {
                if (m == this || m.IsDead) continue;
                var diff = (Vector2)(transform.position - m.transform.position);
                float dist = diff.magnitude;
                if (dist < sepRadius && dist > 0.01f)
                    sep += diff.normalized * ((sepRadius - dist) / sepRadius) * sepForce;
            }
            return sep;
        }

        public void TakeDamage(float amount)
        {
            if (config == null) return;
            _currentHp -= amount;
            _hpBar?.SetRatio(_currentHp / config.hp);
            if (_currentHp <= 0f) Die();
        }

        private void OnDestroy() => AllMonsters.Remove(this);

        private void Die()
        {
            if (config != null)
                GameEvents.MonsterKilled(isInPlayerArena, config);
            Destroy(gameObject);
        }
    }
}
