using System;
using UnityEngine;
namespace SlotDefense
{
    public static class GameEvents
    {
        public static event Action<bool, MonsterConfig> OnMonsterKilled;
        public static event Action<bool, float> OnVillageDamaged;
        public static event Action<BuffEffect> OnGlobalBuffApplied;
        public static event Action<CardData, CardTier> OnCardObtained;
        public static event Action<BattleResult> OnBattleEnded;

        public static void MonsterKilled(bool isPlayerArena, MonsterConfig config) =>
            OnMonsterKilled?.Invoke(isPlayerArena, config);
        public static void VillageDamaged(bool isPlayer, float amount) =>
            OnVillageDamaged?.Invoke(isPlayer, amount);
        public static void GlobalBuffApplied(BuffEffect buff) =>
            OnGlobalBuffApplied?.Invoke(buff);
        public static void CardObtained(CardData card, CardTier tier) =>
            OnCardObtained?.Invoke(card, tier);
        public static void BattleEnded(BattleResult result) =>
            OnBattleEnded?.Invoke(result);
    }
}
