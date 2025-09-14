using System;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 长按按钮组件
    /// 支持长按检测和按压进度显示
    /// </summary>
    public struct LongPressButton : IElement<LongPressButtonState>, IEventfulElement
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        public string Key { get; set; }
        public LongPressButtonState State { get; set; }
        public string Text { get; }
        public Action OnLongPress { get; }
        public float LongPressDuration { get; }
        
        public LongPressButton(string text, Action onLongPress, float longPressDuration = 1.0f)
        {
            Key = null;
            State = null;
            Text = text;
            OnLongPress = onLongPress;
            LongPressDuration = longPressDuration;
            Events = new DeclEvent();
        }
        
        public LongPressButton(string text)
        {
            Key = null;
            State = null;
            Text = text;
            OnLongPress = null;
            LongPressDuration = 1.0f;
            Events = new DeclEvent();
        }
        
        public LongPressButtonState CreateState() => new LongPressButtonState();
        
        public IElement Render(LongPressButtonState state)
        {
            // 使用状态数据渲染
            // 在实际渲染器中处理长按检测和状态更新
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
    /// 长按按钮状态类
    /// </summary>
    public class LongPressButtonState
    {
        public bool IsPressing { get; set; }
        public float PressTime { get; set; }
        public bool WasLongPressed { get; set; }
        public DateTime LastPressTime { get; set; }
        
        public LongPressButtonState()
        {
            IsPressing = false;
            PressTime = 0f;
            WasLongPressed = false;
            LastPressTime = DateTime.MinValue;
        }
    }
}