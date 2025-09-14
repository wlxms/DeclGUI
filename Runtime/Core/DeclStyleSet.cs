using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 高级样式集实现（class 版本）
    /// 支持伪类和过渡效果
    /// </summary>
    public class DeclStyleSet : IDeclStyle
    {
        public Dictionary<PseudoClass, IDeclStyle> Styles { get; } = 
            new Dictionary<PseudoClass, IDeclStyle>();
        
        public TransitionConfig? Transition { get; set; }
        
        public IDeclStyle GetStyleForState(IElementState elementState)
        {
            var pseudoClass = DeterminePseudoClass(elementState);
            
            if (Styles.TryGetValue(pseudoClass, out var style))
            {
                return style;
            }
            
            // 返回默认样式或空样式
            return Styles.TryGetValue(PseudoClass.Normal, out var defaultStyle) 
                ? defaultStyle 
                : new DeclStyle();
        }
        
        public IDeclStyle Merge(IDeclStyle other)
        {
            // 样式集不支持合并，返回自身
            return this;
        }
        
        public float GetWidth() => 0; // 需要具体状态
        public float GetHeight() => 0; // 需要具体状态
        public Color? GetColor() => null; // 需要具体状态
        public Color? GetBackgroundColor() => null; // 需要具体状态
        public Color? GetBorderColor() => null; // 需要具体状态
        public RectOffset GetPadding() => new RectOffset(); // 需要具体状态
        public RectOffset GetMargin() => new RectOffset(); // 需要具体状态
        public int? GetFontSize() => null; // 需要具体状态
        public FontStyle? GetFontStyle() => null; // 需要具体状态
        public TextAnchor? GetAlignment() => null; // 需要具体状态
        public float? GetBorderWidth() => null; // 需要具体状态
        public float? GetBorderRadius() => null; // 需要具体状态
        public string GetStyleSetId() => null; // 样式集本身没有ID
        
        private PseudoClass DeterminePseudoClass(IElementState elementState)
        {
            if (elementState != null && elementState.HoverState.IsHovering)
                return PseudoClass.Hover;
            
            return PseudoClass.Normal;
        }
    }
}