using System;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 元素状态枚举
    /// 定义元素的各种交互状态
    /// </summary>
    [Flags]
    public enum ElementStateFlags
    {
        /// <summary>
        /// 正常状态
        /// </summary>
        Normal = 0,
        
        /// <summary>
        /// 悬停状态
        /// </summary>
        Hover = 1 << 0,
        
        /// <summary>
        /// 激活状态（如按钮按下）
        /// </summary>
        Active = 1 << 1,
        
        /// <summary>
        /// 聚焦状态
        /// </summary>
        Focus = 1 << 2,
        
        /// <summary>
        /// 禁用状态
        /// </summary>
        Disabled = 1 << 3,
        
        /// <summary>
        /// 选中状态
        /// </summary>
        Selected = 1 << 4
    }

    /// <summary>
    /// 聚焦状态结构体
    /// 用于管理聚焦事件的时间状态
    /// </summary>
    public class FocusState
    {
        /// <summary>
        /// 聚焦开始时间（Time.time）
        /// 当元素获得焦点时设置
        /// </summary>
        public float FocusStartTime { get; set; }
        
        /// <summary>
        /// 最后一次失去焦点时间（Time.time）
        /// 当元素失去焦点时设置
        /// </summary>
        public float LastBlurTime { get; set; }
        
        /// <summary>
        /// 是否当前正在聚焦
        /// FocusStartTime > LastBlurTime 表示正在聚焦
        /// </summary>
        public bool IsFocused { get; set; }
        
        /// <summary>
        /// 聚焦持续时间（如果正在聚焦）
        /// </summary>
        public float FocusDuration => IsFocused ? (Time.time - FocusStartTime) : 0f;
        
        /// <summary>
        /// 是否需要触发Focus事件
        /// 当FocusStartTime > LastBlurTime 且 IsFocused为false时触发
        /// </summary>
        public bool ShouldTriggerFocus => !IsFocused && FocusStartTime > LastBlurTime;
        
        /// <summary>
        /// 是否需要触发Blur事件
        /// 当LastBlurTime > FocusStartTime 且 IsFocused为true时触发
        /// </summary>
        public bool ShouldTriggerBlur => IsFocused && LastBlurTime > FocusStartTime;
    }

    /// <summary>
    /// 禁用状态结构体
    /// 用于管理元素的禁用状态
    /// </summary>
    public class DisabledState
    {
        /// <summary>
        /// 是否被禁用
        /// </summary>
        public bool IsDisabled { get; set; }
        
        /// <summary>
        /// 禁用原因（可选，用于调试和状态追踪）
        /// </summary>
        public string Reason { get; set; }
        
        /// <summary>
        /// 禁用开始时间（Time.time）
        /// </summary>
        public float DisabledStartTime { get; set; }
        
        /// <summary>
        /// 禁用持续时间
        /// </summary>
        public float DisabledDuration => IsDisabled ? (Time.time - DisabledStartTime) : 0f;
        
        /// <summary>
        /// 禁用状态是否由外部上下文控制
        /// 如果为true，表示禁用状态由外部逻辑管理
        /// </summary>
        public bool IsContextControlled { get; set; }
    }

    /// <summary>
    /// 默认元素状态实现
    /// 提供IElementState接口的基本实现，包含完整的交互状态管理
    /// </summary>
    public class ElementState : IElementState
    {
        /// <summary>
        /// 上次使用该状态的元素类型
        /// 用于确保状态安全，不同类型元素不能共享状态
        /// </summary>
        public Type ElementType { get; set; }
        
        /// <summary>
        /// 元素的悬停状态
        /// </summary>
        public HoverState HoverState { get; set; }
        
        /// <summary>
        /// 元素的聚焦状态
        /// </summary>
        public FocusState FocusState { get; set; }
        
        /// <summary>
        /// 元素的禁用状态
        /// </summary>
        public DisabledState DisabledState { get; set; }
        
        /// <summary>
        /// 元素的过渡状态
        /// </summary>
        public TransitionState TransitionState { get; set; }
        
        /// <summary>
        /// 通用状态存储
        /// 用于存储任意事件相关的状态数据
        /// </summary>
        public object State { get; set; }
        
        /// <summary>
        /// 当前元素状态标志（组合状态）
        /// </summary>
        public ElementStateFlags CurrentStateFlags { get; private set; }
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ElementState()
        {
            // 保持原有的HoverState初始化，确保向后兼容
            HoverState = new HoverState();
            FocusState = new FocusState();
            DisabledState = new DisabledState();
            TransitionState = new TransitionState();
            CurrentStateFlags = ElementStateFlags.Normal;
        }
        
        /// <summary>
        /// 更新当前状态标志
        /// 根据各个状态的状态值计算组合状态
        /// </summary>
        public void UpdateStateFlags()
        {
            ElementStateFlags flags = ElementStateFlags.Normal;
            
            // 禁用状态具有最高优先级
            if (DisabledState.IsDisabled)
            {
                flags |= ElementStateFlags.Disabled;
                // 禁用状态下，忽略其他交互状态
                CurrentStateFlags = flags;
                return;
            }
            
            // 悬停状态
            if (HoverState.IsHovering)
            {
                flags |= ElementStateFlags.Hover;
            }
            
            // 聚焦状态
            if (FocusState.IsFocused)
            {
                flags |= ElementStateFlags.Focus;
            }
            
            // 激活状态（通过通用状态存储判断）
            if (State is bool isActive && isActive)
            {
                flags |= ElementStateFlags.Active;
            }
            
            CurrentStateFlags = flags;
        }
        
        /// <summary>
        /// 设置禁用状态
        /// </summary>
        /// <param name="disabled">是否禁用</param>
        /// <param name="reason">禁用原因（可选）</param>
        /// <param name="isContextControlled">是否由上下文控制</param>
        public void SetDisabled(bool disabled, string reason = null, bool isContextControlled = false)
        {
            DisabledState.IsDisabled = disabled;
            DisabledState.Reason = reason;
            DisabledState.IsContextControlled = isContextControlled;
            
            if (disabled)
            {
                DisabledState.DisabledStartTime = Time.time;
                
                // 禁用时自动失去焦点
                if (FocusState.IsFocused)
                {
                    FocusState.LastBlurTime = Time.time;
                    FocusState.IsFocused = false;
                }
                
                // 禁用时自动退出悬停
                if (HoverState.IsHovering)
                {
                    HoverState.LastLeaveTime = Time.time;
                    HoverState.IsHovering = false;
                }
            }
            
            UpdateStateFlags();
        }
        
        /// <summary>
        /// 设置聚焦状态
        /// </summary>
        /// <param name="focused">是否聚焦</param>
        public void SetFocused(bool focused)
        {
            // 如果元素被禁用，不允许获得焦点
            if (DisabledState.IsDisabled && focused)
            {
                return;
            }
            
            if (focused)
            {
                FocusState.FocusStartTime = Time.time;
                FocusState.IsFocused = true;
            }
            else
            {
                FocusState.LastBlurTime = Time.time;
                FocusState.IsFocused = false;
            }
            
            UpdateStateFlags();
        }
        
        /// <summary>
        /// 检查是否处于指定状态
        /// </summary>
        /// <param name="stateFlag">要检查的状态标志</param>
        /// <returns>如果处于指定状态则返回true</returns>
        public bool HasState(ElementStateFlags stateFlag)
        {
            return (CurrentStateFlags & stateFlag) == stateFlag;
        }
        
        /// <summary>
        /// 检查是否处于任何指定状态之一
        /// </summary>
        /// <param name="stateFlags">要检查的状态标志组合</param>
        /// <returns>如果处于任何指定状态则返回true</returns>
        public bool HasAnyState(ElementStateFlags stateFlags)
        {
            return (CurrentStateFlags & stateFlags) != ElementStateFlags.Normal;
        }
        
        /// <summary>
        /// 检查是否处于所有指定状态
        /// </summary>
        /// <param name="stateFlags">要检查的状态标志组合</param>
        /// <returns>如果处于所有指定状态则返回true</returns>
        public bool HasAllStates(ElementStateFlags stateFlags)
        {
            return (CurrentStateFlags & stateFlags) == stateFlags;
        }
        
        /// <summary>
        /// 重置所有交互状态（保留通用状态）
        /// </summary>
        public void ResetInteractiveStates()
        {
            HoverState = new HoverState();
            FocusState = new FocusState();
            DisabledState = new DisabledState();
            TransitionState = new TransitionState();
            UpdateStateFlags();
        }
        
        /// <summary>
        /// 获取当前状态的描述字符串（用于调试）
        /// </summary>
        public string GetStateDescription()
        {
            if (DisabledState.IsDisabled)
            {
                return $"Disabled{(string.IsNullOrEmpty(DisabledState.Reason) ? "" : $" ({DisabledState.Reason})")}";
            }
            
            var states = new System.Collections.Generic.List<string>();
            
            if (HasState(ElementStateFlags.Hover)) states.Add("Hover");
            if (HasState(ElementStateFlags.Focus)) states.Add("Focus");
            if (HasState(ElementStateFlags.Active)) states.Add("Active");
            if (HasState(ElementStateFlags.Selected)) states.Add("Selected");
            
            return states.Count > 0 ? string.Join(" + ", states) : "Normal";
        }
    }
}