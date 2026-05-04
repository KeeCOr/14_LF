using UnityEngine;
using UnityEditor;
using SlotDefense;

public static class MonsterPrefabCreator
{
    private static readonly (string name, Color color)[] MonsterDefs =
    {
        ("Monster",      new Color(0.85f, 0.20f, 0.20f)),
        ("EliteMonster", new Color(0.75f, 0.10f, 0.60f)),
    };

    [MenuItem("SlotDefense/몬스터 프리팹 생성")]
    public static void CreateAll()
    {
        EnsureFolder("Assets/Resources");
        EnsureFolder("Assets/Resources/Prefabs");
        EnsureFolder("Assets/Resources/Prefabs/Monsters");

        foreach (var (name, color) in MonsterDefs)
            CreatePrefab(name, color);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("완료", "몬스터 프리팹 생성 완료!\nAssets/Resources/Prefabs/Monsters/ 를 확인하세요.", "확인");
    }

    private static void CreatePrefab(string prefabName, Color color)
    {
        string texPath = $"Assets/Resources/Prefabs/Monsters/{prefabName}_tex.asset";
        if (AssetDatabase.LoadAssetAtPath<Texture2D>(texPath) == null)
        {
            var tex    = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            var pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();
            AssetDatabase.CreateAsset(tex, texPath);

            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
            sprite.name = prefabName;
            AssetDatabase.AddObjectToAsset(sprite, texPath);
            AssetDatabase.SaveAssets();
        }

        string prefabPath = $"Assets/Resources/Prefabs/Monsters/{prefabName}.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            Debug.Log($"[MonsterPrefabCreator] 이미 존재: {prefabPath}");
            return;
        }

        var tex2    = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
        var assets  = AssetDatabase.LoadAllAssetsAtPath(texPath);
        Sprite sp   = null;
        foreach (var a in assets) if (a is Sprite s) { sp = s; break; }

        var go = new GameObject(prefabName);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite       = sp;
        sr.sortingOrder = 2;
        go.AddComponent<MonsterController>();

        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);

        Debug.Log($"[MonsterPrefabCreator] 생성: {prefabPath}");
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        int slash = path.LastIndexOf('/');
        AssetDatabase.CreateFolder(path.Substring(0, slash), path.Substring(slash + 1));
    }
}
