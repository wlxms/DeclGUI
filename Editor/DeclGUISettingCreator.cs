using UnityEngine;
using UnityEditor;

namespace DeclGUI.Editor
{
    /// <summary>
    /// DeclGUISetting资源创建工具
    /// </summary>
    public class DeclGUISettingCreator : EditorWindow
    {
        [MenuItem("Tools/DeclGUI/Create DeclGUISetting Asset")]
        public static void CreateDeclGUISettingAsset()
        {
            // 检查Resources/DeclGUI目录是否存在，如果不存在则创建
            string resourcesPath = "Assets/Resources/DeclGUI";
            if (!AssetDatabase.IsValidFolder(resourcesPath))
            {
                string parentPath = "Assets/Resources";
                if (!AssetDatabase.IsValidFolder(parentPath))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                AssetDatabase.CreateFolder(parentPath, "DeclGUI");
            }
            
            // 创建DeclGUISetting实例
            DeclGUI.Core.DeclGUISetting setting = ScriptableObject.CreateInstance<DeclGUI.Core.DeclGUISetting>();
            
            // 保存到Resources/DeclGUI/DeclGUISetting.asset
            string assetPath = "Assets/Resources/DeclGUI/DeclGUISetting.asset";
            AssetDatabase.CreateAsset(setting, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = setting;
            
            Debug.Log("DeclGUISetting asset created at: " + assetPath);
        }
    }
}