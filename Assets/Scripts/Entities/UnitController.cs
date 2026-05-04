using System.Collections.Generic;
using UnityEngine;
namespace SlotDefense
{
    public class UnitController : MonoBehaviour
    {
        public static float GlobalDamageMultiplier = 1f;
        public static float GlobalSpeedMultiplier  = 1f;

        public static readonly List<UnitController> ActivePlayerUnits = new List<UnitController>();
        public static readonly List<UnitController> AllUnits          = new List<UnitController>();

        private UnitStats _stats;
        private float _currentHp;
        private float _attackCooldown;
        private MonsterController _target;
        private UnitController _unitTarget;  // 적 유닛 타겟
        private float _maxX = float.MaxValue;
        private float _minX = float.MinValue;
        private Vector2 _forwardDir;
        private Portal _portal;
        private HpBar _hpBar;
        private Rigidbody2D _rb;
        private bool _isPlayerUnit;
        private float _luckTimer;

        public void Init(UnitStats stats, bool isPlayerUnit = false, Portal portal = null)
        {
            _stats        = stats;
            _currentHp    = stats.hp;
            _isPlayerUnit = isPlayerUnit;
            _maxX         = isPlayerUnit ? 0f : float.MaxValue;
            _minX         = isPlayerUnit ? float.MinValue : 0f;
            _forwardDir   = isPlayerUnit ? Vector2.right : Vector2.left;
            _portal       = portal;

            if (_hpBar == null)
            {
                _rb               = gameObject.AddComponent<Rigidbody2D>();
                _rb.gravityScale  = 0f;
                _rb.drag          = 6f;
                _rb.constraints   = RigidbodyConstraints2D.FreezeRotation;
                _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

                var col    = gameObject.AddComponent<CircleCollider2D>();
                col.radius = 0.22f;

                _hpBar = gameObject.AddComponent<HpBar>();
                _hpBar.Setup(yOffset: 0.45f, width: 0.65f);
            }

            if (isPlayerUnit) ActivePlayerUnits.Add(this);
            AllUnits.Add(this);
        }

        private void OnDestroy()
        {
            ActivePlayerUnits.Remove(this);
            AllUnits.Remove(this);
            _currentHp = 0f; // 다른 유닛의 _unitTarget 갱신 트리거용
        }

        private void Update()
        {
            if (_hpBar == null || _currentHp <= 0f) return;
            _attackCooldown -= Time.deltaTime;

            // 행운 생성 (비전투 유닛)
            if (_stats.luckGenRate > 0f && _isPlayerUnit && GameManager.Instance != null)
            {
                _luckTimer += Time.deltaTime;
                if (_luckTimer >= 1f / _stats.luckGenRate)
                {
                    _luckTimer = 0f;
                    GameManager.Instance.SlotMachine.AddCharge(1);
                }
            }

            if (_stats.healAmount > 0f)
            {
                var healTarget = AcquireHealTarget();
                if (healTarget != null)
                {
                    SetVelocityToward(healTarget.transform.position);
                    if (InRange(healTarget.transform.position) && _attackCooldown <= 0f)
                    {
                        _attackCooldown = 1f / _stats.attackRate;
                        healTarget.Heal(_stats.healAmount);
                    }
                }
                else
                {
                    AdvanceForward();
                }
                return;
            }

            AcquireTarget();
            AcquireUnitTarget();

            // 몬스터 타겟과 적 유닛 타겟 중 더 가까운 쪽 우선
            bool hasMonster = _target != null;
            bool hasUnit    = _unitTarget != null;

            if (hasMonster || hasUnit)
            {
                Vector3 attackPos;
                bool    attackUnit = false;
                if (hasMonster && hasUnit)
                {
                    float dm = Vector2.Distance(transform.position, _target.transform.position);
                    float du = Vector2.Distance(transform.position, _unitTarget.transform.position);
                    attackUnit = du < dm;
                }
                else
                {
                    attackUnit = hasUnit;
                }

                if (attackUnit)
                {
                    attackPos = _unitTarget.transform.position;
                    SetVelocityToward(attackPos);
                    if (InRange(attackPos) && _attackCooldown <= 0f)
                    {
                        _attackCooldown = 1f / _stats.attackRate;
                        _unitTarget.TakeDamage(_stats.damage * GlobalDamageMultiplier);
                    }
                }
                else
                {
                    attackPos = _target.transform.position;
                    SetVelocityToward(attackPos);
                    if (InRange(attackPos) && _attackCooldown <= 0f)
                    {
                        _attackCooldown = 1f / _stats.attackRate;
                        _target.TakeDamage(_stats.damage * GlobalDamageMultiplier);
                    }
                }
            }
            else if (_portal != null && InRange(_portal.transform.position))
            {
                _rb.velocity = Vector2.zero;
                if (_attackCooldown <= 0f)
                {
                    _attackCooldown = 1f / _stats.attackRate;
                    _portal.TakeDamage(_stats.damage * GlobalDamageMultiplier);
                }
            }
            else
            {
                if (_isPlayerUnit)
                    _rb.velocity = CalcSeparation(); // 시야 내 적 없음 → 아군 영역 대기
                else
                    AdvanceForward();
            }
        }

