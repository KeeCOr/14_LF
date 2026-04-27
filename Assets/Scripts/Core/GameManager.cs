using UnityEngine;
using System;
namespace SlotDefense
{
    public class GameManager : MonoBehaviour
    {
        [Header("Config")]
        public FixedDeckConfig deckConfig;
        public GlobalBuffConfig buffConfig;
        [SerializeField] private float villageHp = 1000f;
        [SerializeField] private float battleDuration = 180f;
        [SerializeField] private float xpPerSpin = 100f;

        public BattleManager Battle { get; private set; }
        public SlotMachineSystem SlotMachine { get; private set; }
        public HandSystem Hand { get; private set; }
        public DeckSystem Deck { get; private set; }

        public static GameManager Instance { get; private set; }

        private System.Random _rng;
        private bool _battleActive;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            _rng = new System.Random();
            Battle = new BattleManager(villageHp, battleDuration);
            SlotMachine = new SlotMachineSystem(xpPerSpin);
            Hand = new HandSystem(4);
            Deck = new DeckSystem(deckConfig.cards);
        }

        private void OnEnable()
        {
            GameEvents.OnMonsterKilled += HandleMonsterKilled;
            GameEvents.OnVillageDamaged += HandleVillageDamaged;
        }

        private void OnDisable()
        {
            GameEvents.OnMonsterKilled -= HandleMonsterKilled;
            GameEvents.OnVillageDamaged -= HandleVillageDamaged;
        }

        private void Update()
        {
            if (!_battleActive) return;
            Battle.Tick(Time.deltaTime);
            var result = Battle.GetResult();
            if (result != BattleResult.Ongoing)
            {
                _battleActive = false;
                GameEvents.BattleEnded(result);
            }
        }

        public void StartBattle() => _battleActive = true;

        public void TrySpin()
        {
            if (!SlotMachine.TrySpin()) return;
            var reels = Deck.DrawReels(_rng);
            var slotResult = DeckSystem.EvaluateReels(reels, out var matchedCard);
            GameEvents.SpinCompleted(reels, slotResult);
            if (slotResult == SlotResult.AllDifferent)
            {
                var buff = buffConfig.possibleBuffs[_rng.Next(buffConfig.possibleBuffs.Length)];
                GameEvents.GlobalBuffApplied(buff);
            }
            else
            {
                var tier = slotResult == SlotResult.Triple ? CardTier.Enhanced : CardTier.Normal;
                if (Hand.TryAdd(matchedCard))
                    GameEvents.CardObtained(matchedCard, tier);
            }
        }

        private void HandleMonsterKilled(bool isPlayerArena, MonsterConfig config)
        {
            if (isPlayerArena) SlotMachine.AddXP(config.xpReward);
        }

        private void HandleVillageDamaged(bool isPlayer, float amount)
        {
            if (isPlayer) Battle.DamagePlayerVillage(amount);
            else Battle.DamageEnemyVillage(amount);
        }
    }
}
