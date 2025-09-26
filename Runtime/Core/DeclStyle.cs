using System;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 扩展的DeclStyle结构体
    /// 支持更多样式属性，保持每帧构建兼容性，支持序列化
    /// 使用统一的StyleProperty<T>来封装属性值，移除GUIStyle字段
    /// </summary>
    [Serializable]
    public struct DeclStyle : ISerializableDeclStyle, System.IEquatable<IDeclStyle>
    {
        public bool Equals(IDeclStyle other)
        {
            if (other == null) return false;
            return this.Color.Equals(other.Color)
                && this.Width.Equals(other.Width)
                && this.Height.Equals(other.Height)
                && this.BackgroundColor.Equals(other.BackgroundColor)
                && this.BorderColor.Equals(other.BorderColor)
                && this.Padding.Equals(other.Padding)
                && this.Margin.Equals(other.Margin)
                && this.FontSize.Equals(other.FontSize)
                && this.FontStyle.Equals(other.FontStyle)
                && this.Alignment.Equals(other.Alignment)
                && this.BorderWidth.Equals(other.BorderWidth)
                && this.BorderRadius.Equals(other.BorderRadius)
                && string.Equals(this.StyleSetId, other.StyleSetId);
        }
        // 基础属性
        [SerializeField] private SerializableRefNullable<string> _styleSetId;
        [SerializeField] private StyleProperty<Color> _color;
        [SerializeField] private StyleProperty<float> _width;
        [SerializeField] private StyleProperty<float> _height;

        // 新增：背景和边框颜色
        [SerializeField] private StyleProperty<Color> _backgroundColor;
        [SerializeField] private StyleProperty<Color> _borderColor;

        // 新增：布局属性
        [SerializeField] private StyleProperty<RectOffset> _padding;
        [SerializeField] private StyleProperty<RectOffset> _margin;

        // 新增：文本属性
        [SerializeField] private StyleProperty<int> _fontSize;
        [SerializeField] private StyleProperty<FontStyle> _fontStyle;
        [SerializeField] private StyleProperty<TextAnchor> _alignment;

        // 新增：边框属性
        [SerializeField] private StyleProperty<float> _borderWidth;
        [SerializeField] private StyleProperty<float> _borderRadius;

        // ISerializableDeclStyle接口实现
        public SerializableRefNullable<string> StyleSetId { get => _styleSetId; set => _styleSetId = value; }
        public StyleProperty<Color> Color { get => _color; set => _color = value; }
        public StyleProperty<float> Width { get => _width; set => _width = value; }
        public StyleProperty<float> Height { get => _height; set => _height = value; }
        public StyleProperty<Color> BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }
        public StyleProperty<Color> BorderColor { get => _borderColor; set => _borderColor = value; }
        public StyleProperty<RectOffset> Padding { get => _padding; set => _padding = value; }
        public StyleProperty<RectOffset> Margin { get => _margin; set => _margin = value; }
        public StyleProperty<int> FontSize { get => _fontSize; set => _fontSize = value; }
        public StyleProperty<FontStyle> FontStyle { get => _fontStyle; set => _fontStyle = value; }
        public StyleProperty<TextAnchor> Alignment { get => _alignment; set => _alignment = value; }
        public StyleProperty<float> BorderWidth { get => _borderWidth; set => _borderWidth = value; }
        public StyleProperty<float> BorderRadius { get => _borderRadius; set => _borderRadius = value; }

        /// <summary>
        /// 主要构造函数（支持所有属性）
        /// </summary>
        public DeclStyle(
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
            // 初始化所有字段
            _styleSetId = styleSetId;
            _color = color.HasValue ? StyleProperty<Color>.Direct(color.Value) : StyleProperty<Color>.None();
            _width = width.HasValue ? StyleProperty<float>.Direct(width.Value) : StyleProperty<float>.None();
            _height = height.HasValue ? StyleProperty<float>.Direct(height.Value) : StyleProperty<float>.None();
            _backgroundColor = backgroundColor.HasValue ? StyleProperty<Color>.Direct(backgroundColor.Value) : StyleProperty<Color>.None();
            _borderColor = borderColor.HasValue ? StyleProperty<Color>.Direct(borderColor.Value) : StyleProperty<Color>.None();
            _padding = padding != null ? StyleProperty<RectOffset>.Direct(padding) : StyleProperty<RectOffset>.None();
            _margin = margin != null ? StyleProperty<RectOffset>.Direct(margin) : StyleProperty<RectOffset>.None();
            _fontSize = fontSize.HasValue ? StyleProperty<int>.Direct(fontSize.Value) : StyleProperty<int>.None();
            _fontStyle = fontStyle.HasValue ? StyleProperty<FontStyle>.Direct(fontStyle.Value) : StyleProperty<FontStyle>.None();
            _alignment = alignment.HasValue ? StyleProperty<TextAnchor>.Direct(alignment.Value) : StyleProperty<TextAnchor>.None();
            _borderWidth = borderWidth.HasValue ? StyleProperty<float>.Direct(borderWidth.Value) : StyleProperty<float>.None();
            _borderRadius = borderRadius.HasValue ? StyleProperty<float>.Direct(borderRadius.Value) : StyleProperty<float>.None();
        }

        /// <summary>
        /// 样式ID专用构造函数
        /// </summary>
        public DeclStyle(string styleSetId) : this()
        {
            _styleSetId = styleSetId;
        }

        /// <summary>
        /// 颜色专用构造函数
        /// </summary>
        public DeclStyle(Color color) : this()
        {
            _color = StyleProperty<Color>.Direct(color);
        }

        /// <summary>
        /// 尺寸专用构造函数
        /// </summary>
        public DeclStyle(float width, float height) : this()
        {
            _width = StyleProperty<float>.Direct(width);
            _height = StyleProperty<float>.Direct(height);
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
        /// 创建只有GUI样式的样式（兼容性方法，不实际使用GUIStyle）
        /// </summary>
        public static DeclStyle WithGUIStyle(GUIStyle guiStyle)
        {
            // 为了向后兼容，但不实际使用guiStyle参数
            return new DeclStyle();
        }

        /// <summary>
        /// 设置颜色并返回新的样式实例
        /// </summary>
        public DeclStyle SetColor(Color color)
        {
            var result = this;
            result._color = StyleProperty<Color>.Direct(color);
            return result;
        }

        /// <summary>
        /// 设置宽度并返回新的样式实例
        /// </summary>
        public DeclStyle SetWidth(float width)
        {
            var result = this;
            result._width = StyleProperty<float>.Direct(width);
            return result;
        }

        /// <summary>
        /// 设置高度并返回新的样式实例
        /// </summary>
        public DeclStyle SetHeight(float height)
        {
            var result = this;
            result._height = StyleProperty<float>.Direct(height);
            return result;
        }

        /// <summary>
        /// 设置尺寸并返回新的样式实例
        /// </summary>
        public DeclStyle SetSize(float width, float height)
        {
            var result = this;
            result._width = StyleProperty<float>.Direct(width);
            result._height = StyleProperty<float>.Direct(height);
            return result;
        }

        /// <summary>
        /// 设置背景颜色并返回新的样式实例
        /// </summary>
        public DeclStyle SetBackgroundColor(Color backgroundColor)
        {
            var result = this;
            result._backgroundColor = StyleProperty<Color>.Direct(backgroundColor);
            return result;
        }

        /// <summary>
        /// 设置边框颜色并返回新的样式实例
        /// </summary>
        public DeclStyle SetBorderColor(Color borderColor)
        {
            var result = this;
            result._borderColor = StyleProperty<Color>.Direct(borderColor);
            return result;
        }

        /// <summary>
        /// 设置内边距并返回新的样式实例
        /// </summary>
        public DeclStyle SetPadding(RectOffset padding)
        {
            var result = this;
            result._padding = StyleProperty<RectOffset>.Direct(padding);
            return result;
        }

        /// <summary>
        /// 设置外边距并返回新的样式实例
        /// </summary>
        public DeclStyle SetMargin(RectOffset margin)
        {
            var result = this;
            result._margin = StyleProperty<RectOffset>.Direct(margin);
            return result;
        }

        /// <summary>
        /// 设置字体大小并返回新的样式实例
        /// </summary>
        public DeclStyle SetFontSize(int fontSize)
        {
            var result = this;
            result._fontSize = StyleProperty<int>.Direct(fontSize);
            return result;
        }

        /// <summary>
        /// 设置字体样式并返回新的样式实例
        /// </summary>
        public DeclStyle SetFontStyle(FontStyle fontStyle)
        {
            var result = this;
            result._fontStyle = StyleProperty<FontStyle>.Direct(fontStyle);
            return result;
        }

        /// <summary>
        /// 设置文本对齐方式并返回新的样式实例
        /// </summary>
        public DeclStyle SetAlignment(TextAnchor alignment)
        {
            var result = this;
            result._alignment = StyleProperty<TextAnchor>.Direct(alignment);
            return result;
        }

        /// <summary>
        /// 设置边框宽度并返回新的样式实例
        /// </summary>
        public DeclStyle SetBorderWidth(float borderWidth)
        {
            var result = this;
            result._borderWidth = StyleProperty<float>.Direct(borderWidth);
            return result;
        }

        /// <summary>
        /// 设置边框半径并返回新的样式实例
        /// </summary>
        public DeclStyle SetBorderRadius(float borderRadius)
        {
            var result = this;
            result._borderRadius = StyleProperty<float>.Direct(borderRadius);
            return result;
        }

        public IDeclStyle GetStyleForState(IElementState elementState)
        {
            // 解析样式集引用
            if (_styleSetId.HasValue && !string.IsNullOrEmpty(_styleSetId.Value))
            {
                var theme = DeclThemeManager.CurrentTheme;
                if (theme != null)
                {
                    var styleSet = theme.GetStyleSet(_styleSetId.Value);
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

            // Debug.Log($"{other.BackgroundColor } with {_backgroundColor}");

            return new DeclStyle()
            {
                Color = other.Color ?? _color,
                Width = other.Width ?? _width,
                Height = other.Height ?? _height,
                BackgroundColor = other.BackgroundColor ?? _backgroundColor,
                BorderColor = other.BorderColor ?? _borderColor,
                Padding = other.Padding ?? _padding,
                Margin = other.Margin ?? _margin,
                FontSize = other.FontSize ?? _fontSize,
                FontStyle = other.FontStyle ?? _fontStyle,
                Alignment = other.Alignment ?? _alignment,
                BorderWidth = other.BorderWidth ?? _borderWidth,
                BorderRadius = other.BorderRadius ?? _borderRadius,
                StyleSetId = other.StyleSetId ?? _styleSetId,
            };
        }

        public int GetContentHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (_color.GetHashCode());
                hash = hash * 23 + (_width.GetHashCode());
                hash = hash * 23 + (_height.GetHashCode());
                hash = hash * 23 + (_styleSetId.Value?.GetHashCode() ?? 0);
                hash = hash * 23 + (_backgroundColor.GetHashCode());
                hash = hash * 23 + (_borderColor.GetHashCode());
                hash = hash * 23 + (_padding.GetHashCode());
                hash = hash * 23 + (_margin.GetHashCode());
                hash = hash * 23 + (_fontSize.GetHashCode());
                hash = hash * 23 + (_fontStyle.GetHashCode());
                hash = hash * 23 + (_alignment.GetHashCode());
                hash = hash * 23 + (_borderWidth.GetHashCode());
                hash = hash * 23 + (_borderRadius.GetHashCode());
                return hash;
            }
        }

        Color? IDeclStyle.Color
        {
            get => _color.IsDirectValue ? _color.DirectValue : (_color.IsPropertyRef ? _color.GetValue(DeclThemeManager.CurrentTheme) : (Color?)null);
            set => _color = value.HasValue ? StyleProperty<Color>.Direct(value.Value) : StyleProperty<Color>.None();
        }

        float? IDeclStyle.Width
        {
            get => _width.IsDirectValue ? _width.DirectValue : (_width.IsPropertyRef ? _width.GetValue(DeclThemeManager.CurrentTheme) : (float?)null);
            set => _width = value.HasValue ? StyleProperty<float>.Direct(value.Value) : StyleProperty<float>.None();
        }

        float? IDeclStyle.Height
        {
            get => _height.IsDirectValue ? _height.DirectValue : (_height.IsPropertyRef ? _height.GetValue(DeclThemeManager.CurrentTheme) : (float?)null);
            set => _height = value.HasValue ? StyleProperty<float>.Direct(value.Value) : StyleProperty<float>.None();
        }

        string? IDeclStyle.StyleSetId
        {
            get => _styleSetId.Value;
            set => _styleSetId.Value = value;
        }

        Color? IDeclStyle.BackgroundColor
        {
            get => _backgroundColor.IsDirectValue ? _backgroundColor.DirectValue : (_backgroundColor.IsPropertyRef ? _backgroundColor.GetValue(DeclThemeManager.CurrentTheme) : (Color?)null);
            set => _backgroundColor = value.HasValue ? StyleProperty<Color>.Direct(value.Value) : StyleProperty<Color>.None();
        }

        Color? IDeclStyle.BorderColor
        {
            get => _borderColor.IsDirectValue ? _borderColor.DirectValue : (_borderColor.IsPropertyRef ? _borderColor.GetValue(DeclThemeManager.CurrentTheme) : (Color?)null);
            set => _borderColor = value.HasValue ? StyleProperty<Color>.Direct(value.Value) : StyleProperty<Color>.None();
        }

        RectOffset? IDeclStyle.Padding
        {
            get => _padding.IsDirectValue ? _padding.DirectValue : (_padding.IsPropertyRef ? _padding.GetValue(DeclThemeManager.CurrentTheme) : (RectOffset?)null);
            set => _padding = value != null ? StyleProperty<RectOffset>.Direct(value) : StyleProperty<RectOffset>.None();
        }

        RectOffset? IDeclStyle.Margin
        {
            get => _margin.IsDirectValue ? _margin.DirectValue : (_margin.IsPropertyRef ? _margin.GetValue(DeclThemeManager.CurrentTheme) : (RectOffset?)null);
            set => _margin = value != null ? StyleProperty<RectOffset>.Direct(value) : StyleProperty<RectOffset>.None();
        }

        int? IDeclStyle.FontSize
        {
            get => _fontSize.IsDirectValue ? _fontSize.DirectValue : (_fontSize.IsPropertyRef ? _fontSize.GetValue(DeclThemeManager.CurrentTheme) : (int?)null);
            set => _fontSize = value.HasValue ? StyleProperty<int>.Direct(value.Value) : StyleProperty<int>.None();
        }

        FontStyle? IDeclStyle.FontStyle
        {
            get => _fontStyle.IsDirectValue ? _fontStyle.DirectValue : (_fontStyle.IsPropertyRef ? _fontStyle.GetValue(DeclThemeManager.CurrentTheme) : (FontStyle?)null);
            set => _fontStyle = value.HasValue ? StyleProperty<FontStyle>.Direct(value.Value) : StyleProperty<FontStyle>.None();
        }

        TextAnchor? IDeclStyle.Alignment
        {
            get => _alignment.IsDirectValue ? _alignment.DirectValue : (_alignment.IsPropertyRef ? _alignment.GetValue(DeclThemeManager.CurrentTheme) : (TextAnchor?)null);
            set => _alignment = value.HasValue ? StyleProperty<TextAnchor>.Direct(value.Value) : StyleProperty<TextAnchor>.None();
        }

        float? IDeclStyle.BorderWidth
        {
            get => _borderWidth.IsDirectValue ? _borderWidth.DirectValue : (_borderWidth.IsPropertyRef ? _borderWidth.GetValue(DeclThemeManager.CurrentTheme) : (float?)null);
            set => _borderWidth = value.HasValue ? StyleProperty<float>.Direct(value.Value) : StyleProperty<float>.None();
        }

        float? IDeclStyle.BorderRadius
        {
            get => _borderRadius.IsDirectValue ? _borderRadius.DirectValue : (_borderRadius.IsPropertyRef ? _borderRadius.GetValue(DeclThemeManager.CurrentTheme) : (float?)null);
            set => _borderRadius = value.HasValue ? StyleProperty<float>.Direct(value.Value) : StyleProperty<float>.None();
        }
    }
}