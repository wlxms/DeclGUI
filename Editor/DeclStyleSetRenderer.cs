using UnityEngine;
using UnityEditor;
using DeclGUI.Core;
using System.Collections.Generic;

namespace DeclGUI.Editor
{
    /// <summary>
    /// DeclStyleSet 渲染器
    /// 抽象 DeclStyleSetWindow 的渲染逻辑，供 DeclStyleSetEditor 和 DeclStyleSetWindow 共同使用
    /// </summary>
    public class DeclStyleSetRenderer
    {
        private DeclStyleSet _styleSet;
        private SerializedObject _serializedObject;
        private DeclStyleEditorRenderer _styleEditor;
        private Dictionary<PseudoClass, DeclStyleEditorRenderer> _pseudoClassEditors = new Dictionary<PseudoClass, DeclStyleEditorRenderer>();
        
        // 折叠状态
        private bool _basePropertiesFoldout = true;
        private bool _pseudoClassStylesFoldout = true;
        private bool _transitionSettingsFoldout = true;
        private Dictionary<PseudoClass, bool> _pseudoClassFoldouts = new Dictionary<PseudoClass, bool>();
        
        // 回调函数
        private System.Action _onSaveCallback;
        private System.Action _onRepaintCallback;

        public DeclStyleSetRenderer(DeclStyleSet styleSet, System.Action onSaveCallback = null, System.Action onRepaintCallback = null)
        {
            _styleSet = styleSet;
            _onSaveCallback = onSaveCallback;
            _onRepaintCallback = onRepaintCallback;
            
            if (_styleSet != null)
            {
                _serializedObject = new SerializedObject(_styleSet);
                DeclTheme parentTheme = GetParentThemeForStyleSet(_styleSet);
                _styleEditor = new DeclStyleEditorRenderer(_styleSet, null, parentTheme);
                
                // 初始化伪类编辑器
                _pseudoClassEditors.Clear();
                foreach (var kvp in _styleSet.Styles)
                {
                    _pseudoClassEditors[kvp.Key] = new DeclStyleEditorRenderer(kvp.Value, null, parentTheme);
                }
            }
        }
        
        /// <summary>
        /// 获取DeclStyleSet的父级Theme
        /// </summary>
        private DeclTheme GetParentThemeForStyleSet(DeclStyleSet styleSet)
        {
            // 通过DeclGUISetting获取样式集对应的主题
            var guiSetting = DeclGUISetting.Instance;
            if (guiSetting != null)
            {
                return guiSetting.GetThemeForStyleSet(styleSet);
            }
            return null;
        }
        
        public void Render()
        {
            if (_styleSet == null)
            {
                EditorGUILayout.HelpBox("请选择一个 DeclStyleSet 资产", MessageType.Info);
                return;
            }

            // 使用 DeclStyleEditor 渲染样式集 - 基础样式属性
            DrawStyleSetWithDeclStyleEditor();
            EditorGUILayout.Space(10);

            // 过渡效果
            DrawTransitionSettings();
            EditorGUILayout.Space(10);

            // 伪类样式
            DrawPseudoClassStyles();
            
            // 保存按钮
            if (GUILayout.Button("保存", GUILayout.Height(30)))
            {
                SaveChanges();
            }
        }
        
        private void DrawStyleSetWithDeclStyleEditor()
        {
            EditorGUILayout.BeginVertical(GetFoldoutBackgroundStyle());
            _basePropertiesFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_basePropertiesFoldout, "基础属性");
            EditorGUILayout.Space(5);

            if (_basePropertiesFoldout && _styleEditor != null)
            {
                _serializedObject.Update();

                EditorGUI.indentLevel++;

                // 使用 DeclStyleEditor 渲染基础属性
                _styleEditor.Render();

                EditorGUI.indentLevel--;

                _serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
        }
        
        private void DrawPseudoClassStyles()
        {
            EditorGUILayout.BeginVertical(GetFoldoutBackgroundStyle());
            _pseudoClassStylesFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_pseudoClassStylesFoldout, "伪类");
            EditorGUILayout.Space(5);
            
            if (!_pseudoClassStylesFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
                return;
            }
            
            if (_styleSet == null)
                return;

