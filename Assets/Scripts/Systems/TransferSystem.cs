using UnityEngine;
using System.Collections.Generic;
namespace SlotDefense
{
    public class TransferSystem : MonoBehaviour
    {
        public ArenaSystem arenaSystem;
        [SerializeField] private float transferDelay = 1.5f;

        private readonly Queue<(MonsterConfig config, bool toPlayerArena)> _queue = new();
        private float _timer;

        private void OnEnable() => GameEvents.OnMonsterKilled += EnqueueTransfer;
        private void OnDisable() => GameEvents.OnMonsterKilled -= EnqueueTransfer;

        private void EnqueueTransfer(bool killedInPlayerArena, MonsterConfig config) =>
            _queue.Enqueue((config, toPlayerArena: !killedInPlayerArena));

        private void Update()
        {
            if (_queue.Count == 0) return;
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            _timer = transferDelay;
            var (config, toPlayer) = _queue.Dequeue();
            arenaSystem.SpawnMonsterInArena(toPlayer, config);
        }
    }
}
