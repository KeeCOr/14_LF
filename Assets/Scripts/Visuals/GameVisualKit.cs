using UnityEngine;

namespace SlotDefense
{
    public enum VisualFacing
    {
        Player,
        Enemy,
        Neutral
    }

    public readonly struct UnitVisualStyle
    {
        public readonly Color accentColor;
        public readonly float visualScale;
        public readonly Vector3 visualOffset;
        public readonly Vector3 weaponOffset;
        public readonly float weaponScale;
        public readonly float weaponRotationZ;
        public readonly string weaponPath;

        public UnitVisualStyle(Color accentColor, float visualScale, Vector3 visualOffset,
            Vector3 weaponOffset, float weaponScale, float weaponRotationZ, string weaponPath)
        {
            this.accentColor = accentColor;
            this.visualScale = visualScale;
            this.visualOffset = visualOffset;
            this.weaponOffset = weaponOffset;
            this.weaponScale = weaponScale;
            this.weaponRotationZ = weaponRotationZ;
            this.weaponPath = weaponPath;
        }
    }

    public static class GameVisualKit
    {
        public const string ArenaBackdropName = "ArenaBackdrop";
        public const string ToonyRtsPrefabRoot = "Asset/ToonyTinyPeople/TT_RTS/TT_RTS_Standard/prefabs";

        public static readonly Color PlayerBlue = new Color(0.05f, 0.32f, 0.85f, 1f);
        public static readonly Color EnemyRed = new Color(0.78f, 0.16f, 0.12f, 1f);
        public static readonly Color RoyalGold = new Color(1f, 0.73f, 0.16f, 1f);
        public static readonly Color NeutralMonsterOrange = new Color(1f, 0.54f, 0.08f, 1f);
        public static readonly Color NeutralMonsterViolet = new Color(0.58f, 0.18f, 0.90f, 1f);

        public static readonly string[] SceneryPaths =
        {
            "Asset/SimpleNaturePack/Prefabs/Tree_01",
            "Asset/SimpleNaturePack/Prefabs/Tree_03",
            "Asset/Polytope Studio/Lowpoly_Environments/Prefabs/Plants/PT_Grass_02",
            "Asset/SimpleNaturePack/Prefabs/Rock_02",
            "Asset/SimpleNaturePack/Prefabs/Bush_01",
            "Asset/SimpleNaturePack/Prefabs/Flowers_01",
            "Asset/Polytope Studio/Lowpoly_Props/Prefabs/PT_Village_Fence_Small_01"
        };

        public static readonly string[] ToonyRtsSceneryPaths =
        {
            $"{ToonyRtsPrefabRoot}/machines/TT_Ballista_lvl1",
            $"{ToonyRtsPrefabRoot}/machines/TT_Catapult_lvl1",
            $"{ToonyRtsPrefabRoot}/banners/TT_Banner_Yellow",
            $"{ToonyRtsPrefabRoot}/banners/TT_Banner_Orange"
        };

        public static string UnitVisualPath(string unitName)
        {
            if (ContainsAny(unitName, "Archer"))
                return $"{ToonyRtsPrefabRoot}/TT_Archer";
            if (ContainsAny(unitName, "Mage", "Pyro"))
                return $"{ToonyRtsPrefabRoot}/TT_Mage";
            if (ContainsAny(unitName, "Healer", "Cleric", "Luck"))
                return $"{ToonyRtsPrefabRoot}/TT_Priest";
            if (ContainsAny(unitName, "Knight", "Paladin", "Crusader"))
                return $"{ToonyRtsPrefabRoot}/TT_Paladin";
            if (ContainsAny(unitName, "Giant", "Golem", "Iron"))
                return $"{ToonyRtsPrefabRoot}/TT_Heavy_Infantry";

            return $"{ToonyRtsPrefabRoot}/TT_Swordman";
        }

