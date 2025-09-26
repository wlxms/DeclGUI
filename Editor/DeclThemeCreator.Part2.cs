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
    /// DeclTheme 默认主题创建工具 (第二部分)
    /// 提供创建亮色和深色主题的菜单项功能
    /// </summary>
    public static partial class DeclThemeCreator
    {
        private static void CreateUnityControlStyleSets(string styleSetsPath, DeclTheme theme, bool isLightTheme)
        {
            // 创建基础控件样式集
            var controlStyleSets = new Dictionary<string, Action<DeclStyleSet, bool>>
            {
                { "Button", CreateButtonStyleSet },
                { "Label", CreateLabelStyleSet },
                { "InputField", CreateInputFieldStyleSet },
                { "Toggle", CreateToggleStyleSet },
                { "Slider", CreateSliderStyleSet },
                { "Scrollbar", CreateScrollbarStyleSet },
                { "Dropdown", CreateDropdownStyleSet },
                { "ScrollView", CreateScrollViewStyleSet },
                { "Image", CreateImageStyleSet },
                { "Text", CreateTextStyleSet },
                
                // 新增控件样式集
                { "PrimaryButton", CreatePrimaryButtonStyleSet },
                { "SecondaryButton", CreateSecondaryButtonStyleSet },
                { "TertiaryButton", CreateTertiaryButtonStyleSet },
                { "SuccessButton", CreateSuccessButtonStyleSet },
                { "WarningButton", CreateWarningButtonStyleSet },
                { "DangerButton", CreateDangerButtonStyleSet },
                { "InfoButton", CreateInfoButtonStyleSet },
                { "IconButton", CreateIconButtonStyleSet },
                { "SmallButton", CreateSmallButtonStyleSet },
                { "LargeButton", CreateLargeButtonStyleSet },
                { "BorderedPanel", CreateBorderedPanelStyleSet },
                { "FilledPanel", CreateFilledPanelStyleSet },
                { "SubtleTextField", CreateSubtleTextFieldStyleSet },
                { "BoldLabel", CreateBoldLabelStyleSet },
                { "HelpBoxText", CreateHelpBoxTextStyleSet },
                { "FoldoutHeader", CreateFoldoutHeaderStyleSet },
                { "TreeViewItem", CreateTreeViewItemStyleSet },
                { "ListViewItem", CreateListViewItemStyleSet },
                { "ToolbarSearchField", CreateToolbarSearchFieldStyleSet },
                { "MiniLabel", CreateMiniLabelStyleSet }
            };

            foreach (var kvp in controlStyleSets)
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                kvp.Value(styleSet, isLightTheme);
                
                string styleSetPath = Path.Combine(styleSetsPath, $"{kvp.Key}.asset");
                AssetDatabase.CreateAsset(styleSet, styleSetPath);
                
                theme.RegisterStyleSet(kvp.Key, styleSet);
            }
        }

        private static void CreateMarkdownStyleSets(string styleSetsPath, DeclTheme theme, bool isLightTheme)
        {
            // 创建Markdown样式集
            var markdownStyleSets = new Dictionary<string, Action<DeclStyleSet, bool>>
            {
                { "H1", (styleSet, light) => CreateHeadingStyleSet(styleSet, light, StyleConfig.FontSizeXXXLarge, FontStyle.Bold) },
                { "H2", (styleSet, light) => CreateHeadingStyleSet(styleSet, light, StyleConfig.FontSizeXXLarge, FontStyle.Bold) },
                { "H3", (styleSet, light) => CreateHeadingStyleSet(styleSet, light, StyleConfig.FontSizeXLarge, FontStyle.Bold) },
                { "H4", (styleSet, light) => CreateHeadingStyleSet(styleSet, light, StyleConfig.FontSizeLarge, FontStyle.Bold) },
                { "H5", (styleSet, light) => CreateHeadingStyleSet(styleSet, light, StyleConfig.FontSizeNormal, FontStyle.Bold) },
                { "H6", (styleSet, light) => CreateHeadingStyleSet(styleSet, light, StyleConfig.FontSizeSmall, FontStyle.Bold) },
                { "Paragraph", CreateParagraphStyleSet },
                { "Link", CreateLinkStyleSet },
                { "Code", CreateCodeStyleSet },
                { "Quote", CreateQuoteStyleSet }
            };

            foreach (var kvp in markdownStyleSets)
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                kvp.Value(styleSet, isLightTheme);
                
                string styleSetPath = Path.Combine(styleSetsPath, $"{kvp.Key}.asset");
                AssetDatabase.CreateAsset(styleSet, styleSetPath);
                
                theme.RegisterStyleSet(kvp.Key, styleSet);
            }
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

        // Unity控件样式集创建方法（优化版：优先使用主题属性引用）
        private static void CreateButtonStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            SetStylePropertyWithReference(styleSet, "Padding", StyleConfig.PaddingNormal, "paddingNormal");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值（通常不需要主题引用）
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);

            // 悬停状态（使用主题属性引用）
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            hoverStyle.BorderColor = StyleProperty<Color>.Ref("primaryColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态（使用主题属性引用）
            var activeStyle = new DeclStyle();
            activeStyle.BackgroundColor = StyleProperty<Color>.Ref("activeColor");
            activeStyle.BorderColor = StyleProperty<Color>.Ref("primaryColor");
            activeStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateLabelStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            SetStylePropertyWithReference(styleSet, "Padding", StyleConfig.PaddingSmall, "paddingSmall");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);
        }

        private static void CreateInputFieldStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusSmall, "borderRadiusSmall");
            SetStylePropertyWithReference(styleSet, "Padding", StyleConfig.PaddingNormal, "paddingNormal");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);

            // 焦点状态（使用主题属性引用）
            var focusStyle = new DeclStyle();
            focusStyle.BorderColor = StyleProperty<Color>.Ref("focusColor");
            focusStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Focus, focusStyle);
        }

        private static void CreateToggleStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusSmall, "borderRadiusSmall");
            SetStylePropertyWithReference(styleSet, "Padding", StyleConfig.PaddingSmall, "paddingSmall");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightSmall, "controlHeightSmall");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeSmall, "fontSizeSmall");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);
        }

        private static void CreateSliderStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusSmall, "borderRadiusSmall");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightSmall, "controlHeightSmall");
        }

        private static void CreateScrollbarStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusSmall, "borderRadiusSmall");
        }

        private static void CreateDropdownStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusSmall, "borderRadiusSmall");
            SetStylePropertyWithReference(styleSet, "Padding", StyleConfig.PaddingNormal, "paddingNormal");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);
        }

        private static void CreateScrollViewStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Background : DarkThemeColors.Instance.Background, "backgroundColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            SetStylePropertyWithReference(styleSet, "Padding", StyleConfig.PaddingNormal, "paddingNormal");
        }

        private static void CreateImageStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            
            // 背景色使用直接值（透明色通常不需要主题引用）
            styleSet.BackgroundColor = StyleProperty<Color>.Direct(Color.clear);
        }

        private static void CreateTextStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            SetStylePropertyWithReference(styleSet, "Padding", StyleConfig.PaddingSmall, "paddingSmall");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft);
        }

        // Markdown样式集创建方法（优化版：优先使用主题属性引用）
        private static void CreateHeadingStyleSet(DeclStyleSet styleSet, bool isLightTheme, int fontSize, FontStyle fontStyle)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            
            // 根据字体大小使用不同的主题属性引用
            string fontSizeProperty = GetFontSizePropertyName(fontSize);
            SetStylePropertyWithReference(styleSet, "FontSize", fontSize, fontSizeProperty);
            
            // 字体样式使用直接值（通常不需要主题引用）
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(fontStyle);
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft);
            
            // 内边距和外边距使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(0, 0, 4, 4), "paddingHeading");
            SetStylePropertyWithReference(styleSet, "Margin", new RectOffset(0, 0, 8, 4), "marginHeading");
        }

        private static void CreateParagraphStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft);
            
            // 内边距和外边距使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(0, 0, 2, 2), "paddingParagraph");
            SetStylePropertyWithReference(styleSet, "Margin", new RectOffset(0, 0, 4, 4), "marginParagraph");
        }

        private static void CreateLinkStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Primary : DarkThemeColors.Instance.Primary, "linkColor");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 字体样式和对齐方式使用直接值
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft);

            // 悬停状态（使用主题属性引用）
            var hoverStyle = new DeclStyle();
            hoverStyle.Color = StyleProperty<Color>.Ref("linkHoverColor");
            hoverStyle.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Italic);
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);
        }

        private static void CreateCodeStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusSmall, "borderRadiusSmall");
            SetStylePropertyWithReference(styleSet, "Padding", StyleConfig.PaddingSmall, "paddingSmall");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeSmall, "fontSizeSmall");
            
            // 字体样式和对齐方式使用直接值
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft);
        }

        private static void CreateQuoteStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 使用智能属性设置方法，优先使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.TextSecondary : DarkThemeColors.Instance.TextSecondary, "textSecondaryColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusSmall, "borderRadiusSmall");
            SetStylePropertyWithReference(styleSet, "Padding", StyleConfig.PaddingNormal, "paddingNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 字体样式和对齐方式使用直接值
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Italic);
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft);
            
            // 外边距使用主题属性引用
            SetStylePropertyWithReference(styleSet, "Margin", new RectOffset(8, 0, 4, 4), "marginQuote");
        }

        // 辅助方法：根据字体大小获取对应的主题属性名称
        private static string GetFontSizePropertyName(int fontSize)
        {
            if (fontSize == StyleConfig.FontSizeXXXLarge) return "fontSizeXXXLarge";
            if (fontSize == StyleConfig.FontSizeXXLarge) return "fontSizeXXLarge";
            if (fontSize == StyleConfig.FontSizeXLarge) return "fontSizeXLarge";
            if (fontSize == StyleConfig.FontSizeLarge) return "fontSizeLarge";
            if (fontSize == StyleConfig.FontSizeNormal) return "fontSizeNormal";
            if (fontSize == StyleConfig.FontSizeSmall) return "fontSizeSmall";
            return "fontSizeNormal";
        }

        // 新增控件样式创建方法
        private static void CreatePrimaryButtonStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 主按钮样式 - 使用主色调，突出视觉表现
            SetStylePropertyWithReference(styleSet, "Color", Color.white, null); // 白色文字
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Primary : DarkThemeColors.Instance.Primary, "primaryColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Primary : DarkThemeColors.Instance.Primary, "primaryColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 8, 4, 4), "buttonPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            hoverStyle.BorderColor = StyleProperty<Color>.Ref("primaryColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态
            var activeStyle = new DeclStyle();
            activeStyle.BackgroundColor = StyleProperty<Color>.Ref("activeColor");
            activeStyle.BorderColor = StyleProperty<Color>.Ref("primaryColor");
            activeStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateSecondaryButtonStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 次按钮样式 - 使用轮廓样式，视觉上更低调
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Primary : DarkThemeColors.Instance.Primary, "primaryColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", Color.clear, null); // 透明背景
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Primary : DarkThemeColors.Instance.Primary, "primaryColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 8, 4, 4), "buttonPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            hoverStyle.Color = StyleProperty<Color>.Ref("textColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态
            var activeStyle = new DeclStyle();
            activeStyle.BackgroundColor = StyleProperty<Color>.Ref("activeColor");
            activeStyle.Color = StyleProperty<Color>.Ref("textColor");
            activeStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateTertiaryButtonStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 三级按钮样式 - 类似文本链接
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Primary : DarkThemeColors.Instance.Primary, "primaryColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", Color.clear, null); // 透明背景
            SetStylePropertyWithReference(styleSet, "BorderColor", Color.clear, null); // 透明边框
            SetStylePropertyWithReference(styleSet, "BorderWidth", 0f, null);
            SetStylePropertyWithReference(styleSet, "BorderRadius", 0f, null);
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(4, 4, 2, 2), "smallButtonPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightSmall, "controlHeightSmall");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.Color = StyleProperty<Color>.Ref("textSecondaryColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态
            var activeStyle = new DeclStyle();
            activeStyle.Color = StyleProperty<Color>.Ref("textColor");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateSuccessButtonStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 成功按钮样式 - 使用绿色系
            SetStylePropertyWithReference(styleSet, "Color", Color.white, null);
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Success : DarkThemeColors.Instance.Success, "successColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Success : DarkThemeColors.Instance.Success, "successColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 8, 4, 4), "buttonPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            hoverStyle.BorderColor = StyleProperty<Color>.Ref("successColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态
            var activeStyle = new DeclStyle();
            activeStyle.BackgroundColor = StyleProperty<Color>.Ref("activeColor");
            activeStyle.BorderColor = StyleProperty<Color>.Ref("successColor");
            activeStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateWarningButtonStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 警告按钮样式 - 使用黄色/橙色系
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Warning : DarkThemeColors.Instance.Warning, "warningColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Warning : DarkThemeColors.Instance.Warning, "warningColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 8, 4, 4), "buttonPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            hoverStyle.BorderColor = StyleProperty<Color>.Ref("warningColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态
            var activeStyle = new DeclStyle();
            activeStyle.BackgroundColor = StyleProperty<Color>.Ref("activeColor");
            activeStyle.BorderColor = StyleProperty<Color>.Ref("warningColor");
            activeStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateDangerButtonStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 危险按钮样式 - 使用红色系
            SetStylePropertyWithReference(styleSet, "Color", Color.white, null);
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Error : DarkThemeColors.Instance.Error, "errorColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Error : DarkThemeColors.Instance.Error, "errorColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 8, 4, 4), "buttonPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            hoverStyle.BorderColor = StyleProperty<Color>.Ref("errorColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态
            var activeStyle = new DeclStyle();
            activeStyle.BackgroundColor = StyleProperty<Color>.Ref("activeColor");
            activeStyle.BorderColor = StyleProperty<Color>.Ref("errorColor");
            activeStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateInfoButtonStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 信息按钮样式 - 使用蓝色系
            SetStylePropertyWithReference(styleSet, "Color", Color.white, null);
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Info : DarkThemeColors.Instance.Info, "infoColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Info : DarkThemeColors.Instance.Info, "infoColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 8, 4, 4), "buttonPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            hoverStyle.BorderColor = StyleProperty<Color>.Ref("infoColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态
            var activeStyle = new DeclStyle();
            activeStyle.BackgroundColor = StyleProperty<Color>.Ref("activeColor");
            activeStyle.BorderColor = StyleProperty<Color>.Ref("infoColor");
            activeStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateIconButtonStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 图标按钮样式 - 适合图标或图标配文字
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(6, 6, 3, 3), "buttonPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightSmall, "controlHeightSmall");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeSmall, "fontSizeSmall");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            hoverStyle.BorderColor = StyleProperty<Color>.Ref("primaryColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态
            var activeStyle = new DeclStyle();
            activeStyle.BackgroundColor = StyleProperty<Color>.Ref("activeColor");
            activeStyle.BorderColor = StyleProperty<Color>.Ref("primaryColor");
            activeStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateSmallButtonStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 小尺寸按钮 - 适用于紧凑布局
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusSmall, "borderRadiusSmall");
            SetStylePropertyWithReference(styleSet, "Padding", StyleConfig.PaddingSmall, "paddingSmall");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightSmall, "controlHeightSmall");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeSmall, "fontSizeSmall");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            hoverStyle.BorderColor = StyleProperty<Color>.Ref("primaryColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态
            var activeStyle = new DeclStyle();
            activeStyle.BackgroundColor = StyleProperty<Color>.Ref("activeColor");
            activeStyle.BorderColor = StyleProperty<Color>.Ref("primaryColor");
            activeStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateLargeButtonStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 大尺寸按钮 - 用于特别强调的场合
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthThick, "borderWidthThick");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusLarge, "borderRadiusLarge");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(12, 12, 6, 6), "largeButtonPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightLarge, "controlHeightLarge");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeLarge, "fontSizeLarge");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            hoverStyle.BorderColor = StyleProperty<Color>.Ref("primaryColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态
            var activeStyle = new DeclStyle();
            activeStyle.BackgroundColor = StyleProperty<Color>.Ref("activeColor");
            activeStyle.BorderColor = StyleProperty<Color>.Ref("primaryColor");
            activeStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateBorderedPanelStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 带边框的面板 - 用于内容分组
            SetStylePropertyWithReference(styleSet, "BackgroundColor", Color.clear, null); // 透明背景
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 8, 8, 8), "panelPadding");
            SetStylePropertyWithReference(styleSet, "Margin", StyleConfig.MarginNormal, "marginNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft);
        }

        private static void CreateFilledPanelStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 带背景填充的面板 - 用于突出显示特定区域内容
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusNormal, "borderRadiusNormal");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 8, 8, 8), "panelPadding");
            SetStylePropertyWithReference(styleSet, "Margin", StyleConfig.MarginNormal, "marginNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft);
        }

        private static void CreateSubtleTextFieldStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 样式更柔和的文本输入框
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal * 0.5f, null); // 较细的边框
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusSmall, "borderRadiusSmall");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(4, 4, 3, 3), "subtleTextFieldPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);

            // 焦点状态
            var focusStyle = new DeclStyle();
            focusStyle.BorderColor = StyleProperty<Color>.Ref("focusColor");
            focusStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
            styleSet.AddStyle(PseudoClass.Focus, focusStyle);
        }

        private static void CreateBoldLabelStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 粗体标签 - 用于标题或强调文本
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeLarge, "fontSizeLarge");
            SetStylePropertyWithReference(styleSet, "Padding", StyleConfig.PaddingSmall, "paddingSmall");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold);
        }

        private static void CreateHelpBoxTextStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 专用于帮助框内的文本样式
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.TextSecondary : DarkThemeColors.Instance.TextSecondary, "textSecondaryColor");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeSmall, "fontSizeSmall");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 8, 4, 4), "paddingNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);
        }

        private static void CreateFoldoutHeaderStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 用于可折叠区域的标题
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Background : DarkThemeColors.Instance.Background, "backgroundColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusSmall, "borderRadiusSmall");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 8, 4, 4), "buttonPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);

            // 激活状态
            var activeStyle = new DeclStyle();
            activeStyle.BackgroundColor = StyleProperty<Color>.Ref("activeColor");
            styleSet.AddStyle(PseudoClass.Active, activeStyle);
        }

        private static void CreateTreeViewItemStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 树形视图项的样式
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", Color.clear, null); // 透明背景
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(16, 4, 2, 2), "labelPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightSmall, "controlHeightSmall");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);

            // 激活状态作为选中状态
            var selectedStyle = new DeclStyle();
            selectedStyle.BackgroundColor = StyleProperty<Color>.Ref("primaryColor");
            selectedStyle.Color = StyleProperty<Color>.Direct(Color.white);
            styleSet.AddStyle(PseudoClass.Active, selectedStyle);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);
        }

        private static void CreateListViewItemStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 列表视图项的样式
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", Color.clear, null); // 透明背景
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 8, 4, 4), "labelPadding");
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightNormal, "controlHeightNormal");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeNormal, "fontSizeNormal");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);

            // 激活状态作为选中状态
            var selectedStyle = new DeclStyle();
            selectedStyle.BackgroundColor = StyleProperty<Color>.Ref("primaryColor");
            selectedStyle.Color = StyleProperty<Color>.Direct(Color.white);
            styleSet.AddStyle(PseudoClass.Active, selectedStyle);

            // 悬停状态
            var hoverStyle = new DeclStyle();
            hoverStyle.BackgroundColor = StyleProperty<Color>.Ref("hoverColor");
            styleSet.AddStyle(PseudoClass.Hover, hoverStyle);
        }

        private static void CreateToolbarSearchFieldStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 工具栏内的搜索框样式
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.Text : DarkThemeColors.Instance.Text, "textColor");
            SetStylePropertyWithReference(styleSet, "BackgroundColor", isLightTheme ? LightThemeColors.Instance.Surface : DarkThemeColors.Instance.Surface, "surfaceColor");
            SetStylePropertyWithReference(styleSet, "BorderColor", isLightTheme ? LightThemeColors.Instance.Border : DarkThemeColors.Instance.Border, "borderColor");
            SetStylePropertyWithReference(styleSet, "BorderWidth", StyleConfig.BorderWidthNormal, "borderWidthNormal");
            SetStylePropertyWithReference(styleSet, "BorderRadius", StyleConfig.BorderRadiusLarge, "borderRadiusLarge"); // 圆角搜索框
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(8, 24, 3, 3), "textFieldPadding"); // 右边留空间给搜索图标
            SetStylePropertyWithReference(styleSet, "Height", StyleConfig.ControlHeightSmall, "controlHeightSmall");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeSmall, "fontSizeSmall");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);

            // 焦点状态
            var focusStyle = new DeclStyle();
            focusStyle.BorderColor = StyleProperty<Color>.Ref("focusColor");
            focusStyle.BorderWidth = StyleProperty<float>.Ref("borderWidthThick");
            styleSet.AddStyle(PseudoClass.Focus, focusStyle);
        }

        private static void CreateMiniLabelStyleSet(DeclStyleSet styleSet, bool isLightTheme)
        {
            // 迷你标签 - 用于辅助说明文字
            SetStylePropertyWithReference(styleSet, "Color", isLightTheme ? LightThemeColors.Instance.TextSecondary : DarkThemeColors.Instance.TextSecondary, "textSecondaryColor");
            SetStylePropertyWithReference(styleSet, "FontSize", StyleConfig.FontSizeSmall, "fontSizeSmall");
            SetStylePropertyWithReference(styleSet, "Padding", new RectOffset(2, 2, 1, 1), "labelPadding");
            
            // 对齐方式使用直接值
            styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft);
            styleSet.FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal);
        }
    }
}