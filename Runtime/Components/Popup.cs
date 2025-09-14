using System;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 下拉选择框组件
    /// 用于从选项列表中选择值
    /// </summary>
    public struct Popup : IElement, IStylefulElement
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
        /// 当前选中的索引
        /// </summary>
        public int SelectedIndex { get; }

        /// <summary>
        /// 选项列表
        /// </summary>
        public string[] Options { get; }

        /// <summary>
        /// 选择变化回调
        /// </summary>
        public System.Action<int> OnSelectionChanged { get; }

        /// <summary>
        /// 样式
        /// </summary>
        public IDeclStyle Style { get; }

        /// <summary>
        /// 创建下拉选择框组件
        /// </summary>
        /// <param name="selectedIndex">当前选中的索引</param>
        /// <param name="options">选项列表</param>
        /// <param name="onSelectionChanged">选择变化回调</param>
        /// <param name="style">样式</param>
        public Popup(int selectedIndex, string[] options,
                    System.Action<int> onSelectionChanged = null, IDeclStyle style = null)
        {
            Key = null;
            SelectedIndex = selectedIndex;
            Options = options;
            OnSelectionChanged = onSelectionChanged;
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
        /// <returns>带样式的下拉选择框组件</returns>
        public Popup WithStyle(IDeclStyle style)
        {
            return new Popup(SelectedIndex, Options, OnSelectionChanged, style);
        }

        /// <summary>
        /// 设置选项列表
        /// </summary>
        /// <param name="options">新选项列表</param>
        /// <returns>带新选项列表的下拉选择框组件</returns>
        public Popup WithOptions(string[] options)
        {
            return new Popup(SelectedIndex, options, OnSelectionChanged, Style);
        }
        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;

    }

}