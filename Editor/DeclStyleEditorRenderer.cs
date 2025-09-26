
using UnityEngine;
using UnityEditor;
using DeclGUI.Core;
using System.Collections.Generic;
using System;

namespace DeclGUI.Editor
{
    /// <summary>
    /// 字段渲染器基类，处理通用渲染逻辑
    /// </summary>
    public abstract class FieldRenderer
    {
        protected DeclStyleEditorRenderer _editor;
        protected string _label;
        protected string _fieldName;

        public FieldRenderer(DeclStyleEditorRenderer editor, string label, string fieldName)
        {
            _editor = editor;
            _label = label;
            _fieldName = fieldName;
        }

        public abstract void Render(bool isEnabled);
        protected abstract void HandleButtonClick();
        protected abstract void RenderControl();

        /// <summary>
        /// 通用的渲染方法实现
        /// </summary>
        protected void RenderCommonLayout(bool isEnabled, System.Action renderEnabledControl, System.Action renderDisabledControl)
        {
            EditorGUILayout.BeginHorizontal();
            
            // 处理缩进
            GUILayout.Space(EditorGUI.indentLevel * 15);
            
            // 使用精确的GUI布局而不是自动布局
            // 确保样式已初始化
            _editor.EnsureStylesInitialized();
            
            Rect buttonRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(16));

            
            if (isEnabled)
            {
                // 减号按钮 - 使用精确的GUI.Button
                if (GUI.Button(buttonRect, "-", _editor.MinusButtonStyle))
                {
                    HandleButtonClick();
                    EditorGUILayout.EndHorizontal();
                    return;
                }
            }
            else
            {
                // 加号按钮 - 使用精确的GUI.Button
                if (GUI.Button(buttonRect, "+", _editor.PlusButtonStyle))
                {
                    HandleButtonClick();
                    EditorGUILayout.EndHorizontal();
                    return;
                }
            }

            // 确定标签样式
            GUIStyle labelStyle = isEnabled ?
                (_editor.FieldModifiedStates.ContainsKey(_fieldName) && _editor.FieldModifiedStates[_fieldName] ?
                    _editor.BoldLabelStyle : EditorStyles.label) :
                GetDisabledLabelStyle();

            GUILayout.Space(5);

            // 使用精确的GUI.Label而不是自动布局
            Rect labelRect = GUILayoutUtility.GetRect(100, 16, GUILayout.Width(100));
            GUI.Label(labelRect, _label, labelStyle);
            
            // 渲染控件
            if (isEnabled)
            {
                renderEnabledControl();
            }
            else
            {
                renderDisabledControl();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 通用的按钮点击处理
        /// </summary>
        protected void HandleButtonClickCommon(System.Action enableAction, System.Action disableAction)
        {
            // 记录Undo操作
            _editor.RecordUndo("Toggle Field " + _fieldName);
            
            if (_editor.FieldEnabledStates.ContainsKey(_fieldName) && _editor.FieldEnabledStates[_fieldName])
            {
                // 禁用字段
                _editor.FieldEnabledStates[_fieldName] = false;
                disableAction();
            }
            else
            {
                // 启用字段
                _editor.FieldEnabledStates[_fieldName] = true;
                enableAction();
            }
            _editor.MarkDirtyPublic();
        }

        /// <summary>
        /// 获取禁用状态的标签样式
        /// </summary>
        protected GUIStyle GetDisabledLabelStyle()
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = Color.gray;
            return style;
        }

        /// <summary>
        /// 通用的控件渲染逻辑
        /// </summary>
        protected void RenderControlCommon<T>(System.Func<T> getCurrentValue, System.Func<T, T> renderControl, System.Action<T> setValue, T defaultValue)
        {
            T currentValue = getCurrentValue();
            T newValue = renderControl(currentValue);
            
            if (!newValue.Equals(currentValue))
            {
                // 在修改值之前记录Undo操作
                _editor.RecordUndo("Modify " + _fieldName);
                setValue(newValue);
                _editor.MarkDirtyPublic();
                _editor.FieldModifiedStates[_fieldName] = true;
            }
        }

        /// <summary>
        /// 通用的引用类型控件渲染逻辑
        /// </summary>
        protected void RenderControlCommonRef<T>(System.Func<T> getCurrentValue, System.Func<T, T> renderControl, System.Action<T> setValue, T defaultValue) where T : class
        {
            T currentValue = getCurrentValue();
            T newValue = renderControl(currentValue);
            
            if (newValue != currentValue) // for reference types, use != instead of !Equals
            {
                // 在修改值之前记录Undo操作
                _editor.RecordUndo("Modify " + _fieldName);
                setValue(newValue);
                _editor.MarkDirtyPublic();
                _editor.FieldModifiedStates[_fieldName] = true;
            }
        }

        /// <summary>
        /// 渲染启用状态的控件
        /// </summary>
        protected void RenderEnabledControlCommon(System.Action renderControl)
        {
            renderControl();
        }

        /// <summary>
        /// 渲染禁用状态的控件
        /// </summary>
        protected void RenderDisabledControlCommon<T>(System.Func<T> getCurrentValue, System.Func<T, T> renderControl, T defaultValue)
        {
            T currentValue = getCurrentValue();
            
            GUI.enabled = false;
            renderControl(currentValue);
            GUI.enabled = true;
        }
    }

