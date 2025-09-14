using System;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 按钮组件
    /// </summary>
    public struct Button : IEventfulElement, IStylefulElement
    {
        /// <summary>
        /// 元素唯一标识符
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 按钮显示文本
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 按钮样式
        /// </summary>
        public DeclStyle? Style { get; }

        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 构造函数（兼容旧版本）
        /// </summary>
        /// <param name="text">按钮文本</param>
        /// <param name="onClick">点击事件</param>
        /// <param name="style">样式</param>
        public Button(string text, Action onClick, DeclStyle? style = null)
        {
            this = default; // 先初始化所有字段为默认值
            Key = null;
            Text = text ?? string.Empty;
            Style = style;
            Events = new DeclEvent();
            
            // 将旧的OnClick事件绑定到新的事件系统
            if (onClick != null)
            {
                var events = Events;
                events.OnClick = onClick;
                Events = events;
            }
        }

        /// <summary>
        /// 构造函数（新版本，使用事件系统）
        /// </summary>
        /// <param name="text">按钮文本</param>
        /// <param name="events">事件注册器</param>
        /// <param name="style">样式</param>
        public Button(string text, DeclEvent events = default, DeclStyle? style = null)
        {
            this = default; // 先初始化所有字段为默认值
            Key = null;
            Text = text ?? string.Empty;
            Style = style;
            Events = events;
        }

        /// <summary>
        /// 渲染方法，返回自身
        /// </summary>
        /// <returns>当前按钮实例</returns>
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

        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;
    }
}