        public static Color FactionColor(VisualFacing facing)
        {
            switch (facing)
            {
                case VisualFacing.Player:
                    return new Color(0.18f, 0.62f, 1f, 0.96f);
                case VisualFacing.Enemy:
                    return new Color(1f, 0.18f, 0.14f, 0.96f);
                default:
                    return NeutralMonsterOrange;
            }
        }

        public static string MonsterVisualPath(bool elite)
        {
            return elite
                ? $"{ToonyRtsPrefabRoot}/TT_King"
                : $"{ToonyRtsPrefabRoot}/TT_HeavySwordman";
        }

        public static string VillageVisualPath(bool isPlayer)
        {
            return isPlayer
                ? $"{ToonyRtsPrefabRoot}/banners/TT_Banner_Blue_A"
                : $"{ToonyRtsPrefabRoot}/banners/TT_Banner_Red";
        }

        public static string PortalAccentPath()
        {
            return $"{ToonyRtsPrefabRoot}/banners/TT_Banner_Purple";
        }

        public static string BuildingVisualPath(CardData card)
        {
            var name = card != null ? card.cardName : string.Empty;
            if (ContainsAny(name, "Sniper", "Tower"))
                return $"{ToonyRtsPrefabRoot}/machines/TT_Ballista_lvl2";
            if (ContainsAny(name, "Fire", "Magic"))
                return $"{ToonyRtsPrefabRoot}/machines/TT_Catapult_lvl2";
            if (ContainsAny(name, "Barracks"))
                return $"{ToonyRtsPrefabRoot}/machines/TT_Ram_lvl1";

            return $"{ToonyRtsPrefabRoot}/machines/TT_Catapult_lvl1";
        }

        public static GameObject LoadVisualPrefab(string resourcesPath)
        {
            return Resources.Load<GameObject>(resourcesPath);
        }

        public static UnitVisualStyle UnitRoleStyle(string unitName)
        {
            if (ContainsAny(unitName, "Archer"))
                return new UnitVisualStyle(
                    new Color(0.20f, 0.95f, 0.35f, 0.88f),
                    0.64f,
                    new Vector3(0f, -0.38f, -0.08f),
                    new Vector3(0.32f, -0.03f, -0.13f),
                    0.28f,
                    -52f,
                    "Asset/Polytope Studio/Lowpoly_Weapons/Prefabs/PT_Sword_01_a");

            if (ContainsAny(unitName, "Mage", "Pyro"))
                return new UnitVisualStyle(
                    new Color(0.95f, 0.28f, 1f, 0.90f),
                    0.70f,
                    new Vector3(0f, -0.40f, -0.08f),
                    new Vector3(0.10f, 0.08f, -0.13f),
                    0.32f,
                    74f,
                    "Asset/Polytope Studio/Lowpoly_Weapons/Prefabs/PT_Sword_01_a");

            if (ContainsAny(unitName, "Healer", "Cleric", "Luck"))
                return new UnitVisualStyle(
                    new Color(0.35f, 1f, 0.64f, 0.88f),
                    0.58f,
                    new Vector3(0f, -0.36f, -0.08f),
                    new Vector3(-0.16f, -0.08f, -0.13f),
                    0.32f,
                    12f,
                    "Asset/Polytope Studio/Lowpoly_Weapons/Prefabs/PT_Shield_01_a");

            if (ContainsAny(unitName, "Giant", "Golem", "Iron"))
                return new UnitVisualStyle(
                    new Color(0.55f, 0.78f, 1f, 0.90f),
                    0.88f,
                    new Vector3(0f, -0.48f, -0.08f),
                    new Vector3(-0.22f, -0.06f, -0.13f),
                    0.44f,
                    -8f,
                    "Asset/Polytope Studio/Lowpoly_Weapons/Prefabs/PT_Shield_01_a");

            if (ContainsAny(unitName, "Knight", "Paladin", "Crusader"))
                return new UnitVisualStyle(
                    new Color(1f, 0.72f, 0.18f, 0.90f),
                    0.80f,
                    new Vector3(0f, -0.44f, -0.08f),
                    new Vector3(-0.18f, -0.08f, -0.13f),
                    0.38f,
                    -6f,
                    "Asset/Polytope Studio/Lowpoly_Weapons/Prefabs/PT_Shield_01_a");

            return new UnitVisualStyle(
                new Color(0.28f, 0.62f, 1f, 0.88f),
                0.76f,
                new Vector3(0f, -0.42f, -0.08f),
                new Vector3(0.22f, -0.10f, -0.13f),
                0.34f,
                -18f,
                "Asset/Polytope Studio/Lowpoly_Weapons/Prefabs/PT_Sword_01_a");
        }

