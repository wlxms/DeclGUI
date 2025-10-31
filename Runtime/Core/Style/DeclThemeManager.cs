using System.Collections.Generic;
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
        
        // 缓存键生成器（保留用于RenderManager）
        public static string GenerateCacheKey(IDeclStyle style, PseudoClass pseudoClass)
        {
            if (style == null) return "null";
            int styleHash = GetStyleChainContentHashCode(style, CurrentTheme);
            return $"{styleHash}_{pseudoClass}";
        }
        
        public static string GenerateResolvedCacheKey(IDeclStyle style, DeclTheme theme)
        {
            if (style == null || theme == null) return "null";
            int styleHash = GetStyleChainContentHashCode(style, theme);
            int themeHash = theme.GetHashCode();
            return $"{styleHash}_{themeHash}";
        }
        
        /// <summary>
        /// 计算整个样式引用链的内容哈希码（包括引用的样式集和属性引用）
        /// </summary>
        /// <param name="style">起始样式</param>
        /// <param name="theme">当前主题（用于计算属性引用值）</param>
        /// <returns>包含整个引用链的哈希码</returns>
        private static int GetStyleChainContentHashCode(IDeclStyle style, DeclTheme theme)
        {
            if (style == null) return 0;
            
            unchecked
            {
                int hash = 17;
                var currentStyle = style;
                var visitedStyles = new HashSet<string>(); // 防止循环引用导致无限循环
                
                // 遍历整个样式引用链
                while (currentStyle != null && !string.IsNullOrEmpty(currentStyle.StyleSetId))
                {
                    // 检查循环引用
                    if (visitedStyles.Contains(currentStyle.StyleSetId))
                    {
                        Debug.LogWarning($"DeclThemeManager: 检测到样式集引用循环: {currentStyle.StyleSetId}");
                        break;
                    }
                    
                    visitedStyles.Add(currentStyle.StyleSetId);
                    
                    // 添加当前样式对象的哈希值（包含主题属性值）
                    hash = hash * 23 + currentStyle.GetContentHashCode(theme);
                    
                    // 获取引用的样式集
                    var styleSet = GetStyleSet(currentStyle.StyleSetId);
                    if (styleSet != null)
                    {
                        // 继续遍历引用的样式集
                        currentStyle = styleSet;
                    }
                    else
                    {
                        // 如果样式集不存在，停止遍历
                        break;
                    }
                }
                
                // 如果最终样式不为null且未在引用链中（即基础样式），也加入哈希计算
                if (currentStyle != null && !visitedStyles.Contains(currentStyle.StyleSetId ?? ""))
                {
                    hash = hash * 23 + currentStyle.GetContentHashCode(theme);
                }
                
                return hash;
            }
        }
        
        /// <summary>
        /// 清空样式缓存（主题变更时调用）
        /// </summary>
        public static void ClearCache()
        {
            // 缓存已移至RenderManager，此方法保留用于向后兼容
        }
        
        /// <summary>
        /// 当前主题
        /// </summary>
        public static DeclTheme CurrentTheme
        {
            get
            {
                // 如果当前主题为空，尝试从设置中获取默认主题
                if (_currentTheme == null)
                {
                    var setting = DeclGUISetting.Instance;
                    if (setting != null)
                    {
                        _currentTheme = setting.DefaultTheme;
                    }
                }
                return _currentTheme;
            }
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    // 主题变更时清空缓存
                    ClearCache();
                    OnThemeChanged?.Invoke(value);
                }
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
        /// 解析样式（处理样式集引用和伪类样式）
        /// </summary>
        /// <param name="style">要解析的样式对象</param>
        /// <param name="pseudoClass">伪类类型</param>
        /// <returns>解析后的样式</returns>
        public static IDeclStyle ResolveStyle(IDeclStyle style, PseudoClass pseudoClass = PseudoClass.Normal)
        {
            if (style == null) return null;
            
            IDeclStyle resolvedStyle = style;
            
            // 使用迭代代替递归处理样式集引用链
            var currentStyle = style;
            var styleChain = new Stack<IDeclStyle>();
            
            // 收集样式链（避免递归）
            while (currentStyle != null && !string.IsNullOrEmpty(currentStyle.StyleSetId))
            {
                var styleSetId = currentStyle.StyleSetId;
                var styleSet = GetStyleSet(styleSetId);
                
                if (styleSet != null)
                {
                    styleChain.Push(currentStyle);
                    currentStyle = styleSet;
                }
                else
                {
                    // 如果样式集不存在，记录警告并跳出循环
                    Debug.LogWarning($"DeclThemeManager: 样式集 '{styleSetId}' 不存在，使用默认样式");
                    resolvedStyle = CreateDefaultStyle();
                    break;
                }
            }
            
            // 如果样式链不为空，从基础样式开始合并
            if (styleChain.Count > 0)
            {
                // 基础样式是链的最后一个（最底层）
                resolvedStyle = currentStyle;
                
                // 从底层到顶层合并样式
                while (styleChain.Count > 0)
                {
                    var childStyle = styleChain.Pop();
                    resolvedStyle = resolvedStyle.Merge(childStyle);
                }
            }
            
            // 处理伪类样式（伪类样式具有最高优先级）
            // 只有 DeclStyleSet 包含伪类样式字典
            if (resolvedStyle is DeclStyleSet styleSetWithPseudo)
            {
                // 从样式集中提取对应伪类的样式
                if (styleSetWithPseudo.Styles.TryGetValue(pseudoClass, out var pseudoStyle))
                {
                    // 伪类样式覆盖当前所有样式（最高优先级）
                    resolvedStyle = resolvedStyle.Merge(pseudoStyle);
                }
            }
            
            // 解析所有StyleProperty中的PropertyRef引用，返回最终样式
            resolvedStyle = ResolveStylePropertyReferences(resolvedStyle);
            
            return resolvedStyle;
        }
        /// <summary>
        /// 解析IDeclStyle中的所有StyleProperty引用，将PropertyRef解析为实际值
        /// </summary>
        private static IDeclStyle ResolveStylePropertyReferences(IDeclStyle style)
        {
            if (style == null) return null;

            // 只处理ISerializableDeclStyle（DeclStyle/DeclStyleSet）
            if (style is ISerializableDeclStyle serializable)
            {
                // 检查是否需要解析（是否有PropertyRef）
                bool needsResolution = serializable.Color.IsPropertyRef || serializable.Width.IsPropertyRef ||
                                      serializable.Height.IsPropertyRef || serializable.BackgroundColor.IsPropertyRef ||
                                      serializable.BorderColor.IsPropertyRef || serializable.Padding.IsPropertyRef ||
                                      serializable.Margin.IsPropertyRef || serializable.FontSize.IsPropertyRef ||
                                      serializable.FontStyle.IsPropertyRef || serializable.Alignment.IsPropertyRef ||
                                      serializable.BorderWidth.IsPropertyRef || serializable.BorderRadius.IsPropertyRef;

                // 如果没有PropertyRef，直接返回原样式
                if (!needsResolution)
                {
                    return style;
                }

                var theme = CurrentTheme;
                
                // 解析每个StyleProperty<T>，如果是PropertyRef则替换为Direct值
                // 注意：此处会生成一个新的DeclStyle实例，避免修改原对象
                var resolved = new DeclStyle(
                    color: ResolvePropertyValue(serializable.Color, theme),
                    width: ResolvePropertyValue(serializable.Width, theme),
                    height: ResolvePropertyValue(serializable.Height, theme),
                    styleSetId: serializable.StyleSetId.HasValue ? serializable.StyleSetId.Value : null,
                    backgroundColor: ResolvePropertyValue(serializable.BackgroundColor, theme),
                    borderColor: ResolvePropertyValue(serializable.BorderColor, theme),
                    padding: ResolvePropertyValue(serializable.Padding, theme),
                    margin: ResolvePropertyValue(serializable.Margin, theme),
                    fontSize: ResolvePropertyValue(serializable.FontSize, theme),
                    fontStyle: ResolvePropertyValue(serializable.FontStyle, theme),
                    alignment: ResolvePropertyValue(serializable.Alignment, theme),
                    borderWidth: ResolvePropertyValue(serializable.BorderWidth, theme),
                    borderRadius: ResolvePropertyValue(serializable.BorderRadius, theme)
                );
                
                return resolved;
            }
            // 其他类型直接返回
            return style;
        }
        
        /// <summary>
        /// 解析单个属性值
        /// </summary>
        private static T ResolvePropertyValue<T>(StyleProperty<T> property, DeclTheme theme)
        {
            return property.IsPropertyRef ? property.GetValue(theme) : property.DirectValue;
        }
        
        /// <summary>
        /// 创建默认样式（当样式集不存在时使用）
        /// </summary>
        private static IDeclStyle CreateDefaultStyle()
        {
            // 返回一个包含基本可见属性的默认样式
            return new DeclStyle(
                color: Color.white,
                backgroundColor: new Color(0.2f, 0.2f, 0.2f, 1f),
                fontSize: 12
            );
        }
        
        /// <summary>
        /// 解析样式（向后兼容的重载）
        /// </summary>
        public static IDeclStyle ResolveStyle(IDeclStyle style, IElementState elementState = null)
        {
            if (style == null) return null;
            
            // 根据元素状态确定伪类
            PseudoClass pseudoClass = DeterminePseudoClassFromState(elementState);
            
            return ResolveStyle(style, pseudoClass);
        }
        
        /// <summary>
        /// 根据元素状态确定伪类
        /// </summary>
        private static PseudoClass DeterminePseudoClassFromState(IElementState elementState)
        {
            if (elementState == null) return PseudoClass.Normal;
            
            // Debug.Log($"DeterminePseudoClassFromState: {elementState.CurrentStateFlags} {elementState.HoverState.GetHashCode()} {elementState.ElementType}");
            // 这里可以根据元素状态确定伪类
            // 目前只处理悬停状态，可以根据需要扩展其他状态
            if (elementState.CurrentStateFlags.HasFlag(ElementStateFlags.Hover))
                return PseudoClass.Hover;
                
            return PseudoClass.Normal;
        }
    }
}