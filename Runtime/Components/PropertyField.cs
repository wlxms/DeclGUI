using DeclGUI.Core;
using UnityEditor;
using UnityEngine;
using System;

namespace DeclGUI.Components
{
    /// <summary>
    /// Unity 属性字段组件，使用 Unity 的 EditorGUI 渲染任意对象的属性
    /// </summary>
    public struct UnityPropertyField : IElement
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 当前值
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 值变化回调
        /// </summary>
        public Action<object> OnValueChanged { get; }

        /// <summary>
        /// 样式
        /// </summary>
        public DeclStyle Style { get; }

        /// <summary>
        /// 创建 Unity 属性字段组件
        /// </summary>
        /// <param name="value">当前值</param>
        /// <param name="onValueChanged">值变化回调</param>
        /// <param name="style">样式</param>
        public UnityPropertyField(object value, Action<object> onValueChanged = null, DeclStyle style = default)
        {
            Value = value;
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
            // 这个组件需要对应的渲染器来实现实际的渲染逻辑
            // 渲染器将在 EditorRenderManager 中处理实际的 GUI 渲染
            return null;
        }

        /// <summary>
        /// 设置样式
        /// </summary>
        /// <param name="style">新样式</param>
        /// <returns>带样式的组件</returns>
        public UnityPropertyField WithStyle(DeclStyle style)
        {
            return new UnityPropertyField(Value, OnValueChanged, style);
        }
    }
}