using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;

namespace DeclGUI.Core
{
    /// <summary>
    /// DeclGUI 主题配置
    /// 使用 ScriptableObject 支持可视化编辑
    /// </summary>
    [CreateAssetMenu(fileName = "NewDeclTheme", menuName = "DeclGUI/Theme")]
    public class DeclTheme : ScriptableObject
    {
        [SerializeField]
        private List<StyleSetEntry> styleSets = new List<StyleSetEntry>();
        
        // 属性配置字典
        [SerializeField]
        private List<ThemeProperty> themeProperties = new List<ThemeProperty>();

        // 属性模板引用
        [SerializeField]
        private DeclPropertyTemplate propertyTemplate;
        
        // 模板属性名称集合（用于快速查找）
        private HashSet<string> _templatePropertyNames = new HashSet<string>();

        private Dictionary<string, IDeclStyle> _styleSetCache;
        private Dictionary<string, object> _propertyCache;
        
        [System.NonSerialized]
        private bool _isAutoCollected = false;

#if UNITY_EDITOR
        private void Reset()
        {
            if (!Application.isPlaying)
            {
                BuildCache();
            }
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                BuildCache();
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                // 在编辑器模式下，当属性模板发生变化时自动应用模板
                if (propertyTemplate != null)
                {
                    ApplyPropertyTemplate();
                }
                else
                {
                    // 如果清除了模板，清除模板属性名称集合
                    _templatePropertyNames.Clear();
                }
                
                // 重建缓存以确保缓存值与序列化数据同步
                BuildCache();
            }
        }
#else
        private void OnEnable()
        {
            // 运行时只构建缓存，不自动收集
            BuildCache();
            DeclGUISetting.Instance.Refresh();
        }
#endif

        void Awake()
        {
        }

        /// <summary>
        /// 属性模板引用
        /// </summary>
        public DeclPropertyTemplate PropertyTemplate
        {
            get => propertyTemplate;
            set
            {
                if (propertyTemplate != value)
                {
                    propertyTemplate = value;
                    ApplyPropertyTemplate();
                }
            }
        }


        /// <summary>
        /// 获取样式集
        /// </summary>
        public IDeclStyle GetStyleSet(string id)
        {
            if (_styleSetCache == null)
                BuildCache();

            return _styleSetCache.TryGetValue(id, out var styleSet) ? styleSet : null;
        }

        /// <summary>
        /// 注册样式集
        /// </summary>
        public void RegisterStyleSet(string id, IDeclStyle styleSet)
        {
            if (_styleSetCache == null)
                BuildCache();

            _styleSetCache[id] = styleSet;

            // 更新序列化数据
            var entry = styleSets.Find(e => e.Id == id);
            if (entry == null)
            {
                entry = new StyleSetEntry { Id = id };
                styleSets.Add(entry);
            }

            if (styleSet is DeclStyleSet styleSetImpl)
            {
                entry.StyleSet = styleSetImpl;
            }
        }

        /// <summary>
        /// 构建样式集缓存（公开方法供编辑器使用）
        /// </summary>
        public void BuildCache()
        {
            _styleSetCache = new Dictionary<string, IDeclStyle>();
            _propertyCache = new Dictionary<string, object>();
            
            // 在编辑器模式下自动收集样式集
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // 总是尝试自动收集，但只在需要时添加
                AutoCollectStyleSets();
                //_isAutoCollected = true; // 注释掉这行，让每次都能检查
            }
#endif
            
            foreach (var entry in styleSets)
            {
                if (!string.IsNullOrEmpty(entry.Id) && entry.StyleSet != null)
                {
                    _styleSetCache[entry.Id] = entry.StyleSet;
                }
            }
            
            // 构建属性缓存
            foreach (var prop in themeProperties)
            {
                if (!string.IsNullOrEmpty(prop.Name))
                {
                    _propertyCache[prop.Name] = prop.GetValue();
                }
            }

