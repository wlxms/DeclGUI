using System;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 过渡效果处理器
    /// 专门负责处理样式过渡动画
    /// </summary>
    public class TransitionProcessor
    {
        /// <summary>
        /// 处理样式过渡
        /// </summary>
        /// <param name="fromStyle">起始样式</param>
        /// <param name="toStyle">目标样式</param>
        /// <param name="elementState">元素状态（包含过渡状态）</param>
        /// <param name="transitionConfig">过渡配置</param>
        /// <returns>当前过渡样式</returns>
        public IDeclStyle ProcessTransition(IDeclStyle fromStyle, IDeclStyle toStyle, IElementState elementState, TransitionConfig? transitionConfig = null)
        {
            if (fromStyle == null || toStyle == null || elementState == null)
                return toStyle;

            // 如果没有过渡配置或样式相同，直接返回目标样式
            if (!transitionConfig.HasValue || fromStyle == toStyle)
                return toStyle;

            var config = transitionConfig.Value;
            
            // 检查是否需要开始新过渡
            if (ShouldStartTransition(fromStyle, toStyle, elementState, config))
            {
                StartTransition(fromStyle, toStyle, elementState, config);
            }

            // 获取当前过渡样式
            return GetCurrentTransitionStyle(elementState.TransitionState);
        }

        /// <summary>
        /// 检查是否需要开始新过渡
        /// </summary>
        private bool ShouldStartTransition(IDeclStyle fromStyle, IDeclStyle toStyle, IElementState elementState, TransitionConfig config)
        {
            var transitionState = elementState.TransitionState;
            
            // 如果没有活跃过渡，需要开始
            if (transitionState.FromStyle == null || transitionState.ToStyle == null)
                return true;

            // 如果目标样式发生变化，需要重新开始过渡
            if (transitionState.ToStyle != toStyle)
                return true;

            // 如果过渡已完成，需要重新开始
            if (transitionState.IsCompleted)
                return true;

            return false;
        }

        /// <summary>
        /// 开始新过渡
        /// </summary>
        private void StartTransition(IDeclStyle fromStyle, IDeclStyle toStyle, IElementState elementState, TransitionConfig config)
        {
            elementState.TransitionState = new TransitionState
            {
                FromStyle = fromStyle,
                ToStyle = toStyle,
                StartTime = Time.time,
                Duration = config.Duration,
                EasingCurve = config.EasingCurve ?? AnimationCurve.EaseInOut(0, 0, 1, 1),
                Properties = config.Properties ?? new[] { "color", "width", "height" }
            };
        }

        /// <summary>
        /// 获取当前过渡样式
        /// </summary>
        private IDeclStyle GetCurrentTransitionStyle(TransitionState transitionState)
        {
            if (transitionState == null || transitionState.FromStyle == null || transitionState.ToStyle == null)
                return new DeclStyle();

            if (transitionState.IsCompleted)
                return transitionState.ToStyle;

            float progress = transitionState.EasedProgress;
            
            return InterpolateStyles(transitionState.FromStyle, transitionState.ToStyle, progress);
        }

        /// <summary>
        /// 样式插值
        /// </summary>
        private IDeclStyle InterpolateStyles(IDeclStyle from, IDeclStyle to, float progress)
        {
            // 颜色插值
            Color? color = InterpolateColor(from.Color, to.Color, progress);
            Color? backgroundColor = InterpolateColor(from.BackgroundColor, to.BackgroundColor, progress);
            Color? borderColor = InterpolateColor(from.BorderColor, to.BorderColor, progress);

            // 尺寸插值
            float width = Mathf.Lerp(from.Width ?? 0, to.Width ?? 0, progress);
            float height = Mathf.Lerp(from.Height ?? 0, to.Height ?? 0, progress);
            float borderWidth = Mathf.Lerp(from.BorderWidth ?? 0, to.BorderWidth ?? 0, progress);
            float borderRadius = Mathf.Lerp(from.BorderRadius ?? 0, to.BorderRadius ?? 0, progress);

            // 布局属性插值
            RectOffset padding = InterpolateRectOffset(from.Padding ?? new RectOffset(), to.Padding ?? new RectOffset(), progress);
            RectOffset margin = InterpolateRectOffset(from.Margin ?? new RectOffset(), to.Margin ?? new RectOffset(), progress);

            // 文本属性（离散值，不插值）
            int? fontSize = progress >= 0.5f ? to.FontSize : from.FontSize;
            FontStyle? fontStyle = progress >= 0.5f ? to.FontStyle : from.FontStyle;
            TextAnchor? alignment = progress >= 0.5f ? to.Alignment : from.Alignment;

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

        /// <summary>
        /// 颜色插值
        /// </summary>
        private Color? InterpolateColor(Color? from, Color? to, float progress)
        {
            if (!from.HasValue && !to.HasValue) return null;
            if (!from.HasValue) return to;
            if (!to.HasValue) return from;
            
            return Color.Lerp(from.Value, to.Value, progress);
        }

        /// <summary>
        /// 矩形偏移插值
        /// </summary>
        private RectOffset InterpolateRectOffset(RectOffset from, RectOffset to, float progress)
        {
            return new RectOffset(
                Mathf.RoundToInt(Mathf.Lerp(from.left, to.left, progress)),
                Mathf.RoundToInt(Mathf.Lerp(from.right, to.right, progress)),
                Mathf.RoundToInt(Mathf.Lerp(from.top, to.top, progress)),
                Mathf.RoundToInt(Mathf.Lerp(from.bottom, to.bottom, progress))
            );
        }
    }
}