using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
namespace SlotDefense
{
    public class GameManager : MonoBehaviour
    {
        [Header("Config")]
        public FixedDeckConfig deckConfig;
        public GlobalBuffConfig buffConfig;
        [SerializeField] private float villageHp = 1000f;
        [SerializeField] private float battleDuration = 180f;

        public bool isSurvivalMode;
        public bool IsSurvivalMode => isSurvivalMode;

        public BattleManager Battle { get; private set; }
        public SlotMachineSystem SlotMachine { get; private set; }
        public HandSystem Hand { get; private set; }
        public DeckSystem Deck { get; private set; }

        public static GameManager Instance { get; private set; }

        private System.Random _rng;
        private CardData[]  _pendingReels;
        private SlotResult  _pendingSlotResult;
        private CardData    _pendingCard;
        private CardTier    _pendingTier;
        private bool _hasPendingSpin;
        private bool _battleActive;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            _rng = new System.Random();
            if (isSurvivalMode) battleDuration = 99999f;
            Battle = new BattleManager(villageHp, battleDuration);
            SlotMachine = new SlotMachineSystem(chargeInterval: 2f, initialCharges: 3);
            Hand = new HandSystem(4);
            Deck = new DeckSystem(deckConfig.cards);
        }

        private void OnEnable()
        {
            GameEvents.OnVillageDamaged    += HandleVillageDamaged;
            GameEvents.OnGlobalBuffApplied += HandleGlobalBuff;
        }

        private void OnDisable()
        {
            GameEvents.OnVillageDamaged    -= HandleVillageDamaged;
            GameEvents.OnGlobalBuffApplied -= HandleGlobalBuff;
        }

        private void Update()
        {
            if (!_battleActive) return;
            Battle.Tick(Time.deltaTime);
            SlotMachine.Tick(Time.deltaTime);
            var result = Battle.GetResult();
            if (result != BattleResult.Ongoing)
            {
                _battleActive = false;
                GameEvents.BattleEnded(result);
            }
        }

        public void StartBattle() => _battleActive = true;

        public bool TryBeginSpin()
        {
            if (_hasPendingSpin) return false;
            if (!SlotMachine.TrySpin()) return false;
            _pendingReels = Deck.DrawReels(_rng);
            _pendingSlotResult = DeckSystem.EvaluateReels(_pendingReels, out var matchedCard);
            if (_pendingSlotResult == SlotResult.AllDifferent)
            {
                var buffEffect = buffConfig.possibleBuffs[_rng.Next(buffConfig.possibleBuffs.Length)];
                var buffCard = ScriptableObject.CreateInstance<CardData>();
                buffCard.cardName      = string.IsNullOrEmpty(buffEffect.displayName) ? "전투 버프" : buffEffect.displayName;
                buffCard.cardType      = CardType.Buff;
                buffCard.buffEffect    = buffEffect;
                _pendingCard = buffCard;
                _pendingTier = CardTier.Normal;
            }
            else
            {
                if (_pendingSlotResult == SlotResult.Triple)
                {
                    var enhanced = ScriptableObject.CreateInstance<CardData>();
                    enhanced.cardName      = $"[강화] {matchedCard.cardName}";
                    enhanced.cardType      = matchedCard.cardType;
                    enhanced.fireCost      = matchedCard.fireCost;
                    enhanced.ironCost      = matchedCard.ironCost;
                    enhanced.lifeCost      = matchedCard.lifeCost;
                    enhanced.unitStats     = new UnitStats
                    {
                        hp          = matchedCard.unitStats.hp          * 1.5f,
                        damage      = matchedCard.unitStats.damage      * 1.5f,
                        moveSpeed   = matchedCard.unitStats.moveSpeed,
                        attackRange = matchedCard.unitStats.attackRange,
                        attackRate  = matchedCard.unitStats.attackRate,
                        sightRange  = matchedCard.unitStats.sightRange,
                        healAmount  = matchedCard.unitStats.healAmount  * 1.5f,
                        canAttackAir = matchedCard.unitStats.canAttackAir,
                        isFlying    = matchedCard.unitStats.isFlying
                    };
                    enhanced.skillEffect = new SkillEffect
                    {
                        type   = matchedCard.skillEffect.type,
                        damage = matchedCard.skillEffect.damage * 1.5f,
                        radius = matchedCard.skillEffect.radius * 1.3f
                    };
                    _pendingCard = enhanced;
                    _pendingTier = CardTier.Enhanced;
                }
                else
                {
                    _pendingCard = matchedCard;
                    _pendingTier = CardTier.Normal;
                }
            }
            _hasPendingSpin = true;
            return true;
        }

        public (CardData[] reels, SlotResult result) PeekSpin() =>
            (_pendingReels, _pendingSlotResult);

        public void CommitSpin()
        {
            if (_pendingReels == null) return;
            GameEvents.SpinCompleted(_pendingReels, _pendingSlotResult);
            if (Hand.TryAdd(_pendingCard))
                GameEvents.CardObtained(_pendingCard, _pendingTier);
            _pendingReels = null;
            _hasPendingSpin = false;
        }

        public void TrySpin()
        {
            if (!TryBeginSpin()) return;
            CommitSpin();
        }

        public void UseSkill(SkillEffect effect, Vector3 worldPos)
        {
            float r = effect.radius;
            switch (effect.type)
            {
                case SkillType.LightningArrow:
                    foreach (var m in FindObjectsOfType<MonsterController>())
                        if (!m.IsDead && Vector2.Distance(worldPos, m.transform.position) <= r)
                            m.TakeDamage(effect.damage);
                    break;
                case SkillType.PortalBomb:
                    var portal = FindObjectOfType<Portal>();
                    if (portal != null && Vector2.Distance(worldPos, portal.transform.position) <= r)
                        portal.TakeDamage(effect.damage);
                    foreach (var m in FindObjectsOfType<MonsterController>())
                        if (!m.IsDead && Vector2.Distance(worldPos, m.transform.position) <= r)
                            m.TakeDamage(effect.damage);
                    break;
            }
        }

        private void HandleVillageDamaged(bool isPlayer, float amount)
        {
            if (isPlayer) Battle.DamagePlayerVillage(amount);
            else Battle.DamageEnemyVillage(amount);
        }

        private void HandleGlobalBuff(BuffEffect buff) => StartCoroutine(BuffCoroutine(buff));

        private IEnumerator BuffCoroutine(BuffEffect buff)
        {
            UnitController.GlobalDamageMultiplier = buff.attackMultiplier;
            UnitController.GlobalSpeedMultiplier  = buff.speedMultiplier;
            yield return new WaitForSeconds(buff.duration);
            UnitController.GlobalDamageMultiplier = 1f;
            UnitController.GlobalSpeedMultiplier  = 1f;
        }
    }
}