        public static GameObject AddArenaBackdrop(bool survivalMode)
        {
            var root = GameObject.Find(ArenaBackdropName);
            if (root != null) return root;

            root = new GameObject(ArenaBackdropName);
            CreateSprite("DistantHills", root.transform, new Vector3(0f, 1.32f, 0.32f), new Vector2(18.8f, 2.0f),
                survivalMode ? new Color(0.12f, 0.14f, 0.24f, 0.95f) : new Color(0.44f, 0.62f, 0.48f, 0.92f), -28);
            CreateSprite("BattlefieldGrass", root.transform, new Vector3(0f, -0.34f, 0.34f), new Vector2(19.6f, 4.1f),
                survivalMode ? new Color(0.13f, 0.18f, 0.14f, 1f) : new Color(0.34f, 0.58f, 0.22f, 1f), -25);
            CreateSprite("CenterDirtLane", root.transform, new Vector3(0f, -0.48f, 0.33f), new Vector2(13.6f, 0.82f),
                survivalMode ? new Color(0.25f, 0.20f, 0.17f, 0.88f) : new Color(0.60f, 0.46f, 0.25f, 0.92f), -24);
            CreateSprite("StoneTerrace", root.transform, new Vector3(0f, -2.16f, 0.31f), new Vector2(19.6f, 0.48f),
                new Color(0.28f, 0.30f, 0.33f, 1f), -23);
            CreateSprite("CenterEmblem", root.transform, new Vector3(0f, -0.48f, 0.30f), new Vector2(0.52f, 0.52f),
                RoyalGold, -22);

            AddBaseSilhouette(root.transform, new Vector3(-7.5f, 0f, 0.28f), true);
            AddToonyBanner(root.transform, new Vector3(-6.75f, 0.30f, 0.16f), VisualFacing.Player);
            CreateGeneratedSprite("PlayerTowerArt", root.transform, UIArtSprite.PlayerTower, new Vector3(-7.5f, -0.10f, 0.20f), 2.65f, -12, LowPolyWindProfile.Banner);
            CreateGeneratedSprite("CenterGroundArt", root.transform, UIArtSprite.GroundPatch, new Vector3(0f, -0.78f, 0.20f), 1.10f, -20);
            if (!survivalMode)
            {
                AddBaseSilhouette(root.transform, new Vector3(7.5f, 0f, 0.28f), false);
                AddToonyBanner(root.transform, new Vector3(6.75f, 0.30f, 0.16f), VisualFacing.Enemy);
                CreateGeneratedSprite("EnemyTowerArt", root.transform, UIArtSprite.EnemyTower, new Vector3(7.5f, -0.10f, 0.20f), 2.65f, -12, LowPolyWindProfile.Banner);
            }
            else
            {
                AddPortalPedestal(root.transform, new Vector3(7.5f, 0f, 0.28f));
                AddToonyBanner(root.transform, new Vector3(6.75f, 0.30f, 0.16f), VisualFacing.Neutral);
                CreateGeneratedSprite("PortalGateArt", root.transform, UIArtSprite.PortalGate, new Vector3(7.5f, -0.05f, 0.20f), 2.25f, -12, LowPolyWindProfile.Portal);
            }

            return root;
        }

