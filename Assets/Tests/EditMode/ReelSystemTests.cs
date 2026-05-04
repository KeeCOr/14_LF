using NUnit.Framework;
using SlotDefense;
using UnityEngine;

public class ReelSystemTests
{
    private CardData MakeCard(int fire, int iron, int life)
    {
        var c = ScriptableObject.CreateInstance<CardData>();
        c.fireCost = fire; c.ironCost = iron; c.lifeCost = life;
        return c;
    }

    [Test]
    public void BuildPool_ReflectsDeckCosts()
    {
        var deck = new[] { MakeCard(2, 0, 0), MakeCard(0, 1, 0) };
        var sys = new ReelSystem(new System.Random(0));
        sys.BuildPool(deck);
        Assert.AreEqual(3, sys.PoolSize); // Fire x2 + Iron x1
    }

    [Test]
    public void BuildPool_EmptyDeck_FallsBackToOneEach()
    {
        var sys = new ReelSystem(new System.Random(0));
        sys.BuildPool(new CardData[0]);
        Assert.AreEqual(3, sys.PoolSize); // fallback: 1 of each
    }

    [Test]
    public void CalcEnergy_AllDifferent_GivesOneEach()
    {
        var reels = new[] { ElementType.Fire, ElementType.Iron, ElementType.Life };
        var (f, i, l) = ReelSystem.CalcEnergy(reels);
        Assert.AreEqual(1, f);
        Assert.AreEqual(1, i);
        Assert.AreEqual(1, l);
    }

    [Test]
    public void CalcEnergy_TwoSame_GivesBonus()
    {
        var reels = new[] { ElementType.Fire, ElementType.Fire, ElementType.Iron };
        var (f, i, l) = ReelSystem.CalcEnergy(reels);
        Assert.AreEqual(3, f); // 2 base + 1 bonus
        Assert.AreEqual(1, i);
        Assert.AreEqual(0, l);
    }

    [Test]
    public void CalcEnergy_ThreeSame_GivesBigBonus()
    {
        var reels = new[] { ElementType.Fire, ElementType.Fire, ElementType.Fire };
        var (f, i, l) = ReelSystem.CalcEnergy(reels);
        Assert.AreEqual(6, f); // 3 base + 3 bonus
        Assert.AreEqual(0, i);
        Assert.AreEqual(0, l);
    }
}
