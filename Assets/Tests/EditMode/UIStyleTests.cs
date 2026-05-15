using NUnit.Framework;
using SlotDefense;
using UnityEngine;

public class UIStyleTests
{
    [Test]
    public void AccentButtonColors_HasDistinctInteractiveStates()
    {
        var colors = UIStyle.AccentButtonColors(UIStyle.Gold);

        Assert.AreNotEqual(colors.normalColor, colors.highlightedColor);
        Assert.AreNotEqual(colors.normalColor, colors.pressedColor);
        Assert.Less(colors.disabledColor.a, colors.normalColor.a);
    }

    [Test]
    public void CardBackground_DistinguishesSelectedAndUnaffordable()
    {
        var selected = UIStyle.CardBackground(CardType.Unit, canAfford: true, selected: true);
        var affordable = UIStyle.CardBackground(CardType.Unit, canAfford: true, selected: false);
        var unaffordable = UIStyle.CardBackground(CardType.Unit, canAfford: false, selected: false);

        Assert.AreNotEqual(selected, affordable);
        Assert.AreNotEqual(affordable, unaffordable);
        Assert.Greater(selected.g, affordable.g);
        Assert.Less(unaffordable.r + unaffordable.g + unaffordable.b, affordable.r + affordable.g + affordable.b);
    }

    [Test]
    public void CardBackground_UsesDistinctTypeColors()
    {
        var skill = UIStyle.CardBackground(CardType.Skill, canAfford: true, selected: false);
        var buff = UIStyle.CardBackground(CardType.Buff, canAfford: true, selected: false);
        var unit = UIStyle.CardBackground(CardType.Unit, canAfford: true, selected: false);

        Assert.AreNotEqual(skill, unit);
        Assert.AreNotEqual(buff, unit);
    }
}
