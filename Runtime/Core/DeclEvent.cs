using System;

namespace DeclGUI.Core
{
    /// <summary>
    /// 事件注册结构
    /// 使用值类型减少GC，存储事件处理程序
    /// </summary>
    public struct DeclEvent
    {
        public Action OnClick;
        public Action OnPressDown;
        public Action OnPressUp;
        public Action OnHoverEnter;
        public Action OnHoverExit;
        public Action OnDrag;
        public Action OnScroll;
        
        /// <summary>
        /// 获取指定事件类型的处理程序
        /// </summary>
        public Action GetHandler(DeclEventType eventType)
        {
            return eventType switch
            {
                DeclEventType.Click => OnClick,
                DeclEventType.PressDown => OnPressDown,
                DeclEventType.PressUp => OnPressUp,
                DeclEventType.HoverEnter => OnHoverEnter,
                DeclEventType.HoverExit => OnHoverExit,
                DeclEventType.Drag => OnDrag,
                DeclEventType.Scroll => OnScroll,
                _ => null
            };
        }
        
        /// <summary>
        /// 设置指定事件类型的处理程序
        /// </summary>
        public void SetHandler(DeclEventType eventType, Action handler)
        {
            switch (eventType)
            {
                case DeclEventType.Click:
                    OnClick = handler;
                    break;
                case DeclEventType.PressDown:
                    OnPressDown = handler;
                    break;
                case DeclEventType.PressUp:
                    OnPressUp = handler;
                    break;
                case DeclEventType.HoverEnter:
                    OnHoverEnter = handler;
                    break;
                case DeclEventType.HoverExit:
                    OnHoverExit = handler;
                    break;
                case DeclEventType.Drag:
                    OnDrag = handler;
                    break;
                case DeclEventType.Scroll:
                    OnScroll = handler;
                    break;
            }
        }
        
        /// <summary>
        /// 清除所有事件处理程序
        /// </summary>
        public void Clear()
        {
            OnClick = null;
            OnPressDown = null;
            OnPressUp = null;
            OnHoverEnter = null;
            OnHoverExit = null;
            OnDrag = null;
            OnScroll = null;
        }
    }
}