        public static GameObject AttachUnitVisual(GameObject root, string unitName, VisualFacing facing)
        {
            var style = UnitRoleStyle(unitName);
            var visual = AttachVisual(root, UnitVisualPath(unitName), "UnitVisual", facing, style.visualOffset, style.visualScale);
            BoostToonyVisual(visual, 1.55f, 9);
            AttachFactionMarker(root, facing, false);
            AttachRoleAccent(root, style, facing);
            AttachWeapon(root, style, facing);
            FadeFallbackSprite(root, visual != null ? 0.035f : 0.18f);
            return visual;
        }

        public static GameObject AttachMonsterVisual(GameObject root, bool elite, VisualFacing facing)
        {
            var visual = AttachVisual(root, MonsterVisualPath(elite), elite ? "EliteMonsterVisual" : "MonsterVisual",
                facing, new Vector3(0f, -0.34f, -0.08f), elite ? 0.78f : 0.64f);
            BoostToonyVisual(visual, elite ? 1.70f : 1.45f, 9);
            AttachFactionMarker(root, VisualFacing.Neutral, elite);
            FadeFallbackSprite(root, visual != null ? 0.06f : elite ? 0.36f : 0.30f);
            return visual;
        }

        public static GameObject AttachVillageVisual(GameObject root, bool isPlayer)
        {
            var visual = AttachVisual(root, VillageVisualPath(isPlayer), "VillageVisual", isPlayer ? VisualFacing.Player : VisualFacing.Enemy,
                new Vector3(0f, -0.78f, -0.12f), 0.90f);
            FadeFallbackSprite(root, 0.20f);
            return visual;
        }

        public static GameObject AttachPortalVisual(GameObject root)
        {
            var visual = AttachVisual(root, PortalAccentPath(), "PortalRelic", VisualFacing.Neutral, new Vector3(0f, -0.72f, -0.10f), 0.70f);
            FadeFallbackSprite(root, 0.55f);
            return visual;
        }

        public static GameObject AttachBuildingVisual(GameObject root, CardData card)
        {
            var visual = AttachVisual(root, BuildingVisualPath(card), "BuildingVisual", VisualFacing.Player, new Vector3(0f, -0.42f, -0.10f), 0.48f);
            BoostToonyVisual(visual, 1.35f, 8);
            FadeFallbackSprite(root, visual != null ? 0.06f : 0.28f);
            return visual;
        }

        public static void AddArenaScenery(Transform parent, bool survivalMode)
        {
            var positions = survivalMode
                ? new[]
                {
                    new Vector3(-8.8f, -1.35f, 0.18f), new Vector3(-6.2f, 1.45f, 0.18f),
                    new Vector3(-3.4f, -1.55f, 0.18f), new Vector3(2.4f, 1.35f, 0.18f),
                    new Vector3(6.0f, -1.45f, 0.18f), new Vector3(8.8f, 1.30f, 0.18f)
                }
                : new[]
                {
                    new Vector3(-8.8f, -1.35f, 0.18f), new Vector3(-6.2f, 1.45f, 0.18f),
                    new Vector3(-3.2f, -1.55f, 0.18f), new Vector3(3.2f, 1.45f, 0.18f),
                    new Vector3(6.2f, -1.35f, 0.18f), new Vector3(8.8f, 1.30f, 0.18f)
                };

            for (int i = 0; i < positions.Length; i++)
            {
                CreateGeneratedSprite($"GeneratedProp_{i}", parent, ScenerySprite(i), positions[i] + new Vector3(0f, -0.08f, -0.04f), SceneryHeight(i), -10 + i, SceneryWindProfile(i));

                var prefab = LoadVisualPrefab(SceneryPaths[i % SceneryPaths.Length]);
                if (prefab == null) continue;

                var go = Object.Instantiate(prefab, positions[i], Quaternion.Euler(0f, 0f, 0f), parent);
                go.name = $"Scenery_{i}_{prefab.name}";
                go.transform.localScale = Vector3.one * (i % 2 == 0 ? 0.82f : 0.68f);
                StripPhysics(go);
                LowPolyWindAnimator.Attach(go, SceneryWindProfile(i), i * 0.73f);
            }

            AddToonyBattlefieldProps(parent, survivalMode);
            AddWindGrass(parent, survivalMode);
        }