            // 更新模板属性名称集合
            UpdateTemplatePropertyNames();
        }
        
        /// <summary>
        /// 自动收集同目录下的所有DeclStyleSet文件
        /// </summary>
        private void AutoCollectStyleSets()
        {
#if UNITY_EDITOR
            try
            {
                string themePath = UnityEditor.AssetDatabase.GetAssetPath(this);
                if (string.IsNullOrEmpty(themePath))
                    return;
                    
                string directory = Path.GetDirectoryName(themePath);
                if (string.IsNullOrEmpty(directory))
                    return;
                    
                // 获取同目录下所有的DeclStyleSet文件
                string[] styleSetGuids = UnityEditor.AssetDatabase.FindAssets("t:DeclStyleSet", new[] { directory });
                
                bool hasChanges = false;
                
                foreach (string guid in styleSetGuids)
                {
                    string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    DeclStyleSet styleSet = UnityEditor.AssetDatabase.LoadAssetAtPath<DeclStyleSet>(assetPath);
                    
                    if (styleSet != null)
                    {
                        string key = Path.GetFileNameWithoutExtension(assetPath);
                        
                        // 查找是否已存在该样式集
                        var existingEntry = styleSets.FirstOrDefault(e => e.StyleSet == styleSet);
                        
                        if (existingEntry != null)
                        {
                            // 如果已存在，更新其ID为文件名
                            if (existingEntry.Id != key)
                            {
                                existingEntry.Id = key;
                                hasChanges = true;
                                Debug.Log($"DeclTheme: 更新样式集ID '{key}' -> {assetPath}");
                            }
                        }
                        else
                        {
                            // 如果不存在，添加新的条目
                            var entry = new StyleSetEntry
                            {
                                Id = key,
                                StyleSet = styleSet
                            };
                            
                            styleSets.Add(entry);
                            hasChanges = true;
                            
                            Debug.Log($"DeclTheme: 自动收集样式集 '{key}' -> {assetPath}");
                        }
                    }
                }
                
                // 如果有更改，标记为脏以保存更改
                if (hasChanges)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.AssetDatabase.SaveAssets();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"DeclTheme 自动收集样式集时出错: {ex.Message}");
            }
#endif
        }
        
        /// <summary>
        /// 获取主题属性值
        /// </summary>
        public T GetThemeProperty<T>(string name, T defaultValue = default(T))
        {
#if UNITY_EDITOR
            // // 在编辑器模式下，确保缓存与序列化数据同步
            // if (!Application.isPlaying && _propertyCache != null)
            // {
            //     // 检查当前缓存值与序列化数据是否一致，如果不一致则重建缓存
            //     var prop = themeProperties.Find(p => p.Name == name);
            //     if (prop != null && _propertyCache.ContainsKey(name))
            //     {
            //         var cachedValue = _propertyCache[name];
            //         var serializedValue = prop.GetValue();
                    
            //         // 如果缓存值与序列化值不一致，则重建整个缓存
            //         if (!AreValuesEqual(cachedValue, serializedValue))
            //         {
            //             BuildCache();
            //         }
            //     }
            //     else if (prop != null && !_propertyCache.ContainsKey(name))
            //     {
            //         // 如果属性存在但缓存中没有，则重建缓存
            //         BuildCache();
            //     }
            // }
            // else
            // {
            //     if (_propertyCache == null)
            //         BuildCache();
            // }
#else
            if (_propertyCache == null)
                BuildCache();
#endif
                
            if (_propertyCache.TryGetValue(name, out var value))
            {
                if (value is T typedValue)
                    return typedValue;
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// 设置主题属性值
        /// </summary>
        public void SetThemeProperty(string name, object value)
        {
            if (_propertyCache == null)
                BuildCache();
                
            // 更新缓存
            _propertyCache[name] = value;
            
            // 更新序列化数据
            var prop = themeProperties.Find(p => p.Name == name);
            if (prop == null)
            {
                prop = new ThemeProperty { Name = name };
                themeProperties.Add(prop);
            }
            
            prop.SetValue(value);
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                
                // 在编辑器中，确保缓存与序列化数据同步
                // 重新构建缓存以确保所有值都正确更新
                BuildCache();
            }
#endif
        }
        
        /// <summary>
        /// 获取所有主题属性
        /// </summary>
        public ReadOnlyCollection<ThemeProperty> ThemeProperties => themeProperties.AsReadOnly();
        
        /// <summary>
        /// 获取属性引用的值
        /// </summary>
        public T GetPropertyRefValue<T>(string propertyRef, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(propertyRef))
                return defaultValue;
                
            return GetThemeProperty<T>(propertyRef, defaultValue);
        }

        /// <summary>
        /// 应用属性模板
        /// </summary>
        private void ApplyPropertyTemplate()
        {
            if (propertyTemplate == null)
            {
                _templatePropertyNames.Clear();
                return;
            }

            // 创建模板属性
            var templateProps = propertyTemplate.CreateThemeProperties();
            
            // 更新模板属性名称集合
            _templatePropertyNames.Clear();
            foreach (var prop in templateProps)
            {
                _templatePropertyNames.Add(prop.Name);
            }

            // 合并现有属性（保留用户自定义值）
            var mergedProperties = new List<ThemeProperty>();
            
            // 首先添加模板属性
            foreach (var templateProp in templateProps)
            {
                // 查找是否已存在同名属性
                var existingProp = themeProperties.Find(p => p.Name == templateProp.Name);
                
                if (existingProp != null)
                {
                    // 保留现有属性的值，但确保类型匹配
                    if (existingProp.Type == templateProp.Type)
                    {
                        mergedProperties.Add(existingProp);
                    }
                    else
                    {
                        // 类型不匹配，使用模板默认值但保留名称
                        var newProp = new ThemeProperty
                        {
                            Name = templateProp.Name,
                            Type = templateProp.Type
                        };
                        newProp.SetValue(templateProp.GetValue());
                        mergedProperties.Add(newProp);
                    }
                }
                else
                {
                    // 添加新的模板属性
                    mergedProperties.Add(templateProp);
                }
            }
            
            // 然后添加非模板的自定义属性
            foreach (var customProp in themeProperties)
            {
                if (!_templatePropertyNames.Contains(customProp.Name))
                {
                    mergedProperties.Add(customProp);
                }
            }

            themeProperties = mergedProperties;
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }

        /// <summary>
        /// 更新模板属性名称集合
        /// </summary>
        private void UpdateTemplatePropertyNames()
        {
            _templatePropertyNames.Clear();
            
            if (propertyTemplate != null)
            {
                foreach (var templateProp in propertyTemplate.TemplateProperties)
                {
                    _templatePropertyNames.Add(templateProp.Name);
                }
            }
        }
        
        /// <summary>
        /// 在编辑器模式下监听属性模板的变化
        /// </summary>