        private void AdvanceForward()
        {
            var vel = _forwardDir * _stats.moveSpeed * GlobalSpeedMultiplier + CalcSeparation();
            EnforceBoundary(ref vel);
            _rb.velocity = vel;
        }

        private void SetVelocityToward(Vector3 target)
        {
            var sep = CalcSeparation();
            if (InRange(target)) { _rb.velocity = sep; return; }

            var dir = ((Vector2)target - (Vector2)transform.position).normalized;
            var vel = dir * _stats.moveSpeed * GlobalSpeedMultiplier + sep;
            EnforceBoundary(ref vel);
            _rb.velocity = vel;
        }

        // 주변 유닛들로부터 밀어내는 분리 벡터
        private Vector2 CalcSeparation()
        {
            const float sepRadius = 0.65f;
            const float sepForce  = 2.0f;
            var sep = Vector2.zero;
            foreach (var u in AllUnits)
            {
                if (u == this || u._currentHp <= 0f) continue;
                var diff = (Vector2)(transform.position - u.transform.position);
                float dist = diff.magnitude;
                if (dist < sepRadius && dist > 0.01f)
                    sep += diff.normalized * ((sepRadius - dist) / sepRadius) * sepForce;
            }
            return sep;
        }

        private void EnforceBoundary(ref Vector2 vel)
        {
            if (_maxX < float.MaxValue && transform.position.x >= _maxX && vel.x > 0f) vel.x = 0f;
            if (_minX > float.MinValue && transform.position.x <= _minX && vel.x < 0f) vel.x = 0f;
        }

        private bool InRange(Vector3 target) =>
            Vector2.Distance(transform.position, target) <= _stats.attackRange;

        private void AcquireTarget()
        {
            if (_target != null && !_target.IsDead) return;
            _target = null;
            float nearest = float.MaxValue;
            float sight = _stats.sightRange > 0f ? _stats.sightRange : 999f;
            foreach (var m in FindObjectsOfType<MonsterController>())
            {
                if (m.IsDead) continue;
                if (m.isFlying && !_stats.canAttackAir) continue; // 공중 공격 불가 시 스킵
                var dist = Vector2.Distance(transform.position, m.transform.position);
                if (dist > sight) continue;
                if (dist < nearest) { nearest = dist; _target = m; }
            }
        }

        // 적 유닛(UnitController) 타겟 탐색 — 플레이어↔AI 양방향
        private void AcquireUnitTarget()
        {
            if (_unitTarget != null && _unitTarget._currentHp > 0f) return;
            _unitTarget = null;
            float nearest = float.MaxValue;
            float sight   = _stats.sightRange > 0f ? _stats.sightRange : 999f;
            foreach (var u in AllUnits)
            {
                if (u == this || u._currentHp <= 0f) continue;
                if (u._isPlayerUnit == _isPlayerUnit) continue; // 같은 편 제외
                var dist = Vector2.Distance(transform.position, u.transform.position);
                if (dist > sight) continue;
                if (dist < nearest) { nearest = dist; _unitTarget = u; }
            }
        }

        private UnitController AcquireHealTarget()
        {
            UnitController lowestHp = null;
            float lowestRatio = 1f;
            float sight = _stats.sightRange > 0f ? _stats.sightRange : 999f;
            foreach (var u in ActivePlayerUnits)
            {
                if (u == null || u == this || u._currentHp <= 0f) continue;
                if (u._currentHp >= u._stats.hp) continue;
                float dist = Vector2.Distance(transform.position, u.transform.position);
                if (dist > sight) continue;
                float ratio = u._currentHp / u._stats.hp;
                if (ratio < lowestRatio) { lowestRatio = ratio; lowestHp = u; }
            }
            return lowestHp;
        }

        public void TakeDamage(float amount)
        {
            if (_hpBar == null) return;
            _currentHp -= amount;
            _hpBar?.SetRatio(_currentHp / _stats.hp);
            if (_currentHp <= 0f) Destroy(gameObject);
        }

        public void Heal(float amount)
        {
            if (_hpBar == null) return;
            _currentHp = Mathf.Min(_currentHp + amount, _stats.hp);
            _hpBar.SetRatio(_currentHp / _stats.hp);
        }
    }
}
