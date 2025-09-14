using System;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 文本输入框组件
    /// </summary>
    public struct TextField : IElement, IStylefulElement
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        /// <summary>
        /// 元素唯一标识符
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 当前文本值
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 文本变化回调
        /// </summary>
        public Action<string> OnValueChanged { get; }

        /// <summary>
        /// 输入框样式
        /// </summary>
        public IDeclStyle Style { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="onValueChanged">文本变化回调</param>
        /// <param name="style">样式</param>
        public TextField(Action<string> onValueChanged, IDeclStyle style = null)
        {
            Value = string.Empty;
            OnValueChanged = onValueChanged;
            Key = null;
            Value = string.Empty;
            OnValueChanged = onValueChanged;
            Style = style;
            Events = new DeclEvent();
        }

        /// <summary>
        /// 构造函数 - 带初始值
        /// </summary>
        /// <param name="value">初始值</param>
        /// <param name="onValueChanged">文本变化回调</param>
        /// <param name="style">样式</param>
        public TextField(string value, Action<string> onValueChanged, IDeclStyle style = null)
        {
            Key = null;
            Value = value ?? string.Empty;
            OnValueChanged = onValueChanged;
            Style = style;
            Events = new DeclEvent();
        }

        /// <summary>
        /// 渲染方法，返回自身
        /// </summary>
        /// <returns>当前文本输入框实例</returns>
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