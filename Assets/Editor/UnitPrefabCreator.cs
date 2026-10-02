using UnityEngine;
using UnityEditor;
using SlotDefense;

public static class UnitPrefabCreator
{
    private static readonly (string name, Color color)[] UnitDefs =
    {
        ("검사",   new Color(0.30f, 0.55f, 1.00f)),
        ("궁수",   new Color(0.20f, 0.75f, 0.35f)),
        ("기사",   new Color(0.80f, 0.55f, 0.15f)),
        ("마법사", new Color(0.70f, 0.20f, 0.90f)),
        ("힐러",   new Color(1.00f, 0.35f, 0.55f)),
    };

    [MenuItem("SlotDefense/유닛 프리팹 생성")]
    public static void CreateAll()
    {
        EnsureFolder("Assets/Resources");
        EnsureFolder("Assets/Resources/Prefabs");
        EnsureFolder("Assets/Resources/Prefabs/Units");

        foreach (var (name, color) in UnitDefs)
        {
            CreatePrefab(name, color);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("완료", "유닛 프리팹 생성 완료!\nAssets/Resources/Prefabs/Units/ 를 확인하세요.", "확인");
    }

    private static void CreatePrefab(string unitName, Color color)
    {
        // 텍스처 생성 및 저장
        string texPath = $"Assets/Resources/Prefabs/Units/{unitName}_tex.asset";
        var tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        var pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        AssetDatabase.CreateAsset(tex, texPath);

        // 스프라이트를 텍스처 에셋에 서브에셋으로 추가
        var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
        sprite.name = unitName;
        AssetDatabase.AddObjectToAsset(sprite, texPath);
        AssetDatabase.SaveAssets();

        // GameObject 구성
        var go = new GameObject(unitName);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite       = sprite;   // 위에서 만든 sprite 참조를 그대로 사용
        sr.sortingOrder = 2;
        go.AddComponent<UnitController>();

        // 프리팹 저장
        string prefabPath = $"Assets/Resources/Prefabs/Units/{unitName}.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);

        Debug.Log($"[UnitPrefabCreator] 생성: {prefabPath}");
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        int slash = path.LastIndexOf('/');
        AssetDatabase.CreateFolder(path.Substring(0, slash), path.Substring(slash + 1));
    }
}
