using UnityEngine;

namespace SlotDefense
{
    public enum VisualFacing
    {
        Player,
        Enemy,
        Neutral
    }

    public static class GameVisualKit
    {
        public static readonly string[] SceneryPaths =
        {
            "Asset/SimpleNaturePack/Prefabs/Tree_01",
            "Asset/SimpleNaturePack/Prefabs/Tree_03",
            "Asset/SimpleNaturePack/Prefabs/Rock_02",
            "Asset/SimpleNaturePack/Prefabs/Bush_01",
            "Asset/SimpleNaturePack/Prefabs/Flowers_01",
            "Asset/Polytope Studio/Lowpoly_Props/Prefabs/PT_Village_Fence_Small_01"
        };

        public static string UnitVisualPath(string unitName)
        {
            if (unitName.Contains("궁수") || unitName.Contains("Archer"))
                return "Asset/Polytope Studio/Lowpoly_Characters/Prefabs/Modular_NPC/Skeleton/PT_Skeleton_Male_Modular";
            if (unitName.Contains("마법") || unitName.Contains("Mage"))
                return "Asset/Polytope Studio/Lowpoly_Characters/Prefabs/Modular_NPC/Skeleton/Separate_Parts/PT_Male_Skeleton_01_upper";
            if (unitName.Contains("힐러") || unitName.Contains("Healer"))
                return "Asset/Polytope Studio/Lowpoly_Characters/Prefabs/Modular_NPC/Skeleton/Separate_Parts/PT_Male_Skeleton_01_head";

            return "Asset/Polytope Studio/Lowpoly_Characters/Prefabs/Modular_Armors/PT_Male_Armors_Skeleton_Modular";
        }

        public static string MonsterVisualPath(bool elite)
        {
            return elite
                ? "Asset/Polytope Studio/Lowpoly_Characters/Prefabs/Modular_Armors/Separate_Parts/PT_Male_Armor_Skeleton_01_head"
                : "Asset/Polytope Studio/Lowpoly_Characters/Prefabs/Modular_NPC/Skeleton/Separate_Parts/PT_Male_Skeleton_01_head";
        }

        public static string VillageVisualPath(bool isPlayer)
        {
            return isPlayer
                ? "Asset/SimpleNaturePack/Prefabs/Tree_05"
                : "Asset/SimpleNaturePack/Prefabs/Tree_04";
        }

        public static string PortalAccentPath()
        {
            return "Asset/Polytope Studio/Lowpoly_Props/Prefabs/PT_Wooden_Cross_03";
        }

        public static GameObject LoadVisualPrefab(string resourcesPath)
        {
            return Resources.Load<GameObject>(resourcesPath);
        }

        public static GameObject AttachUnitVisual(GameObject root, string unitName, VisualFacing facing)
        {
            var visual = AttachVisual(root, UnitVisualPath(unitName), "UnitVisual", facing, new Vector3(0f, -0.42f, -0.08f), 0.72f);
            AttachWeapon(root, unitName, facing);
            FadeFallbackSprite(root, 0.28f);
            return visual;
        }

        public static GameObject AttachMonsterVisual(GameObject root, bool elite, VisualFacing facing)
        {
            var visual = AttachVisual(root, MonsterVisualPath(elite), elite ? "EliteMonsterVisual" : "MonsterVisual",
                facing, new Vector3(0f, -0.34f, -0.08f), elite ? 0.78f : 0.64f);
            FadeFallbackSprite(root, elite ? 0.36f : 0.30f);
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
            string path = card != null && card.cardName.Contains("탑")
                ? "Asset/Polytope Studio/Lowpoly_Props/Prefabs/PT_Wooden_Cross_02"
                : "Asset/Polytope Studio/Lowpoly_Props/Prefabs/PT_Village_Fence_Small_03";
            var visual = AttachVisual(root, path, "BuildingVisual", VisualFacing.Player, new Vector3(0f, -0.42f, -0.10f), 0.62f);
            FadeFallbackSprite(root, 0.28f);
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
                var prefab = LoadVisualPrefab(SceneryPaths[i % SceneryPaths.Length]);
                if (prefab == null) continue;

                var go = Object.Instantiate(prefab, positions[i], Quaternion.Euler(0f, 0f, 0f), parent);
                go.name = $"Scenery_{i}_{prefab.name}";
                go.transform.localScale = Vector3.one * (i % 2 == 0 ? 0.82f : 0.68f);
                StripPhysics(go);
            }
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
            return visual;
        }

        private static void AttachWeapon(GameObject root, string unitName, VisualFacing facing)
        {
            string path = unitName.Contains("궁수")
                ? "Asset/Polytope Studio/Lowpoly_Weapons/Prefabs/PT_Sword_01_a"
                : "Asset/Polytope Studio/Lowpoly_Weapons/Prefabs/PT_Shield_01_a";
            var weapon = AttachVisual(root, path, "WeaponAccent", facing, new Vector3(0.18f, -0.12f, -0.12f), 0.34f);
            if (weapon != null) weapon.transform.localRotation *= Quaternion.Euler(0f, 0f, -18f);
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
    }
}