    /// <summary>
    /// StyleProperty字段渲染器
    /// </summary>
    public class StylePropertyFieldRenderer<T> : FieldRenderer
    {
        private System.Action<StyleProperty<T>> _setter;
        private System.Func<StyleProperty<T>> _getter;
        private System.Func<T, T> _renderControl;
        private T _defaultValue;

        public StylePropertyFieldRenderer(DeclStyleEditorRenderer editor, string label, string fieldName, 
            T defaultValue, System.Action<StyleProperty<T>> setter, 
            System.Func<StyleProperty<T>> getter, System.Func<T, T> renderControl)
            : base(editor, label, fieldName)
        {
            _defaultValue = defaultValue;
            _setter = setter;
            _getter = getter;
            _renderControl = renderControl;
        }

        public override void Render(bool isEnabled)
        {
            RenderCommonLayout(isEnabled, RenderEnabledControl, RenderDisabledControl);
        }

        protected override void HandleButtonClick()
        {
            HandleButtonClickCommon(
                () => _setter(StyleProperty<T>.Direct(_defaultValue)),
                () => _setter(StyleProperty<T>.None())
            );
        }

        protected override void RenderControl()
        {
            RenderControlCommon(
                () => {
                    StyleProperty<T> currentProperty = _getter();
                    return currentProperty.HasValue ? currentProperty.GetValue(null, _defaultValue) : _defaultValue;
                },
                _renderControl,
                (value) => _setter(StyleProperty<T>.Direct(value)),
                _defaultValue
            );
        }

        private void RenderEnabledControl()
        {
            RenderEnabledControlCommon(RenderControl);
        }

        private void RenderDisabledControl()
        {
            RenderDisabledControlCommon(
                () => {
                    StyleProperty<T> currentProperty = _getter();
                    return currentProperty.HasValue ? currentProperty.GetValue(null, _defaultValue) : _defaultValue;
                },
                _renderControl,
                _defaultValue
            );
        }
    }

    /// <summary>
    /// 可空引用类型字段渲染器（用于RectOffset等引用类型）
    /// </summary>
    public class StylePropertyRefFieldRenderer<T> : FieldRenderer where T : class
    {
        private System.Action<StyleProperty<T>> _setter;
        private System.Func<StyleProperty<T>> _getter;
        private System.Func<T, T> _renderControl;
        private T _defaultValue;

        public StylePropertyRefFieldRenderer(DeclStyleEditorRenderer editor, string label, string fieldName,
            T defaultValue, System.Action<StyleProperty<T>> setter,
            System.Func<StyleProperty<T>> getter, System.Func<T, T> renderControl)
            : base(editor, label, fieldName)
        {
            _defaultValue = defaultValue;
            _setter = setter;
            _getter = getter;
            _renderControl = renderControl;
        }

        public override void Render(bool isEnabled)
        {
            RenderCommonLayout(isEnabled, RenderEnabledControl, RenderDisabledControl);
        }

        protected override void HandleButtonClick()
        {
            HandleButtonClickCommon(
                () => _setter(StyleProperty<T>.Direct(_defaultValue)),
                () => _setter(StyleProperty<T>.None())
            );
        }

        protected override void RenderControl()
        {
            RenderControlCommonRef(
                () => {
                    StyleProperty<T> currentProperty = _getter();
                    return currentProperty.HasValue ? currentProperty.GetValue(null, _defaultValue) : _defaultValue;
                },
                _renderControl,
                (value) => _setter(StyleProperty<T>.Direct(value)),
                _defaultValue
            );
        }

        private void RenderEnabledControl()
        {
            RenderEnabledControlCommon(RenderControl);
        }

        private void RenderDisabledControl()
        {
            RenderDisabledControlCommon(
                () => {
                    StyleProperty<T> currentProperty = _getter();
                    return currentProperty.HasValue ? currentProperty.GetValue(null, _defaultValue) : _defaultValue;
                },
                _renderControl,
                _defaultValue
            );
        }
    }

    /// <summary>
    /// StyleProperty字段渲染器（支持主题属性引用）
    /// </summary>
    public class StylePropertyWithThemeRefFieldRenderer<T> : FieldRenderer
    {
        private System.Action<StyleProperty<T>> _setter;
        private System.Func<StyleProperty<T>> _getter;
        private System.Func<T, T> _renderControl;
        private T _defaultValue;
        private DeclTheme _theme;

        public StylePropertyWithThemeRefFieldRenderer(DeclStyleEditorRenderer editor, string label, string fieldName,
            T defaultValue, System.Action<StyleProperty<T>> setter,
            System.Func<StyleProperty<T>> getter, System.Func<T, T> renderControl, DeclTheme theme)
            : base(editor, label, fieldName)
        {
            _defaultValue = defaultValue;
            _setter = setter;
            _getter = getter;
            _renderControl = renderControl;
            _theme = theme;
        }

        public override void Render(bool isEnabled)
        {
            RenderCommonLayout(isEnabled, RenderEnabledControl, RenderDisabledControl);
        }

