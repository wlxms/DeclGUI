using UnityEngine;
using UnityEditor;
using DeclGUI.Core;

namespace DeclGUI.Editor
{
    /// <summary>
    /// DeclStyleSet 创建工具
    /// </summary>
    public static class DeclStyleSetCreator
    {
        [MenuItem("Assets/Create/DeclGUI/DeclStyleSet", false, 100)]
        public static void CreateDeclStyleSet()
        {
            var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
            
            // 设置默认值
            styleSet.Color = Color.white;
            styleSet.Width = 100f;
            styleSet.Height = 50f;
            styleSet.BackgroundColor = new Color(0.2f, 0.2f, 0.2f);
            
            // 添加默认的伪类样式
            var normalStyle = new DeclStyle(
                color: Color.white,
                width: 200f,
                height: 100f,
                backgroundColor: new Color(0.2f, 0.2f, 0.2f)
            );
            
            var hoverStyle = new DeclStyle(
                color: Color.yellow,
                width: 200f,
                height: 100f,
                backgroundColor: new Color(0.3f, 0.3f, 0.3f)
            );
            
            styleSet.AddStyle(PseudoClass.Normal, normalStyle);
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);
            
            // 创建资产
            string path = GetUniqueAssetPath("NewDeclStyleSet.asset");
            AssetDatabase.CreateAsset(styleSet, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = styleSet;
        }
        
        private static string GetUniqueAssetPath(string fileName)
        {
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/" + fileName);
            return path;
        }
    }
}