using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace SlotDefense.Editor
{
    public class DevRefreshWindow : EditorWindow
    {
        [MenuItem("SlotDefense/개발 새로고침 &r")]
        public static void ShowWindow()
        {
            var win = GetWindow<DevRefreshWindow>("개발 새로고침");
            win.minSize = new Vector2(260f, 120f);
            win.maxSize = new Vector2(260f, 120f);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(12);

            GUI.backgroundColor = new Color(0.35f, 0.8f, 0.35f);
            if (GUILayout.Button("🔄  최신 코드 적용 (새로고침)", GUILayout.Height(48)))
            {
                Refresh();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(6);

            GUI.backgroundColor = new Color(0.6f, 0.6f, 1f);
            if (GUILayout.Button("⚡  강제 재컴파일", GUILayout.Height(32)))
            {
                CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
                Debug.Log("[DevRefresh] 강제 재컴파일 요청 완료");
            }
            GUI.backgroundColor = Color.white;
        }

        private static void Refresh()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            CompilationPipeline.RequestScriptCompilation();
            Debug.Log("[DevRefresh] 에셋 새로고침 + 재컴파일 요청 완료");
        }
    }
}
