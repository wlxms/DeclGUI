using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 扩展的DeclStyle结构体
    /// 支持更多样式属性，保持每帧构建兼容性
    /// </summary>
    public struct DeclStyle : IDeclStyle
    {
        // 基础属性
        public GUIStyle? GUIStyle { get; }
        public Color? Color { get; }
        public float? Width { get; }
        public float? Height { get; }
        public string StyleSetId { get; }
        
        // 新增：背景和边框颜色
        public Color? BackgroundColor { get; }
        public Color? BorderColor { get; }
        
        // 新增：布局属性
        public RectOffset? Padding { get; }
        public RectOffset? Margin { get; }
        
        // 新增：文本属性
        public int? FontSize { get; }
        public FontStyle? FontStyle { get; }
        public TextAnchor? Alignment { get; }
        
        // 新增：边框属性
        public float? BorderWidth { get; }
        public float? BorderRadius { get; }

        /// <summary>
        /// 主要构造函数（支持所有属性）
        /// </summary>
        public DeclStyle(
            GUIStyle? guiStyle = null,
            Color? color = null,
            float? width = null,
            float? height = null,
            string styleSetId = null,
            Color? backgroundColor = null,
            Color? borderColor = null,
            RectOffset? padding = null,
            RectOffset? margin = null,
            int? fontSize = null,
            FontStyle? fontStyle = null,
            TextAnchor? alignment = null,
            float? borderWidth = null,
            float? borderRadius = null)
        {
            GUIStyle = guiStyle;
            Color = color;
            Width = width;
            Height = height;
            StyleSetId = styleSetId;
            BackgroundColor = backgroundColor;
            BorderColor = borderColor;
            Padding = padding;
            Margin = margin;
            FontSize = fontSize;
            FontStyle = fontStyle;
            Alignment = alignment;
            BorderWidth = borderWidth;
            BorderRadius = borderRadius;
        }
        
        /// <summary>
        /// 样式ID专用构造函数
        /// </summary>
        public DeclStyle(string styleSetId) : this()
        {
            StyleSetId = styleSetId;
        }
        
        /// <summary>
        /// 颜色专用构造函数
        /// </summary>
        public DeclStyle(Color color) : this()
        {
            Color = color;
        }
        
        /// <summary>
        /// 尺寸专用构造函数
        /// </summary>
        public DeclStyle(float width, float height) : this()
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 创建只有宽度的样式
        /// </summary>
        public static DeclStyle WithWidth(float width)
        {
            return new DeclStyle(width: width);
        }

        /// <summary>
        /// 创建只有高度的样式
        /// </summary>
        public static DeclStyle WithHeight(float height)
        {
            return new DeclStyle(height: height);
        }

        /// <summary>
        /// 创建指定尺寸的样式
        /// </summary>
        public static DeclStyle WithSize(float width, float height)
        {
            return new DeclStyle(width: width, height: height);
        }

        /// <summary>
        /// 创建只有颜色的样式
        /// </summary>
        public static DeclStyle WithColor(Color color)
        {
            return new DeclStyle(color: color);
        }

        /// <summary>
        /// 创建只有GUI样式的样式
        /// </summary>
        public static DeclStyle WithGUIStyle(GUIStyle guiStyle)
        {
            return new DeclStyle(guiStyle: guiStyle);
        }

        /// <summary>
        /// 设置颜色并返回新的样式实例
        /// </summary>
        public DeclStyle SetColor(Color color)
        {
            return new DeclStyle(GUIStyle, color, Width, Height, StyleSetId,
                BackgroundColor, BorderColor, Padding, Margin, FontSize,
                FontStyle, Alignment, BorderWidth, BorderRadius);
        }

        /// <summary>
        /// 设置宽度并返回新的样式实例
        /// </summary>
        public DeclStyle SetWidth(float width)
        {
            return new DeclStyle(GUIStyle, Color, width, Height, StyleSetId,
                BackgroundColor, BorderColor, Padding, Margin, FontSize,
                FontStyle, Alignment, BorderWidth, BorderRadius);
        }

        /// <summary>
        /// 设置高度并返回新的样式实例
        /// </summary>
        public DeclStyle SetHeight(float height)
        {
            return new DeclStyle(GUIStyle, Color, Width, height, StyleSetId,
                BackgroundColor, BorderColor, Padding, Margin, FontSize,
                FontStyle, Alignment, BorderWidth, BorderRadius);
        }

        /// <summary>
        /// 设置尺寸并返回新的样式实例
        /// </summary>
        public DeclStyle SetSize(float width, float height)
        {
            return new DeclStyle(GUIStyle, Color, width, height, StyleSetId,
                BackgroundColor, BorderColor, Padding, Margin, FontSize,
                FontStyle, Alignment, BorderWidth, BorderRadius);
        }

        public IDeclStyle GetStyleForState(ElementState elementState)
        {
            // 解析样式集引用
            if (!string.IsNullOrEmpty(StyleSetId))
            {
                var theme = DeclThemeManager.CurrentTheme;
                if (theme != null)
                {
                    var styleSet = theme.GetStyleSet(StyleSetId);
                    if (styleSet != null)
                    {
                        return styleSet.GetStyleForState(elementState).Merge(this);
                    }
                }
            }
            
            return this;
        }
        
        public IDeclStyle Merge(IDeclStyle other)
        {
            if (other == null) return this;
            
            return new DeclStyle(
                guiStyle: GUIStyle, // 保持原GUIStyle
                color: other.GetColor() ?? Color,
                width: other.GetWidth() > 0 ? other.GetWidth() : Width,
                height: other.GetHeight() > 0 ? other.GetHeight() : Height,
                styleSetId: StyleSetId, // 保持原样式集ID
                backgroundColor: other.GetBackgroundColor() ?? BackgroundColor,
                borderColor: other.GetBorderColor() ?? BorderColor,
                padding: other.GetPadding() ?? Padding,
                margin: other.GetMargin() ?? Margin,
                fontSize: other.GetFontSize() ?? FontSize,
                fontStyle: other.GetFontStyle() ?? FontStyle,
                alignment: other.GetAlignment() ?? Alignment,
                borderWidth: other.GetBorderWidth() ?? BorderWidth,
                borderRadius: other.GetBorderRadius() ?? BorderRadius
            );
        }
        
        // 接口实现
        public float GetWidth() => Width ?? 0;
        public float GetHeight() => Height ?? 0;
        public Color? GetColor() => Color;
        public Color? GetBackgroundColor() => BackgroundColor;
        public Color? GetBorderColor() => BorderColor;
        public RectOffset GetPadding() => Padding ?? new RectOffset();
        public RectOffset GetMargin() => Margin ?? new RectOffset();
        public int? GetFontSize() => FontSize;
        public FontStyle? GetFontStyle() => FontStyle;
        public TextAnchor? GetAlignment() => Alignment;
        public float? GetBorderWidth() => BorderWidth;
        public float? GetBorderRadius() => BorderRadius;
        public string GetStyleSetId() => StyleSetId;
    }
}