using UnityEngine;
using UnityEditor;
using DeclGUI.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeclGUI.Editor
{
    /// <summary>
    /// DeclTheme 自定义编辑器
    /// 支持在Inspector和DeclGUISettingWindow中使用
    /// </summary>
    [CustomEditor(typeof(DeclTheme))]
    public class DeclThemeEditor : UnityEditor.Editor
    {
        private SerializedProperty _styleSets;
        private DeclThemeRenderer _renderer;

        private void OnEnable()
        {
            _styleSets = serializedObject.FindProperty("styleSets");

            // 初始化渲染器
            if (target is DeclTheme theme)
            {
                _renderer = new DeclThemeRenderer(theme, SaveChanges, Repaint);
            }
        }

        public override void OnInspectorGUI()
        {
            // 更新serializedObject以支持Undo/Redo
            serializedObject.Update();

            // 处理键盘快捷键
            HandleKeyboardShortcuts();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("DeclTheme 配置", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 确保渲染器存在并使用它渲染界面
            if (_renderer == null && target is DeclTheme theme)
            {
                _renderer = new DeclThemeRenderer(theme, SaveChanges, Repaint);
            }

            if (_renderer != null)
            {
                _renderer.Render();
            }

            // 应用修改并处理Undo/Redo
            if (serializedObject.ApplyModifiedProperties())
            {
                // 确保主题缓存与序列化数据同步
                if (target is DeclTheme declTheme)
                {
                    declTheme.BuildCache();
                }
            }
        }

        private void SaveChanges()
        {
            // 实时脏标记已启用，只需应用序列化对象的更改
            serializedObject.ApplyModifiedProperties();

            // 在编辑器中修改后，确保DeclTheme的缓存与序列化数据同步
            if (target is DeclTheme theme)
            {
                theme.BuildCache(); // 重建缓存以确保与序列化数据同步
            }

            // 显示保存成功消息
            EditorUtility.DisplayDialog("保存成功", "主题已成功保存", "确定");
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
    }

    /// <summary>
    /// DeclTheme 编辑器工具
    /// </summary>
    public class DeclThemeEditorTools
    {
        [MenuItem("Tools/DeclGUI/手动收集样式集")]
        public static void ManualCollectStyleSets()
        {
            var theme = Selection.activeObject as DeclTheme;
            if (theme != null)
            {
                theme.BuildCache();
                EditorUtility.SetDirty(theme);
                AssetDatabase.SaveAssets();
                Debug.Log("手动收集样式集完成");
            }
            else
            {
                Debug.LogWarning("请选择一个 DeclTheme 资产");
            }
        }
        
        [MenuItem("Tools/DeclGUI/创建默认亮色主题")]
        public static void CreateDefaultLightTheme()
        {
            string selectedPath = GetSelectedPathOrFallback();
            DeclThemeCreator.CreateThemeAtPath(Path.Combine(selectedPath, "LightTheme"), "LightTheme", true);
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Tools/DeclGUI/创建默认深色主题")]
        public static void CreateDefaultDarkTheme()
        {
            string selectedPath = GetSelectedPathOrFallback();
            DeclThemeCreator.CreateThemeAtPath(Path.Combine(selectedPath, "DarkTheme"), "DarkTheme", false);
            AssetDatabase.Refresh();
        }
        
        private static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
            
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }
    }

    /// <summary>
    /// DeclTheme 渲染器
    /// 抽象 DeclTheme 的渲染逻辑，供 DeclThemeEditor 和 DeclGUISettingWindow 共同使用
    /// </summary>
    public class DeclThemeRenderer
    {
        private DeclTheme _theme;
        private SerializedObject _serializedObject;

        // 回调函数
        private System.Action _onSaveCallback;
        private System.Action _onRepaintCallback;

        // 折叠状态
        private bool _styleSetsFoldout = true;
        private Dictionary<int, bool> _styleSetFoldouts = new Dictionary<int, bool>();
        private Dictionary<int, bool> _propertyFoldouts = new Dictionary<int, bool>();

        public DeclThemeRenderer(DeclTheme theme, System.Action onSaveCallback = null, System.Action onRepaintCallback = null)
        {
            _theme = theme;
            _onSaveCallback = onSaveCallback;
            _onRepaintCallback = onRepaintCallback;

            if (_theme != null)
            {
                _serializedObject = new SerializedObject(_theme);
            }
        }

        public void Render()
        {
            if (_theme == null)
            {
                EditorGUILayout.HelpBox("请选择一个 DeclTheme 资产", MessageType.Info);
                return;
            }

            // 渲染主题属性
            DrawThemeProperties();
            EditorGUILayout.Space(10);

            // 使用 SerializedObject 渲染样式集列表
            DrawStyleSets();
            EditorGUILayout.Space(10);
        }

        private void DrawStyleSets()
        {
            EditorGUILayout.BeginVertical(GetImprovedBackgroundStyle());

            if (_serializedObject == null)
                return;

            _serializedObject.Update();

            var styleSets = _serializedObject.FindProperty("styleSets");

            if (styleSets == null)
            {
                EditorGUILayout.HelpBox("样式集属性未找到", MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUI.indentLevel++;

            // 显示样式集列表标题、数量在同一行
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"样式集列表({styleSets.arraySize})", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);

            // 渲染每个样式集条目，使用Inline布局
            for (int i = 0; i < styleSets.arraySize; i++)
            {
                var entry = styleSets.GetArrayElementAtIndex(i);
                if (entry == null) continue;

                var idProperty = entry.FindPropertyRelative("Id");
                var styleSetProperty = entry.FindPropertyRelative("StyleSet");

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();

                GUI.enabled = false;
                // 显示样式集ObjectField（只读）
                if (styleSetProperty != null)
                {
                    EditorGUILayout.ObjectField(styleSetProperty.objectReferenceValue, typeof(DeclStyleSet), false);
                }
                GUI.enabled = true;

                // 编辑按钮
                if (styleSetProperty != null && styleSetProperty.objectReferenceValue != null)
                {
                    if (GUILayout.Button("编辑", GUILayout.Width(60)))
                    {
                        Selection.activeObject = styleSetProperty.objectReferenceValue;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            EditorGUI.indentLevel--;

            if (_serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(_theme);
                
                // 在应用修改后，重新构建整个缓存以确保所有值都是最新的
                if (_theme != null)
                {
                    _theme.BuildCache();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawThemeProperties()
        {
            EditorGUILayout.BeginVertical(GetImprovedBackgroundStyle());

            if (_serializedObject == null)
                return;

            _serializedObject.Update();

            // 显示属性模板字段
            var propertyTemplate = _serializedObject.FindProperty("propertyTemplate");
            if (propertyTemplate != null)
            {
                EditorGUILayout.PropertyField(propertyTemplate, new GUIContent("属性模板"));
                EditorGUILayout.Space(5);
            }

            var themeProperties = _serializedObject.FindProperty("themeProperties");
            if (themeProperties == null)
            {
                EditorGUILayout.HelpBox("主题属性未找到", MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUI.indentLevel++;

            // 显示主题属性标题、数量和添加按钮在同一行
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"主题属性({themeProperties.arraySize})", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                AddNewProperty(themeProperties);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);

            // 使用字典来跟踪折叠状态
            if (_propertyFoldouts == null)
                _propertyFoldouts = new Dictionary<int, bool>();

            // 渲染每个属性，使用Inline布局
            for (int i = 0; i < themeProperties.arraySize; i++)
            {
                var property = themeProperties.GetArrayElementAtIndex(i);
                if (property == null) continue;

                var nameProperty = property.FindPropertyRelative("Name");
                var typeProperty = property.FindPropertyRelative("Type");

                // 确保折叠状态字典中有该条目的记录
                if (!_propertyFoldouts.ContainsKey(i))
                {
                    _propertyFoldouts[i] = false;
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();

                // 检查是否为模板属性
                bool isTemplateProperty = false;
                if (_theme != null && nameProperty != null)
                {
                    isTemplateProperty = _theme.IsTemplateProperty(nameProperty.stringValue);
                }

                // 显示类型下拉框（如果是模板属性则禁用编辑）
                if (typeProperty != null)
                {
                    // 保存旧的类型值
                    var oldType = (PropertyType)typeProperty.enumValueIndex;

                    // 使用自定义下拉按钮样式
                    EditorGUI.BeginDisabledGroup(isTemplateProperty);
                    if (DrawTypeDropdownButton(typeProperty, isTemplateProperty, GUILayout.Width(20)))
                    {
                        // 如果类型改变，更新名称
                        var newType = (PropertyType)typeProperty.enumValueIndex;
                        if (newType != oldType && nameProperty != null)
                        {
                            // 如果名称是默认名称，则更新为新类型的默认名称
                            string currentName = nameProperty.stringValue;
                            if (IsDefaultPropertyName(currentName, oldType))
                            {
                                nameProperty.stringValue = GetDefaultPropertyName(newType, i);
                            }
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }

                // 显示名称（如果是模板属性则禁用编辑）
                if (nameProperty != null)
                {
                    EditorGUI.BeginDisabledGroup(isTemplateProperty);

                    // 创建带悬停提示的文本字段
                    string propertyName = nameProperty.stringValue;
                    string tooltip = isTemplateProperty ? GetTemplatePropertyDescription(propertyName) : "";

                    var content = new GUIContent(propertyName, tooltip);
                    if (isTemplateProperty)
                        GUILayout.Label(content, GUILayout.Width(120));
                    else
                        nameProperty.stringValue = EditorGUILayout.TextField(nameProperty.stringValue, GUILayout.Width(120));

                    EditorGUI.EndDisabledGroup();
                }

                // 显示值
                if (typeProperty != null)
                {
                    switch ((PropertyType)typeProperty.enumValueIndex)
                    {
                        case PropertyType.Float:
                            var floatValue = property.FindPropertyRelative("FloatValue");
                            if (floatValue != null)
                            {
                                floatValue.floatValue = EditorGUILayout.FloatField(floatValue.floatValue);
                                // 检查值是否改变，如果改变则标记为脏并重建缓存
                                if (GUI.changed)
                                {
                                    EditorUtility.SetDirty(_theme);
                                    _theme.BuildCache(); // 确保缓存与序列化数据同步
                                }
                            }
                            break;
                        case PropertyType.Int:
                            var intValue = property.FindPropertyRelative("IntValue");
                            if (intValue != null)
                            {
                                intValue.intValue = EditorGUILayout.IntField(intValue.intValue);
                                // 检查值是否改变，如果改变则标记为脏并重建缓存
                                if (GUI.changed)
                                {
                                    EditorUtility.SetDirty(_theme);
                                    _theme.BuildCache(); // 确保缓存与序列化数据同步
                                }
                            }
                            break;
                        case PropertyType.Color:
                            var colorValue = property.FindPropertyRelative("ColorValue");
                            if (colorValue != null)
                            {
                                colorValue.colorValue = EditorGUILayout.ColorField(colorValue.colorValue);
                                // 检查值是否改变，如果改变则标记为脏并重建缓存
                                if (GUI.changed)
                                {
                                    EditorUtility.SetDirty(_theme);
                                    _theme.BuildCache(); // 确保缓存与序列化数据同步
                                }
                            }
                            break;
                        case PropertyType.String:
                            var stringValue = property.FindPropertyRelative("StringValue");
                            if (stringValue != null)
                            {
                                stringValue.stringValue = EditorGUILayout.TextField(stringValue.stringValue);
                                // 检查值是否改变，如果改变则标记为脏并重建缓存
                                if (GUI.changed)
                                {
                                    EditorUtility.SetDirty(_theme);
                                    _theme.BuildCache(); // 确保缓存与序列化数据同步
                                }
                            }
                            break;
                        case PropertyType.Boolean:
                            var boolValue = property.FindPropertyRelative("BoolValue");
                            if (boolValue != null)
                            {
                                boolValue.boolValue = EditorGUILayout.Toggle(boolValue.boolValue, GUILayout.Width(40));
                                // 检查值是否改变，如果改变则标记为脏并重建缓存
                                if (GUI.changed)
                                {
                                    EditorUtility.SetDirty(_theme);
                                    _theme.BuildCache(); // 确保缓存与序列化数据同步
                                }
                                // 右对齐布尔值切换框
                                GUILayout.FlexibleSpace();
                            }
                            break;
                        case PropertyType.Vector2:
                            var vector2Value = property.FindPropertyRelative("Vector2Value");
                            if (vector2Value != null)
                            {
                                vector2Value.vector2Value = EditorGUILayout.Vector2Field("", vector2Value.vector2Value, GUILayout.MinWidth(150));
                                // 检查值是否改变，如果改变则标记为脏并重建缓存
                                if (GUI.changed)
                                {
                                    EditorUtility.SetDirty(_theme);
                                    _theme.BuildCache(); // 确保缓存与序列化数据同步
                                }
                            }
                            break;
                        case PropertyType.Vector3:
                            var vector3Value = property.FindPropertyRelative("Vector3Value");
                            if (vector3Value != null)
                            {
                                vector3Value.vector3Value = EditorGUILayout.Vector3Field("", vector3Value.vector3Value, GUILayout.MinWidth(200));
                                // 检查值是否改变，如果改变则标记为脏并重建缓存
                                if (GUI.changed)
                                {
                                    EditorUtility.SetDirty(_theme);
                                    _theme.BuildCache(); // 确保缓存与序列化数据同步
                                }
                            }
                            break;
                        case PropertyType.RectOffset:
                            GUILayout.Space(18);
                            var rectOffsetValue = property.FindPropertyRelative("RectOffsetValue");
                            if (rectOffsetValue != null)
                            {
                                // 使用L[输入框] R[输入框] T[输入框] B[输入框]的方式渲染RectOffset字段
                                EditorGUILayout.BeginHorizontal();

                                // L (left)
                                GUILayout.Label("L", new GUIStyle(EditorStyles.label) { }, GUILayout.Width(15));
                                var leftValue = rectOffsetValue.FindPropertyRelative("m_Left");
                                if (leftValue != null)
                                {
                                    int oldLeftValue = leftValue.intValue;
                                    string leftStr = leftValue.intValue.ToString();
                                    string newLeftStr = EditorGUI.TextField(GUILayoutUtility.GetRect(40, EditorGUIUtility.singleLineHeight), leftStr);
                                    if (int.TryParse(newLeftStr, out int leftInt))
                                    {
                                        if (leftValue.intValue != leftInt)
                                        {
                                            leftValue.intValue = leftInt;
                                            EditorUtility.SetDirty(_theme);
                                            _theme.BuildCache(); // 确保缓存与序列化数据同步
                                        }
                                    }
                                    // 检查输入框内容是否改变，即使输入的是无效值
                                    if (oldLeftValue.ToString() != newLeftStr)
                                    {
                                        GUI.changed = true;
                                    }
                                }

                                // R (right)
                                GUILayout.Label("R", GUILayout.Width(15));
                                var rightValue = rectOffsetValue.FindPropertyRelative("m_Right");
                                if (rightValue != null)
                                {
                                    int oldRightValue = rightValue.intValue;
                                    string rightStr = rightValue.intValue.ToString();
                                    string newRightStr = EditorGUI.TextField(GUILayoutUtility.GetRect(40, EditorGUIUtility.singleLineHeight), rightStr);
                                    if (int.TryParse(newRightStr, out int rightInt))
                                    {
                                        if (rightValue.intValue != rightInt)
                                        {
                                            rightValue.intValue = rightInt;
                                            EditorUtility.SetDirty(_theme);
                                            _theme.BuildCache(); // 确保缓存与序列化数据同步
                                        }
                                    }
                                    // 检查输入框内容是否改变，即使输入的是无效值
                                    if (oldRightValue.ToString() != newRightStr)
                                    {
                                        GUI.changed = true;
                                    }
                                }

                                // T (top)
                                GUILayout.Label("T", GUILayout.Width(15));
                                var topValue = rectOffsetValue.FindPropertyRelative("m_Top");
                                if (topValue != null)
                                {
                                    int oldTopValue = topValue.intValue;
                                    string topStr = topValue.intValue.ToString();
                                    string newTopStr = EditorGUI.TextField(GUILayoutUtility.GetRect(40, EditorGUIUtility.singleLineHeight), topStr);
                                    if (int.TryParse(newTopStr, out int topInt))
                                    {
                                        if (topValue.intValue != topInt)
                                        {
                                            topValue.intValue = topInt;
                                            EditorUtility.SetDirty(_theme);
                                            _theme.BuildCache(); // 确保缓存与序列化数据同步
                                        }
                                    }
                                    // 检查输入框内容是否改变，即使输入的是无效值
                                    if (oldTopValue.ToString() != newTopStr)
                                    {
                                        GUI.changed = true;
                                    }
                                }

                                // B (bottom)
                                GUILayout.Label("B", GUILayout.Width(15));
                                var bottomValue = rectOffsetValue.FindPropertyRelative("m_Bottom");
                                if (bottomValue != null)
                                {
                                    int oldBottomValue = bottomValue.intValue;
                                    string bottomStr = bottomValue.intValue.ToString();
                                    string newBottomStr = EditorGUI.TextField(GUILayoutUtility.GetRect(40, EditorGUIUtility.singleLineHeight), bottomStr);
                                    if (int.TryParse(newBottomStr, out int bottomInt))
                                    {
                                        if (bottomValue.intValue != bottomInt)
                                        {
                                            bottomValue.intValue = bottomInt;
                                            EditorUtility.SetDirty(_theme);
                                            _theme.BuildCache(); // 确保缓存与序列化数据同步
                                        }
                                    }
                                    // 检查输入框内容是否改变，即使输入的是无效值
                                    if (oldBottomValue.ToString() != newBottomStr)
                                    {
                                        GUI.changed = true;
                                    }
                                }

                                EditorGUILayout.EndHorizontal();
                            }
                            break;
                    }
                }

                // 删除按钮（如果是模板属性则禁用删除）
                var buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = 10;
                buttonStyle.padding = new RectOffset(2, 2, 2, 2);

                // 重用之前定义的isTemplateProperty变量
                if (_theme != null && nameProperty != null)
                {
                    isTemplateProperty = _theme.IsTemplateProperty(nameProperty.stringValue);
                }

                EditorGUI.BeginDisabledGroup(isTemplateProperty);
                if (GUILayout.Button("×", buttonStyle, GUILayout.Width(20)))
                {
                    themeProperties.DeleteArrayElementAtIndex(i);
                    if (_serializedObject != null)
                    {
                        _serializedObject.ApplyModifiedProperties();
                    }
                    if (_theme != null)
                    {
                        EditorUtility.SetDirty(_theme);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(5);
                    continue; // 跳过当前迭代，继续下一个属性
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            EditorGUI.indentLevel--;

            if (_serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(_theme);
            }

            EditorGUILayout.EndVertical();
        }

        private bool IsDefaultPropertyName(string name, PropertyType type)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            // 检查名称是否为默认格式：类型名称 + 数字
            string defaultPrefix = GetDefaultPropertyPrefix(type);
            return name.StartsWith(defaultPrefix) && name.Length > defaultPrefix.Length &&
                   int.TryParse(name.Substring(defaultPrefix.Length), out _);
        }

        /// <summary>
        /// 获取模板属性的描述
        /// </summary>
        private string GetTemplatePropertyDescription(string propertyName)
        {
            if (_theme == null || _theme.PropertyTemplate == null)
                return "";

            foreach (var templateProp in _theme.PropertyTemplate.TemplateProperties)
            {
                if (templateProp.Name == propertyName)
                {
                    return templateProp.Description;
                }
            }

            return "";
        }


        private string GetDefaultPropertyPrefix(PropertyType type)
        {
            switch (type)
            {
                case PropertyType.Float:
                    return "floatValue";
                case PropertyType.Int:
                    return "intValue";
                case PropertyType.Color:
                    return "colorValue";
                case PropertyType.String:
                    return "stringValue";
                case PropertyType.Boolean:
                    return "boolValue";
                case PropertyType.Vector2:
                    return "vector2Value";
                case PropertyType.Vector3:
                    return "vector3Value";
                case PropertyType.RectOffset:
                    return "rectOffsetValue";
                default:
                    return "propertyValue";
            }
        }

        private string GetDefaultPropertyName(PropertyType type, int index)
        {
            return GetDefaultPropertyPrefix(type) + (index + 1);
        }

        private bool PropertyExists(SerializedProperty themeProperties, string propertyName)
        {
            for (int i = 0; i < themeProperties.arraySize; i++)
            {
                var property = themeProperties.GetArrayElementAtIndex(i);
                if (property == null) continue;

                var nameProperty = property.FindPropertyRelative("Name");
                if (nameProperty != null && nameProperty.stringValue == propertyName)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddNewProperty(SerializedProperty themeProperties)
        {
            // 增加数组大小
            themeProperties.arraySize++;

            // 获取新添加的属性
            var newProperty = themeProperties.GetArrayElementAtIndex(themeProperties.arraySize - 1);
            if (newProperty != null)
            {
                // 设置默认值
                var nameProperty = newProperty.FindPropertyRelative("Name");
                var typeProperty = newProperty.FindPropertyRelative("Type");

                if (typeProperty != null)
                    typeProperty.enumValueIndex = (int)PropertyType.String; // 默认类型为String

                // 生成唯一的属性名称
                if (nameProperty != null && typeProperty != null)
                {
                    string baseName = GetDefaultPropertyName((PropertyType)typeProperty.enumValueIndex, themeProperties.arraySize - 1);
                    string uniqueName = baseName;
                    int counter = 1;

                    // 检查是否已存在同名属性
                    while (PropertyExists(themeProperties, uniqueName))
                    {
                        uniqueName = baseName + "_" + counter;
                        counter++;
                    }

                    nameProperty.stringValue = uniqueName;
                }
            }

            // 应用更改
            if (_serializedObject != null)
            {
                _serializedObject.ApplyModifiedProperties();
            }

            // 展开新添加的属性
            if (!_propertyFoldouts.ContainsKey(themeProperties.arraySize - 1))
            {
                _propertyFoldouts[themeProperties.arraySize - 1] = true;
            }
            else
            {
                _propertyFoldouts[themeProperties.arraySize - 1] = true;
            }

            // 标记为脏并保存
            if (_theme != null)
            {
                EditorUtility.SetDirty(_theme);
            }
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

        private GUIStyle GetImprovedBackgroundStyle()
        {
            var style = new GUIStyle();
            style.normal.background = MakeTex(2, 2, EditorGUIUtility.isProSkin ?
                new Color(0.25f, 0.25f, 0.25f, 0.8f) :
                new Color(0.9f, 0.9f, 0.9f, 0.8f));
            style.border = new RectOffset(1, 1, 1, 1);
            style.margin = new RectOffset(0, 0, 2, 2);
            style.padding = new RectOffset(10, 10, 10, 10);
            style.border = new RectOffset(1, 1, 1, 1);
            return style;
        }

        private GUIStyle GetPropertyHeaderStyle()
        {
            var style = new GUIStyle(EditorStyles.foldout);
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 12;
            style.margin = new RectOffset(2, 2, 2, 2);
            return style;
        }

        private GUIStyle GetPropertyLabelStyle()
        {
            var style = new GUIStyle(EditorStyles.label);
            style.fontStyle = FontStyle.Normal;
            style.fontSize = 11;
            style.fixedWidth = 60; // 设置固定宽度使标签对齐
            return style;
        }
        
        private GUIStyle GetHeaderButtonStyle()
        {
            var style = new GUIStyle(GUI.skin.button);
            style.fontSize = 12;
            style.fontStyle = FontStyle.Bold;
            style.padding = new RectOffset(5, 5, 3, 3);
            return style;
        }
        
        /// <summary>
        /// 绘制类型下拉按钮，类似Unity组件的三点菜单按钮
        /// </summary>
        private bool DrawTypeDropdownButton(SerializedProperty typeProperty, bool isDisabled, params GUILayoutOption[] options)
        {
            // 获取当前类型值
            var currentType = (PropertyType)typeProperty.enumValueIndex;
            
            // 创建按钮样式，模仿Unity的组件菜单按钮
            var buttonStyle = new GUIStyle(EditorStyles.miniButton);
            buttonStyle.fontSize = 12;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.padding = new RectOffset(2, 2, 2, 2);
            
            // 绘制按钮
            var buttonRect = GUILayoutUtility.GetRect(new GUIContent("▾"), buttonStyle, options);
            bool changed = false;
            
            // 如果按钮被点击且未禁用
            if (GUI.Button(buttonRect, "▾", buttonStyle) && !isDisabled)
            {
                // 创建上下文菜单
                GenericMenu menu = new GenericMenu();
                
                // 获取所有PropertyType枚举值
                var enumValues = System.Enum.GetValues(typeof(PropertyType));
                
                // 为每个枚举值添加菜单项
                foreach (PropertyType type in enumValues)
                {
                    bool isSelected = (currentType == type);
                    menu.AddItem(new GUIContent(type.ToString()), isSelected, () => {
                        // 在EditorApplication.delayCall中更新类型值，确保UI正确刷新
                        EditorApplication.delayCall += () => {
                            typeProperty.enumValueIndex = (int)type;
                            typeProperty.serializedObject.ApplyModifiedProperties();
                            EditorUtility.SetDirty(_theme);
                            // 触发重绘
                            if (_onRepaintCallback != null)
                                _onRepaintCallback();
                        };
                    });
                }
                
                // 显示菜单
                menu.DropDown(buttonRect);
                changed = true;
            }
            
            return changed;
        }
    }
}