        private static UIArtSprite ScenerySprite(int index)
        {
            switch (index % 6)
            {
                case 0:
                    return UIArtSprite.PineTree;
                case 1:
                    return UIArtSprite.WoodenFence;
                case 2:
                    return UIArtSprite.RockCluster;
                case 3:
                    return UIArtSprite.FlowerBush;
                case 4:
                    return UIArtSprite.Campfire;
                default:
                    return UIArtSprite.StoneWall;
            }
        }

        private static float SceneryHeight(int index)
        {
            switch (index % 6)
            {
                case 0:
                    return 1.35f;
                case 1:
                    return 0.72f;
                case 2:
                    return 0.88f;
                case 3:
                    return 0.72f;
                case 4:
                    return 0.92f;
                default:
                    return 0.82f;
            }
        }

        private static LowPolyWindProfile SceneryWindProfile(int index)
        {
            switch (index % 6)
            {
                case 0:
                    return LowPolyWindProfile.Tree;
                case 1:
                    return LowPolyWindProfile.Banner;
                case 2:
                    return LowPolyWindProfile.AmbientProp;
                case 3:
                    return LowPolyWindProfile.Grass;
                case 4:
                    return LowPolyWindProfile.Torch;
                default:
                    return LowPolyWindProfile.Banner;
            }
        }

        private static void AddWindGrass(Transform parent, bool survivalMode)
        {
            var grassPrefab = LoadVisualPrefab("Asset/Polytope Studio/Lowpoly_Environments/Prefabs/Plants/PT_Grass_02");
            var grassPositions = survivalMode
                ? new[]
                {
                    new Vector3(-7.1f, -0.78f, 0.12f), new Vector3(-4.8f, -0.93f, 0.12f),
                    new Vector3(-1.9f, -0.66f, 0.12f), new Vector3(1.9f, -0.86f, 0.12f),
                    new Vector3(4.6f, -0.65f, 0.12f), new Vector3(7.0f, -0.88f, 0.12f)
                }
                : new[]
                {
                    new Vector3(-7.3f, -0.86f, 0.12f), new Vector3(-4.9f, -0.66f, 0.12f),
                    new Vector3(-2.4f, -1.02f, 0.12f), new Vector3(2.4f, -1.02f, 0.12f),
                    new Vector3(4.9f, -0.66f, 0.12f), new Vector3(7.3f, -0.86f, 0.12f)
                };

            for (int i = 0; i < grassPositions.Length; i++)
            {
                if (grassPrefab != null)
                {
                    var grass = Object.Instantiate(grassPrefab, grassPositions[i], Quaternion.Euler(0f, 0f, 0f), parent);
                    grass.name = $"WindGrass_{i}";
                    grass.transform.localScale = Vector3.one * (0.45f + 0.05f * (i % 3));
                    StripPhysics(grass);
                    LowPolyWindAnimator.Attach(grass, LowPolyWindProfile.Grass, i * 0.91f);
                }
                else
                {
                    CreateGeneratedSprite($"WindGrassFallback_{i}", parent, UIArtSprite.FlowerBush, grassPositions[i], 0.45f, -12 + i, LowPolyWindProfile.Grass);
                }
            }
        }

