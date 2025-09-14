using System;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// Vector2输入字段组件
    /// 用于输入二维向量值
    /// </summary>
    public struct Vector2Field : IElement
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 当前Vector2值
        /// </summary>
        public Vector2 Value { get; }
        
        /// <summary>
        /// 值变化回调
        /// </summary>
        public System.Action<Vector2> OnValueChanged { get; }
        
        /// <summary>
        /// 样式
        /// </summary>
        public DeclStyle Style { get; }

        /// <summary>
        /// 创建Vector2输入字段组件
        /// </summary>
        /// <param name="value">当前Vector2值</param>
        /// <param name="onValueChanged">值变化回调</param>
        /// <param name="style">样式</param>
        public Vector2Field(Vector2 value, System.Action<Vector2> onValueChanged = null, DeclStyle style = default)
        {
            Value = value;
            OnValueChanged = onValueChanged;
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
        /// <returns>带样式的Vector2输入字段组件</returns>
        public Vector2Field WithStyle(DeclStyle style)
        {
            return new Vector2Field(Value, OnValueChanged, style);
        }
    }
}