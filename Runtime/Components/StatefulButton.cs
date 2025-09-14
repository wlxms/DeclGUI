using System;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 有状态按钮组件
    /// 支持点击计数和悬停状态
    /// </summary>
    public struct StatefulButton : IElement<ButtonState>, IEventfulElement
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        public string Key { get; set; }
        public ButtonState State { get; set; }
        public string Text { get; }
        
        public StatefulButton(string text)
        {
            Key = null;
            State = null;
            Text = text;
            Events = new DeclEvent();
        }
        
        public ButtonState CreateState() => new ButtonState();
        
        public IElement Render(ButtonState state)
        {
            // 使用状态数据渲染
            // 在实际渲染器中处理按钮点击和状态更新
            return this;
        }

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
    
    
    /// <summary>
    /// 按钮状态类
    /// </summary>
    public class ButtonState
    {
        public int ClickCount { get; set; }
        public bool IsHovered { get; set; }
        public DateTime LastClickTime { get; set; }
        
        public ButtonState()
        {
            ClickCount = 0;
            IsHovered = false;
            LastClickTime = DateTime.MinValue;
        }
    }
}