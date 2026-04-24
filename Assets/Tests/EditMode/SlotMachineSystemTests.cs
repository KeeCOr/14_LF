using NUnit.Framework;
using SlotDefense;

public class SlotMachineSystemTests
{
    [Test]
    public void AddXP_BelowThreshold_DoesNotGrantSpin()
    {
        var sys = new SlotMachineSystem(xpPerSpin: 100f);
        sys.AddXP(99f);
        Assert.AreEqual(0, sys.SpinCharges);
    }

    [Test]
    public void AddXP_ReachesThreshold_GrantsOneSpin()
    {
        var sys = new SlotMachineSystem(xpPerSpin: 100f);
        sys.AddXP(100f);
        Assert.AreEqual(1, sys.SpinCharges);
    }

    [Test]
    public void AddXP_DoubleThreshold_GrantsTwoSpins()
    {
        var sys = new SlotMachineSystem(xpPerSpin: 100f);
        sys.AddXP(200f);
        Assert.AreEqual(2, sys.SpinCharges);
    }

    [Test]
    public void AddXP_Accumulates_AcrossMultipleCalls()
    {
        var sys = new SlotMachineSystem(xpPerSpin: 100f);
        sys.AddXP(60f);
        sys.AddXP(60f);
        Assert.AreEqual(1, sys.SpinCharges);
    }

    [Test]
    public void TrySpin_WithCharge_ReturnsTrueAndDecrementsCharge()
    {
        var sys = new SlotMachineSystem(xpPerSpin: 100f);
        sys.AddXP(100f);
        Assert.IsTrue(sys.TrySpin());
        Assert.AreEqual(0, sys.SpinCharges);
    }

    [Test]
    public void TrySpin_WithoutCharge_ReturnsFalse()
    {
        var sys = new SlotMachineSystem(xpPerSpin: 100f);
        Assert.IsFalse(sys.TrySpin());
    }
}
