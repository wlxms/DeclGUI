using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Buffers;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 禁用组组件
    /// 用于将禁用状态应用到子元素组
    /// </summary>
    public struct DisableGroup : IContainerElement, IEventfulElement, IStylefulElement
    {
        public string Key { get; set; }
        private IElement[] _elements;
        private int _count;
        private int _capacity;
        private bool _isDisabled;

        /// <summary>
        /// 布局样式
        /// </summary>
        public IDeclStyle Style { get; }
        
        /// <summary>
        /// 事件注册结构
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled => _isDisabled;

        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// 构造函数 - 使用参数数组
        /// </summary>
        /// <param name="children">子元素</param>
        public DisableGroup(params IElement[] children) : this(true, children)
        {
        }

        /// <summary>
        /// 构造函数 - 使用参数数组和禁用状态
        /// </summary>
        /// <param name="isDisabled">是否禁用</param>
        /// <param name="children">子元素</param>
        public DisableGroup(bool isDisabled, params IElement[] children)
        {
            Key = null;
            _elements = null;
            _count = 0;
            _capacity = 0;
            Style = null;
            Events = new DeclEvent();
            _isDisabled = isDisabled;

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
        public DisableGroup(IDeclStyle style, params IElement[] children) : this(true, style, children)
        {
        }

        /// <summary>
        /// 构造函数 - 使用禁用状态、样式和参数数组
        /// </summary>
        /// <param name="isDisabled">是否禁用</param>
        /// <param name="style">样式</param>
        /// <param name="children">子元素</param>
        public DisableGroup(bool isDisabled, IDeclStyle style, params IElement[] children) : this(isDisabled, children)
        {
            Style = style;
        }

        /// <summary>
        /// 构造函数 - 使用集合
        /// </summary>
        /// <param name="children">子元素集合</param>
        /// <param name="style">样式</param>
        public DisableGroup(IEnumerable<IElement> children, IDeclStyle style = null) : this(true, children, style)
        {
        }

        /// <summary>
        /// 构造函数 - 使用禁用状态和集合
        /// </summary>
        /// <param name="isDisabled">是否禁用</param>
        /// <param name="children">子元素集合</param>
        /// <param name="style">样式</param>
        public DisableGroup(bool isDisabled, IEnumerable<IElement> children, IDeclStyle style = null)
        {
            Key = null;
            _elements = null;
            _count = 0;
            _capacity = 0;
            Style = style;
            Events = new DeclEvent();
            _isDisabled = isDisabled;

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
    }
}