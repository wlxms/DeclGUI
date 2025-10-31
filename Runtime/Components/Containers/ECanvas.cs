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
    /// 编辑器画布组件 - 本身参与自动布局，但内容物不参与自动布局
    /// 用于在自动布局系统中创建固定位置的子元素
    /// </summary>
    public struct ECanvas : IContainerElement, IEventfulElement, IStylefulElement
    {
        public string Key { get; set; }
        private IElement[] _elements;
        private int _count;
        private int _capacity;
        private BoxSkin _boxSkin;
        private IContextProvider[] _contextParams;

        /// <summary>
        /// 画布样式
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
        public ECanvas(params IElement[] children) : this(BoxSkin.Auto, children)
        {
        }

        /// <summary>
        /// 构造函数 - 使用盒模型皮肤和参数数组
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="children">子元素</param>
        public ECanvas(BoxSkin boxSkin, params IElement[] children)
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
                    ArrayPoolHelper.InitializeFromArray(ref _elements, ref _capacity, ref _count, validChildren);
                }
            }
        }

        /// <summary>
        /// 构造函数 - 使用样式和参数数组
        /// </summary>
        /// <param name="style">样式</param>
        /// <param name="children">子元素</param>
        public ECanvas(IDeclStyle style, params IElement[] children) : this(BoxSkin.Auto, style, children)
        {
        }

        /// <summary>
        /// 构造函数 - 使用盒模型皮肤、样式和参数数组
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="style">样式</param>
        /// <param name="children">子元素</param>
        public ECanvas(BoxSkin boxSkin, IDeclStyle style, params IElement[] children) : this(boxSkin, children)
        {
            Style = style;
        }

        /// <summary>
        /// 构造函数 - 使用集合
        /// </summary>
        /// <param name="children">子元素集合</param>
        /// <param name="style">样式</param>
        public ECanvas(IEnumerable<IElement> children, IDeclStyle style = null) : this(BoxSkin.Auto, children, style)
        {
        }

        /// <summary>
        /// 构造函数 - 使用盒模型皮肤、集合和样式
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="children">子元素集合</param>
        /// <param name="style">样式</param>
        public ECanvas(BoxSkin boxSkin, IEnumerable<IElement> children, IDeclStyle style = null)
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
                    ArrayPoolHelper.InitializeFromArray(ref _elements, ref _capacity, ref _count, validChildren);
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
        /// 渲染方法
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
        /// IStylefulElement 接口实现 - 设置样式
        /// </summary>
        /// <param name="style">新样式</param>
        /// <returns>带样式的元素</returns>
        IStylefulElement IStylefulElement.WithStyle(IDeclStyle style)
        {
            // 获取当前ECanvas中的元素数组并创建新的ECanvas实例
            var elements = new IElement[_count];
            for (int i = 0; i < _count; i++)
            {
                elements[i] = _elements[i];
            }
            return new ECanvas(_boxSkin, style, elements);
        }

        /// <summary>
        /// 设置盒模型皮肤样式
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <returns>带有新盒模型皮肤的ECanvas实例</returns>
        public ECanvas WithBoxSkin(BoxSkin boxSkin)
        {
            var newECanvas = new ECanvas(boxSkin, Style);
            newECanvas.Key = Key;
            newECanvas.Events = Events;
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newECanvas.Add(_elements[i]);
                }
            }
            return newECanvas;
        }

        /// <summary>
        /// 添加上下文参数
        /// </summary>
        /// <param name="contextParams">上下文参数</param>
        /// <returns>带有上下文参数的新容器实例</returns>
        IContainerElement IContainerElement.WithContext(params IContextProvider[] contextParams)
        {
            var newECanvas = new ECanvas(_boxSkin, Style);
            newECanvas.Key = Key;
            newECanvas.Events = Events;
            newECanvas._contextParams = contextParams?.Where(c => c != null).ToArray();
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newECanvas.Add(_elements[i]);
                }
            }
            return newECanvas;
        }

        /// <summary>
        /// 添加上下文参数
        /// </summary>
        /// <param name="contextParams">上下文参数</param>
        /// <returns>带有上下文参数的新容器实例</returns>
        public ECanvas WithContext(params IContextProvider[] contextParams)
        {
            var newECanvas = new ECanvas(_boxSkin, Style);
            newECanvas.Key = Key;
            newECanvas.Events = Events;
            newECanvas._contextParams = contextParams?.Where(c => c != null).ToArray();
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newECanvas.Add(_elements[i]);
                }
            }
            return newECanvas;
        }

        /// <summary>
        /// 按索引获取子元素
        /// </summary>
        /// <param name="index">子元素索引</param>
        /// <returns>子元素</returns>
        public IElement this[int index] => _elements[index];
    }
}