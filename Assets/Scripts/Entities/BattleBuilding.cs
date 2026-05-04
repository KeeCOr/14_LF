using UnityEngine;
namespace SlotDefense
{
    public class BattleBuilding : BuildingController
    {
        private float _attackCooldown;

        private void Update()
        {
            if (_data == null) return;
            if (_data.attackRate <= 0f) return;
            _attackCooldown -= Time.deltaTime;
            if (_attackCooldown > 0f) return;

            float nearest = _data.attackRange;
            MonsterController target = null;
            foreach (var m in MonsterController.AllMonsters)
            {
                if (m == null) continue;
                if (m.IsDead) continue;
                if (m.isFlying && !_data.canAttackAir) continue;
                float d = Vector2.Distance(transform.position, m.transform.position);
                if (d < nearest) { nearest = d; target = m; }
            }

            if (target != null)
            {
                _attackCooldown = 1f / _data.attackRate;
                target.TakeDamage(_data.attackDamage);
            }
        }
    }
}