        private static void AddToonyBattlefieldProps(Transform parent, bool survivalMode)
        {
            var positions = survivalMode
                ? new[]
                {
                    new Vector3(-6.8f, -1.05f, 0.10f),
                    new Vector3(-2.8f, 1.12f, 0.10f),
                    new Vector3(2.8f, 1.12f, 0.10f),
                    new Vector3(6.8f, -1.05f, 0.10f)
                }
                : new[]
                {
                    new Vector3(-6.9f, -1.02f, 0.10f),
                    new Vector3(-3.8f, 1.16f, 0.10f),
                    new Vector3(3.8f, 1.16f, 0.10f),
                    new Vector3(6.9f, -1.02f, 0.10f)
                };

            for (int i = 0; i < positions.Length; i++)
            {
                var root = new GameObject($"ToonyBattlefieldProp_{i}");
                root.transform.SetParent(parent, false);
                root.transform.localPosition = positions[i];
                var facing = positions[i].x > 0f ? VisualFacing.Enemy : VisualFacing.Player;
                var visual = AttachVisual(root, ToonyRtsSceneryPaths[i % ToonyRtsSceneryPaths.Length],
                    "ToonyPropVisual", facing, Vector3.zero, i < 2 ? 0.42f : 0.48f);
                if (visual != null && i >= 2)
                    LowPolyWindAnimator.Attach(visual, LowPolyWindProfile.Banner, positions[i].x);
            }
        }

        private static void AddBaseSilhouette(Transform parent, Vector3 position, bool isPlayer)
        {
            var side = isPlayer ? PlayerBlue : EnemyRed;
            var root = new GameObject(isPlayer ? "PlayerBaseSilhouette" : "EnemyBaseSilhouette");
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            CreateSprite("BackBanner", root.transform, new Vector3(0f, 0.02f, 0f), new Vector2(1.55f, 2.65f), Darken(side, 0.72f), -18);
            CreateSprite("KeepWall", root.transform, new Vector3(0f, -0.44f, -0.01f), new Vector2(1.78f, 1.54f), new Color(0.40f, 0.38f, 0.35f, 1f), -17);
            CreateSprite("Roof", root.transform, new Vector3(0f, 0.52f, -0.02f), new Vector2(1.98f, 0.34f), side, -16);
            CreateSprite("Gate", root.transform, new Vector3(0f, -0.80f, -0.03f), new Vector2(0.56f, 0.68f), new Color(0.09f, 0.08f, 0.07f, 1f), -15);
            CreateSprite("CrownPlaque", root.transform, new Vector3(0f, 0.04f, -0.04f), new Vector2(0.58f, 0.36f), RoyalGold, -14);

            float flagX = isPlayer ? -0.72f : 0.72f;
            CreateSprite("FlagPole", root.transform, new Vector3(flagX, 1.24f, -0.05f), new Vector2(0.05f, 1.12f), RoyalGold, -14);
            CreateSprite("FactionFlag", root.transform, new Vector3(flagX + (isPlayer ? 0.22f : -0.22f), 1.46f, -0.06f),
                new Vector2(0.46f, 0.28f), side, -13);
        }

        private static void AddPortalPedestal(Transform parent, Vector3 position)
        {
            var root = new GameObject("PortalPedestalSilhouette");
            root.transform.SetParent(parent, false);
            root.transform.position = position;
            CreateSprite("PedestalGlow", root.transform, new Vector3(0f, -0.12f, 0f), new Vector2(1.42f, 2.25f), new Color(0.48f, 0.16f, 0.80f, 0.46f), -18);
            CreateSprite("StoneBase", root.transform, new Vector3(0f, -0.90f, -0.01f), new Vector2(1.68f, 0.38f), new Color(0.35f, 0.32f, 0.42f, 1f), -17);
            CreateSprite("Runestone", root.transform, new Vector3(0f, -0.10f, -0.02f), new Vector2(0.56f, 1.52f), new Color(0.20f, 0.12f, 0.34f, 1f), -16);
            CreateSprite("Rune", root.transform, new Vector3(0f, 0.06f, -0.03f), new Vector2(0.28f, 0.64f), RoyalGold, -15);
        }

