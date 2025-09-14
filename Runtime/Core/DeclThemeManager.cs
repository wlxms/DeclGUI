using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// DeclGUI 主题管理器
    /// 提供全局主题访问和样式集解析
    /// </summary>
    public static class DeclThemeManager
    {
        private static DeclTheme _currentTheme;
        
        /// <summary>
        /// 当前主题
        /// </summary>
        public static DeclTheme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                OnThemeChanged?.Invoke(value);
            }
        }
        
        /// <summary>
        /// 主题变更事件
        /// </summary>
        public static event System.Action<DeclTheme> OnThemeChanged;
        
        /// <summary>
        /// 获取样式集
        /// </summary>
        public static IDeclStyle GetStyleSet(string id)
        {
            return CurrentTheme?.GetStyleSet(id);
        }
        
        /// <summary>
        /// 解析样式（处理样式集引用）
        /// </summary>
        public static IDeclStyle ResolveStyle(IDeclStyle style, ElementState elementState = null)
        {
            if (style == null) return null;
            
            // 如果有样式集ID，先解析样式集
            var styleSetId = style.GetStyleSetId();
            if (!string.IsNullOrEmpty(styleSetId))
            {
                var styleSet = GetStyleSet(styleSetId);
                if (styleSet != null)
                {
                    var resolvedStyle = styleSet.GetStyleForState(elementState ?? new ElementState());
                    return resolvedStyle.Merge(style);
                }
            }
            
            return style;
        }
    }
}