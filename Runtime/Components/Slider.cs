using System;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 滑块组件
    /// 原子化的滑块控件，只负责显示和编辑浮点数值
    /// </summary>
    public struct Slider : IElement, IStylefulElement
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
        public float Value { get; }
        
        /// <summary>
        /// 最小值
        /// </summary>
        public float MinValue { get; }
        
        /// <summary>
        /// 最大值
        /// </summary>
        public float MaxValue { get; }
        
        /// <summary>
        /// 值变化回调
        /// </summary>
        public System.Action<float> OnValueChanged { get; }
        
        /// <summary>
        /// 样式
        /// </summary>
        public IDeclStyle Style { get; }

        /// <summary>
        /// 创建滑块组件
        /// </summary>
        /// <param name="value">当前值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="onValueChanged">值变化回调</param>
        /// <param name="style">样式</param>
        public Slider(float value, float minValue, float maxValue, 
                     System.Action<float> onValueChanged = null, IDeclStyle style = null)
        {
            Key = null;
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
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
        /// <returns>带样式的滑块组件</returns>
        public Slider WithStyle(IDeclStyle style)
        {
            return new Slider(Value, MinValue, MaxValue, OnValueChanged, style);
        }

        /// <summary>
        /// 设置值范围
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>带新值范围的滑块组件</returns>
        public Slider WithRange(float min, float max)
        {
            return new Slider(Value, min, max, OnValueChanged, Style);
        }
        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;
    }
}