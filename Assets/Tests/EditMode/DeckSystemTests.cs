using NUnit.Framework;
using UnityEngine;
using SlotDefense;
using System;

public class DeckSystemTests
{
    private CardData[] MakeDeck(int uniqueCount)
    {
        var deck = new CardData[12];
        var cards = new CardData[uniqueCount];
        for (int i = 0; i < uniqueCount; i++)
        {
            cards[i] = ScriptableObject.CreateInstance<CardData>();
            cards[i].cardName = $"Card{i}";
        }
        for (int i = 0; i < 12; i++) deck[i] = cards[i % uniqueCount];
        return deck;
    }

    [Test]
    public void EvaluateReels_AllSame_ReturnsTriple()
    {
        var card = ScriptableObject.CreateInstance<CardData>();
        var reels = new[] { card, card, card };
        var result = DeckSystem.EvaluateReels(reels, out var matched);
        Assert.AreEqual(SlotResult.Triple, result);
        Assert.AreEqual(card, matched);
    }

    [Test]
    public void EvaluateReels_TwoSame_ReturnsDouble()
    {
        var cardA = ScriptableObject.CreateInstance<CardData>();
        var cardB = ScriptableObject.CreateInstance<CardData>();
        var reels = new[] { cardA, cardA, cardB };
        var result = DeckSystem.EvaluateReels(reels, out var matched);
        Assert.AreEqual(SlotResult.Double, result);
        Assert.AreEqual(cardA, matched);
    }

    [Test]
    public void EvaluateReels_AllDifferent_ReturnsAllDifferent()
    {
        var reels = new[]
        {
            ScriptableObject.CreateInstance<CardData>(),
            ScriptableObject.CreateInstance<CardData>(),
            ScriptableObject.CreateInstance<CardData>()
        };
        var result = DeckSystem.EvaluateReels(reels, out var matched);
        Assert.AreEqual(SlotResult.AllDifferent, result);
        Assert.IsNull(matched);
    }

    [Test]
    public void DrawReels_AlwaysReturnsThreeCardsFromDeck()
    {
        var deck = MakeDeck(3);
        var system = new DeckSystem(deck);
        var rng = new System.Random(42);
        var reels = system.DrawReels(rng);
        Assert.AreEqual(3, reels.Length);
        foreach (var r in reels) Assert.Contains(r, deck);
    }
}
