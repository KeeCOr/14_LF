using System.Collections;
using UnityEngine;
namespace SlotDefense
{
    public class GameManager : MonoBehaviour
    {
        [Header("Config")]
        public FixedDeckConfig deckConfig;
        public GlobalBuffConfig buffConfig;
        [SerializeField] private float villageHp      = 1000f;
        [SerializeField] private float battleDuration = 180f;

        public bool isSurvivalMode;
        public bool IsSurvivalMode => isSurvivalMode;

        public BattleManager          Battle          { get; private set; }
        public SlotMachineSystem      SlotMachine     { get; private set; }
        public HandSystem             Hand            { get; private set; }
        public DeckSystem             Deck            { get; private set; }
        public ElementalEnergySystem  ElementalEnergy { get; private set; }
        public ReelSystem             Reels           { get; private set; }

        public static GameManager Instance { get; private set; }

        private System.Random             _rng;
        private ElementType[]             _pendingReels;
        private (int fire, int iron, int life) _pendingEnergy;
        private bool                      _hasPendingSpin;
        private bool                      _battleActive;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            _rng = new System.Random();
            if (isSurvivalMode) battleDuration = 99999f;
            Battle          = new BattleManager(villageHp, battleDuration);
            SlotMachine     = new SlotMachineSystem(chargeInterval: 2f, initialCharges: 3);
            Hand            = new HandSystem(4);
            Deck            = new DeckSystem(deckConfig.cards);
            ElementalEnergy = new ElementalEnergySystem();
            Reels           = new ReelSystem(_rng);
            Reels.BuildPool(deckConfig.cards);
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

            // 손패가 비면 덱에서 다음 4장 자동 지급
            if (Hand.IsEmpty)
            {
                for (int i = 0; i < 4; i++)
                {
                    var card = Deck.DealNext();
                    if (card != null) Hand.TryAdd(card);
                }
            }

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
            _pendingReels  = Reels.Spin();
            _pendingEnergy = ReelSystem.CalcEnergy(_pendingReels);
            _hasPendingSpin = true;
            return true;
        }

        public (ElementType[] reels, (int fire, int iron, int life) energy) PeekSpin() =>
            (_pendingReels, _pendingEnergy);

        public void CommitSpin()
        {
            if (_pendingReels == null) return;
            ElementalEnergy.Add(_pendingEnergy.fire, _pendingEnergy.iron, _pendingEnergy.life);
            _pendingReels   = null;
            _hasPendingSpin = false;
        }

        public void UseSkill(SkillEffect effect, Vector3 worldPos)
        {
            float r = effect.radius;
            foreach (var m in FindObjectsOfType<MonsterController>())
                if (!m.IsDead && Vector2.Distance(worldPos, m.transform.position) <= r)
                    m.TakeDamage(effect.damage);

            // 적 기지에도 피해 적용
            foreach (var v in FindObjectsOfType<Village>())
                if (!v.IsPlayerVillage && Vector2.Distance(worldPos, v.transform.position) <= r)
                    GameEvents.VillageDamaged(false, effect.damage);
        }

        private void HandleVillageDamaged(bool isPlayer, float amount)
        {
            if (isPlayer) Battle.DamagePlayerVillage(amount);
            else          Battle.DamageEnemyVillage(amount);
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
