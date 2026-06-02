using NUnit.Framework;
using SlotDefense;
using UnityEngine;

public class GameVisualKitTests
{
    [Test]
    public void UnitVisualPath_MapsKnownRolesToIncludedAssets()
    {
        Assert.That(GameVisualKit.UnitVisualPath("Swordsman"), Does.Contain("Polytope Studio"));
        Assert.That(GameVisualKit.UnitVisualPath("Archer"), Does.Contain("Polytope Studio"));
        Assert.AreNotEqual(GameVisualKit.UnitVisualPath("Swordsman"), GameVisualKit.UnitVisualPath("Archer"));
    }

    [Test]
    public void UnitRoleStyle_DistinguishesCoreRoles()
    {
        var sword = GameVisualKit.UnitRoleStyle("Swordsman");
        var archer = GameVisualKit.UnitRoleStyle("Archer");
        var mage = GameVisualKit.UnitRoleStyle("Mage");
        var healer = GameVisualKit.UnitRoleStyle("Healer");

        Assert.AreNotEqual(sword.accentColor, archer.accentColor);
        Assert.AreNotEqual(mage.accentColor, healer.accentColor);
        Assert.AreNotEqual(archer.weaponOffset, sword.weaponOffset);
        Assert.Greater(sword.visualScale, archer.visualScale);
    }

    [Test]
    public void FactionColor_DistinguishesPlayerEnemyAndNeutral()
    {
        Assert.AreNotEqual(GameVisualKit.FactionColor(VisualFacing.Player), GameVisualKit.FactionColor(VisualFacing.Enemy));
        Assert.AreNotEqual(GameVisualKit.FactionColor(VisualFacing.Enemy), GameVisualKit.FactionColor(VisualFacing.Neutral));
        Assert.AreEqual(GameVisualKit.NeutralMonsterOrange, GameVisualKit.FactionColor(VisualFacing.Neutral));
    }

    [Test]
    public void AttachUnitVisual_AddsFactionMarkerForPlayerAndEnemy()
    {
        var player = new GameObject("PlayerUnit");
        var enemy = new GameObject("EnemyUnit");
        try
        {
            GameVisualKit.AttachUnitVisual(player, "Swordsman", VisualFacing.Player);
            GameVisualKit.AttachUnitVisual(enemy, "Swordsman", VisualFacing.Enemy);

            Assert.IsNotNull(player.transform.Find("FactionMarker"));
            Assert.IsNotNull(enemy.transform.Find("FactionMarker"));
            Assert.IsNotNull(player.transform.Find("FactionMarker/FactionGroundStripe"));
            Assert.IsNotNull(enemy.transform.Find("FactionMarker/FactionSideBadge"));
        }
        finally
        {
            Object.DestroyImmediate(player);
            Object.DestroyImmediate(enemy);
        }
    }

    [Test]
    public void AttachMonsterVisual_AlwaysUsesNeutralMonsterMarker()
    {
        var monster = new GameObject("Monster");
        try
        {
            GameVisualKit.AttachMonsterVisual(monster, elite: true, facing: VisualFacing.Enemy);

            Assert.IsNotNull(monster.transform.Find("NeutralMonsterMarker"));
            Assert.IsNotNull(monster.transform.Find("NeutralMonsterMarker/MonsterWarningCore"));
            Assert.IsNotNull(monster.transform.Find("NeutralMonsterMarker/MonsterWarningTop"));
            Assert.IsNull(monster.transform.Find("FactionMarker"));
        }
        finally
        {
            Object.DestroyImmediate(monster);
        }
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
