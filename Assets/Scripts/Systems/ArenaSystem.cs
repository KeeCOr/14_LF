using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
namespace SlotDefense
{
    public class ArenaSystem : MonoBehaviour
    {
        [Header("Player Arena")]
        public Transform playerSpawnPoint;
        public Village playerVillage;
        public GameObject monsterPrefab;

        [Header("Enemy Arena")]
        public Transform enemySpawnPoint;
        public Village enemyVillage;

        [Header("Config")]
        public MonsterConfig monsterConfig;
        [SerializeField] private float waveInterval = 5f;

        [Header("Stage Config")]
        public MonsterConfig eliteMonsterConfig;

        public GameObject unitPrefab;
        public Portal portal;

        public int CurrentStage { get; private set; } = 1;

        private int _selectedHandSlot = -1;
        public int SelectedSlot => _selectedHandSlot;

        private void Start()
        {
            StartCoroutine(WaveLoop());
            if (GameManager.Instance != null)
                GameManager.Instance.StartBattle();
        }

        private IEnumerator WaveLoop()
        {
            float elapsed = 0f;
            while (true)
            {
                int stage = GetStage(elapsed);
                float interval = stage == 1 ? 5f : stage == 2 ? 4.5f : 4f;
                yield return new WaitForSeconds(interval);
                elapsed += interval;
                CurrentStage = stage;
                SpawnWave(isPlayerArena: true,  stage: stage);
                SpawnWave(isPlayerArena: false, stage: stage);
            }
        }

        private static int GetStage(float elapsed)
        {
            if (elapsed < 60f)  return 1;
            if (elapsed < 120f) return 2;
            return 3;
        }

        private void SpawnWave(bool isPlayerArena, int stage)
        {
            MonsterConfig cfg = monsterConfig;
            if (stage == 2 && eliteMonsterConfig != null && UnityEngine.Random.Range(0, 3) == 0)
                cfg = eliteMonsterConfig;
            else if (stage == 3 && eliteMonsterConfig != null && UnityEngine.Random.Range(0, 2) == 0)
                cfg = eliteMonsterConfig;
            SpawnMonsterInArena(isPlayerArena, cfg);
        }

        public void SpawnMonsterInArena(bool isPlayerArena, MonsterConfig overrideConfig = null, Vector3? atPosition = null)
        {
            var cfg      = overrideConfig ?? monsterConfig;
            var spawnPos = atPosition ?? (isPlayerArena ? playerSpawnPoint.position : enemySpawnPoint.position);
            var village  = isPlayerArena ? playerVillage : enemyVillage;
            var go = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
            go.GetComponent<MonsterController>().Init(cfg, village, isPlayerArena);
            go.SetActive(true);
        }

        public void SelectHandSlot(int slotIndex) => _selectedHandSlot = slotIndex;

        private void Update()
        {
            if (_selectedHandSlot < 0) return;
            if (!Input.GetMouseButtonDown(0)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            var card = GameManager.Instance.Hand.GetSlot(_selectedHandSlot);
            if (card == null || card.cardType != CardType.Unit)
            {
                _selectedHandSlot = -1;
                return;
            }

            if (!GameManager.Instance.SlotMachine.TryConsume(card.placementCost))
            {
                _selectedHandSlot = -1;
                return;
            }

            GameManager.Instance.Hand.Use(_selectedHandSlot);
            _selectedHandSlot = -1;

            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;
            worldPos.x = Mathf.Min(worldPos.x, -0.5f);

            var go = Instantiate(unitPrefab, worldPos, Quaternion.identity);
            go.GetComponent<UnitController>().Init(card.unitStats, isPlayerUnit: true, portal: portal);
            go.SetActive(true);
        }
    }
}
