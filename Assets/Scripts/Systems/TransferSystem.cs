using UnityEngine;
using System.Collections.Generic;
namespace SlotDefense
{
    public class TransferSystem : MonoBehaviour
    {
        public ArenaSystem arenaSystem;
        public Portal portal;
        [SerializeField] private float transferDelay = 1.5f;

        private readonly Queue<(MonsterConfig config, bool toPlayerArena)> _queue = new();
        private float _timer;

        private void Awake() => _timer = transferDelay;

        private void OnEnable() => GameEvents.OnMonsterKilled += EnqueueTransfer;
        private void OnDisable() => GameEvents.OnMonsterKilled -= EnqueueTransfer;

        // 적을 처치하면 같은 진영 포탈에서 추가 병력이 나오는 강화 메커닉
        private void EnqueueTransfer(bool killedInPlayerArena, MonsterConfig config) =>
            _queue.Enqueue((config, toPlayerArena: killedInPlayerArena));

        private void Update()
        {
            if (_queue.Count == 0) return;
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            _timer = transferDelay;
            var (config, toPlayer) = _queue.Dequeue();
            // 포탈 오른쪽에서 출현해야 포탈 스프라이트에 가려지지 않고 보임
            Vector3? spawnPos = portal != null
                ? portal.transform.position + new Vector3(1.0f, 0, 0)
                : (Vector3?)null;
            arenaSystem.SpawnMonsterInArena(toPlayer, config, spawnPos);
        }
    }
}
