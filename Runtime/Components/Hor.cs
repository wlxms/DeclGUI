using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Buffers;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 水平布局组件
    /// </summary>
    public struct Hor : IContainerElement, IEventfulElement, IStylefulElement
    {
        public string Key { get; set; }
        private IElement[] _elements;
        private int _count;
        private int _capacity;
        private BoxSkin _boxSkin;

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
        /// 构造函数 - 使用参数数组
        /// </summary>
        /// <param name="children">子元素</param>
        public Hor(params IElement[] children) : this(BoxSkin.Auto, children)
        {
        }
        
        /// <summary>
        /// 构造函数 - 使用盒模型皮肤和参数数组
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="children">子元素</param>
        public Hor(BoxSkin boxSkin, params IElement[] children)
        {
            Key = null;
            _elements = null;
            _count = 0;
            _capacity = 0;
            Style = null;
            Events = new DeclEvent();
            _boxSkin = boxSkin;

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
        public Hor(IDeclStyle style, params IElement[] children) : this(BoxSkin.Auto, style, children)
        {
        }
        
        /// <summary>
        /// 构造函数 - 使用盒模型皮肤、样式和参数数组
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="style">样式</param>
        /// <param name="children">子元素</param>
        public Hor(BoxSkin boxSkin, IDeclStyle style, params IElement[] children) : this(boxSkin, children)
        {
            Style = style;
        }

        /// <summary>
        /// 构造函数 - 使用集合
        /// </summary>
        /// <param name="children">子元素集合</param>
        /// <param name="style">样式</param>
        public Hor(IEnumerable<IElement> children, IDeclStyle style = null) : this(BoxSkin.Auto, children, style)
        {
        }
        
        /// <summary>
        /// 构造函数 - 使用盒模型皮肤、集合和样式
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="children">子元素集合</param>
        /// <param name="style">样式</param>
        public Hor(BoxSkin boxSkin, IEnumerable<IElement> children, IDeclStyle style = null)
        {
            Key = null;
            _elements = null;
            _count = 0;
            _capacity = 0;
            Style = style;
            Events = new DeclEvent();
            _boxSkin = boxSkin;

            if (children != null)
            {
                var validChildren = children.Where(c => c != null).ToArray();
                if (validChildren.Length > 0)
                {
                    EnsureCapacity(validChildren.Length);
                    Array.Copy(validChildren, _elements, validChildren.Length);
                    _count = validChildren.Length;
                }
            }
        }

        /// <summary>
        /// 支持集合初始化语法
        /// </summary>
        public void Add(IElement element)
        {
            if (element == null) return;

            EnsureCapacity(_count + 1);
            _elements[_count] = element;
            _count++;
        }

        /// <summary>
        /// 确保有足够的容量
        /// </summary>
        private void EnsureCapacity(int requiredCapacity)
        {
            if (requiredCapacity <= _capacity) return;

            int newCapacity = GetNextCapacity(requiredCapacity);
            var newArray = ArrayPool<IElement>.Shared.Rent(newCapacity);
            
            if (_count > 0)
            {
                Array.Copy(_elements, newArray, _count);
            }

            if (_capacity > 0)
            {
                ArrayPool<IElement>.Shared.Return(_elements, true);
            }

            _elements = newArray;
            _capacity = newCapacity;
        }

        /// <summary>
        /// 获取下一个容量大小
        /// </summary>
        private int GetNextCapacity(int requiredCapacity)
        {
            if (requiredCapacity <= 0) return 0;

            int capacity = 1;
            while (capacity < requiredCapacity)
            {
                capacity *= 2;
            }
            return capacity;
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
            if (_capacity > 0)
            {
                ArrayPool<IElement>.Shared.Return(_elements, true);
                _elements = null;
                _capacity = 0;
                _count = 0;
            }
        }
        
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
        /// <summary>
        /// IStylefulElement 显式实现，返回 IStylefulElement
        /// </summary>
        IStylefulElement IStylefulElement.WithStyle(IDeclStyle style)
        {
            return WithStyle(style);
        }

        /// <summary>
        /// 便于链式调用的 WithStyle，返回 Hor 类型
        /// </summary>
        public Hor WithStyle(IDeclStyle style)
        {
            var newHor = new Hor(_boxSkin, style);
            newHor.Key = Key;
            newHor.Events = Events;
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newHor.Add(_elements[i]);
                }
            }
            return newHor;
        }
        
        /// <summary>
        /// 设置盒模型皮肤样式
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <returns>带有新盒模型皮肤的Hor实例</returns>
        public Hor WithBoxSkin(BoxSkin boxSkin)
        {
            var newHor = new Hor(boxSkin, Style);
            newHor.Key = Key;
            newHor.Events = Events;
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newHor.Add(_elements[i]);
                }
            }
            return newHor;
        }
    }
}
