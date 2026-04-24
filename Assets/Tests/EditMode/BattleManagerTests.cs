using NUnit.Framework;
using SlotDefense;

public class BattleManagerTests
{
    [Test]
    public void GetResult_Initially_ReturnsOngoing()
    {
        var bm = new BattleManager(villageHp: 1000f, battleDuration: 180f);
        Assert.AreEqual(BattleResult.Ongoing, bm.GetResult());
    }

    [Test]
    public void DamageEnemyVillage_ReducesTo0_ReturnsPlayerWin()
    {
        var bm = new BattleManager(1000f, 180f);
        bm.DamageEnemyVillage(1000f);
        Assert.AreEqual(BattleResult.PlayerWin, bm.GetResult());
    }

    [Test]
    public void DamagePlayerVillage_ReducesTo0_ReturnsPlayerLose()
    {
        var bm = new BattleManager(1000f, 180f);
        bm.DamagePlayerVillage(1000f);
        Assert.AreEqual(BattleResult.PlayerLose, bm.GetResult());
    }

    [Test]
    public void Tick_ExpiresTimer_PlayerWinWhenHigherHp()
    {
        var bm = new BattleManager(1000f, 1f);
        bm.DamageEnemyVillage(100f);
        bm.Tick(2f);
        Assert.AreEqual(BattleResult.PlayerWin, bm.GetResult());
    }

    [Test]
    public void Tick_ExpiresTimer_DrawWhenEqualHp()
    {
        var bm = new BattleManager(1000f, 1f);
        bm.Tick(2f);
        Assert.AreEqual(BattleResult.Draw, bm.GetResult());
    }

    [Test]
    public void DamagePlayerVillage_CannotGoBelowZero()
    {
        var bm = new BattleManager(1000f, 180f);
        bm.DamagePlayerVillage(9999f);
        Assert.AreEqual(0f, bm.PlayerHp);
    }
}
