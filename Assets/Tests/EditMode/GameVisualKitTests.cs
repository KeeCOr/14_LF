using NUnit.Framework;
using SlotDefense;
using UnityEngine;

public class GameVisualKitTests
{
    [Test]
    public void UnitVisualPath_MapsKnownRolesToIncludedAssets()
    {
        Assert.That(GameVisualKit.UnitVisualPath("검사"), Does.Contain("Polytope Studio"));
        Assert.That(GameVisualKit.UnitVisualPath("궁수"), Does.Contain("Polytope Studio"));
        Assert.AreNotEqual(GameVisualKit.UnitVisualPath("검사"), GameVisualKit.UnitVisualPath("궁수"));
    }

    [Test]
    public void MonsterVisualPath_SeparatesNormalAndElite()
    {
        Assert.That(GameVisualKit.MonsterVisualPath(elite: false), Does.Contain("Polytope Studio"));
        Assert.That(GameVisualKit.MonsterVisualPath(elite: true), Does.Contain("Polytope Studio"));
        Assert.AreNotEqual(GameVisualKit.MonsterVisualPath(false), GameVisualKit.MonsterVisualPath(true));
    }

    [Test]
    public void StructureAndSceneryPaths_UseIncludedNatureAndProps()
    {
        Assert.That(GameVisualKit.VillageVisualPath(isPlayer: true), Does.Contain("SimpleNaturePack"));
        Assert.That(GameVisualKit.PortalAccentPath(), Does.Contain("Polytope Studio"));
        Assert.That(GameVisualKit.SceneryPaths, Has.Length.GreaterThanOrEqualTo(4));
        Assert.That(GameVisualKit.SceneryPaths[0], Does.Contain("SimpleNaturePack"));
    }

    [Test]
    public void LoadVisualPrefab_ReturnsAssetForKnownPath()
    {
        var prefab = GameVisualKit.LoadVisualPrefab(GameVisualKit.SceneryPaths[0]);

        Assert.IsNotNull(prefab);
        Assert.IsInstanceOf<GameObject>(prefab);
    }
}
