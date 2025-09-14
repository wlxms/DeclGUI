using System;

namespace DeclGUI.Core
{
    /// <summary>
    /// 可处理事件的元素接口
    /// 实现此接口的元素可以接收和处理事件
    /// </summary>
    public interface IEventfulElement : IElementWithKey
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        DeclEvent Events { get; set; }
        
        /// <summary>
        /// 元素唯一标识（用于状态管理）
        /// </summary>
        string Key { get; set; }
        
        /// <summary>
        /// 绑定事件处理器
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="handler">事件处理器</param>
        void BindEvent(DeclEventType eventType, Action handler);
        
        /// <summary>
        /// 解绑事件处理器
        /// </summary>
        /// <param name="eventType">事件类型</param>
        void UnbindEvent(DeclEventType eventType);
    }
}