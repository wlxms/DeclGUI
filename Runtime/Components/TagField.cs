using System;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 标签选择器组件
    /// 用于选择Unity标签
    /// </summary>
    public struct TagField : IElement
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 当前选中的标签
        /// </summary>
        public string Tag { get; }
        
        /// <summary>
        /// 值变化回调
        /// </summary>
        public System.Action<string> OnValueChanged { get; }
        
        /// <summary>
        /// 样式
        /// </summary>
        public DeclStyle Style { get; }

        /// <summary>
        /// 创建标签选择器组件
        /// </summary>
        /// <param name="tag">当前选中的标签</param>
        /// <param name="onValueChanged">值变化回调</param>
        /// <param name="style">样式</param>
        public TagField(string tag, System.Action<string> onValueChanged = null, DeclStyle style = default)
        {
            Tag = tag;
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
        /// <returns>带样式的标签选择器组件</returns>
        public TagField WithStyle(DeclStyle style)
        {
            return new TagField(Tag, OnValueChanged, style);
        }
    }
}