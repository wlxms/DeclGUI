using System;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 颜色选择器组件
    /// 用于选择颜色值
    /// </summary>
    public struct ColorField : IElement
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 当前颜色值
        /// </summary>
        public Color Value { get; }

        /// <summary>
        /// 是否显示Alpha通道
        /// </summary>
        public bool ShowAlpha { get; }

        /// <summary>
        /// 值变化回调
        /// </summary>
        public System.Action<Color> OnValueChanged { get; }

        /// <summary>
        /// 样式
        /// </summary>
        public DeclStyle Style { get; }

        /// <summary>
        /// 创建颜色选择器组件
        /// </summary>
        /// <param name="value">当前颜色值</param>
        /// <param name="showAlpha">是否显示Alpha通道</param>
        /// <param name="onValueChanged">值变化回调</param>
        /// <param name="style">样式</param>
        public ColorField(Color value, bool showAlpha = true, System.Action<Color> onValueChanged = null, DeclStyle style = default)
        {
            Value = value;
            ShowAlpha = showAlpha;
            OnValueChanged = onValueChanged;
            Style = style;
            Events = new DeclEvent();
        }

        /// <summary>
        /// 渲染方法
        /// </summary>
        /// <returns>UI元素</returns>
        public IElement Render()
        {
            return null;
        }

        /// <summary>
        /// 设置样式
        /// </summary>
        /// <param name="style">新样式</param>
        /// <returns>带样式的颜色选择器组件</returns>
        public ColorField WithStyle(DeclStyle style)
        {
            return new ColorField(Value, ShowAlpha, OnValueChanged, style);
        }

        /// <summary>
        /// 设置是否显示Alpha通道
        /// </summary>
        /// <param name="showAlpha">是否显示Alpha通道</param>
        /// <returns>带新Alpha设置的颜色选择器组件</returns>
        public ColorField WithShowAlpha(bool showAlpha)
        {
            return new ColorField(Value, showAlpha, OnValueChanged, Style);
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