        private static void AddToonyBanner(Transform parent, Vector3 position, VisualFacing facing)
        {
            string path;
            switch (facing)
            {
                case VisualFacing.Player:
                    path = $"{ToonyRtsPrefabRoot}/banners/TT_Banner_Blue_A";
                    break;
                case VisualFacing.Enemy:
                    path = $"{ToonyRtsPrefabRoot}/banners/TT_Banner_Red";
                    break;
                default:
                    path = $"{ToonyRtsPrefabRoot}/banners/TT_Banner_Purple";
                    break;
            }

            var root = new GameObject(facing == VisualFacing.Neutral ? "ToonyNeutralBanner" : "ToonyFactionBanner");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;
            var banner = AttachVisual(root, path, "ToonyBannerVisual", facing, Vector3.zero, 0.62f);
            if (banner != null)
                LowPolyWindAnimator.Attach(banner, LowPolyWindProfile.Banner, position.x);
        }

        private static GameObject AttachVisual(GameObject root, string path, string name, VisualFacing facing, Vector3 localPosition, float scale)
        {
            if (root == null) return null;
            var prefab = LoadVisualPrefab(path);
            if (prefab == null) return null;

            var visual = Object.Instantiate(prefab, root.transform);
            visual.name = name;
            visual.transform.localPosition = localPosition;
            visual.transform.localRotation = Quaternion.Euler(0f, facing == VisualFacing.Enemy ? 180f : 0f, 0f);
            visual.transform.localScale = Vector3.one * scale;
            StripPhysics(visual);
            if (name.Contains("Village") || name.Contains("Portal") || name.Contains("Building"))
                LowPolyWindAnimator.Attach(visual, name.Contains("Portal") ? LowPolyWindProfile.Portal : LowPolyWindProfile.Banner, root.transform.position.x);
            return visual;
        }

        private static void BoostToonyVisual(GameObject visual, float scaleMultiplier, int sortingOrder)
        {
            if (visual == null) return;
            visual.SetActive(true);
            visual.transform.localScale *= scaleMultiplier;

            foreach (var renderer in visual.GetComponentsInChildren<Renderer>(true))
            {
                renderer.enabled = true;
                renderer.sortingOrder = sortingOrder;
            }
        }

        private static void AttachFactionMarker(GameObject root, VisualFacing facing, bool eliteMonster)
        {
            if (root == null) return;

            var marker = new GameObject(facing == VisualFacing.Neutral ? "NeutralMonsterMarker" : "FactionMarker");
            marker.transform.SetParent(root.transform, false);
            marker.transform.localPosition = Vector3.zero;

            var main = FactionColor(facing);
            var glow = new Color(main.r, main.g, main.b, facing == VisualFacing.Neutral ? 0.34f : 0.28f);
            var badgeX = facing == VisualFacing.Enemy ? -0.46f : 0.46f;
            var baseWidth = eliteMonster ? 1.04f : 0.86f;

            CreateSprite("FactionGroundGlow", marker.transform, new Vector3(0f, -0.65f, 0.07f), new Vector2(baseWidth, 0.20f), glow, 2);
            CreateSprite("FactionGroundStripe", marker.transform, new Vector3(0f, -0.52f, 0.04f), new Vector2(baseWidth * 0.82f, 0.07f), main, 4);
            CreateSprite("FactionSideBadge", marker.transform, new Vector3(badgeX, 0.02f, 0.03f), new Vector2(0.17f, 0.17f), main, 5);

            if (facing != VisualFacing.Neutral) return;

            CreateSprite("MonsterWarningCore", marker.transform, new Vector3(0f, -0.16f, 0.02f), new Vector2(eliteMonster ? 0.30f : 0.22f, eliteMonster ? 0.30f : 0.22f), NeutralMonsterOrange, 6);
            CreateSprite("MonsterWarningTop", marker.transform, new Vector3(0f, eliteMonster ? 0.22f : 0.14f, 0.01f), new Vector2(eliteMonster ? 0.38f : 0.28f, 0.07f), NeutralMonsterViolet, 7);
        }

