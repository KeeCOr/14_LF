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
        private System.Random _rng;
        public Portal portal;

        private void Awake()
        {
            _hand        = new HandSystem(4);
            _deck        = new DeckSystem(deckConfig.cards);
            _slotMachine = new SlotMachineSystem(chargeInterval: 3f, initialCharges: 3);
            _rng         = new System.Random();
            _spinTimer   = spinInterval;
            _placeTimer  = placeInterval;
        }

        private void Update() => OnUpdate(Time.deltaTime);

        public void OnUpdate(float deltaTime)
        {
            _slotMachine.Tick(deltaTime);

            _spinTimer  -= deltaTime;
            _placeTimer -= deltaTime;

            if (_spinTimer <= 0f && _slotMachine.TrySpin())
            {
                _spinTimer = spinInterval;
                ExecuteSpin();
            }

            if (_placeTimer <= 0f)
            {
                float effectiveInterval = (portal != null && portal.HpRatio < 0.4f) ? 2f : placeInterval;
                _placeTimer = effectiveInterval;
                PlaceRandomUnit();
            }
        }

        public void ReceiveTransferredMonster(MonsterConfig monster) { }

        private void ExecuteSpin()
        {
            var reels  = _deck.DrawReels(_rng);
            var result = DeckSystem.EvaluateReels(reels, out var matched);
            if (result == SlotResult.AllDifferent)
            {
                if (deckConfig != null && deckConfig.cards.Length > 0)
                {
                    var bonus = deckConfig.cards[_rng.Next(deckConfig.cards.Length)];
                    if (bonus.cardType == CardType.Unit) _hand.TryAdd(bonus);
                }
            }
            else if (matched != null && matched.cardType == CardType.Unit)
            {
                _hand.TryAdd(matched);
            }
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
                go.SetActive(true);
                break;
            }
        }
    }
}
