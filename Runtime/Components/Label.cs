using System;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 标签组件
    /// </summary>
    public struct Label : IEventfulElement, IStylefulElement
    {
        /// <summary>
        /// 元素唯一标识符
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 标签显示文本
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 标签样式
        /// </summary>
        public IDeclStyle Style { get; }
        
        /// <summary>
        /// 事件注册结构
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="text">标签文本</param>
        /// <param name="style">样式</param>
        public Label(string text, IDeclStyle style = null)
        {
            Key = null;
            Text = text ?? string.Empty;
            Style = style;
            Events = new DeclEvent();
        }

        /// <summary>
        /// 渲染方法，返回自身
        /// </summary>
        /// <returns>当前标签实例</returns>
        public IElement Render() => null;
        
        /// <summary>
        /// 绑定事件处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="handler">事件处理程序</param>
        public void BindEvent(DeclEventType eventType, Action handler)
        {
            Events.SetHandler(eventType, handler);
        }
        
        /// <summary>
        /// 解绑事件处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        public void UnbindEvent(DeclEventType eventType)
        {
            Events.SetHandler(eventType, null);
        }

        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;
    }
}