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

        public GameObject unitPrefab;

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
            while (true)
            {
                yield return new WaitForSeconds(waveInterval);
                SpawnMonsterInArena(isPlayerArena: true);
                SpawnMonsterInArena(isPlayerArena: false);
            }
        }

        public void SpawnMonsterInArena(bool isPlayerArena, MonsterConfig overrideConfig = null)
        {
            var cfg = overrideConfig ?? monsterConfig;
            var spawnPos = isPlayerArena ? playerSpawnPoint.position : enemySpawnPoint.position;
            var village = isPlayerArena ? playerVillage : enemyVillage;
            var go = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
            go.GetComponent<MonsterController>().Init(cfg, village, isPlayerArena);
        }

        public void SelectHandSlot(int slotIndex) => _selectedHandSlot = slotIndex;

        private void Update()
        {
            if (_selectedHandSlot < 0) return;
            if (!Input.GetMouseButtonDown(0)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;

            var card = GameManager.Instance.Hand.Use(_selectedHandSlot);
            _selectedHandSlot = -1;

            if (card == null || card.cardType != CardType.Unit) return;

            var go = Instantiate(unitPrefab, worldPos, Quaternion.identity);
            go.GetComponent<UnitController>().Init(card.unitStats);
        }
    }
}
