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
    /// 元素状态接口
    /// 用于事件处理的状态管理，包含悬停状态和通用状态存储
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
        /// 通用状态存储
        /// 用于存储任意事件相关的状态数据
        /// </summary>
        object State { get; set; }
    }
}