        protected override void HandleButtonClick()
        {
            HandleButtonClickCommon(
                () => _setter(StyleProperty<T>.Direct(_defaultValue)),
                () => _setter(StyleProperty<T>.None())
            );
        }

        protected override void RenderControl()
        {
            StyleProperty<T> currentProperty = _getter();
            
            switch (currentProperty.ValueType)
            {
                case PropertyValueType.Direct:
                    RenderDirectValueControl(currentProperty);
                    break;
                case PropertyValueType.PropertyRef:
                    RenderPropertyRefControl(currentProperty);
                    break;
                case PropertyValueType.None:
                default:
                    RenderNoneValueControl();
                    break;
            }
        }

        private void RenderDirectValueControl(StyleProperty<T> currentProperty)
        {
            T currentValue = currentProperty.HasValue ? currentProperty.GetValue(null, _defaultValue) : _defaultValue;
            
            EditorGUILayout.BeginHorizontal();
            
            // 渲染值控件
            T newValue = _renderControl(currentValue);
            
            // 添加下拉按钮用于切换引用模式，使用Unity组件标题下拉样式
            if (GUILayout.Button(GUIContent.none, EditorStyles.foldoutHeaderIcon, GUILayout.Width(20), GUILayout.Height(16)))
            {
                ShowValueTypeMenu();
            }
            
            EditorGUILayout.EndHorizontal();
            
            if (!newValue.Equals(currentValue))
            {
                // 在修改值之前记录Undo操作
                _editor.RecordUndo("Modify " + _fieldName);
                _setter(StyleProperty<T>.Direct(newValue));
                _editor.MarkDirtyPublic();
                _editor.FieldModifiedStates[_fieldName] = true;
            }
        }

        private void RenderPropertyRefControl(StyleProperty<T> currentProperty)
        {
            string propertyRef = currentProperty.PropertyRef;
            
            // 获取主题属性列表
            List<string> themePropertyNames = GetThemePropertyNames();
            string[] options = new string[themePropertyNames.Count + 1];
            options[0] = "None";
            themePropertyNames.CopyTo(options, 1);
            
            int currentIndex = 0;
            if (!string.IsNullOrEmpty(propertyRef))
            {
                currentIndex = Array.IndexOf(options, propertyRef);
                if (currentIndex == -1) currentIndex = 0;
            }
            
            EditorGUILayout.BeginHorizontal();
            
            // 渲染引用名称下拉选择框
            int newIndex = EditorGUILayout.Popup(currentIndex, options, GUILayout.ExpandWidth(true));
            
            // 渲染引用值预览（只读）
            if (!string.IsNullOrEmpty(propertyRef) && _theme != null)
            {
                T refValue = _theme.GetThemeProperty(propertyRef, _defaultValue);
                
                // 使用BeginDisabledGroup禁用引用值的编辑
                EditorGUI.BeginDisabledGroup(true);
                
                // 根据类型渲染具体的值控件，而不是纯label
                if (typeof(T) == typeof(Color))
                {
                    EditorGUILayout.ColorField((Color)(object)refValue);
                }
                else if (typeof(T) == typeof(float))
                {
                    EditorGUILayout.FloatField((float)(object)refValue);
                }
                else if (typeof(T) == typeof(int))
                {
                    EditorGUILayout.IntField((int)(object)refValue);
                }
                else if (typeof(T) == typeof(string))
                {
                    EditorGUILayout.TextField((string)(object)refValue);
                }
                else if (typeof(T) == typeof(bool))
                {
                    EditorGUILayout.Toggle((bool)(object)refValue);
                }
                else if (typeof(T) == typeof(Vector2))
                {
                    EditorGUILayout.Vector2Field("", (Vector2)(object)refValue);
                }
                else if (typeof(T) == typeof(Vector3))
                {
                    EditorGUILayout.Vector3Field("", (Vector3)(object)refValue);
                }
                else if (typeof(T) == typeof(RectOffset))
                {
                    // 使用紧凑布局显示RectOffset预览
                    EditorGUILayout.BeginHorizontal();
                    
                    // 显示紧凑的RectOffset格式：[左,右,上,下]
                    RectOffset rectValue = (RectOffset)(object)refValue;
                    string previewText = $"[{rectValue.left},{rectValue.right},{rectValue.top},{rectValue.bottom}]";
                    EditorGUILayout.LabelField(previewText);
                    
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    // 默认使用ToString显示
                    EditorGUILayout.LabelField(refValue.ToString());
                }
                
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                // 如果没有引用，显示空值提示
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField("无引用", GUILayout.Width(60));
                EditorGUI.EndDisabledGroup();
            }
            
            // 添加下拉按钮用于切换引用模式，使用Unity组件标题下拉样式
            if (GUILayout.Button(GUIContent.none, EditorStyles.foldoutHeaderIcon, GUILayout.Width(20), GUILayout.Height(16)))
            {
                ShowValueTypeMenu();
            }
            
            EditorGUILayout.EndHorizontal();
            
            if (newIndex != currentIndex)
            {
                _editor.RecordUndo("Change Theme Property Ref " + _fieldName);
                if (newIndex == 0)
                {
                    _setter(StyleProperty<T>.None());
                }
                else
                {
                    string newPropertyRef = options[newIndex];
                    _setter(StyleProperty<T>.Ref(newPropertyRef));
                }
                _editor.MarkDirtyPublic();
                _editor.FieldModifiedStates[_fieldName] = true;
            }
        }

