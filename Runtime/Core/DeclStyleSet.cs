using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 高级样式集实现（ScriptableObject 版本）
    /// 包含 DeclStyle 的所有属性，并支持伪类和过渡效果
    /// 使用统一的StyleProperty<T>来封装属性值，移除GUIStyle字段
    /// </summary>
    [Serializable]
    public class DeclStyleSet : ScriptableObject, ISerializableDeclStyle, ISerializationCallbackReceiver, System.IEquatable<IDeclStyle>
    {
        public bool Equals(IDeclStyle other)
        {
            if (other == null) return false;
            if (!BaseStyleEquals(other)) return false;
            if (other is DeclStyleSet set)
            {
                if (this.Styles.Count != set.Styles.Count) return false;
                foreach (var kv in this.Styles)
                {
                    if (!set.Styles.TryGetValue(kv.Key, out var otherStyle)) return false;
                    if (!kv.Value.Equals(otherStyle)) return false;
                }
            }
            return true;
        }

        private bool BaseStyleEquals(IDeclStyle other)
        {
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

        private static bool NullableEquals<T>(T? a, T? b) where T : struct
        {
            if (a.HasValue != b.HasValue) return false;
            if (!a.HasValue) return true;
            return a.Value.Equals(b.Value);
        }
    
        // 基础样式，使用 DeclStyle 对象
        [SerializeField] private DeclStyle _baseStyle;
        
        // 伪类样式字典
        [SerializeField] private PseudoClassStyleDictionary _styles = new PseudoClassStyleDictionary();
        [SerializeField] private bool _hasTransition;
        [SerializeField] private TransitionConfig _serializedTransition;
        
        /// <summary>
        /// 样式字典（通过可序列化字典包装）
        /// </summary>
        public Dictionary<PseudoClass, DeclStyle> Styles => _styles.Dictionary;
        
        /// <summary>
        /// 过渡效果配置
        /// </summary>
        public TransitionConfig? Transition
        {
            get => _hasTransition ? _serializedTransition : (TransitionConfig?)null;
            set
            {
                _hasTransition = value.HasValue;
                if (value.HasValue)
                {
                    _serializedTransition = value.Value;
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeclStyleSet()
        {
            _baseStyle = new DeclStyle();
        }

        // ISerializableDeclStyle接口实现
        public SerializableRefNullable<string> StyleSetId { get => _baseStyle.StyleSetId; set => _baseStyle.StyleSetId = value; }
        public StyleProperty<Color> Color { get => _baseStyle.Color; set => _baseStyle.Color = value; }
        public StyleProperty<float> Width { get => _baseStyle.Width; set => _baseStyle.Width = value; }
        public StyleProperty<float> Height { get => _baseStyle.Height; set => _baseStyle.Height = value; }
        public StyleProperty<Color> BackgroundColor { get => _baseStyle.BackgroundColor; set => _baseStyle.BackgroundColor = value; }
        public StyleProperty<Color> BorderColor { get => _baseStyle.BorderColor; set => _baseStyle.BorderColor = value; }
        public StyleProperty<RectOffset> Padding { get => _baseStyle.Padding; set => _baseStyle.Padding = value; }
        public StyleProperty<RectOffset> Margin { get => _baseStyle.Margin; set => _baseStyle.Margin = value; }
        public StyleProperty<int> FontSize { get => _baseStyle.FontSize; set => _baseStyle.FontSize = value; }
        public StyleProperty<FontStyle> FontStyle { get => _baseStyle.FontStyle; set => _baseStyle.FontStyle = value; }
        public StyleProperty<TextAnchor> Alignment { get => _baseStyle.Alignment; set => _baseStyle.Alignment = value; }
        public StyleProperty<float> BorderWidth { get => _baseStyle.BorderWidth; set => _baseStyle.BorderWidth = value; }
        public StyleProperty<float> BorderRadius { get => _baseStyle.BorderRadius; set => _baseStyle.BorderRadius = value; }
        
        /// <summary>
        /// 序列化前调用
        /// </summary>
        public void OnBeforeSerialize()
        {
            // 可序列化字典会自动处理序列化
        }
        
        /// <summary>
        /// 反序列化后调用
        /// </summary>
        public void OnAfterDeserialize()
        {
            // 可序列化字典会自动处理反序列化
        }
        
        // 字典操作方法
        public void AddStyle(PseudoClass pseudoClass, DeclStyle style)
        {
            _styles.Dictionary[pseudoClass] = style;
        }
        
        public bool RemoveStyle(PseudoClass pseudoClass) => _styles.Dictionary.Remove(pseudoClass);
        
        public bool TryGetStyle(PseudoClass pseudoClass, out DeclStyle style) =>
            _styles.Dictionary.TryGetValue(pseudoClass, out style);
        
        public void ClearStyles() => _styles.Dictionary.Clear();
        
        public IDeclStyle GetStyleForState(IElementState elementState)
        {
            var pseudoClass = DeterminePseudoClass(elementState);
            
            // Normal 伪类始终指向自身
            if (pseudoClass == PseudoClass.Normal)
            {
                return this;
            }
            
            if (Styles.TryGetValue(pseudoClass, out var style))
            {
                return style;
            }
            
            // 如果没有找到对应伪类的样式，返回一个包含当前属性的 DeclStyle
            return this._baseStyle;
        }
        
        public IDeclStyle Merge(IDeclStyle other)
        {
            if (other == null) return this;
            
            // 创建新的 DeclStyle 实例来合并属性
            return new DeclStyle()
            {
                Color = other.Color ?? Color,
                Width = other.Width ?? Width,
                Height = other.Height ?? Height,
                StyleSetId = StyleSetId,
                BackgroundColor = other.BackgroundColor ?? BackgroundColor,
                BorderColor = other.BorderColor ?? BorderColor,
                Padding = other.Padding ?? Padding,
                Margin = other.Margin ?? Margin,
                FontSize = other.FontSize ?? FontSize,
                FontStyle = other.FontStyle ?? FontStyle,
                Alignment = other.Alignment ?? Alignment,
                BorderWidth = other.BorderWidth ?? BorderWidth,
                BorderRadius = other.BorderRadius ?? BorderRadius,
            };
        }

        public int GetContentHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (_baseStyle.GetContentHashCode());
                // 对于样式集，我们还需要考虑伪类样式
                foreach (var kv in _styles.Dictionary)
                {
                    hash = hash * 23 + kv.Key.GetHashCode();
                    hash = hash * 23 + kv.Value.GetContentHashCode();
                }
                hash = hash * 23 + (_hasTransition.GetHashCode());
                if (_hasTransition)
                {
                    hash = hash * 23 + _serializedTransition.GetHashCode();
                }
                return hash;
            }
        }

        Color? IDeclStyle.Color {
            get => _baseStyle.Color.IsDirectValue ? _baseStyle.Color.DirectValue : (_baseStyle.Color.IsPropertyRef ? _baseStyle.Color.GetValue(DeclThemeManager.CurrentTheme) : (Color?)null);
            set => _baseStyle = _baseStyle.SetColor(value ?? default);
        }

        float? IDeclStyle.Width {
            get => _baseStyle.Width.IsDirectValue ? _baseStyle.Width.DirectValue : (_baseStyle.Width.IsPropertyRef ? _baseStyle.Width.GetValue(DeclThemeManager.CurrentTheme) : (float?)null);
            set => _baseStyle = _baseStyle.SetWidth(value ?? 0f);
        }

        float? IDeclStyle.Height {
            get => _baseStyle.Height.IsDirectValue ? _baseStyle.Height.DirectValue : (_baseStyle.Height.IsPropertyRef ? _baseStyle.Height.GetValue(DeclThemeManager.CurrentTheme) : (float?)null);
            set => _baseStyle = _baseStyle.SetHeight(value ?? 0f);
        }

        string? IDeclStyle.StyleSetId {
            get => _baseStyle.StyleSetId.HasValue ? _baseStyle.StyleSetId.Value : null;
            set => _baseStyle = new DeclStyle(styleSetId: value);
        }

        Color? IDeclStyle.BackgroundColor {
            get => _baseStyle.BackgroundColor.IsDirectValue ? _baseStyle.BackgroundColor.DirectValue : (_baseStyle.BackgroundColor.IsPropertyRef ? _baseStyle.BackgroundColor.GetValue(DeclThemeManager.CurrentTheme) : (Color?)null);
            set => _baseStyle = _baseStyle.SetBackgroundColor(value ?? default);
        }

        Color? IDeclStyle.BorderColor {
            get => _baseStyle.BorderColor.IsDirectValue ? _baseStyle.BorderColor.DirectValue : (_baseStyle.BorderColor.IsPropertyRef ? _baseStyle.BorderColor.GetValue(DeclThemeManager.CurrentTheme) : (Color?)null);
            set => _baseStyle = _baseStyle.SetBorderColor(value ?? default);
        }

        RectOffset? IDeclStyle.Padding {
            get => _baseStyle.Padding.IsDirectValue ? _baseStyle.Padding.DirectValue : (_baseStyle.Padding.IsPropertyRef ? _baseStyle.Padding.GetValue(DeclThemeManager.CurrentTheme) : (RectOffset?)null);
            set => _baseStyle = _baseStyle.SetPadding(value ?? new RectOffset());
        }

        RectOffset? IDeclStyle.Margin {
            get => _baseStyle.Margin.IsDirectValue ? _baseStyle.Margin.DirectValue : (_baseStyle.Margin.IsPropertyRef ? _baseStyle.Margin.GetValue(DeclThemeManager.CurrentTheme) : (RectOffset?)null);
            set => _baseStyle = _baseStyle.SetMargin(value ?? new RectOffset());
        }

        int? IDeclStyle.FontSize {
            get => _baseStyle.FontSize.IsDirectValue ? _baseStyle.FontSize.DirectValue : (_baseStyle.FontSize.IsPropertyRef ? _baseStyle.FontSize.GetValue(DeclThemeManager.CurrentTheme) : (int?)null);
            set => _baseStyle = _baseStyle.SetFontSize(value ?? 0);
        }

        FontStyle? IDeclStyle.FontStyle {
            get => _baseStyle.FontStyle.IsDirectValue ? _baseStyle.FontStyle.DirectValue : (_baseStyle.FontStyle.IsPropertyRef ? _baseStyle.FontStyle.GetValue(DeclThemeManager.CurrentTheme) : (FontStyle?)null);
            set => _baseStyle = _baseStyle.SetFontStyle(value ?? default);
        }

        TextAnchor? IDeclStyle.Alignment {
            get => _baseStyle.Alignment.IsDirectValue ? _baseStyle.Alignment.DirectValue : (_baseStyle.Alignment.IsPropertyRef ? _baseStyle.Alignment.GetValue(DeclThemeManager.CurrentTheme) : (TextAnchor?)null);
            set => _baseStyle = _baseStyle.SetAlignment(value ?? default);
        }

        float? IDeclStyle.BorderWidth {
            get => _baseStyle.BorderWidth.IsDirectValue ? _baseStyle.BorderWidth.DirectValue : (_baseStyle.BorderWidth.IsPropertyRef ? _baseStyle.BorderWidth.GetValue(DeclThemeManager.CurrentTheme) : (float?)null);
            set => _baseStyle = _baseStyle.SetBorderWidth(value ?? 0f);
        }

        float? IDeclStyle.BorderRadius {
            get => _baseStyle.BorderRadius.IsDirectValue ? _baseStyle.BorderRadius.DirectValue : (_baseStyle.BorderRadius.IsPropertyRef ? _baseStyle.BorderRadius.GetValue(DeclThemeManager.CurrentTheme) : (float?)null);
            set => _baseStyle = _baseStyle.SetBorderRadius(value ?? 0f);
        }
        
        
        private PseudoClass DeterminePseudoClass(IElementState elementState)
        {
            if (elementState == null)
                return PseudoClass.Normal;

            // 禁用状态具有最高优先级
            if (elementState.DisabledState.IsDisabled)
                return PseudoClass.Disabled;

            // 激活状态（如按钮按下）
            if (elementState.HasState(ElementStateFlags.Active))
                return PseudoClass.Active;

            // 聚焦状态
            if (elementState.HasState(ElementStateFlags.Focus))
                return PseudoClass.Focus;

            // 悬停状态
            if (elementState.HasState(ElementStateFlags.Hover))
                return PseudoClass.Hover;

            return PseudoClass.Normal;
        }
    }
}