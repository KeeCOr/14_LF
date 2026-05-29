using NUnit.Framework;
using SlotDefense;
using UnityEngine;

public class UIArtKitTests
{
    [Test]
    public void GeneratedSheets_LoadAsTextures()
    {
        Assert.IsNotNull(UIArtKit.LoadSheet(UIArtSheet.Buttons));
        Assert.IsNotNull(UIArtKit.LoadSheet(UIArtSheet.Frames));
        Assert.IsNotNull(UIArtKit.LoadSheet(UIArtSheet.CardIcons));
        Assert.IsNotNull(UIArtKit.LoadSheet(UIArtSheet.UiAtlas));
    }

    [Test]
    public void NamedSprites_CreateFromGeneratedSheets()
    {
        AssertSprite(UIArtKit.Sprite(UIArtSprite.RedButton));
        AssertSprite(UIArtKit.Sprite(UIArtSprite.BlueButton));
        AssertSprite(UIArtKit.Sprite(UIArtSprite.SlotMachineFrame));
        AssertSprite(UIArtKit.CardIcon("archer"));
        AssertSprite(UIArtKit.CardIcon("mage"));
    }

    [Test]
    public void CardTypeArt_SeparatesMagicFromUnits()
    {
        var unit = ScriptableObject.CreateInstance<CardData>();
        var skill = ScriptableObject.CreateInstance<CardData>();
        try
        {
            unit.cardType = CardType.Unit;
            unit.cardName = "archer";
            skill.cardType = CardType.Skill;
            skill.cardName = "fire spell";
            skill.fireCost = 2;

            Assert.AreNotEqual(UIArtKit.CardIcon(unit), UIArtKit.CardIcon(skill));
            Assert.AreNotEqual(UIArtKit.CardFrame(unit.cardType, 0), UIArtKit.CardFrame(skill.cardType, 0));
        }
        finally
        {
            Object.DestroyImmediate(unit);
            Object.DestroyImmediate(skill);
        }
    }

    [Test]
    public void ReelSprites_UseGeneratedElementIcons()
    {
        AssertSprite(SlotMachineUI.ReelSprite(ElementType.Fire));
        AssertSprite(SlotMachineUI.ReelSprite(ElementType.Iron));
        AssertSprite(SlotMachineUI.ReelSprite(ElementType.Life));
    }

    private static void AssertSprite(Sprite sprite)
    {
        Assert.IsNotNull(sprite);
        Assert.Greater(sprite.rect.width, 32f);
        Assert.Greater(sprite.rect.height, 32f);
    }
}
