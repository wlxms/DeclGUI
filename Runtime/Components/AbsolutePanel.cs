using System;
using System.Collections.Generic;
using System.Linq;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 绝对定位面板组件
    /// 不参与自动布局，但可以使用自动布局系统渲染内容物
    /// 支持自动大小、指定大小、最大最小大小限制
    /// </summary>
    public struct AbsolutePanel : IElement
    {
        /// <summary>
        /// 面板位置
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        /// 子元素
        /// </summary>
        public IElement Child { get; }

        /// <summary>
        /// 面板样式
        /// </summary>
        public DeclStyle? Style { get; }

        /// <summary>
        /// 最小宽度
        /// </summary>
        public float? MinWidth { get; }

        /// <summary>
        /// 最小高度
        /// </summary>
        public float? MinHeight { get; }

        /// <summary>
        /// 最大宽度
        /// </summary>
        public float? MaxWidth { get; }

        /// <summary>
        /// 最大高度
        /// </summary>
        public float? MaxHeight { get; }

        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 构造函数 - 基本版本
        /// </summary>
        /// <param name="position">面板位置</param>
        /// <param name="child">子元素</param>
        /// <param name="style">样式</param>
        public AbsolutePanel(Vector2 position, IElement child, DeclStyle? style = null)
        {
            Position = position;
            Child = child;
            Style = style;
            MinWidth = null;
            MinHeight = null;
            MaxWidth = null;
            MaxHeight = null;
            Events = new DeclEvent();
        }

        /// <summary>
        /// 构造函数 - 完整版本
        /// </summary>
        /// <param name="position">面板位置</param>
        /// <param name="child">子元素</param>
        /// <param name="style">样式</param>
        /// <param name="minWidth">最小宽度</param>
        /// <param name="minHeight">最小高度</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public AbsolutePanel(Vector2 position, IElement child, DeclStyle? style = null,
                           float? minWidth = null, float? minHeight = null,
                           float? maxWidth = null, float? maxHeight = null)
        {
            Position = position;
            Child = child;
            Style = style;
            MinWidth = minWidth;
            MinHeight = minHeight;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            Events = new DeclEvent();
        }

        /// <summary>
        /// 设置最小尺寸
        /// </summary>
        public AbsolutePanel WithMinSize(float minWidth, float minHeight)
        {
            return new AbsolutePanel(Position, Child, Style, minWidth, minHeight, MaxWidth, MaxHeight);
        }

        /// <summary>
        /// 设置最大尺寸
        /// </summary>
        public AbsolutePanel WithMaxSize(float maxWidth, float maxHeight)
        {
            return new AbsolutePanel(Position, Child, Style, MinWidth, MinHeight, maxWidth, maxHeight);
        }

        /// <summary>
        /// 设置尺寸限制
        /// </summary>
        public AbsolutePanel WithSizeConstraints(float? minWidth = null, float? minHeight = null,
                                               float? maxWidth = null, float? maxHeight = null)
        {
            return new AbsolutePanel(Position, Child, Style, minWidth, minHeight, maxWidth, maxHeight);
        }

        /// <summary>
        /// 渲染方法，返回自身
        /// </summary>
        /// <returns>当前绝对定位面板实例</returns>
        public IElement Render()
        {
            return null;
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
}