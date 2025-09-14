using System;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 可伸缩空白空间组件
    /// 用于在布局中创建可伸缩的空白空间，类似于GUILayout.FlexibleSpace()
    /// </summary>
    public struct FixableSpace : IElement
    {
        /// <summary>
        /// 空间样式
        /// </summary>
        public DeclStyle? Style { get; }

        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="style">样式</param>
        public FixableSpace(DeclStyle? style = null)
        {
            Style = style;
            Events = new DeclEvent();
        }

        /// <summary>
        /// 渲染方法，返回自身
        /// </summary>
        /// <returns>当前空间实例</returns>
        public IElement Render() => null;

        /// <summary>
        /// 绑定事件处理器
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="handler">事件处理器</param>
        public void BindEvent(DeclEventType eventType, Action handler)
        {
            var events = Events;
            events.SetHandler(eventType, handler);
            Events = events;
        }

        /// <summary>
        /// 解绑事件处理器
        /// </summary>
        /// <param name="eventType">事件类型</param>
        public void UnbindEvent(DeclEventType eventType)
        {
            var events = Events;
            events.SetHandler(eventType, null);
            Events = events;
        }
    }
}