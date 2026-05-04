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

        public bool survivalMode;

        public int CurrentStage  { get; private set; } = 1;
        public int SurvivalWave  { get; private set; } = 0;

        private int _selectedHandSlot = -1;
        private int _selectedSkillSlot = -1;
        public int SelectedSlot      => _selectedHandSlot;
        public int SelectedSkillSlot => _selectedSkillSlot;

        private void Start()
        {
            StartCoroutine(survivalMode ? SurvivalWaveLoop() : WaveLoop());
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
            var basePos  = atPosition ?? (isPlayerArena ? playerSpawnPoint.position : enemySpawnPoint.position);
            // 같은 위치에 스폰되면 분리력이 작동 안 하므로 작은 랜덤 오프셋 추가
            var spawnPos = atPosition.HasValue ? basePos
                : basePos + new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.3f, 0.3f), 0);
            var village  = isPlayerArena ? playerVillage : enemyVillage;
            var prefab   = cfg.prefab != null ? cfg.prefab : monsterPrefab;
            var go = Instantiate(prefab, spawnPos, Quaternion.identity);
            go.GetComponent<MonsterController>().Init(cfg, village, isPlayerArena);
            go.SetActive(true);
        }

        private IEnumerator SurvivalWaveLoop()
        {
            yield return new WaitForSeconds(3f);
            while (true)
            {
                SurvivalWave++;
                int count       = Mathf.Min(1 + (SurvivalWave - 1) / 3, 5);
                float eliteProb = Mathf.Min((SurvivalWave - 1) * 0.07f, 0.6f);
                for (int i = 0; i < count; i++)
                {
                    bool isElite = eliteMonsterConfig != null && UnityEngine.Random.value < eliteProb;
                    SpawnSurvivalMonster(isElite ? eliteMonsterConfig : monsterConfig);
                    if (i < count - 1) yield return new WaitForSeconds(0.6f);
                }
                yield return new WaitForSeconds(Mathf.Max(2f, 6f - SurvivalWave * 0.18f));
            }
        }

        private void SpawnSurvivalMonster(MonsterConfig cfg)
        {
            var pos    = enemySpawnPoint != null ? enemySpawnPoint.position : new Vector3(5f, 0, 0);
            var prefab = cfg.prefab != null ? cfg.prefab : monsterPrefab;
            var go     = Instantiate(prefab, pos, Quaternion.identity);
            go.GetComponent<MonsterController>().Init(cfg, playerVillage, playerArena: true);
            go.SetActive(true);
        }

        public void SelectHandSlot(int slotIndex)
        {
            _selectedHandSlot = slotIndex;
            _selectedSkillSlot = -1;
        }

        public void SelectSkillSlot(int slotIndex)
        {
            _selectedSkillSlot = slotIndex;
            _selectedHandSlot  = -1;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;

            if (_selectedSkillSlot >= 0)
            {
                var card = GameManager.Instance.Hand.GetSlot(_selectedSkillSlot);
                int slot = _selectedSkillSlot;
                _selectedSkillSlot = -1;
                if (card != null && card.cardType == CardType.Skill)
                {
                    GameManager.Instance.Hand.Use(slot);
                    GameManager.Instance.UseSkill(card.skillEffect, worldPos);
                    ScreenFlash.Instance?.Play(new Color(0.5f, 0.2f, 1f), 0.3f, 0.08f, 0.25f);
                }
                return;
            }

            if (_selectedHandSlot < 0) return;

            var unitCard = GameManager.Instance.Hand.GetSlot(_selectedHandSlot);
            if (unitCard == null || unitCard.cardType != CardType.Unit)
            {
                _selectedHandSlot = -1;
                return;
            }

            if (!GameManager.Instance.SlotMachine.TryConsume(unitCard.ElementalCost.Total))
            {
                _selectedHandSlot = -1;
                return;
            }

            GameManager.Instance.Hand.Use(_selectedHandSlot);
            _selectedHandSlot = -1;

            worldPos.x = Mathf.Min(worldPos.x, -0.5f);
            var prefab = unitCard.unitPrefab != null ? unitCard.unitPrefab : unitPrefab;
            var go = Instantiate(prefab, worldPos, Quaternion.identity);
            go.GetComponent<UnitController>().Init(unitCard.unitStats, isPlayerUnit: true, portal: portal);
            go.SetActive(true);
        }
    }
}
