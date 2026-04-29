using NUnit.Framework;
using SlotDefense;

public class SlotMachineSystemTests
{
    [Test]
    public void Tick_BelowInterval_DoesNotGrantSpin()
    {
        var sys = new SlotMachineSystem(chargeInterval: 10f);
        sys.Tick(9.9f);
        Assert.AreEqual(0, sys.SpinCharges);
    }

    [Test]
    public void Tick_ReachesInterval_GrantsOneSpin()
    {
        var sys = new SlotMachineSystem(chargeInterval: 10f);
        sys.Tick(10f);
        Assert.AreEqual(1, sys.SpinCharges);
    }

    [Test]
    public void Tick_DoubleInterval_GrantsTwoSpins()
    {
        var sys = new SlotMachineSystem(chargeInterval: 10f);
        sys.Tick(20f);
        Assert.AreEqual(2, sys.SpinCharges);
    }

    [Test]
    public void Tick_Accumulates_AcrossMultipleCalls()
    {
        var sys = new SlotMachineSystem(chargeInterval: 10f);
        sys.Tick(6f);
        sys.Tick(6f);
        Assert.AreEqual(1, sys.SpinCharges);
    }

    [Test]
    public void TrySpin_WithCharge_ReturnsTrueAndDecrementsCharge()
    {
        var sys = new SlotMachineSystem(chargeInterval: 10f, initialCharges: 1);
        Assert.IsTrue(sys.TrySpin());
        Assert.AreEqual(0, sys.SpinCharges);
    }

    [Test]
    public void TrySpin_WithoutCharge_ReturnsFalse()
    {
        var sys = new SlotMachineSystem(chargeInterval: 10f);
        Assert.IsFalse(sys.TrySpin());
    }
}
