using UnityEngine;
using System;
namespace SlotDefense
{
    public class AIOpponent : MonoBehaviour, IOpponent
    {
        public FixedDeckConfig deckConfig;
        public GlobalBuffConfig buffConfig;
        public ArenaSystem arenaSystem;
        public GameObject unitPrefab;

        [Header("AI Difficulty")]
        [SerializeField] private float spinInterval = 4f;
        [SerializeField] private float placeInterval = 6f;

        private HandSystem _hand;
        private DeckSystem _deck;
        private SlotMachineSystem _slotMachine;
        private float _spinTimer;
        private float _placeTimer;
        private Random _rng;

        private void Awake()
        {
            _hand = new HandSystem(4);
            _deck = new DeckSystem(deckConfig.cards);
            _slotMachine = new SlotMachineSystem(xpPerSpin: 80f);
            _rng = new Random();
        }

        private void OnEnable() => GameEvents.OnMonsterKilled += OnMonsterKilled;
        private void OnDisable() => GameEvents.OnMonsterKilled -= OnMonsterKilled;

        private void Update() => OnUpdate(Time.deltaTime);

        public void OnUpdate(float deltaTime)
        {
            _spinTimer -= deltaTime;
            _placeTimer -= deltaTime;

            if (_spinTimer <= 0f && _slotMachine.TrySpin())
            {
                _spinTimer = spinInterval;
                ExecuteSpin();
            }

            if (_placeTimer <= 0f)
            {
                _placeTimer = placeInterval;
                PlaceRandomUnit();
            }
        }

        public void ReceiveTransferredMonster(MonsterConfig monster) =>
            _slotMachine.AddXP(monster.xpReward);

        private void OnMonsterKilled(bool isPlayerArena, MonsterConfig config)
        {
            if (!isPlayerArena) _slotMachine.AddXP(config.xpReward);
        }

        private void ExecuteSpin()
        {
            var reels = _deck.DrawReels(_rng);
            var result = DeckSystem.EvaluateReels(reels, out var matched);
            if (result != SlotResult.AllDifferent && matched != null)
                _hand.TryAdd(matched);
        }

        private void PlaceRandomUnit()
        {
            for (int i = 0; i < 4; i++)
            {
                var card = _hand.GetSlot(i);
                if (card == null || card.cardType != CardType.Unit) continue;
                _hand.Use(i);
                var spawnPos = arenaSystem.enemySpawnPoint.position
                    + new Vector3(_rng.Next(-3, 3), _rng.Next(-2, 2), 0);
                var go = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
                go.GetComponent<UnitController>().Init(card.unitStats);
                break;
            }
        }
    }
}
