using System;

namespace DeclGUI.Core
{
    /// <summary>
    /// 默认元素状态实现
    /// 提供IElementState接口的基本实现
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
        /// 通用状态存储
        /// 用于存储任意事件相关的状态数据
        /// </summary>
        public object State { get; set; }
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ElementState()
        {
            HoverState = new HoverState();
        }
    }
}