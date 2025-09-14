using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 过渡效果处理器
    /// 专门负责处理样式过渡动画
    /// </summary>
    public class TransitionProcessor
    {
        private class TransitionState
        {
            public IDeclStyle FromStyle { get; set; }
            public IDeclStyle ToStyle { get; set; }
            public float StartTime { get; set; }
            public float Duration { get; set; }
            public AnimationCurve EasingCurve { get; set; }
            public string[] Properties { get; set; }
        }
        
        private readonly Dictionary<string, TransitionState> _activeTransitions = 
            new Dictionary<string, TransitionState>();
        
        private float _currentTime;
        
        /// <summary>
        /// 处理样式过渡
        /// </summary>
        public IDeclStyle ProcessTransition(IDeclStyle targetStyle, ElementState elementState, string elementKey)
        {
            if (targetStyle == null || elementState == null || string.IsNullOrEmpty(elementKey))
                return targetStyle;
            
            // 检查是否需要开始新过渡
            if (ShouldStartTransition(targetStyle, elementState, elementKey))
            {
                StartTransition(targetStyle, elementState, elementKey);
            }
            
            // 获取当前过渡状态
            if (_activeTransitions.TryGetValue(elementKey, out var transition))
            {
                return GetCurrentTransitionStyle(transition);
            }
            
            return targetStyle;
        }
        
        /// <summary>
        /// 更新所有活跃的过渡
        /// </summary>
        public void UpdateTransitions(float deltaTime)
        {
            _currentTime += deltaTime;
            
            // 清理完成的过渡
            var keysToRemove = new List<string>();
            foreach (var kvp in _activeTransitions)
            {
                var transition = kvp.Value;
                if (_currentTime - transition.StartTime >= transition.Duration)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            foreach (var key in keysToRemove)
            {
                _activeTransitions.Remove(key);
            }
        }
        
        private bool ShouldStartTransition(IDeclStyle targetStyle, ElementState elementState, string elementKey)
        {
            // 检查样式是否有过渡配置
            // 检查元素状态是否发生变化
            // 这里需要具体实现状态变化检测逻辑
            return true; // 简化实现
        }
        
        private void StartTransition(IDeclStyle targetStyle, ElementState elementState, string elementKey)
        {
            // 创建新的过渡状态
            var transition = new TransitionState
            {
                FromStyle = GetCurrentStyle(elementKey), // 需要实现当前样式获取
                ToStyle = targetStyle,
                StartTime = _currentTime,
                Duration = 0.3f, // 从配置获取
                EasingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1), // 默认缓动曲线
                Properties = new[] { "color", "width", "height" } // 从配置获取
            };
            
            _activeTransitions[elementKey] = transition;
        }
        
        private IDeclStyle GetCurrentTransitionStyle(TransitionState transition)
        {
            float progress = Mathf.Clamp01((_currentTime - transition.StartTime) / transition.Duration);
            float easedProgress = transition.EasingCurve.Evaluate(progress);
            
            return InterpolateStyles(transition.FromStyle, transition.ToStyle, easedProgress);
        }
        
        private IDeclStyle InterpolateStyles(IDeclStyle from, IDeclStyle to, float progress)
        {
            // 颜色插值
            Color? color = InterpolateColor(from.GetColor(), to.GetColor(), progress);
            Color? backgroundColor = InterpolateColor(from.GetBackgroundColor(), to.GetBackgroundColor(), progress);
            Color? borderColor = InterpolateColor(from.GetBorderColor(), to.GetBorderColor(), progress);
            
            // 尺寸插值
            float width = Mathf.Lerp(from.GetWidth(), to.GetWidth(), progress);
            float height = Mathf.Lerp(from.GetHeight(), to.GetHeight(), progress);
            float borderWidth = Mathf.Lerp(from.GetBorderWidth() ?? 0, to.GetBorderWidth() ?? 0, progress);
            float borderRadius = Mathf.Lerp(from.GetBorderRadius() ?? 0, to.GetBorderRadius() ?? 0, progress);
            
            // 布局属性插值
            RectOffset padding = InterpolateRectOffset(from.GetPadding(), to.GetPadding(), progress);
            RectOffset margin = InterpolateRectOffset(from.GetMargin(), to.GetMargin(), progress);
            
            // 文本属性（离散值，不插值）
            int? fontSize = progress >= 0.5f ? to.GetFontSize() : from.GetFontSize();
            FontStyle? fontStyle = progress >= 0.5f ? to.GetFontStyle() : from.GetFontStyle();
            TextAnchor? alignment = progress >= 0.5f ? to.GetAlignment() : from.GetAlignment();
            
            return new DeclStyle(
                color: color,
                width: width,
                height: height,
                backgroundColor: backgroundColor,
                borderColor: borderColor,
                padding: padding,
                margin: margin,
                fontSize: fontSize,
                fontStyle: fontStyle,
                alignment: alignment,
                borderWidth: borderWidth,
                borderRadius: borderRadius
            );
        }
        
        private Color? InterpolateColor(Color? from, Color? to, float progress)
        {
            if (!from.HasValue && !to.HasValue) return null;
            if (!from.HasValue) return to;
            if (!to.HasValue) return from;
            
            return Color.Lerp(from.Value, to.Value, progress);
        }
        
        private RectOffset InterpolateRectOffset(RectOffset from, RectOffset to, float progress)
        {
            return new RectOffset(
                Mathf.RoundToInt(Mathf.Lerp(from.left, to.left, progress)),
                Mathf.RoundToInt(Mathf.Lerp(from.right, to.right, progress)),
                Mathf.RoundToInt(Mathf.Lerp(from.top, to.top, progress)),
                Mathf.RoundToInt(Mathf.Lerp(from.bottom, to.bottom, progress))
            );
        }
        
        private IDeclStyle GetCurrentStyle(string elementKey)
        {
            // 需要实现当前样式获取逻辑
            // 可以从渲染管理器或元素状态获取
            return new DeclStyle();
        }
    }
}