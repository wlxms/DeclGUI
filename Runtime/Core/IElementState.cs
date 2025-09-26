using System;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 悬停状态结构体
    /// 用于管理悬停事件的时间状态
    /// </summary>
    public class HoverState
    {
        /// <summary>
        /// 悬停开始时间（Time.time）
        /// 当鼠标进入元素时设置
        /// </summary>
        public float HoverStartTime { get; set; }
        
        /// <summary>
        /// 最后一次离开时间（Time.time）
        /// 当鼠标离开元素时设置
        /// </summary>
        public float LastLeaveTime { get; set; }
        
        /// <summary>
        /// 最后一次悬停时间（Time.time）
        /// 每次鼠标移动时更新，用于区分Hovering事件
        /// </summary>
        public float LastHoverTime { get; set; }
        
        /// <summary>
        /// 是否当前正在悬停
        /// HoverStartTime > LastLeaveTime 表示正在悬停
        /// </summary>
        public bool IsHovering { get; set; }
        
        /// <summary>
        /// 悬停持续时间（如果正在悬停）
        /// </summary>
        public float HoverDuration => IsHovering ? (Time.time - HoverStartTime) : 0f;
        
        /// <summary>
        /// 是否需要触发Enter事件
        /// 当HoverStartTime > LastLeaveTime 且 LastHoverTime < HoverStartTime 时触发
        /// </summary>
        public bool ShouldTriggerEnter => !IsHovering && LastHoverTime <= HoverStartTime;
        
        /// <summary>
        /// 是否需要触发Leave事件
        /// 当LastLeaveTime > HoverStartTime 且 LastHoverTime < LastLeaveTime 时触发
        /// </summary>
        public bool ShouldTriggerLeave => IsHovering;
    }

    /// <summary>
    /// 过渡状态结构体
    /// 用于管理样式过渡动画的状态
    /// </summary>
    public class TransitionState
    {
        /// <summary>
        /// 起始样式
        /// </summary>
        public IDeclStyle FromStyle { get; set; }
        
        /// <summary>
        /// 目标样式
        /// </summary>
        public IDeclStyle ToStyle { get; set; }
        
        /// <summary>
        /// 过渡开始时间
        /// </summary>
        public float StartTime { get; set; }
        
        /// <summary>
        /// 过渡持续时间
        /// </summary>
        public float Duration { get; set; }
        
        /// <summary>
        /// 缓动曲线
        /// </summary>
        public AnimationCurve EasingCurve { get; set; }
        
        /// <summary>
        /// 过渡属性列表
        /// </summary>
        public string[] Properties { get; set; }
        
        /// <summary>
        /// 当前过渡进度（0-1）
        /// </summary>
        public float Progress => Mathf.Clamp01((Time.time - StartTime) / Duration);
        
        /// <summary>
        /// 是否已完成过渡
        /// </summary>
        public bool IsCompleted => Progress >= 1f;
        
        /// <summary>
        /// 缓动后的进度
        /// </summary>
        public float EasedProgress => EasingCurve?.Evaluate(Progress) ?? Progress;
    }

    /// <summary>
    /// 元素状态接口
    /// 用于事件处理的状态管理，包含完整的交互状态管理
    /// </summary>
    public interface IElementState
    {
        /// <summary>
        /// 上次使用该状态的元素类型
        /// 用于确保状态安全，不同类型元素不能共享状态
        /// </summary>
        Type ElementType { get; set; }
        
        /// <summary>
        /// 元素的悬停状态
        /// </summary>
        HoverState HoverState { get; set; }
        
        /// <summary>
        /// 元素的聚焦状态
        /// </summary>
        FocusState FocusState { get; set; }
        
        /// <summary>
        /// 元素的禁用状态
        /// </summary>
        DisabledState DisabledState { get; set; }
        
        /// <summary>
        /// 元素的过渡状态
        /// </summary>
        TransitionState TransitionState { get; set; }
        
        /// <summary>
        /// 通用状态存储
        /// 用于存储任意事件相关的状态数据
        /// </summary>
        object State { get; set; }
        
        /// <summary>
        /// 当前元素状态标志（组合状态）
        /// </summary>
        ElementStateFlags CurrentStateFlags { get; }
        
        /// <summary>
        /// 更新当前状态标志
        /// 根据各个状态的状态值计算组合状态
        /// </summary>
        void UpdateStateFlags();
        
        /// <summary>
        /// 检查是否处于指定状态
        /// </summary>
        /// <param name="stateFlag">要检查的状态标志</param>
        /// <returns>如果处于指定状态则返回true</returns>
        bool HasState(ElementStateFlags stateFlag);
        
        /// <summary>
        /// 检查是否处于任何指定状态之一
        /// </summary>
        /// <param name="stateFlags">要检查的状态标志组合</param>
        /// <returns>如果处于任何指定状态则返回true</returns>
        bool HasAnyState(ElementStateFlags stateFlags);
        
        /// <summary>
        /// 检查是否处于所有指定状态
        /// </summary>
        /// <param name="stateFlags">要检查的状态标志组合</param>
        /// <returns>如果处于所有指定状态则返回true</returns>
        bool HasAllStates(ElementStateFlags stateFlags);
        
        /// <summary>
        /// 设置禁用状态
        /// </summary>
        /// <param name="disabled">是否禁用</param>
        /// <param name="reason">禁用原因（可选）</param>
        /// <param name="isContextControlled">是否由上下文控制</param>
        void SetDisabled(bool disabled, string reason = null, bool isContextControlled = false);
        
        /// <summary>
        /// 设置聚焦状态
        /// </summary>
        /// <param name="focused">是否聚焦</param>
        void SetFocused(bool focused);
        
        /// <summary>
        /// 重置所有交互状态（保留通用状态）
        /// </summary>
        void ResetInteractiveStates();
        
        /// <summary>
        /// 获取当前状态的描述字符串（用于调试）
        /// </summary>
        string GetStateDescription();
    }
}