        private static void AttachRoleAccent(GameObject root, UnitVisualStyle style, VisualFacing facing)
        {
            if (root == null) return;

            CreateSprite("UnitShadow", root.transform, new Vector3(0f, -0.60f, 0.08f), new Vector2(0.72f, 0.12f),
                new Color(0f, 0f, 0f, 0.34f), 0);
            CreateSprite("RoleAccent", root.transform, new Vector3(0f, -0.52f, 0.06f), new Vector2(0.64f, 0.08f),
                style.accentColor, 1);
            var badgeX = facing == VisualFacing.Enemy ? -0.38f : 0.38f;
            CreateSprite("RoleBadge", root.transform, new Vector3(badgeX, -0.10f, 0.05f), new Vector2(0.16f, 0.16f),
                style.accentColor, 3);
        }

        private static void AttachWeapon(GameObject root, UnitVisualStyle style, VisualFacing facing)
        {
            var offset = style.weaponOffset;
            if (facing == VisualFacing.Enemy) offset.x *= -1f;
            var weapon = AttachVisual(root, style.weaponPath, "WeaponAccent", facing, offset, style.weaponScale);
            if (weapon != null) weapon.transform.localRotation *= Quaternion.Euler(0f, 0f,
                facing == VisualFacing.Enemy ? -style.weaponRotationZ : style.weaponRotationZ);
        }

        private static void FadeFallbackSprite(GameObject root, float alpha)
        {
            var sr = root != null ? root.GetComponent<SpriteRenderer>() : null;
            if (sr == null) return;
            var c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        private static void StripPhysics(GameObject root)
        {
            foreach (var rb in root.GetComponentsInChildren<Rigidbody>(true))
                Object.Destroy(rb);
            foreach (var rb in root.GetComponentsInChildren<Rigidbody2D>(true))
                Object.Destroy(rb);
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
                Object.Destroy(collider);
            foreach (var collider in root.GetComponentsInChildren<Collider2D>(true))
                Object.Destroy(collider);
        }

        private static GameObject CreateSprite(string name, Transform parent, Vector3 localPosition, Vector2 size, Color color, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeSprite(color, size.x, size.y);
            sr.sortingOrder = sortingOrder;
            return go;
        }

        private static GameObject CreateGeneratedSprite(string name, Transform parent, UIArtSprite spriteId, Vector3 localPosition, float targetHeight, int sortingOrder, LowPolyWindProfile? windProfile = null)
        {
            var sprite = UIArtKit.Sprite(spriteId);
            if (sprite == null) return null;

            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = sortingOrder;

            var height = Mathf.Max(sprite.bounds.size.y, 0.01f);
            var scale = Mathf.Max(0.01f, targetHeight / height);
            go.transform.localScale = Vector3.one * scale;
            if (windProfile.HasValue)
                LowPolyWindAnimator.Attach(go, windProfile.Value, localPosition.x * 0.47f + localPosition.y);
            return go;
        }

        private static Sprite MakeSprite(Color color, float width, float height)
        {
            int pw = Mathf.Max(1, Mathf.RoundToInt(width * 32f));
            int ph = Mathf.Max(1, Mathf.RoundToInt(height * 32f));
            var texture = new Texture2D(pw, ph);
            var pixels = new Color[pw * ph];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, pw, ph), new Vector2(0.5f, 0.5f), 32f);
        }

        private static Color Darken(Color color, float amount)
        {
            return new Color(color.r * amount, color.g * amount, color.b * amount, color.a);
        }

        private static bool ContainsAny(string value, params string[] terms)
        {
            if (string.IsNullOrEmpty(value)) return false;
            foreach (var term in terms)
                if (value.Contains(term))
                    return true;
            return false;
        }
    }
}
