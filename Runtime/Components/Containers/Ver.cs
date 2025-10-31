using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Buffers;
using DeclGUI.Core;
using static DeclGUI.Core.ArrayPoolHelper;

namespace DeclGUI.Components
{
    /// <summary>
    /// 垂直布局组件
    /// </summary>
    public struct Ver : IContainerElement, IEventfulElement, IStylefulElement
    {
        public string Key { get; set; }
        public StateManager State { get; set; }
        private IElement[] _elements;
        private int _count;
        private int _capacity;
        private BoxSkin _boxSkin;
        private IContextProvider[] _contextParams;

        /// <summary>
        /// 布局样式
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
        public Ver(params IElement[] children) : this(BoxSkin.Auto, children)
        {
        }

        /// <summary>
        /// 构造函数 - 使用盒模型皮肤和参数数组
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="children">子元素</param>
        public Ver(BoxSkin boxSkin, params IElement[] children)
        {
            Key = null;
            State = null;
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
        public Ver(IDeclStyle style, params IElement[] children) : this(BoxSkin.Auto, style, children)
        {
        }

        /// <summary>
        /// 构造函数 - 使用盒模型皮肤、样式和参数数组
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="style">样式</param>
        /// <param name="children">子元素</param>
        public Ver(BoxSkin boxSkin, IDeclStyle style, params IElement[] children) : this(boxSkin, children)
        {
            Style = style;
        }

        /// <summary>
        /// 构造函数 - 使用集合
        /// </summary>
        /// <param name="children">子元素集合</param>
        /// <param name="style">样式</param>
        public Ver(IEnumerable<IElement> children, IDeclStyle style = null) : this(BoxSkin.Auto, children, style)
        {
        }

        /// <summary>
        /// 构造函数 - 使用盒模型皮肤、集合和样式
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="children">子元素集合</param>
        /// <param name="style">样式</param>
        public Ver(BoxSkin boxSkin, IEnumerable<IElement> children, IDeclStyle style = null)
        {
            Key = null;
            State = null;
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
        /// 构造函数 - 使用上下文参数和子元素
        /// </summary>
        /// <param name="contextParams">上下文参数</param>
        /// <param name="children">子元素</param>
        public Ver(IContextProvider[] contextParams, params IElement[] children) : this(BoxSkin.Auto, contextParams, children)
        {
        }

        /// <summary>
        /// 构造函数 - 使用盒模型皮肤、上下文参数和子元素
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="contextParams">上下文参数</param>
        /// <param name="children">子元素</param>
        public Ver(BoxSkin boxSkin, IContextProvider[] contextParams, params IElement[] children) : this(boxSkin, children)
        {
            _contextParams = contextParams?.Where(c => c != null).ToArray();
        }

        /// <summary>
        /// 构造函数 - 使用样式、上下文参数和子元素
        /// </summary>
        /// <param name="style">样式</param>
        /// <param name="contextParams">上下文参数</param>
        /// <param name="children">子元素</param>
        public Ver(IDeclStyle style, IContextProvider[] contextParams, params IElement[] children) : this(BoxSkin.Auto, style, contextParams, children)
        {
        }

        /// <summary>
        /// 构造函数 - 使用盒模型皮肤、样式、上下文参数和子元素
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="style">样式</param>
        /// <param name="contextParams">上下文参数</param>
        /// <param name="children">子元素</param>
        public Ver(BoxSkin boxSkin, IDeclStyle style, IContextProvider[] contextParams, params IElement[] children) : this(boxSkin, style, children)
        {
            _contextParams = contextParams?.Where(c => c != null).ToArray();
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
        /// 创建容器状态
        /// </summary>
        public StateManager CreateState() => new StateManager();

        /// <summary>
        /// 渲染方法，返回自身
        /// </summary>
        /// <returns>当前垂直布局实例</returns>
        public IElement Render(StateManager state) => this;

        /// <summary>
        /// 渲染方法（无状态参数）
        /// </summary>
        public IElement Render() => null;

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
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;

        /// <summary>
        /// IStylefulElement 显式实现，返回 IStylefulElement
        /// </summary>
        IStylefulElement IStylefulElement.WithStyle(IDeclStyle style)
        {
            return WithStyle(style);
        }

        /// <summary>
        /// 便于链式调用的 WithStyle，返回 Ver 类型
        /// </summary>
        public Ver WithStyle(IDeclStyle style)
        {
            var newVer = new Ver(_boxSkin, style);
            newVer.Key = Key;
            newVer.State = State;
            newVer.Events = Events;
            newVer._contextParams = _contextParams;
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newVer.Add(_elements[i]);
                }


            }
            return newVer;
        }
        /// <summary>
        /// 添加上下文参数
        /// </summary>
        /// <param name="contextParams">上下文参数</param>
        /// <returns>带有上下文参数的新容器实例</returns>
        IContainerElement IContainerElement.WithContext(params IContextProvider[] contextParams)
        {
            var newVer = new Ver(_boxSkin, Style);
            newVer.Key = Key;
            newVer.State = State;
            newVer.Events = Events;
            newVer._contextParams = contextParams?.Where(c => c != null).ToArray();
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newVer.Add(_elements[i]);
                }
            }
            return newVer;
        }

        /// <summary>
        /// 添加上下文参数
        /// </summary>
        /// <param name="contextParams">上下文参数</param>
        /// <returns>带有上下文参数的新容器实例</returns>
        public Ver WithContext(params IContextProvider[] contextParams)
        {
            var newVer = new Ver(_boxSkin, Style);
            newVer.Key = Key;
            newVer.State = State;
            newVer.Events = Events;
            newVer._contextParams = contextParams?.Where(c => c != null).ToArray();
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newVer.Add(_elements[i]);
                }
            }
            return newVer;
        }

        /// <summary>
        /// 设置盒模型皮肤样式
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <returns>带有新盒模型皮肤的Ver实例</returns>
        public Ver WithBoxSkin(BoxSkin boxSkin)
        {
            var newVer = new Ver(boxSkin, Style);
            newVer.Key = Key;
            newVer.State = State;
            newVer.Events = Events;
            newVer._contextParams = _contextParams;
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newVer.Add(_elements[i]);
                }
            }
            return newVer;
        }

        /// <summary>
        /// 按索引获取子元素
        /// </summary>
        /// <param name="index">子元素索引</param>
        /// <returns>子元素</returns>
        public IElement this[int index] => _elements[index];

    }
}