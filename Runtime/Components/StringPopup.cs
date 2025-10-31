using System;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 字符串列表下拉选择框组件
    /// 用于从字符串数组中选择值
    /// </summary>
    public struct StringPopup : IElement
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 当前选中的索引
        /// </summary>
        public int SelectedIndex { get; }

        /// <summary>
        /// 值变化回调
        /// </summary>
        public Action<int> OnValueChanged { get; }

        /// <summary>
        /// 选项数组
        /// </summary>
        public string[] Options { get; }

        /// <summary>
        /// 样式
        /// </summary>
        public DeclStyle Style { get; }

        /// <summary>
        /// 创建字符串列表下拉选择框组件
        /// </summary>
        /// <param name="selectedIndex">当前选中的索引</param>
        /// <param name="onValueChanged">值变化回调</param>
        /// <param name="options">选项数组</param>
        /// <param name="style">样式</param>
        public StringPopup(int selectedIndex, Action<int> onValueChanged = null, string[] options = null, DeclStyle style = default)
        {
            SelectedIndex = selectedIndex;
            OnValueChanged = onValueChanged;
            Options = options ?? new string[0];
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
        /// <returns>带样式的字符串列表下拉选择框组件</returns>
        public StringPopup WithStyle(DeclStyle style)
        {
            return new StringPopup(SelectedIndex, OnValueChanged, Options, style);
        }
    }
}