#if UNITY_EDITOR
        private void OnPropertyTemplateChanged()
        {
            if (!Application.isPlaying)
            {
                UpdateTemplatePropertyNames();
                BuildCache();
            }
        }
#endif

        /// <summary>
        /// 检查属性是否为模板属性
        /// </summary>
        public bool IsTemplateProperty(string propertyName)
        {
            return _templatePropertyNames.Contains(propertyName);
        }

        /// <summary>
        /// 比较两个值是否相等
        /// </summary>
        private bool AreValuesEqual(object value1, object value2)
        {
            if (value1 == null && value2 == null)
                return true;
            if (value1 == null || value2 == null)
                return false;
            
            // 对于不同类型的值，直接返回false
            if (value1.GetType() != value2.GetType())
                return false;
            
            // 对于基本类型，直接比较
            if (value1.Equals(value2))
                return true;
            
            // 对于复杂类型如Vector2, Vector3, Color, RectOffset等进行特殊处理
            if (value1 is Vector2 v1 && value2 is Vector2 v2)
                return v1 == v2;
            if (value1 is Vector3 v3 && value2 is Vector3 v4)
                return v3 == v4;
            if (value1 is Color c1 && value2 is Color c2)
                return c1 == c2;
            if (value1 is RectOffset r1 && value2 is RectOffset r2)
            {
                // RectOffset需要比较各个分量
                return r1.left == r2.left && r1.right == r2.right &&
                       r1.top == r2.top && r1.bottom == r2.bottom;
            }
            
            return false;
        }

        [System.Serializable]
        public class StyleSetEntry
        {
            public string Id;
            public DeclStyleSet StyleSet;
        }
        
        /// <summary>
        /// 样式集列表
        /// </summary>
        public List<StyleSetEntry> StyleSets => styleSets;
        
        /// <summary>
        /// 主题属性
        /// </summary>
        [System.Serializable]
        public class ThemeProperty
        {
            public string Name;
            public PropertyType Type;
            
            // 根据类型存储值
            public float FloatValue;
            public int IntValue;
            public Color ColorValue;
            public string StringValue;
            public bool BoolValue;
            public Vector2 Vector2Value;
            public Vector3 Vector3Value;
            public RectOffset RectOffsetValue;
            
            /// <summary>
            /// 获取属性值
            /// </summary>
            public object GetValue()
            {
                switch (Type)
                {
                    case PropertyType.Float:
                        return FloatValue;
                    case PropertyType.Int:
                        return IntValue;
                    case PropertyType.Color:
                        return ColorValue;
                    case PropertyType.String:
                        return StringValue;
                    case PropertyType.Boolean:
                        return BoolValue;
                    case PropertyType.Vector2:
                        return Vector2Value;
                    case PropertyType.Vector3:
                        return Vector3Value;
                    case PropertyType.RectOffset:
                        return RectOffsetValue;
                    default:
                        return null;
                }
            }
            
            /// <summary>
            /// 设置属性值
            /// </summary>
            public void SetValue(object value)
            {
                if (value == null)
                    return;
                    
                switch (value)
                {
                    case float f:
                        Type = PropertyType.Float;
                        FloatValue = f;
                        break;
                    case int i:
                        Type = PropertyType.Int;
                        IntValue = i;
                        break;
                    case Color c:
                        Type = PropertyType.Color;
                        ColorValue = c;
                        break;
                    case string s:
                        Type = PropertyType.String;
                        StringValue = s;
                        break;
                    case bool b:
                        Type = PropertyType.Boolean;
                        BoolValue = b;
                        break;
                    case Vector2 v2:
                        Type = PropertyType.Vector2;
                        Vector2Value = v2;
                        break;
                    case Vector3 v3:
                        Type = PropertyType.Vector3;
                        Vector3Value = v3;
                        break;
                    case RectOffset ro:
                        Type = PropertyType.RectOffset;
                        RectOffsetValue = ro;
                        break;
                }
            }
        }
    }
}