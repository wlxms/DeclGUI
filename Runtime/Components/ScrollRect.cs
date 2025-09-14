using System;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 滚动视图容器组件
    /// 用于创建可滚动的UI区域
    /// </summary>
    public struct ScrollRect : IElement
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 滚动位置
        /// </summary>
        public Vector2 ScrollPosition { get; }
        
        /// <summary>
        /// 内容元素
        /// </summary>
        public IElement Content { get; }
        
        /// <summary>
        /// 滚动位置变化回调
        /// </summary>
        public System.Action<Vector2> OnScroll { get; }
        
        /// <summary>
        /// 是否总是显示垂直滚动条
        /// </summary>
        public bool AlwaysShowVertical { get; }
        
        /// <summary>
        /// 是否总是显示水平滚动条
        /// </summary>
        public bool AlwaysShowHorizontal { get; }
        
        /// <summary>
        /// 样式
        /// </summary>
        public DeclStyle Style { get; }

        /// <summary>
        /// 创建滚动视图容器
        /// </summary>
        /// <param name="scrollPosition">滚动位置</param>
        /// <param name="content">内容元素</param>
        /// <param name="onScroll">滚动位置变化回调</param>
        /// <param name="alwaysShowVertical">是否总是显示垂直滚动条</param>
        /// <param name="alwaysShowHorizontal">是否总是显示水平滚动条</param>
        /// <param name="style">样式</param>
        public ScrollRect(Vector2 scrollPosition, IElement content, 
                         System.Action<Vector2> onScroll = null,
                         bool alwaysShowVertical = false, bool alwaysShowHorizontal = false,
                         DeclStyle style = default)
        {
            ScrollPosition = scrollPosition;
            Content = content;
            OnScroll = onScroll;
            AlwaysShowVertical = alwaysShowVertical;
            AlwaysShowHorizontal = alwaysShowHorizontal;
            Style = style;
                    Events = new DeclEvent();}

        /// <summary>
        /// 渲染方法
        /// </summary>
        /// <returns>UI元素</returns>
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
        /// 设置样式
        /// </summary>
        /// <param name="style">新样式</param>
        /// <returns>带样式的滚动视图容器</returns>
        public ScrollRect WithStyle(DeclStyle style)
        {
            return new ScrollRect(ScrollPosition, Content, OnScroll, 
                                AlwaysShowVertical, AlwaysShowHorizontal, style);
        }

        /// <summary>
        /// 设置滚动条显示选项
        /// </summary>
        /// <param name="showVertical">显示垂直滚动条</param>
        /// <param name="showHorizontal">显示水平滚动条</param>
        /// <returns>带新设置的滚动视图容器</returns>
        public ScrollRect WithScrollbars(bool showVertical, bool showHorizontal)
        {
            return new ScrollRect(ScrollPosition, Content, OnScroll, 
                                showVertical, showHorizontal, Style);
        }
    }
}