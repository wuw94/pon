#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UI.Style))]
public class StyleAssetCreatorEditor : Editor
{
    [MenuItem("Assets/Create/GUI Style")]
    public static void CreateScriptableAsset()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        CreateAssetIfNotExist(ScriptableObject.CreateInstance(typeof(UI.Style)), path);
    }

    public static void CreateAssetIfNotExist(ScriptableObject scriptableObject, string path, bool selectOnCreate = true)
    {
        string assetPathAndName = path + "/style.asset";

        if (AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(ScriptableObject)) == null)
        {
            AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);

            Undo.RegisterCreatedObjectUndo(scriptableObject, "Create " + scriptableObject.name);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (selectOnCreate)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = scriptableObject;
            }
        }
    }
}
#endif