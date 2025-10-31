using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Buffers;
using DeclGUI.Core;
using static DeclGUI.Core.ArrayPoolHelper;
using UnityEngine;

namespace DeclGUI.Components.Advanced
{
    /// <summary>
    /// 高级Panel组件
    /// 基于Ver布局实现，提供默认样式和交互反馈
    /// </summary>
    public struct Panel : IContainerElement, IEventfulElement, IStylefulElement
    {
        /// <summary>
        /// IStylefulElement 显式实现，返回 IStylefulElement
        /// </summary>
        IStylefulElement IStylefulElement.WithStyle(IDeclStyle style)
        {
            return WithStyle(style);
        }

        /// <summary>
        /// 便于链式调用的 WithStyle，返回 Panel 类型
        /// </summary>
        public Panel WithStyle(IDeclStyle style)
        {
            var newPanel = new Panel(_boxSkin, style, _elements?.Take(_count).ToArray() ?? Array.Empty<IElement>());
            newPanel.Key = Key;
            newPanel.Events = Events;
            newPanel._contextParams = _contextParams;
            return newPanel;
        }
        public string Key { get; set; }
        private IElement[] _elements;
        private int _count;
        private int _capacity;
        private BoxSkin _boxSkin;
        private IContextProvider[] _contextParams;

        /// <summary>
        /// Panel样式
        /// </summary>
        public IDeclStyle Style { get; }

        /// <summary>
        /// 盒模型皮肤样式
        /// </summary>
        public BoxSkin BoxSkin
        {
            get => _boxSkin;
            private set => _boxSkin = value;
        }

        /// <summary>
        /// 事件注册结构
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// 上下文参数列表
        /// </summary>
        public IReadOnlyList<IContextProvider> ContextParams => _contextParams ?? Array.Empty<IContextProvider>();

        /// <summary>
        /// 构造函数 - 使用参数数组
        /// </summary>
        /// <param name="children">子元素</param>
        public Panel(params IElement[] children) : this(BoxSkin.Auto, children)
        {
        }

        /// <summary>
        /// 构造函数 - 使用盒模型皮肤和参数数组
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="children">子元素</param>
        public Panel(BoxSkin boxSkin, params IElement[] children)
        {
            Key = null;
            _elements = null;
            _count = 0;
            _capacity = 0;
            Style = null;
            Events = new DeclEvent();
            _boxSkin = boxSkin;
            _contextParams = null;

            if (children != null && children.Length > 0)
            {
                var validChildren = children.Where(c => c != null).ToArray();
                if (validChildren.Length > 0)
                {
                    InitializeFromArray(ref _elements, ref _capacity, ref _count, validChildren);
                }
            }
        }

        /// <summary>
        /// 构造函数 - 使用样式和参数数组
        /// </summary>
        /// <param name="style">样式</param>
        /// <param name="children">子元素</param>
        public Panel(IDeclStyle style, params IElement[] children) : this(BoxSkin.Auto, style, children)
        {
        }

        /// <summary>
        /// 构造函数 - 使用盒模型皮肤、样式和参数数组
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="style">样式</param>
        /// <param name="children">子元素</param>
        public Panel(BoxSkin boxSkin, IDeclStyle style, params IElement[] children) : this(boxSkin, children)
        {
            Style = style;
        }

        /// <summary>
        /// 构造函数 - 使用集合
        /// </summary>
        /// <param name="children">子元素集合</param>
        /// <param name="style">样式</param>
        public Panel(IEnumerable<IElement> children, IDeclStyle style = null) : this(BoxSkin.Auto, children, style)
        {
        }

        /// <summary>
        /// 构造函数 - 使用盒模型皮肤、集合和样式
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="children">子元素集合</param>
        /// <param name="style">样式</param>
        public Panel(BoxSkin boxSkin, IEnumerable<IElement> children, IDeclStyle style = null)
        {
            Key = null;
            _elements = null;
            _count = 0;
            _capacity = 0;
            Style = style;
            Events = new DeclEvent();
            _boxSkin = boxSkin;
            _contextParams = null;

            if (children != null)
            {
                var validChildren = children.Where(c => c != null).ToArray();
                if (validChildren.Length > 0)
                {
                    InitializeFromArray(ref _elements, ref _capacity, ref _count, validChildren);
                }
            }
        }