        private void RenderNoneValueControl()
        {
            EditorGUILayout.LabelField("未设置值");
        }

        private void ShowValueTypeMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("直接值"), false, () => SwitchToDirectValue());
            menu.AddItem(new GUIContent("主题属性引用"), false, () => SwitchToPropertyRef());
            menu.ShowAsContext();
        }

        private void SwitchToDirectValue()
        {
            _editor.RecordUndo("Switch to Direct Value " + _fieldName);
            StyleProperty<T> currentProperty = _getter();
            T currentValue = currentProperty.HasValue ? currentProperty.GetValue(null, _defaultValue) : _defaultValue;
            _setter(StyleProperty<T>.Direct(currentValue));
            _editor.MarkDirtyPublic();
        }

        private void SwitchToPropertyRef()
        {
            _editor.RecordUndo("Switch to Property Ref " + _fieldName);
            StyleProperty<T> currentProperty = _getter();
            string currentRef = currentProperty.PropertyRef;
            if (string.IsNullOrEmpty(currentRef))
            {
                // 如果没有当前引用，设置为空引用
                _setter(StyleProperty<T>.Ref(""));
            }
            else
            {
                // 保持当前引用
                _setter(StyleProperty<T>.Ref(currentRef));
            }
            _editor.MarkDirtyPublic();
        }

        private void RenderEnabledControl()
        {
            RenderControl();
        }

        private void RenderDisabledControl()
        {
            GUI.enabled = false;
            RenderControl();
            GUI.enabled = true;
        }

        private List<string> GetThemePropertyNames()
        {
            List<string> names = new List<string>();
            
            if (_theme != null)
            {
                var properties = _theme.ThemeProperties;
                foreach (var prop in properties)
                {
                    // 只添加与当前类型匹配的属性
                    if (IsPropertyTypeMatch(prop.Type, typeof(T)))
                    {
                        names.Add(prop.Name);
                    }
                }
            }
            
            // 如果没有找到匹配的属性，添加一个默认选项
            if (names.Count == 0)
            {
                names.Add("无可用属性");
            }
            
            return names;
        }

        private bool IsPropertyTypeMatch(PropertyType propertyType, Type targetType)
        {
            if (targetType == typeof(Color) && propertyType == PropertyType.Color)
                return true;
            if (targetType == typeof(float) && propertyType == PropertyType.Float)
                return true;
            if (targetType == typeof(int) && propertyType == PropertyType.Int)
                return true;
            if (targetType == typeof(string) && propertyType == PropertyType.String)
                return true;
            if (targetType == typeof(bool) && propertyType == PropertyType.Boolean)
                return true;
            if (targetType == typeof(Vector2) && propertyType == PropertyType.Vector2)
                return true;
            if (targetType == typeof(Vector3) && propertyType == PropertyType.Vector3)
                return true;
            if (targetType == typeof(RectOffset) && propertyType == PropertyType.RectOffset)
                return true;
                
            return false;
        }
    }

    /// <summary>
    /// DeclStyle 编辑器，模仿 Unity 材质编辑器风格
    /// </summary>
    public class DeclStyleEditorRenderer
    {
        private ISerializableDeclStyle _style;
        private SerializedObject _serializedObject;
        private DeclTheme _declTheme;
        
        // 字段修改状态跟踪
        private Dictionary<string, bool> _fieldModifiedStates = new Dictionary<string, bool>();
        
        // 字段启用状态跟踪（true表示启用，false表示禁用）
        private Dictionary<string, bool> _fieldEnabledStates = new Dictionary<string, bool>();
        
        // 区块折叠状态
        private bool _enabledFieldsExpanded = true;
        private bool _defaultFieldsExpanded = true;
        
        // 字段渲染器缓存
        private Dictionary<string, FieldRenderer> _fieldRenderers = new Dictionary<string, FieldRenderer>();
        
        // 自定义样式
        private GUIStyle _boldLabelStyle;
        private GUIStyle _checkboxStyle;
        private GUIStyle _minusButtonStyle;
        private GUIStyle _plusButtonStyle;
        private GUIStyle _foldoutStyle;
        private GUIStyle _separatorStyle;
        
        // 外部刷新回调
        private System.Action _repaintCallback;

        // 公共属性用于访问内部状态
        public Dictionary<string, bool> FieldModifiedStates => _fieldModifiedStates;
        public Dictionary<string, bool> FieldEnabledStates => _fieldEnabledStates;
        public GUIStyle BoldLabelStyle => _boldLabelStyle;
        public GUIStyle MinusButtonStyle => _minusButtonStyle;
        public GUIStyle PlusButtonStyle => _plusButtonStyle;

        /// <summary>
        /// 公开的标记脏方法，供字段渲染器使用
        /// </summary>
        public void MarkDirtyPublic()
        {
            MarkDirty();
        }
        
        /// <summary>
        /// 记录Undo操作
        /// </summary>
        public void RecordUndo(string actionName)
        {
            if (_serializedObject != null && _serializedObject.targetObject != null)
            {
                Undo.RecordObject(_serializedObject.targetObject, actionName);
            }
        }
        
        /// <summary>
        /// 刷新内部状态以匹配底层对象的状态
        /// </summary>
        public void RefreshState()
        {
            // 重新初始化字段启用状态
            InitializeFieldEnabledStates();
        }
        
        /// <summary>
        /// 初始化字段启用状态
        /// </summary>
        private void InitializeFieldEnabledStates()
        {
            // 清空现有的状态
            _fieldEnabledStates.Clear();
            
            // 初始化所有字段为启用状态
            foreach (var fieldName in _fieldRenderers.Keys)
            {
                bool hasValue = false;
                switch (fieldName)
                {
                    case "Color": hasValue = _style.Color.HasValue; break;
                    case "BackgroundColor": hasValue = _style.BackgroundColor.HasValue; break;
                    case "BorderColor": hasValue = _style.BorderColor.HasValue; break;
                    case "Width": hasValue = _style.Width.HasValue; break;
                    case "Height": hasValue = _style.Height.HasValue; break;
                    case "Padding": hasValue = _style.Padding.HasValue; break;
                    case "Margin": hasValue = _style.Margin.HasValue; break;
                    case "FontSize": hasValue = _style.FontSize.HasValue; break;
                    case "FontStyle": hasValue = _style.FontStyle.HasValue; break;
                    case "Alignment": hasValue = _style.Alignment.HasValue; break;
                    case "BorderWidth": hasValue = _style.BorderWidth.HasValue; break;
                    case "BorderRadius": hasValue = _style.BorderRadius.HasValue; break;
                }
                _fieldEnabledStates[fieldName] = hasValue;
            }
        }

        /// <summary>
        /// 从 ISerializableDeclStyle 创建编辑器
        /// </summary>
        public DeclStyleEditorRenderer(ISerializableDeclStyle style, System.Action repaintCallback = null, DeclTheme declTheme = null)
        {
            _style = style;
            _repaintCallback = repaintCallback;
            _declTheme = declTheme;

            // 如果 style 是 UnityEngine.Object，可以创建 SerializedObject
            if (style is UnityEngine.Object unityObject)
            {
                _serializedObject = new SerializedObject(unityObject);
            }

            // 初始化样式
            InitializeStyles();
            // 初始化字段渲染器
            InitializeFieldRenderers();
        }

        /// <summary>
        /// 初始化自定义样式
        /// </summary>
        private void InitializeStyles()
        {
            // 延迟初始化样式，确保在OnGUI上下文中调用
        }
        
        /// <summary>
        /// 确保样式已初始化
        /// </summary>
        public void EnsureStylesInitialized()
        {
            if (_boldLabelStyle == null)
            {
                // 加粗标签样式用于修改的字段
                _boldLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Bold
                };
            }

            if (_checkboxStyle == null)
            {
                // 复选框样式
                _checkboxStyle = new GUIStyle(EditorStyles.toggle)
                {
                    margin = new RectOffset(2, 4, 2, 2),
                    fixedWidth = 16,
                    fixedHeight = 16
                };
            }

            if (_minusButtonStyle == null)
            {
                // 减号按钮样式（圆形）- 使用miniButtonLeft样式
                _minusButtonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
            }

            if (_plusButtonStyle == null)
            {
                // 加号按钮样式（圆形）- 使用miniButtonLeft样式
                _plusButtonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
            }

            if (_foldoutStyle == null)
            {
                // 折叠样式
                _foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };
            }

            if (_separatorStyle == null)
            {
                // 分隔线样式
                _separatorStyle = new GUIStyle(GUI.skin.horizontalSlider)
                {
                    fixedHeight = 1,
                    margin = new RectOffset(0, 0, 2, 2)
                };
            }
        }

        /// <summary>
        /// 初始化字段渲染器
        /// </summary>
        private void InitializeFieldRenderers()
        {
            // 颜色属性 - 使用支持主题属性的渲染器
            _fieldRenderers["Color"] = new StylePropertyWithThemeRefFieldRenderer<Color>(this, "主颜色", "Color",
                GetDefaultValueFromStyleSet("Color", Color.white),
                (value) => _style.Color = value, () => _style.Color, (value) => EditorGUILayout.ColorField(value), _declTheme);
                
            _fieldRenderers["BackgroundColor"] = new StylePropertyWithThemeRefFieldRenderer<Color>(this, "背景颜色", "BackgroundColor",
                GetDefaultValueFromStyleSet("BackgroundColor", Color.clear),
                (value) => _style.BackgroundColor = value, () => _style.BackgroundColor, (value) => EditorGUILayout.ColorField(value), _declTheme);
                
            _fieldRenderers["BorderColor"] = new StylePropertyWithThemeRefFieldRenderer<Color>(this, "边框颜色", "BorderColor",
                GetDefaultValueFromStyleSet("BorderColor", Color.black),
                (value) => _style.BorderColor = value, () => _style.BorderColor, (value) => EditorGUILayout.ColorField(value), _declTheme);

            // 尺寸属性 - 使用支持主题属性的渲染器
            _fieldRenderers["Width"] = new StylePropertyWithThemeRefFieldRenderer<float>(this, "宽度", "Width",
                GetDefaultValueFromStyleSet("Width", 0f),
                (value) => _style.Width = value, () => _style.Width, (value) => EditorGUILayout.FloatField(value), _declTheme);
                
            _fieldRenderers["Height"] = new StylePropertyWithThemeRefFieldRenderer<float>(this, "高度", "Height",
                GetDefaultValueFromStyleSet("Height", 0f),
                (value) => _style.Height = value, () => _style.Height, (value) => EditorGUILayout.FloatField(value), _declTheme);

            // 布局属性 - 使用支持主题属性的渲染器
            _fieldRenderers["Padding"] = new StylePropertyWithThemeRefFieldRenderer<RectOffset>(this, "内边距", "Padding", new RectOffset(),
                (value) => _style.Padding = value, () => _style.Padding, (value) => RenderRectOffsetControl(value), _declTheme);
                
            _fieldRenderers["Margin"] = new StylePropertyWithThemeRefFieldRenderer<RectOffset>(this, "外边距", "Margin", new RectOffset(),
                (value) => _style.Margin = value, () => _style.Margin, (value) => RenderRectOffsetControl(value), _declTheme);

            // 文本属性 - 使用支持主题属性的渲染器
            _fieldRenderers["FontSize"] = new StylePropertyWithThemeRefFieldRenderer<int>(this, "字体大小", "FontSize",
                GetDefaultValueFromStyleSet("FontSize", 0),
                (value) => _style.FontSize = value, () => _style.FontSize, (value) => EditorGUILayout.IntField(value), _declTheme);
                
            _fieldRenderers["FontStyle"] = new StylePropertyWithThemeRefFieldRenderer<FontStyle>(this, "字体样式", "FontStyle",
                GetDefaultValueFromStyleSet("FontStyle", FontStyle.Normal),
                (value) => _style.FontStyle = value, () => _style.FontStyle, (value) => (FontStyle)EditorGUILayout.EnumPopup(value), _declTheme);
                
            _fieldRenderers["Alignment"] = new StylePropertyWithThemeRefFieldRenderer<TextAnchor>(this, "对齐方式", "Alignment",
                GetDefaultValueFromStyleSet("Alignment", TextAnchor.UpperLeft),
                (value) => _style.Alignment = value, () => _style.Alignment, (value) => (TextAnchor)EditorGUILayout.EnumPopup(value), _declTheme);

            // 边框属性 - 使用支持主题属性的渲染器
            _fieldRenderers["BorderWidth"] = new StylePropertyWithThemeRefFieldRenderer<float>(this, "边框宽度", "BorderWidth",
                GetDefaultValueFromStyleSet("BorderWidth", 0f),
                (value) => _style.BorderWidth = value, () => _style.BorderWidth, (value) => EditorGUILayout.FloatField(value), _declTheme);
                
            _fieldRenderers["BorderRadius"] = new StylePropertyWithThemeRefFieldRenderer<float>(this, "边框圆角", "BorderRadius",
                GetDefaultValueFromStyleSet("BorderRadius", 0f),
                (value) => _style.BorderRadius = value, () => _style.BorderRadius, (value) => EditorGUILayout.FloatField(value), _declTheme);
        }

        /// <summary>
        /// 渲染 DeclStyle 界面（使用分块布局）
        /// </summary>
        public void Render()
        {
            if (_style == null) return;
            
            // 确保样式已初始化
            EnsureStylesInitialized();

            // 样式集引用（放在属性开头）
            RenderStyleSetReference();
            
            EditorGUILayout.Space();

            // 使用分块布局渲染字段
            RenderFieldsWithBlocks();

            EditorGUILayout.Space();
        }

        /// <summary>
        /// 使用分块布局渲染所有字段
        /// </summary>
        private void RenderFieldsWithBlocks()
        {
            // 初始化字段启用状态（如果尚未初始化）
            InitializeFieldEnabledStates();

            // 渲染启用字段区块
            RenderEnabledFieldsBlock();

            // 分隔线 - 进一步减少间距
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("", _separatorStyle);
            EditorGUILayout.Space(2);

            // 渲染缺省字段区块
            RenderDefaultFieldsBlock();
        }

        /// <summary>
        /// 渲染启用字段区块
        /// </summary>
        private void RenderEnabledFieldsBlock()
        {
            // 折叠标题
            _enabledFieldsExpanded = EditorGUILayout.Foldout(_enabledFieldsExpanded, "启用字段", _foldoutStyle);
            
            if (_enabledFieldsExpanded)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;

                bool hasEnabledFields = false;
                string previousCategory = null;

                foreach (var fieldName in _fieldRenderers.Keys)
                {
                    if (_fieldEnabledStates.ContainsKey(fieldName) && _fieldEnabledStates[fieldName])
                    {
                        // 按类别添加间距 - 只在确实有字段时才添加
                        string currentCategory = GetFieldCategory(fieldName);
                        if (previousCategory != null && previousCategory != currentCategory && hasEnabledFields)
                        {
                            EditorGUILayout.Space(4); // 减少间距
                        }
                        previousCategory = currentCategory;

                        _fieldRenderers[fieldName].Render(true);
                        EditorGUILayout.Space(2); // 增加常规字段间距
                        hasEnabledFields = true;
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// 渲染缺省字段区块
        /// </summary>
        private void RenderDefaultFieldsBlock()
        {
            // 折叠标题
            _defaultFieldsExpanded = EditorGUILayout.Foldout(_defaultFieldsExpanded, "缺省字段", _foldoutStyle);
            
            if (_defaultFieldsExpanded)
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel++;

                bool hasDefaultFields = false;
                string previousCategory = null;

                foreach (var fieldName in _fieldRenderers.Keys)
                {
                    if (!_fieldEnabledStates.ContainsKey(fieldName) || !_fieldEnabledStates[fieldName])
                    {
                        // 按类别添加间距 - 只在确实有字段时才添加
                        string currentCategory = GetFieldCategory(fieldName);
                        if (previousCategory != null && previousCategory != currentCategory && hasDefaultFields)
                        {
                            EditorGUILayout.Space(4); // 减少间距
                        }
                        previousCategory = currentCategory;

                        _fieldRenderers[fieldName].Render(false);
                        EditorGUILayout.Space(2); // 增加常规字段间距
                        hasDefaultFields = true;
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// 获取字段类别用于间距控制
        /// </summary>
        private string GetFieldCategory(string fieldName)
        {
            switch (fieldName)
            {
                case "Color":
                case "BackgroundColor":
                case "BorderColor":
                    return "Color";
                case "Width":
                case "Height":
                    return "Size";
                case "Padding":
                case "Margin":
                    return "Layout";
                case "FontSize":
                case "FontStyle":
                case "Alignment":
                    return "Text";
                case "BorderWidth":
                case "BorderRadius":
                    return "Border";
                default:
                    return "Other";
            }
        }

        /// <summary>
        /// 渲染样式集引用（使用下拉选择框）
        /// </summary>
        private void RenderStyleSetReference()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("父级样式", EditorStyles.boldLabel);
            
            // 获取当前值
            SerializableRefNullable<string> currentNullable = _style.StyleSetId;
            string currentValue = currentNullable.HasValue ? currentNullable.Value : "";
            
            // 获取可用的样式集键
            List<string> availableKeys = GetStyleSetKeysFromTheme();
            
            // 如果没有 DeclTheme 引用，只能选择当前值或 None
            if (_declTheme == null)
            {
                if (!string.IsNullOrEmpty(currentValue) && !availableKeys.Contains(currentValue))
                {
                    availableKeys.Add(currentValue);
                }
            }
            
            // 创建选项数组
            string[] options = availableKeys.ToArray();
            int currentIndex = Array.IndexOf(options, currentValue);
            if (currentIndex == -1) currentIndex = 0; // 默认为 None
            
            // 渲染下拉选择框
            EditorGUILayout.BeginHorizontal();
            
            // 处理缩进
            GUILayout.Space(EditorGUI.indentLevel * 15);
            
            // // 使用精确的GUI.Label而不是自动布局
            // Rect labelRect = GUILayoutUtility.GetRect(100, 16, GUILayout.Width(100));
            // GUI.Label(labelRect, "样式集", EditorStyles.label);
            
            // 渲染下拉选择框
            int newIndex = EditorGUILayout.Popup(currentIndex, options);
            
            EditorGUILayout.EndHorizontal();
            
            // 处理值变化
            if (newIndex != currentIndex)
            {
                string newValue = newIndex == 0 ? "" : options[newIndex];
                
                if (string.IsNullOrEmpty(newValue))
                {
                    _style.StyleSetId = new SerializableRefNullable<string>();
                }
                else
                {
                    _style.StyleSetId = new SerializableRefNullable<string>(newValue);
                }
                
                MarkDirty();
                _fieldModifiedStates["StyleSetId"] = !string.IsNullOrEmpty(newValue);
            }
            
            // 显示引用信息提示
            if (_style.StyleSetId.HasValue && !string.IsNullOrEmpty(_style.StyleSetId.Value))
            {
                EditorGUILayout.HelpBox($"此样式引用了样式集: {_style.StyleSetId.Value}\n将继承该样式集的所有属性", MessageType.Info);
            }
        }

        /// <summary>
        /// 渲染 RectOffset 控件（使用精确的GUI布局）
        /// </summary>
        private RectOffset RenderRectOffsetControl(RectOffset value)
        {
            if (value == null) value = new RectOffset();
            
            // 获取当前控件的矩形区域
            Rect controlRect = EditorGUILayout.GetControlRect();

            controlRect.x += 30;
            
            // 计算每个字段的宽度 - 减少间距，增加输入框宽度
            float fieldWidth = (controlRect.width - 50) / 4; // 4个字段，留出20像素的间距
            float spacing = 5f; // 减少间距
            
            // 左边距字段
            Rect leftRect = new Rect(controlRect.x, controlRect.y, fieldWidth, controlRect.height);
            
            // 右边距字段
            Rect rightRect = new Rect(controlRect.x + fieldWidth + spacing, controlRect.y, fieldWidth, controlRect.height);
            
            // 上边距字段
            Rect topRect = new Rect(controlRect.x + (fieldWidth + spacing) * 2, controlRect.y, fieldWidth, controlRect.height);
            
            // 下边距字段
            Rect bottomRect = new Rect(controlRect.x + (fieldWidth + spacing) * 3, controlRect.y, fieldWidth, controlRect.height);
            
            // 绘制左边距字段 - 标签和输入框更贴近
            GUI.Label(new Rect(leftRect.x, leftRect.y, 40, leftRect.height), "左边距");
            value.left = EditorGUI.IntField(new Rect(leftRect.x + 40, leftRect.y, fieldWidth - 40, leftRect.height), value.left);
            
            // 绘制右边距字段 - 标签和输入框更贴近
            GUI.Label(new Rect(rightRect.x, rightRect.y, 40, rightRect.height), "右边距");
            value.right = EditorGUI.IntField(new Rect(rightRect.x + 40, rightRect.y, fieldWidth - 40, rightRect.height), value.right);
            
            // 绘制上边距字段 - 标签和输入框更贴近
            GUI.Label(new Rect(topRect.x, topRect.y, 40, topRect.height), "上边距");
            value.top = EditorGUI.IntField(new Rect(topRect.x + 40, topRect.y, fieldWidth - 40, topRect.height), value.top);
            
            // 绘制下边距字段 - 标签和输入框更贴近
            GUI.Label(new Rect(bottomRect.x, bottomRect.y, 40, bottomRect.height), "下边距");
            value.bottom = EditorGUI.IntField(new Rect(bottomRect.x + 40, bottomRect.y, fieldWidth - 40, bottomRect.height), value.bottom);
            
            // 增加一些垂直间距
            GUILayout.Space(controlRect.height + 2);
            
            return value;
        }

        /// <summary>
        /// 标记对象为脏（已修改）
        /// </summary>
        private void MarkDirty()
        {
            if (_serializedObject != null && _serializedObject.targetObject != null)
            {
                // 记录Undo操作
                Undo.RecordObject(_serializedObject.targetObject, "Modify DeclStyle");
                EditorUtility.SetDirty(_serializedObject.targetObject);
            }
            
            // 调用外部刷新回调
            _repaintCallback?.Invoke();
        }

        /// <summary>
        /// 获取 DeclTheme 中所有样式集的键（除了当前样式集）
        /// </summary>
        private List<string> GetStyleSetKeysFromTheme()
        {
            var keys = new List<string> { "None" };
            
            if (_declTheme == null)
                return keys;

            // 使用反射获取私有字段 styleSets
            var styleSetsField = typeof(DeclTheme).GetField("styleSets",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (styleSetsField != null)
            {
                var styleSets = styleSetsField.GetValue(_declTheme) as System.Collections.IList;
                if (styleSets != null)
                {
                    foreach (var entry in styleSets)
                    {
                        var idField = entry.GetType().GetField("Id");
                        var styleSetField = entry.GetType().GetField("StyleSet");
                        
                        if (idField != null && styleSetField != null)
                        {
                            string id = idField.GetValue(entry) as string;
                            object styleSet = styleSetField.GetValue(entry);
                            
                            // 排除当前样式集（如果当前样式是 DeclStyleSet）
                            if (!string.IsNullOrEmpty(id) && styleSet != null)
                            {
                                if (_style is UnityEngine.Object currentStyleObj &&
                                    styleSet is UnityEngine.Object themeStyleObj)
                                {
                                    if (currentStyleObj != themeStyleObj)
                                    {
                                        keys.Add(id);
                                    }
                                }
                                else
                                {
                                    keys.Add(id);
                                }
                            }
                        }
                    }
                }
            }
            
            return keys;
        }

        /// <summary>
        /// 获取选中样式集的默认值
        /// </summary>
        private T GetDefaultValueFromStyleSet<T>(string fieldName, T defaultValue)
        {
            if (_declTheme == null || !_style.StyleSetId.HasValue || string.IsNullOrEmpty(_style.StyleSetId.Value))
                return defaultValue;

            // 获取样式集
            var styleSet = _declTheme.GetStyleSet(_style.StyleSetId.Value);
            if (styleSet == null)
                return defaultValue;

            // 根据字段名获取对应的值
            switch (fieldName)
            {
                case "Color": return (T)(object)(styleSet.Color ?? (Color)(object)defaultValue);
                case "BackgroundColor": return (T)(object)(styleSet.BackgroundColor ?? (Color)(object)defaultValue);
                case "BorderColor": return (T)(object)(styleSet.BorderColor ?? (Color)(object)defaultValue);
                case "Width": return (T)(object)(styleSet.Width ?? (float)(object)defaultValue);
                case "Height": return (T)(object)(styleSet.Height ?? (float)(object)defaultValue);
                case "FontSize": return (T)(object)(styleSet.FontSize ?? (int)(object)defaultValue);
                case "FontStyle": return (T)(object)(styleSet.FontStyle ?? (FontStyle)(object)defaultValue);
                case "Alignment": return (T)(object)(styleSet.Alignment ?? (TextAnchor)(object)defaultValue);
                case "BorderWidth": return (T)(object)(styleSet.BorderWidth ?? (float)(object)defaultValue);
                case "BorderRadius": return (T)(object)(styleSet.BorderRadius ?? (float)(object)defaultValue);
                default: return defaultValue;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 清理资源
        }
    }
}