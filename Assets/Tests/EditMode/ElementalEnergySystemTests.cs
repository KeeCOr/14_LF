using NUnit.Framework;
using SlotDefense;

public class ElementalEnergySystemTests
{
    [Test]
    public void Add_IncreasesEachType()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(2, 1, 3);
        Assert.AreEqual(2, sys.Fire);
        Assert.AreEqual(1, sys.Iron);
        Assert.AreEqual(3, sys.Life);
    }

    [Test]
    public void Add_CapsAtMax()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(99, 99, 99);
        Assert.AreEqual(10, sys.Fire);
        Assert.AreEqual(10, sys.Iron);
        Assert.AreEqual(10, sys.Life);
    }

    [Test]
    public void CanAfford_ReturnsTrueWhenSufficient()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(3, 2, 1);
        Assert.IsTrue(sys.CanAfford(new ElementalCost(3, 2, 1)));
    }

    [Test]
    public void CanAfford_ReturnsFalseWhenInsufficient()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(2, 2, 2);
        Assert.IsFalse(sys.CanAfford(new ElementalCost(3, 0, 0)));
    }

    [Test]
    public void TryConsume_DeductsOnSuccess()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(5, 5, 5);
        Assert.IsTrue(sys.TryConsume(new ElementalCost(2, 1, 3)));
        Assert.AreEqual(3, sys.Fire);
        Assert.AreEqual(4, sys.Iron);
        Assert.AreEqual(2, sys.Life);
    }

    [Test]
    public void TryConsume_ReturnsFalseAndDoesNotDeductOnFailure()
    {
        var sys = new ElementalEnergySystem();
        sys.Add(1, 1, 1);
        Assert.IsFalse(sys.TryConsume(new ElementalCost(2, 0, 0)));
        Assert.AreEqual(1, sys.Fire);
    }
}
