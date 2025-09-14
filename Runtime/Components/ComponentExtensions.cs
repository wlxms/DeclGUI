using System;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 组件扩展方法
    /// </summary>
    public static class ComponentExtensions
    {
        
        /// <summary>
        /// 设置长按回调并返回元素本身，用于流畅API
        /// </summary>
        public static LongPressButton WithLongPress(this LongPressButton button, Action onLongPress, float longPressDuration = 1.0f)
        {
            // 由于LongPressButton是结构体，我们需要创建一个新的实例
            return new LongPressButton(button.Text, onLongPress, longPressDuration)
            {
                Key = button.Key,
                State = button.State,
                Events = button.Events
            };
        }
        
        /// <summary>
        /// 绑定悬停进入事件并返回元素本身，用于流畅API
        /// </summary>
        public static T WithHoverEnter<T>(this T element, Action handler) where T : IEventfulElement
        {
            element.BindEvent(DeclEventType.HoverEnter, handler);
            return element;
        }
        
        /// <summary>
        /// 绑定悬停退出事件并返回元素本身，用于流畅API
        /// </summary>
        public static T WithHoverExit<T>(this T element, Action handler) where T : IEventfulElement
        {
            element.BindEvent(DeclEventType.HoverExit, handler);
            return element;
        }
        
        /// <summary>
        /// 绑定点击事件并返回元素本身，用于流畅API
        /// </summary>
        public static T WithClick<T>(this T element, Action handler) where T : IEventfulElement
        {
            element.BindEvent(DeclEventType.Click, handler);
            return element;
        }
    }
}