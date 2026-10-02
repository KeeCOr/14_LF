using NUnit.Framework;
using UnityEngine;
using SlotDefense;

public class HandSystemTests
{
    private HandSystem _hand;

    [SetUp]
    public void SetUp() => _hand = new HandSystem(4);

    [Test]
    public void TryAdd_ToEmptyHand_ReturnsTrue()
    {
        var card = ScriptableObject.CreateInstance<CardData>();
        Assert.IsTrue(_hand.TryAdd(card));
    }

    [Test]
    public void TryAdd_ToFullHand_ReturnsFalse()
    {
        var card = ScriptableObject.CreateInstance<CardData>();
        _hand.TryAdd(card);
        _hand.TryAdd(card);
        _hand.TryAdd(card);
        _hand.TryAdd(card);
        Assert.IsFalse(_hand.TryAdd(card));
    }

    [Test]
    public void Use_FilledSlot_ReturnsCardAndClearsSlot()
    {
        var card = ScriptableObject.CreateInstance<CardData>();
        card.cardName = "Test";
        _hand.TryAdd(card);
        var used = _hand.Use(0);
        Assert.AreEqual(card, used);
        Assert.IsNull(_hand.GetSlot(0));
    }

    [Test]
    public void Use_EmptySlot_ReturnsNull()
    {
        Assert.IsNull(_hand.Use(0));
    }

    [Test]
    public void IsFull_WhenAllSlotsFilled_ReturnsTrue()
    {
        var card = ScriptableObject.CreateInstance<CardData>();
        for (int i = 0; i < 4; i++) _hand.TryAdd(card);
        Assert.IsTrue(_hand.IsFull);
    }
}
