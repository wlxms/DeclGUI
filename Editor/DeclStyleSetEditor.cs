using UnityEngine;
using UnityEditor;
using DeclGUI.Core;

namespace DeclGUI.Editor
{
    /// <summary>
    /// DeclStyleSet 自定义编辑器
    /// </summary>
    [CustomEditor(typeof(DeclStyleSet))]
    public class DeclStyleSetEditor : UnityEditor.Editor
    {
        private SerializedProperty _styles;
        private SerializedProperty _hasTransition;
        private SerializedProperty _serializedTransition;

        // DeclStyleEditor 实例
        private DeclStyleEditorRenderer _styleEditor;
        private DeclStyleSetRenderer _renderer;

        // 保存状态跟踪（已弃用，使用实时脏标记）
        // private bool _hasUnsavedChanges = false;

        private void OnEnable()
        {
            // 获取序列化属性
            _styles = serializedObject.FindProperty("_styles");
            _hasTransition = serializedObject.FindProperty("_hasTransition");
            _serializedTransition = serializedObject.FindProperty("_serializedTransition");

            // 初始化 DeclStyleEditor
            if (target is ISerializableDeclStyle style)
            {
                DeclTheme parentTheme = GetParentThemeForStyleSet(target as DeclStyleSet);
                _styleEditor = new DeclStyleEditorRenderer(style, null, parentTheme);
            }
            
            // 初始化渲染器
            if (target is DeclStyleSet styleSet)
            {
                _renderer = new DeclStyleSetRenderer(styleSet, SaveChanges, Repaint);
            }
            
            // 监听Undo/Redo事件
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // 处理快捷键
            HandleKeyboardShortcuts();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("DeclStyleSet 配置", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // 确保渲染器存在并使用它渲染界面
            if (_renderer == null && target is DeclStyleSet styleSet)
            {
                _renderer = new DeclStyleSetRenderer(styleSet, SaveChanges, Repaint);
            }
            
            if (_renderer != null)
            {
                _renderer.Render();
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBaseProperties()
        {
            EditorGUILayout.LabelField("基础样式属性", EditorStyles.miniBoldLabel);

            // 使用 DeclStyleEditor 渲染基础属性
            if (_styleEditor != null)
            {
                _styleEditor.Render();

                // 实时脏标记已启用，无需手动检查
                // 修改会自动通过 EditorUtility.SetDirty 标记
            }
            else
            {
                EditorGUILayout.HelpBox("无法创建样式编辑器", MessageType.Warning);
            }
        }

        private void DrawStylesDictionary()
        {
            var keys = _styles.FindPropertyRelative("_keys");
            var values = _styles.FindPropertyRelative("_values");

            if (keys == null || values == null)
            {
                EditorGUILayout.HelpBox("样式字典未初始化", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField($"伪类样式数量: {keys.arraySize}", EditorStyles.miniBoldLabel);

            for (int i = 0; i < keys.arraySize; i++)
            {
                var key = keys.GetArrayElementAtIndex(i);
                var value = values.GetArrayElementAtIndex(i);

                if (key == null || value == null) continue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"伪类: {(PseudoClass)key.enumValueIndex}", EditorStyles.boldLabel, GUILayout.Width(120));

                if (GUILayout.Button("删除", GUILayout.Width(60)))
                {
                    keys.DeleteArrayElementAtIndex(i);
                    values.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
                EditorGUILayout.EndHorizontal();

                // 使用 DeclStyleEditor 渲染伪类样式
                EditorGUI.indentLevel++;

                // 从 SerializedProperty 获取 DeclStyle 对象
                if (value.objectReferenceValue is ISerializableDeclStyle style)
                {
                    DeclTheme parentTheme = GetParentThemeForStyleSet(target as DeclStyleSet);
                    var styleEditor = new DeclStyleEditorRenderer(style, null, parentTheme);
                    styleEditor.Render();
                }
                else
                {
                    EditorGUILayout.HelpBox("样式对象为空或类型不正确", MessageType.Warning);
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("添加伪类样式", GUILayout.Height(25)))
            {
                // 显示添加伪类样式的下拉菜单
                ShowAddPseudoClassMenu();
            }
        }

        private void ShowAddPseudoClassMenu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (PseudoClass pseudoClass in System.Enum.GetValues(typeof(PseudoClass)))
            {
                menu.AddItem(new GUIContent(pseudoClass.ToString()), false, () => AddPseudoClassStyle(pseudoClass));
            }

            menu.ShowAsContext();
        }

        private void AddPseudoClassStyle(PseudoClass pseudoClass)
        {
            var keys = _styles.FindPropertyRelative("_keys");
            var values = _styles.FindPropertyRelative("_values");

            // 检查是否已存在该伪类
            for (int i = 0; i < keys.arraySize; i++)
            {
                if (keys.GetArrayElementAtIndex(i).enumValueIndex == (int)pseudoClass)
                {
                    EditorUtility.DisplayDialog("警告", $"伪类 {pseudoClass} 已存在", "确定");
                    return;
                }
            }

            keys.arraySize++;
            values.arraySize++;

            keys.GetArrayElementAtIndex(keys.arraySize - 1).enumValueIndex = (int)pseudoClass;

            // 创建默认的 DeclStyle 并序列化到 values 中
            var newStyle = new DeclStyle(
                color: Color.white,
                width: 100f,
                height: 50f,
                backgroundColor: new Color(0.2f, 0.2f, 0.2f)
            );

            // 由于 DeclStyle 是结构体，直接设置序列化属性值
            var valueProperty = values.GetArrayElementAtIndex(values.arraySize - 1);
            if (valueProperty != null)
            {
                // 设置颜色
                var colorProp = valueProperty.FindPropertyRelative("_color");
                colorProp.FindPropertyRelative("_value").colorValue = newStyle.Color.HasValue ? newStyle.Color.DirectValue : Color.clear;
                colorProp.FindPropertyRelative("_hasValue").boolValue = newStyle.Color.HasValue;

                // 设置宽度
                var widthProp = valueProperty.FindPropertyRelative("_width");
                widthProp.FindPropertyRelative("_value").floatValue = newStyle.Width.HasValue ? newStyle.Width.DirectValue : 0f;
                widthProp.FindPropertyRelative("_hasValue").boolValue = newStyle.Width.HasValue && newStyle.Width.DirectValue > 0;

                // 设置高度
                var heightProp = valueProperty.FindPropertyRelative("_height");
                heightProp.FindPropertyRelative("_value").floatValue = newStyle.Height.HasValue ? newStyle.Height.DirectValue : 0f;
                heightProp.FindPropertyRelative("_hasValue").boolValue = newStyle.Height.HasValue && newStyle.Height.DirectValue > 0;

                // 设置背景颜色
                var bgColorProp = valueProperty.FindPropertyRelative("_backgroundColor");
                bgColorProp.FindPropertyRelative("_value").colorValue = newStyle.BackgroundColor.HasValue ? newStyle.BackgroundColor.DirectValue : Color.clear;
                bgColorProp.FindPropertyRelative("_hasValue").boolValue = newStyle.BackgroundColor.HasValue;
            }

            // 记录Undo操作
            Undo.RecordObject(target, "Add Pseudo Class Style");
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTransitionConfig()
        {
            EditorGUILayout.PropertyField(_hasTransition, new GUIContent("启用过渡效果"));

            if (_hasTransition.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_serializedTransition.FindPropertyRelative("Duration"), new GUIContent("持续时间"));
                EditorGUILayout.PropertyField(_serializedTransition.FindPropertyRelative("EasingCurve"), new GUIContent("缓动曲线"));

                var properties = _serializedTransition.FindPropertyRelative("Properties");
                EditorGUILayout.PropertyField(properties, new GUIContent("过渡属性"), true);
                EditorGUI.indentLevel--;
            }


        }
        
        private void SaveChanges()
        {
            // 实时脏标记已启用，只需应用序列化对象的更改
            serializedObject.ApplyModifiedProperties();
            
            // 显示保存成功消息
            EditorUtility.DisplayDialog("保存成功", "样式集已成功保存", "确定");
        }
        
        private void HandleKeyboardShortcuts()
        {
            // 处理 CTRL+S 快捷键保存
            if (Event.current.type == EventType.KeyDown &&
                Event.current.keyCode == KeyCode.S &&
                Event.current.control)
            {
                SaveChanges();
                Event.current.Use(); // 标记事件已被使用，防止其他处理程序处理
            }
        }
        
        private void OnDisable()
        {
            // 实时脏标记已启用，无需手动检查未保存更改
            // 移除Undo/Redo事件监听
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }
        
        private void OnUndoRedoPerformed()
        {
            // 当Undo/Redo操作发生时，刷新状态并重新绘制界面
            if (_styleEditor != null)
            {
                _styleEditor.RefreshState();
            }
            Repaint();
        }
        
        /// <summary>
        /// 获取DeclStyleSet的父级Theme
        /// </summary>
        private DeclTheme GetParentThemeForStyleSet(DeclStyleSet styleSet)
        {
            if (styleSet == null) return null;
            
            // 通过DeclGUISetting获取样式集对应的主题
            var guiSetting = DeclGUISetting.Instance;
            if (guiSetting != null)
            {
                return guiSetting.GetThemeForStyleSet(styleSet);
            }
            return null;
        }
    }
}
