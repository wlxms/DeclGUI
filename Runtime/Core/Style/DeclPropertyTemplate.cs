using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 属性模板配置
    /// 定义属性的名称、类型和默认值
    /// </summary>
    [CreateAssetMenu(fileName = "NewDeclPropertyTemplate", menuName = "DeclGUI/Property Template")]
    public class DeclPropertyTemplate : ScriptableObject
    {
        [SerializeField]
        private List<TemplateProperty> templateProperties = new List<TemplateProperty>();

        private void Reset()
        {
            // 在创建新资源时自动添加默认的dark样式属性
            AddDefaultDarkStyleProperties();
        }

        /// <summary>
        /// 获取模板属性列表
        /// </summary>
        public List<TemplateProperty> TemplateProperties => templateProperties;

        /// <summary>
        /// 根据模板属性创建主题属性
        /// </summary>
        public List<DeclTheme.ThemeProperty> CreateThemeProperties()
        {
            var themeProperties = new List<DeclTheme.ThemeProperty>();
            
            foreach (var templateProp in templateProperties)
            {
                var themeProp = new DeclTheme.ThemeProperty
                {
                    Name = templateProp.Name,
                    Type = templateProp.Type
                };
                
                // 设置默认值
                switch (templateProp.Type)
                {
                    case PropertyType.Float:
                        themeProp.FloatValue = templateProp.FloatValue;
                        break;
                    case PropertyType.Int:
                        themeProp.IntValue = templateProp.IntValue;
                        break;
                    case PropertyType.Color:
                        themeProp.ColorValue = templateProp.ColorValue;
                        break;
                    case PropertyType.String:
                        themeProp.StringValue = templateProp.StringValue;
                        break;
                    case PropertyType.Boolean:
                        themeProp.BoolValue = templateProp.BoolValue;
                        break;
                    case PropertyType.Vector2:
                        themeProp.Vector2Value = templateProp.Vector2Value;
                        break;
                    case PropertyType.Vector3:
                        themeProp.Vector3Value = templateProp.Vector3Value;
                        break;
                    case PropertyType.RectOffset:
                        themeProp.RectOffsetValue = templateProp.RectOffsetValue;
                        break;
                }
                
                themeProperties.Add(themeProp);
            }
            
            return themeProperties;
        }

        /// <summary>
        /// 添加默认的dark样式属性
        /// </summary>
        private void AddDefaultDarkStyleProperties()
        {
            templateProperties.Clear();

            // 基础颜色属性
            AddTemplateProperty("backgroundColor", PropertyType.Color, new Color(0.22f, 0.22f, 0.22f), "基础背景颜色");
            AddTemplateProperty("textColor", PropertyType.Color, new Color(0.8f, 0.8f, 0.8f), "基础文本颜色");
            AddTemplateProperty("borderColor", PropertyType.Color, new Color(0.4f, 0.4f, 0.4f), "基础边框颜色");
            AddTemplateProperty("accentColor", PropertyType.Color, new Color(0.0f, 0.47f, 0.84f), "强调和选中状态的颜色");
            
            // 控件状态颜色
            AddTemplateProperty("hoverColor", PropertyType.Color, new Color(0.3f, 0.3f, 0.3f), "鼠标悬停状态的颜色");
            AddTemplateProperty("activeColor", PropertyType.Color, new Color(0.15f, 0.15f, 0.15f), "激活/按下状态的颜色");
            AddTemplateProperty("focusColor", PropertyType.Color, new Color(0.25f, 0.25f, 0.4f), "获得焦点状态的颜色");
            
            // 文本属性
            AddTemplateProperty("fontSize", PropertyType.Int, 12, "基础字体大小");
            AddTemplateProperty("fontSizeSmall", PropertyType.Int, 10, "小号字体大小");
            AddTemplateProperty("fontSizeLarge", PropertyType.Int, 14, "大号字体大小");
            
            // 间距和边框属性
            AddTemplateProperty("padding", PropertyType.RectOffset, new RectOffset(4, 4, 4, 4), "基础内边距");
            AddTemplateProperty("margin", PropertyType.RectOffset, new RectOffset(2, 2, 2, 2), "基础外边距");
            AddTemplateProperty("borderWidth", PropertyType.Float, 1f, "基础边框宽度");
            AddTemplateProperty("borderRadius", PropertyType.Float, 3f, "基础边框圆角半径");
            
            // 特殊控件属性
            AddTemplateProperty("buttonHeight", PropertyType.Float, 24f, "标准按钮高度");
            AddTemplateProperty("textFieldHeight", PropertyType.Float, 20f, "标准文本框高度");
            AddTemplateProperty("sliderHeight", PropertyType.Float, 16f, "标准滑块高度");
            
            // 透明度属性
            AddTemplateProperty("disabledAlpha", PropertyType.Float, 0.5f, "禁用状态的透明度");
            AddTemplateProperty("hoverAlpha", PropertyType.Float, 0.8f, "悬停状态的透明度");
            
            // 动画属性
            AddTemplateProperty("transitionDuration", PropertyType.Float, 0.15f, "状态过渡动画时长");
            AddTemplateProperty("hoverTransitionDuration", PropertyType.Float, 0.1f, "悬停状态过渡动画时长");
        }

        /// <summary>
        /// 添加模板属性辅助方法
        /// </summary>
        private void AddTemplateProperty(string name, PropertyType type, object defaultValue, string description = null)
        {
            var prop = new TemplateProperty
            {
                Name = name,
                Type = type,
                Description = description ?? string.Empty
            };

            // 设置默认值
            switch (type)
            {
                case PropertyType.Float:
                    prop.FloatValue = (float)defaultValue;
                    break;
                case PropertyType.Int:
                    prop.IntValue = (int)defaultValue;
                    break;
                case PropertyType.Color:
                    prop.ColorValue = (Color)defaultValue;
                    break;
                case PropertyType.String:
                    prop.StringValue = (string)defaultValue;
                    break;
                case PropertyType.Boolean:
                    prop.BoolValue = (bool)defaultValue;
                    break;
                case PropertyType.Vector2:
                    prop.Vector2Value = (Vector2)defaultValue;
                    break;
                case PropertyType.Vector3:
                    prop.Vector3Value = (Vector3)defaultValue;
                    break;
                case PropertyType.RectOffset:
                    prop.RectOffsetValue = (RectOffset)defaultValue;
                    break;
            }

            templateProperties.Add(prop);
        }

        /// <summary>
        /// 模板属性定义
        /// </summary>
        [Serializable]
        public class TemplateProperty
        {
            [Tooltip("属性名称（唯一标识符）")]
            public string Name;
            
            [Tooltip("属性类型")]
            public PropertyType Type;
            
            [Tooltip("属性描述（悬停提示）")]
            public string Description;
            
            // 根据类型存储默认值
            [Tooltip("Float类型默认值")]
            public float FloatValue;
            
            [Tooltip("Int类型默认值")]
            public int IntValue;
            
            [Tooltip("Color类型默认值")]
            public Color ColorValue = Color.white;
            
            [Tooltip("String类型默认值")]
            public string StringValue;
            
            [Tooltip("Boolean类型默认值")]
            public bool BoolValue;
            
            [Tooltip("Vector2类型默认值")]
            public Vector2 Vector2Value;
            
            [Tooltip("Vector3类型默认值")]
            public Vector3 Vector3Value;
            
            [Tooltip("RectOffset类型默认值")]
            public RectOffset RectOffsetValue = new RectOffset();
        }
    }
}