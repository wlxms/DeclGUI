using System;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 间距组件
    /// </summary>
    public struct Spc : IElement
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 间距大小
        /// </summary>
        public float Size { get; }

        /// <summary>
        /// 间距样式
        /// </summary>
        public DeclStyle? Style { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="size">间距大小</param>
        /// <param name="style">样式</param>
        public Spc(float size, DeclStyle? style = null)
        {
            Size = size;
            Style = style;
                    Events = new DeclEvent();}

        /// <summary>
        /// 渲染方法，返回自身
        /// </summary>
        /// <returns>当前间距实例</returns>
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