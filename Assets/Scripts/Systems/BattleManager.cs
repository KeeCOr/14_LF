using UnityEngine;
namespace SlotDefense
{
    public class BattleManager
    {
        private float _playerHp;
        private float _enemyHp;
        private float _timeRemaining;

        public float PlayerHp => _playerHp;
        public float EnemyHp => _enemyHp;
        public float TimeRemaining => _timeRemaining;

        public BattleManager(float villageHp, float battleDuration)
        {
            _playerHp = villageHp;
            _enemyHp = villageHp;
            _timeRemaining = battleDuration;
        }

        public void DamagePlayerVillage(float amount) =>
            _playerHp = Mathf.Max(0f, _playerHp - amount);

        public void DamageEnemyVillage(float amount) =>
            _enemyHp = Mathf.Max(0f, _enemyHp - amount);

        public void Tick(float deltaTime) => _timeRemaining -= deltaTime;

        public BattleResult GetResult()
        {
            if (_enemyHp <= 0f) return BattleResult.PlayerWin;
            if (_playerHp <= 0f) return BattleResult.PlayerLose;
            if (_timeRemaining <= 0f)
            {
                if (_playerHp > _enemyHp) return BattleResult.PlayerWin;
                if (_enemyHp > _playerHp) return BattleResult.PlayerLose;
                return BattleResult.Draw;
            }
            return BattleResult.Ongoing;
        }
    }
}
