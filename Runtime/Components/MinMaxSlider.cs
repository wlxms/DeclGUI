using System;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 最小-最大范围滑块组件
    /// 用于选择数值范围
    /// </summary>
    public struct MinMaxSlider : IElement
    {
        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 当前最小值
        /// </summary>
        public float MinValue { get; }
        
        /// <summary>
        /// 当前最大值
        /// </summary>
        public float MaxValue { get; }
        
        /// <summary>
        /// 允许的最小值限制
        /// </summary>
        public float MinLimit { get; }
        
        /// <summary>
        /// 允许的最大值限制
        /// </summary>
        public float MaxLimit { get; }
        
        /// <summary>
        /// 值变化回调
        /// </summary>
        public System.Action<float, float> OnValueChanged { get; }
        
        /// <summary>
        /// 样式
        /// </summary>
        public DeclStyle Style { get; }

        /// <summary>
        /// 创建最小-最大范围滑块组件
        /// </summary>
        /// <param name="minValue">当前最小值</param>
        /// <param name="maxValue">当前最大值</param>
        /// <param name="minLimit">允许的最小值限制</param>
        /// <param name="maxLimit">允许的最大值限制</param>
        /// <param name="onValueChanged">值变化回调</param>
        /// <param name="style">样式</param>
        public MinMaxSlider(float minValue, float maxValue, float minLimit, float maxLimit, 
                          System.Action<float, float> onValueChanged = null, DeclStyle style = default)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            MinLimit = minLimit;
            MaxLimit = maxLimit;
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
        /// <returns>带样式的最小-最大范围滑块组件</returns>
        public MinMaxSlider WithStyle(DeclStyle style)
        {
            return new MinMaxSlider(MinValue, MaxValue, MinLimit, MaxLimit, OnValueChanged, style);
        }
    }
}