        /// <summary>
        /// 支持集合初始化语法
        /// </summary>
        public void Add(IElement element)
        {
            if (element == null) return;

            ArrayPoolHelper.EnsureCapacity(ref _elements, ref _capacity, _count, _count + 1);
            _elements[_count] = element;
            _count++;
        }

        /// <summary>
        /// 渲染方法，返回基于Ver布局的Panel
        /// </summary>
        public IElement Render()
        {
            // 使用DefaultPanel样式，如果用户没有提供自定义样式
            var panelStyle = Style ?? GetDefaultPanelStyle();

            // 创建基于Ver布局的Panel
            return new Ver(panelStyle, _elements?.Take(_count).ToArray() ?? Array.Empty<IElement>());
        }

        /// <summary>
        /// 获取默认Panel样式
        /// </summary>
        private IDeclStyle GetDefaultPanelStyle()
        {
            // 默认Panel样式：浅灰色背景，适当的内边距
            return new DeclStyle(
                backgroundColor: new Color(0.95f, 0.95f, 0.95f, 1f),
                padding: new RectOffset(8, 8, 8, 8),
                borderRadius: 4f
            );
        }

        /// <summary>
        /// 获取枚举器（实现IEnumerable）
        /// </summary>
        public IEnumerator<IElement> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return _elements[i];
            }
        }

        /// <summary>
        /// 获取枚举器（实现IEnumerable）
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ArrayPoolHelper.Dispose(ref _elements, ref _capacity, ref _count);
        }

        /// <summary>
        /// 绑定事件处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="handler">事件处理程序</param>
        public void BindEvent(DeclEventType eventType, Action handler)
        {
            var events = Events;
            events.SetHandler(eventType, handler);
            Events = events;
        }

        /// <summary>
        /// 解绑事件处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        public void UnbindEvent(DeclEventType eventType)
        {
            var events = Events;
            events.SetHandler(eventType, null);
            Events = events;
        }

        /// <summary>
        /// 设置盒模型皮肤样式
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <returns>带有新盒模型皮肤的Panel实例</returns>
        public Panel WithBoxSkin(BoxSkin boxSkin)
        {
            var newPanel = new Panel(boxSkin, Style);
            newPanel.Key = Key;
            newPanel.Events = Events;
            newPanel._contextParams = _contextParams;
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newPanel.Add(_elements[i]);
                }
            }
            return newPanel;
        }

        /// <summary>
        /// 添加上下文参数
        /// </summary>
        /// <param name="contextParams">上下文参数</param>
        /// <returns>带有上下文参数的新容器实例</returns>
        IContainerElement IContainerElement.WithContext(params IContextProvider[] contextParams)
        {
            var newPanel = new Panel(_boxSkin, Style);
            newPanel.Key = Key;
            newPanel.Events = Events;
            newPanel._contextParams = contextParams?.Where(c => c != null).ToArray();
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newPanel.Add(_elements[i]);
                }
            }
            return newPanel;
        }

        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;

        /// <summary>
        /// 添加上下文参数
        /// </summary>
        /// <param name="contextParams">上下文参数</param>
        /// <returns>带有上下文参数的新容器实例</returns>
        public Panel WithContext(params IContextProvider[] contextParams)
        {
            var newPanel = new Panel(_boxSkin, Style);
            newPanel.Key = Key;
            newPanel.Events = Events;
            newPanel._contextParams = contextParams?.Where(c => c != null).ToArray();
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newPanel.Add(_elements[i]);
                }
            }
            return newPanel;
        }


        /// <summary>
        /// 按索引获取子元素
        /// </summary>
        /// <param name="index">子元素索引</param>
        /// <returns>子元素</returns>
        public IElement this[int index] => _elements[index];
    }
}