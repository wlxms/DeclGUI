using UnityEngine;
using UnityEditor;
using DeclGUI.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace DeclGUI.Editor
{
    /// <summary>
    /// DeclTheme 默认主题创建工具 (第一部分)
    /// 提供创建亮色和深色主题的菜单项功能
    /// </summary>
    public static partial class DeclThemeCreator
    {
        /// <summary>
        /// 亮色主题配色方案
        /// </summary>
        public class LightThemeColors
        {
            public static readonly LightThemeColors Instance = new LightThemeColors();
            
            // 基础颜色
            public readonly Color Background = new Color(0.95f, 0.95f, 0.95f); // 浅灰背景
            public readonly Color Surface = Color.white; // 白色表面
            public readonly Color Primary = new Color(0.0f, 0.47f, 0.84f); // 蓝色主色调
            public readonly Color Secondary = new Color(0.3f, 0.3f, 0.3f); // 深灰次要色
            public readonly Color Text = new Color(0.1f, 0.1f, 0.1f); // 深灰文本
            public readonly Color TextSecondary = new Color(0.4f, 0.4f, 0.4f); // 中灰次要文本
            public readonly Color Border = new Color(0.8f, 0.8f, 0.8f); // 浅灰边框
            
            // 状态颜色
            public readonly Color Hover = new Color(0.9f, 0.9f, 0.9f); // 悬停浅灰
            public readonly Color Active = new Color(0.8f, 0.8f, 0.8f); // 激活中灰
            public readonly Color Focus = new Color(0.7f, 0.85f, 1.0f); // 焦点浅蓝
            public readonly Color Disabled = new Color(0.7f, 0.7f, 0.7f); // 禁用灰
            
            // 功能颜色
            public readonly Color Success = new Color(0.2f, 0.7f, 0.3f); // 成功绿
            public readonly Color Warning = new Color(1.0f, 0.8f, 0.2f); // 警告黄
            public readonly Color Error = new Color(0.9f, 0.3f, 0.3f); // 错误红
            public readonly Color Info = new Color(0.2f, 0.6f, 0.9f); // 信息蓝
        }

        /// <summary>
        /// 深色主题配色方案
        /// </summary>
        public class DarkThemeColors
        {
            public static readonly DarkThemeColors Instance = new DarkThemeColors();
            
            // 基础颜色
            public readonly Color Background = new Color(0.12f, 0.12f, 0.12f); // 深灰背景
            public readonly Color Surface = new Color(0.18f, 0.18f, 0.18f); // 表面深灰
            public readonly Color Primary = new Color(0.0f, 0.6f, 1.0f); // 亮蓝色主色调
            public readonly Color Secondary = new Color(0.7f, 0.7f, 0.7f); // 浅灰次要色
            public readonly Color Text = new Color(0.9f, 0.9f, 0.9f); // 浅灰文本
            public readonly Color TextSecondary = new Color(0.6f, 0.6f, 0.6f); // 中灰次要文本
            public readonly Color Border = new Color(0.3f, 0.3f, 0.3f); // 深灰边框
            
            // 状态颜色
            public readonly Color Hover = new Color(0.25f, 0.25f, 0.25f); // 悬停深灰
            public readonly Color Active = new Color(0.35f, 0.35f, 0.35f); // 激活中灰
            public readonly Color Focus = new Color(0.2f, 0.4f, 0.6f); // 焦点深蓝
            public readonly Color Disabled = new Color(0.4f, 0.4f, 0.4f); // 禁用灰
            
            // 功能颜色
            public readonly Color Success = new Color(0.3f, 0.8f, 0.4f); // 成功亮绿
            public readonly Color Warning = new Color(1.0f, 0.9f, 0.3f); // 警告亮黄
            public readonly Color Error = new Color(1.0f, 0.4f, 0.4f); // 错误亮红
            public readonly Color Info = new Color(0.3f, 0.7f, 1.0f); // 信息亮蓝
        }

        /// <summary>
        /// 通用样式配置
        /// </summary>
        public static class StyleConfig
        {
            // 字体大小
            public static readonly int FontSizeSmall = 10;
            public static readonly int FontSizeNormal = 12;
            public static readonly int FontSizeLarge = 14;
            public static readonly int FontSizeXLarge = 16;
            public static readonly int FontSizeXXLarge = 18;
            public static readonly int FontSizeXXXLarge = 24;

            // 间距
            public static readonly RectOffset PaddingSmall = new RectOffset(4, 4, 4, 4);
            public static readonly RectOffset PaddingNormal = new RectOffset(8, 8, 8, 8);
            public static readonly RectOffset PaddingLarge = new RectOffset(12, 12, 12, 12);
            
            public static readonly RectOffset MarginSmall = new RectOffset(2, 2, 2, 2);
            public static readonly RectOffset MarginNormal = new RectOffset(4, 4, 4, 4);
            public static readonly RectOffset MarginLarge = new RectOffset(8, 8, 8, 8);

            // 边框
            public static readonly float BorderWidthNormal = 1f;
            public static readonly float BorderWidthThick = 2f;
            public static readonly float BorderRadiusSmall = 2f;
            public static readonly float BorderRadiusNormal = 4f;
            public static readonly float BorderRadiusLarge = 8f;

            // 控件高度
            public static readonly float ControlHeightSmall = 20f;
            public static readonly float ControlHeightNormal = 24f;
            public static readonly float ControlHeightLarge = 32f;
        }

        [MenuItem("Assets/Create/DeclGUI/主题/创建亮色主题", false, 200)]
        public static void CreateLightTheme()
        {
            CreateTheme("LightTheme", true);
        }

        [MenuItem("Assets/Create/DeclGUI/主题/创建深色主题", false, 201)]
        public static void CreateDarkTheme()
        {
            CreateTheme("DarkTheme", false);
        }

        [MenuItem("Tools/DeclGUI/创建默认主题资源", false, 300)]
        public static void CreateDefaultThemes()
        {
            string selectedPath = GetSelectedPathOrFallback();
            
            // 创建亮色主题
            CreateThemeAtPath(Path.Combine(selectedPath, "LightTheme"), "LightTheme", true);
            
            // 创建深色主题
            CreateThemeAtPath(Path.Combine(selectedPath, "DarkTheme"), "DarkTheme", false);
            
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("创建完成", "默认亮色和深色主题资源已创建完成", "确定");
        }

        private static void CreateTheme(string themeName, bool isLightTheme)
        {
            string selectedPath = GetSelectedPathOrFallback();
            CreateThemeAtPath(selectedPath, themeName, isLightTheme);
        }

        public static void CreateThemeAtPath(string directoryPath, string themeName, bool isLightTheme)
        {
            // 确保目录存在
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 创建主题文件
            var theme = ScriptableObject.CreateInstance<DeclTheme>();
            string themePath = Path.Combine(directoryPath, $"{themeName}.asset");
            AssetDatabase.CreateAsset(theme, themePath);

            // 创建样式集目录
            string styleSetsPath = Path.Combine(directoryPath, "StyleSets");
            if (!Directory.Exists(styleSetsPath))
            {
                Directory.CreateDirectory(styleSetsPath);
            }

            // 创建属性模板
            var propertyTemplate = CreatePropertyTemplate(themeName, isLightTheme);
            string templatePath = Path.Combine(directoryPath, $"{themeName}PropertyTemplate.asset");
            AssetDatabase.CreateAsset(propertyTemplate, templatePath);
            theme.PropertyTemplate = propertyTemplate;

            // 创建Unity基础控件样式集
            CreateUnityControlStyleSets(styleSetsPath, theme, isLightTheme);

            // 创建Markdown样式集
            CreateMarkdownStyleSets(styleSetsPath, theme, isLightTheme);

            // 保存主题
            EditorUtility.SetDirty(theme);
            AssetDatabase.SaveAssets();

            // 选中创建的主题
            Selection.activeObject = theme;
            EditorUtility.FocusProjectWindow();
        }

        private static DeclPropertyTemplate CreatePropertyTemplate(string themeName, bool isLightTheme)
        {
            var template = ScriptableObject.CreateInstance<DeclPropertyTemplate>();
            
            // 清空默认属性
            template.TemplateProperties.Clear();

            if (isLightTheme)
            {
                // 亮色主题属性
                AddTemplateProperty(template, "backgroundColor", PropertyType.Color, LightThemeColors.Instance.Background, "基础背景颜色");
                AddTemplateProperty(template, "surfaceColor", PropertyType.Color, LightThemeColors.Instance.Surface, "表面颜色");
                AddTemplateProperty(template, "primaryColor", PropertyType.Color, LightThemeColors.Instance.Primary, "主色调");
                AddTemplateProperty(template, "secondaryColor", PropertyType.Color, LightThemeColors.Instance.Secondary, "次要色调");
                AddTemplateProperty(template, "textColor", PropertyType.Color, LightThemeColors.Instance.Text, "基础文本颜色");
                AddTemplateProperty(template, "textSecondaryColor", PropertyType.Color, LightThemeColors.Instance.TextSecondary, "次要文本颜色");
                AddTemplateProperty(template, "borderColor", PropertyType.Color, LightThemeColors.Instance.Border, "基础边框颜色");
                
                // 状态颜色
                AddTemplateProperty(template, "hoverColor", PropertyType.Color, LightThemeColors.Instance.Hover, "鼠标悬停状态的颜色");
                AddTemplateProperty(template, "activeColor", PropertyType.Color, LightThemeColors.Instance.Active, "激活/按下状态的颜色");
                AddTemplateProperty(template, "focusColor", PropertyType.Color, LightThemeColors.Instance.Focus, "获得焦点状态的颜色");
                AddTemplateProperty(template, "disabledColor", PropertyType.Color, LightThemeColors.Instance.Disabled, "禁用状态的颜色");
                
                // 功能颜色
                AddTemplateProperty(template, "successColor", PropertyType.Color, LightThemeColors.Instance.Success, "成功状态颜色");
                AddTemplateProperty(template, "warningColor", PropertyType.Color, LightThemeColors.Instance.Warning, "警告状态颜色");
                AddTemplateProperty(template, "errorColor", PropertyType.Color, LightThemeColors.Instance.Error, "错误状态颜色");
                AddTemplateProperty(template, "infoColor", PropertyType.Color, LightThemeColors.Instance.Info, "信息状态颜色");
            }
            else
            {
                // 深色主题属性
                AddTemplateProperty(template, "backgroundColor", PropertyType.Color, DarkThemeColors.Instance.Background, "基础背景颜色");
                AddTemplateProperty(template, "surfaceColor", PropertyType.Color, DarkThemeColors.Instance.Surface, "表面颜色");
                AddTemplateProperty(template, "primaryColor", PropertyType.Color, DarkThemeColors.Instance.Primary, "主色调");
                AddTemplateProperty(template, "secondaryColor", PropertyType.Color, DarkThemeColors.Instance.Secondary, "次要色调");
                AddTemplateProperty(template, "textColor", PropertyType.Color, DarkThemeColors.Instance.Text, "基础文本颜色");
                AddTemplateProperty(template, "textSecondaryColor", PropertyType.Color, DarkThemeColors.Instance.TextSecondary, "次要文本颜色");
                AddTemplateProperty(template, "borderColor", PropertyType.Color, DarkThemeColors.Instance.Border, "基础边框颜色");
                
                // 状态颜色
                AddTemplateProperty(template, "hoverColor", PropertyType.Color, DarkThemeColors.Instance.Hover, "鼠标悬停状态的颜色");
                AddTemplateProperty(template, "activeColor", PropertyType.Color, DarkThemeColors.Instance.Active, "激活/按下状态的颜色");
                AddTemplateProperty(template, "focusColor", PropertyType.Color, DarkThemeColors.Instance.Focus, "获得焦点状态的颜色");
                AddTemplateProperty(template, "disabledColor", PropertyType.Color, DarkThemeColors.Instance.Disabled, "禁用状态的颜色");
                
                // 功能颜色
                AddTemplateProperty(template, "successColor", PropertyType.Color, DarkThemeColors.Instance.Success, "成功状态颜色");
                AddTemplateProperty(template, "warningColor", PropertyType.Color, DarkThemeColors.Instance.Warning, "警告状态颜色");
                AddTemplateProperty(template, "errorColor", PropertyType.Color, DarkThemeColors.Instance.Error, "错误状态颜色");
                AddTemplateProperty(template, "infoColor", PropertyType.Color, DarkThemeColors.Instance.Info, "信息状态颜色");
            }

            // 通用属性（不依赖主题类型）
            // 文本属性
            AddTemplateProperty(template, "fontSizeSmall", PropertyType.Int, StyleConfig.FontSizeSmall, "小号字体大小");
            AddTemplateProperty(template, "fontSizeNormal", PropertyType.Int, StyleConfig.FontSizeNormal, "标准字体大小");
            AddTemplateProperty(template, "fontSizeLarge", PropertyType.Int, StyleConfig.FontSizeLarge, "大号字体大小");
            AddTemplateProperty(template, "fontSizeXLarge", PropertyType.Int, StyleConfig.FontSizeXLarge, "超大号字体大小");
            AddTemplateProperty(template, "fontSizeXXLarge", PropertyType.Int, StyleConfig.FontSizeXXLarge, "特大号字体大小");
            AddTemplateProperty(template, "fontSizeXXXLarge", PropertyType.Int, StyleConfig.FontSizeXXXLarge, "最大号字体大小");
            
            // 间距属性
            AddTemplateProperty(template, "paddingSmall", PropertyType.RectOffset, StyleConfig.PaddingSmall, "小内边距");
            AddTemplateProperty(template, "paddingNormal", PropertyType.RectOffset, StyleConfig.PaddingNormal, "标准内边距");
            AddTemplateProperty(template, "paddingLarge", PropertyType.RectOffset, StyleConfig.PaddingLarge, "大内边距");
            
            AddTemplateProperty(template, "marginSmall", PropertyType.RectOffset, StyleConfig.MarginSmall, "小外边距");
            AddTemplateProperty(template, "marginNormal", PropertyType.RectOffset, StyleConfig.MarginNormal, "标准外边距");
            AddTemplateProperty(template, "marginLarge", PropertyType.RectOffset, StyleConfig.MarginLarge, "大外边距");
            
            // Markdown专用间距属性
            AddTemplateProperty(template, "paddingHeading", PropertyType.RectOffset, new RectOffset(0, 0, 4, 4), "标题内边距");
            AddTemplateProperty(template, "marginHeading", PropertyType.RectOffset, new RectOffset(0, 0, 8, 4), "标题外边距");
            AddTemplateProperty(template, "paddingParagraph", PropertyType.RectOffset, new RectOffset(0, 0, 2, 2), "段落内边距");
            AddTemplateProperty(template, "marginParagraph", PropertyType.RectOffset, new RectOffset(0, 0, 4, 4), "段落外边距");
            AddTemplateProperty(template, "marginQuote", PropertyType.RectOffset, new RectOffset(8, 0, 4, 4), "引用外边距");
            
            // 边框属性
            AddTemplateProperty(template, "borderWidthNormal", PropertyType.Float, StyleConfig.BorderWidthNormal, "标准边框宽度");
            AddTemplateProperty(template, "borderWidthThick", PropertyType.Float, StyleConfig.BorderWidthThick, "粗边框宽度");
            AddTemplateProperty(template, "borderRadiusSmall", PropertyType.Float, StyleConfig.BorderRadiusSmall, "小圆角半径");
            AddTemplateProperty(template, "borderRadiusNormal", PropertyType.Float, StyleConfig.BorderRadiusNormal, "标准圆角半径");
            AddTemplateProperty(template, "borderRadiusLarge", PropertyType.Float, StyleConfig.BorderRadiusLarge, "大圆角半径");
            
            // 控件高度
            AddTemplateProperty(template, "controlHeightSmall", PropertyType.Float, StyleConfig.ControlHeightSmall, "小控件高度");
            AddTemplateProperty(template, "controlHeightNormal", PropertyType.Float, StyleConfig.ControlHeightNormal, "标准控件高度");
            AddTemplateProperty(template, "controlHeightLarge", PropertyType.Float, StyleConfig.ControlHeightLarge, "大控件高度");
            
            // Markdown专用属性
            AddTemplateProperty(template, "linkColor", PropertyType.Color, isLightTheme ? LightThemeColors.Instance.Primary : DarkThemeColors.Instance.Primary, "链接颜色");
            AddTemplateProperty(template, "linkHoverColor", PropertyType.Color, isLightTheme ? LightThemeColors.Instance.Secondary : new Color(0.8f, 0.8f, 0.8f), "链接悬停颜色");
            
            // 按钮专用内边距属性
            AddTemplateProperty(template, "buttonPadding", PropertyType.RectOffset, new RectOffset(8, 8, 4, 4), "按钮内边距");
            AddTemplateProperty(template, "smallButtonPadding", PropertyType.RectOffset, new RectOffset(4, 4, 2, 2), "小按钮内边距");
            AddTemplateProperty(template, "largeButtonPadding", PropertyType.RectOffset, new RectOffset(12, 12, 6, 6), "大按钮内边距");
            
            // 面板专用内边距属性
            AddTemplateProperty(template, "panelPadding", PropertyType.RectOffset, new RectOffset(8, 8, 8, 8), "面板内边距");
            
            // 输入框专用内边距属性
            AddTemplateProperty(template, "textFieldPadding", PropertyType.RectOffset, new RectOffset(6, 6, 4, 4), "文本输入框内边距");
            AddTemplateProperty(template, "subtleTextFieldPadding", PropertyType.RectOffset, new RectOffset(4, 4, 3, 3), "柔和文本输入框内边距");
            
            // 标签专用内边距属性
            AddTemplateProperty(template, "labelPadding", PropertyType.RectOffset, new RectOffset(2, 2, 1, 1), "标签内边距");

            return template;
        }

        private static void AddTemplateProperty(DeclPropertyTemplate template, string name, PropertyType type, object defaultValue, string description)
        {
            var prop = new DeclPropertyTemplate.TemplateProperty
            {
                Name = name,
                Type = type,
                Description = description
            };

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

            template.TemplateProperties.Add(prop);
        }

        /// <summary>
        /// 智能属性设置方法：优先使用主题属性引用，其次使用直接值
        /// </summary>
        private static void SetStylePropertyWithReference<T>(DeclStyleSet styleSet, string propertyName, T directValue, string themePropertyName = null)
        {
            if (!string.IsNullOrEmpty(themePropertyName))
            {
                // 使用主题属性引用
                SetPropertyRef(styleSet, propertyName, themePropertyName);
            }
            else
            {
                // 使用直接值
                SetPropertyDirect(styleSet, propertyName, directValue);
            }
        }

        /// <summary>
        /// 设置属性引用
        /// </summary>
        private static void SetPropertyRef(DeclStyleSet styleSet, string propertyName, string themePropertyName)
        {
            switch (propertyName.ToLower())
            {
                case "color":
                    styleSet.Color = StyleProperty<Color>.Ref(themePropertyName);
                    break;
                case "backgroundcolor":
                    styleSet.BackgroundColor = StyleProperty<Color>.Ref(themePropertyName);
                    break;
                case "bordercolor":
                    styleSet.BorderColor = StyleProperty<Color>.Ref(themePropertyName);
                    break;
                case "borderwidth":
                    styleSet.BorderWidth = StyleProperty<float>.Ref(themePropertyName);
                    break;
                case "borderradius":
                    styleSet.BorderRadius = StyleProperty<float>.Ref(themePropertyName);
                    break;
                case "padding":
                    styleSet.Padding = StyleProperty<RectOffset>.Ref(themePropertyName);
                    break;
                case "margin":
                    styleSet.Margin = StyleProperty<RectOffset>.Ref(themePropertyName);
                    break;
                case "fontsize":
                    styleSet.FontSize = StyleProperty<int>.Ref(themePropertyName);
                    break;
                case "fontstyle":
                    styleSet.FontStyle = StyleProperty<FontStyle>.Ref(themePropertyName);
                    break;
                case "alignment":
                    styleSet.Alignment = StyleProperty<TextAnchor>.Ref(themePropertyName);
                    break;
                case "height":
                    styleSet.Height = StyleProperty<float>.Ref(themePropertyName);
                    break;
                case "width":
                    styleSet.Width = StyleProperty<float>.Ref(themePropertyName);
                    break;
                default:
                    Debug.LogWarning($"未知的属性名称: {propertyName}");
                    break;
            }
        }

        /// <summary>
        /// 设置直接值属性
        /// </summary>
        private static void SetPropertyDirect<T>(DeclStyleSet styleSet, string propertyName, T directValue)
        {
            switch (propertyName.ToLower())
            {
                case "color":
                    styleSet.Color = StyleProperty<Color>.Direct((Color)(object)directValue);
                    break;
                case "backgroundcolor":
                    styleSet.BackgroundColor = StyleProperty<Color>.Direct((Color)(object)directValue);
                    break;
                case "bordercolor":
                    styleSet.BorderColor = StyleProperty<Color>.Direct((Color)(object)directValue);
                    break;
                case "borderwidth":
                    styleSet.BorderWidth = StyleProperty<float>.Direct((float)(object)directValue);
                    break;
                case "borderradius":
                    styleSet.BorderRadius = StyleProperty<float>.Direct((float)(object)directValue);
                    break;
                case "padding":
                    styleSet.Padding = StyleProperty<RectOffset>.Direct((RectOffset)(object)directValue);
                    break;
                case "margin":
                    styleSet.Margin = StyleProperty<RectOffset>.Direct((RectOffset)(object)directValue);
                    break;
                case "fontsize":
                    styleSet.FontSize = StyleProperty<int>.Direct((int)(object)directValue);
                    break;
                case "fontstyle":
                    styleSet.FontStyle = StyleProperty<FontStyle>.Direct((FontStyle)(object)directValue);
                    break;
                case "alignment":
                    styleSet.Alignment = StyleProperty<TextAnchor>.Direct((TextAnchor)(object)directValue);
                    break;
                case "height":
                    styleSet.Height = StyleProperty<float>.Direct((float)(object)directValue);
                    break;
                case "width":
                    styleSet.Width = StyleProperty<float>.Direct((float)(object)directValue);
                    break;
                default:
                    Debug.LogWarning($"未知的属性名称: {propertyName}");
                    break;
            }
        }
    }
}