            // 遍历所有可用的伪类状态，除了 Normal
            foreach (PseudoClass pseudoClass in System.Enum.GetValues(typeof(PseudoClass)))
            {
                // 跳过 Normal 伪类
                if (pseudoClass == PseudoClass.Normal)
                    continue;
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // 检查当前伪类是否已启用
                bool isEnabled = _styleSet.Styles.ContainsKey(pseudoClass);
                DeclStyle pseudoStyle = isEnabled ? _styleSet.Styles[pseudoClass] : default;
                
                EditorGUILayout.BeginHorizontal();
                
                // 复选框控制伪类启用/禁用
                bool newEnabledState = EditorGUILayout.Toggle(isEnabled, GUILayout.Width(20));
                
                if (newEnabledState != isEnabled)
                {
                    if (newEnabledState)
                    {
                        // 启用伪类 - 添加到样式集
                        _styleSet.AddStyle(pseudoClass, new DeclStyle(
                            color: Color.white,
                            width: 100f,
                            height: 50f,
                            backgroundColor: new Color(0.2f, 0.2f, 0.2f)
                        ));
                        
                        // 创建并缓存该伪类的编辑器
                        var newStyle = _styleSet.Styles[pseudoClass];
                        DeclTheme parentTheme = GetParentThemeForStyleSet(_styleSet);
                        _pseudoClassEditors[pseudoClass] = new DeclStyleEditorRenderer(newStyle, null, parentTheme);
                    }
                    else
                    {
                        // 禁用伪类 - 从样式集移除
                        _styleSet.RemoveStyle(pseudoClass);
                        
                        // 从缓存中移除编辑器
                        if (_pseudoClassEditors.ContainsKey(pseudoClass))
                        {
                            _pseudoClassEditors.Remove(pseudoClass);
                        }
                    }
                    EditorUtility.SetDirty(_styleSet);
                    
                    // 结束当前布局组后再返回，避免 GUI 布局错误
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndVertical();
                    
                    // 触发重绘
                    _onRepaintCallback?.Invoke();
                    return; // 重新渲染界面
                }
                
                // 只有启用的伪类才显示折叠标题
                if (isEnabled)
                {
                    // 确保折叠状态字典中有该伪类的记录
                    if (!_pseudoClassFoldouts.ContainsKey(pseudoClass))
                    {
                        _pseudoClassFoldouts[pseudoClass] = false;
                    }
                    
                    // 折叠标题控制配置区域的显隐
                    _pseudoClassFoldouts[pseudoClass] = EditorGUILayout.Foldout(
                        _pseudoClassFoldouts[pseudoClass],
                        pseudoClass.ToString(),
                        true
                    );
                }
                else
                {
                    EditorGUILayout.LabelField(pseudoClass.ToString(), EditorStyles.miniLabel);
                }
                
                EditorGUILayout.EndHorizontal();
                
                // 显示伪类配置区域（仅当启用且折叠展开时）
                if (isEnabled && _pseudoClassFoldouts[pseudoClass])
                {
                    EditorGUI.indentLevel++;
                    
                    // 确保该伪类有对应的编辑器
                    if (!_pseudoClassEditors.ContainsKey(pseudoClass))
                    {
                        DeclTheme parentTheme = GetParentThemeForStyleSet(_styleSet);
                        _pseudoClassEditors[pseudoClass] = new DeclStyleEditorRenderer(pseudoStyle, null, parentTheme);
                    }
                    
                    // 使用 DeclStyleEditor 渲染伪类样式
                    _pseudoClassEditors[pseudoClass].Render();
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
        }
        
        private void DrawTransitionSettings()
        {
            EditorGUILayout.BeginVertical(GetFoldoutBackgroundStyle());
            _transitionSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_transitionSettingsFoldout, "过渡效果");
            EditorGUILayout.Space(5);

            if (_transitionSettingsFoldout)
            {
                if (_styleSet == null || _serializedObject == null)
                    return;

                _serializedObject.Update();

                var hasTransition = _serializedObject.FindProperty("_hasTransition");
                var transition = _serializedObject.FindProperty("_serializedTransition");

                EditorGUILayout.PropertyField(hasTransition, new GUIContent("启用过渡效果"));

                if (hasTransition.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(transition.FindPropertyRelative("Duration"), new GUIContent("持续时间"));
                    EditorGUILayout.PropertyField(transition.FindPropertyRelative("EasingCurve"), new GUIContent("缓动曲线"));

                    // 过渡属性字段 - 根据用户要求不渲染这个字段
                    // var properties = transition.FindPropertyRelative("Properties");
                    // 这个字段被注释掉，不进行渲染
                    EditorGUI.indentLevel--;
                }

                if (_serializedObject.ApplyModifiedProperties())
                {
                    EditorUtility.SetDirty(_styleSet);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
        }
        
        private void SaveChanges()
        {
            // 保存序列化对象的更改
            if (_serializedObject != null)
            {
                _serializedObject.ApplyModifiedProperties();
            }

            // 显示保存成功消息
            EditorUtility.DisplayDialog("保存成功", "样式集已成功保存", "确定");
            
            // 调用保存回调
            _onSaveCallback?.Invoke();
        }
        
        private GUIStyle GetFoldoutBackgroundStyle()
        {
            var style = new GUIStyle();
            style.normal.background = MakeTex(2, 2, EditorGUIUtility.isProSkin ?
                new Color(0.22f, 0.22f, 0.22f, 0.8f) :
                new Color(0.95f, 0.95f, 0.95f, 0.8f));
            style.margin = new RectOffset(0, 0, 2, 2);
            style.padding = new RectOffset(10, 5, 5, 5);
            return style;
        }
        
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}