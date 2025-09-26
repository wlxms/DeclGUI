using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DeclGUI.Core;

namespace DeclGUI.Editor
{
    /// <summary>
    /// DeclGUI 设置管理窗口
    /// </summary>
    public class DeclGUISettingWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private DeclTheme _selectedTheme;
        private DeclThemeRenderer _themeRenderer;
        private Vector2 _themeScrollPosition;
        
        [MenuItem("Tools/DeclGUI/DeclGUI Settings")]
        public static void ShowWindow()
        {
            GetWindow<DeclGUISettingWindow>("DeclGUI Settings");
        }
        
        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("DeclGUI 主题管理", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // 检查DeclGUISetting是否存在
            var setting = DeclGUI.Core.DeclGUISetting.Instance;
            if (setting == null)
            {
                EditorGUILayout.HelpBox("DeclGUISetting 资源不存在。请先创建配置。", MessageType.Warning);
                
                if (GUILayout.Button("创建 DeclGUISetting"))
                {
                    DeclGUISettingCreator.CreateDeclGUISettingAsset();
                }
                return;
            }
            
            // 如果有选中的主题，显示主题编辑器
            if (_selectedTheme != null)
            {
                DrawThemeEditor();
            }
            else
            {
                DrawThemeList(setting);
            }
        }
        
        private void DrawThemeList(DeclGUI.Core.DeclGUISetting setting)
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            // 显示当前主题数量
            EditorGUILayout.LabelField($"已注册主题: {setting.Themes.Count} 个");
            
            // 收集所有主题按钮
            if (GUILayout.Button("收集项目中的所有主题"))
            {
                setting.CollectAllThemes();
                EditorUtility.SetDirty(setting);
                AssetDatabase.SaveAssets();
            }
            
            EditorGUILayout.Space();
            
            // 默认主题配置
            EditorGUILayout.LabelField("默认主题配置", EditorStyles.boldLabel);
            setting.DefaultTheme = (DeclGUI.Core.DeclTheme)EditorGUILayout.ObjectField("默认主题", setting.DefaultTheme, typeof(DeclGUI.Core.DeclTheme), false);
            EditorGUILayout.Space();
            
            // 显示所有主题列表
            if (setting.Themes.Count > 0)
            {
                EditorGUILayout.LabelField("主题列表:", EditorStyles.boldLabel);
                
                foreach (var themePair in setting.Themes)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    // 显示主题名称而不是GUID
                    string themeName = themePair.Value != null ? themePair.Value.name : "Null Theme";
                    EditorGUILayout.LabelField(themeName, GUILayout.Width(150));
                    EditorGUILayout.ObjectField(themePair.Value, typeof(DeclGUI.Core.DeclTheme), false);
                    
                    if (GUILayout.Button("选择", GUILayout.Width(60)))
                    {
                        Selection.activeObject = themePair.Value;
                    }
                    
                    if (GUILayout.Button("编辑", GUILayout.Width(60)))
                    {
                        _selectedTheme = themePair.Value;
                        if (_selectedTheme != null)
                        {
                            _themeRenderer = new DeclThemeRenderer(_selectedTheme, null, Repaint);
                        }
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("没有找到任何DeclTheme资源。请确保项目中存在DeclTheme资源并且设置了ThemeName。", MessageType.Info);
            }
            
            EditorGUILayout.EndScrollView();
            
            // 显示样式集信息
            if (setting.Themes.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("样式集信息", EditorStyles.boldLabel);
                
                var allStyleSets = setting.GetAllStyleSets();
                EditorGUILayout.LabelField($"总样式集数量: {allStyleSets.Count}");
                
                if (allStyleSets.Count > 0)
                {
                    EditorGUILayout.LabelField("样式集ID示例:");
                    int count = 0;
                    foreach (var styleSetPair in allStyleSets)
                    {
                        if (count++ >= 5) break; // 只显示前5个
                        EditorGUILayout.LabelField($"  - {styleSetPair.Key}");
                    }
                    if (allStyleSets.Count > 5)
                    {
                        EditorGUILayout.LabelField($"  - ... 还有 {allStyleSets.Count - 5} 个");
                    }
                }
            }
        }
        
        private void DrawThemeEditor()
        {
            EditorGUILayout.BeginHorizontal();
            
            // 返回按钮
            if (GUILayout.Button("← 返回列表", GUILayout.Width(100)))
            {
                _selectedTheme = null;
                _themeRenderer = null;
                EditorGUILayout.EndHorizontal();
                return;
            }
            
            EditorGUILayout.LabelField($"正在编辑: {_selectedTheme.name}", EditorStyles.boldLabel);
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 使用主题渲染器显示主题配置
            _themeScrollPosition = EditorGUILayout.BeginScrollView(_themeScrollPosition);
            
            if (_themeRenderer != null)
            {
                _themeRenderer.Render();
            }
            else
            {
                EditorGUILayout.HelpBox("主题渲染器未初始化", MessageType.Warning);
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}