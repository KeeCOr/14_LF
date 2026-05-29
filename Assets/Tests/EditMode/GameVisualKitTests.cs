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
    public void UnitRoleStyle_DistinguishesCoreRoles()
    {
        var sword = GameVisualKit.UnitRoleStyle("검사");
        var archer = GameVisualKit.UnitRoleStyle("궁수");
        var mage = GameVisualKit.UnitRoleStyle("마법사");
        var healer = GameVisualKit.UnitRoleStyle("힐러");

        Assert.AreNotEqual(sword.accentColor, archer.accentColor);
        Assert.AreNotEqual(mage.accentColor, healer.accentColor);
        Assert.AreNotEqual(archer.weaponOffset, sword.weaponOffset);
        Assert.Greater(sword.visualScale, archer.visualScale);
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
    public void AddArenaBackdrop_CreatesFactionBaseDecor()
    {
        var backdrop = GameVisualKit.AddArenaBackdrop(survivalMode: false);

        Assert.IsNotNull(backdrop);
        Assert.AreEqual(GameVisualKit.ArenaBackdropName, backdrop.name);
        Assert.IsNotNull(backdrop.transform.Find("PlayerBaseSilhouette"));
        Assert.IsNotNull(backdrop.transform.Find("EnemyBaseSilhouette"));

        Object.DestroyImmediate(backdrop);
    }

    [Test]
    public void LoadVisualPrefab_ReturnsAssetForKnownPath()
    {
        var prefab = GameVisualKit.LoadVisualPrefab(GameVisualKit.SceneryPaths[0]);

        Assert.IsNotNull(prefab);
        Assert.IsInstanceOf<GameObject>(prefab);
    }

    [Test]
    public void AddArenaScenery_AttachesWindMotion()
    {
        var root = new GameObject("WindSceneryRoot");
        try
        {
            GameVisualKit.AddArenaScenery(root.transform, survivalMode: false);

            Assert.GreaterOrEqual(root.GetComponentsInChildren<LowPolyWindAnimator>().Length, 6);
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }
}
