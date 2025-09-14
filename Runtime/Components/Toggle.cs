using System;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 开关/复选框组件
    /// 用于显示和编辑布尔值
    /// </summary>
    public struct Toggle : IElement, IStylefulElement
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
        /// 当前值
        /// </summary>
        public bool Value { get; }
        
        /// <summary>
        /// 值变化回调
        /// </summary>
        public System.Action<bool> OnValueChanged { get; }
        
        /// <summary>
        /// 样式
        /// </summary>
        public IDeclStyle Style { get; }

        /// <summary>
        /// 创建开关组件
        /// </summary>
        /// <param name="value">当前值</param>
        /// <param name="onValueChanged">值变化回调</param>
        /// <param name="style">样式</param>
        public Toggle(bool value, System.Action<bool> onValueChanged = null, IDeclStyle style = null)
        {
            Value = value;
            OnValueChanged = onValueChanged;
            Key = null;
            Value = value;
            OnValueChanged = onValueChanged;
            Style = style;
            Events = new DeclEvent();
        }

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
        /// <returns>带样式的开关组件</returns>
        public Toggle WithStyle(IDeclStyle style)
        {
            return new Toggle(Value, OnValueChanged, style);
        }
        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;
    }
}