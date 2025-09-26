using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Buffers;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 滚动视图容器组件
    /// 用于创建可滚动的UI区域
    /// </summary>
    public struct ScrollRect : IContainerElement, IEventfulElement, IStylefulElement
    {
        public string Key { get; set; }
        private IElement[] _elements;
        private int _count;
        private int _capacity;

        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 滚动位置
        /// </summary>
        public Vector2 ScrollPosition { get; }
        
        /// <summary>
        /// 滚动位置变化回调
        /// </summary>
        public System.Action<Vector2> OnScroll { get; }
        
        /// <summary>
        /// 是否总是显示垂直滚动条
        /// </summary>
        public bool AlwaysShowVertical { get; }
        
        /// <summary>
        /// 是否总是显示水平滚动条
        /// </summary>
        public bool AlwaysShowHorizontal { get; }
        
        /// <summary>
        /// 样式
        /// </summary>
        public IDeclStyle Style { get; }

        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// 构造函数 - 使用参数数组
        /// </summary>
        /// <param name="scrollPosition">滚动位置</param>
        /// <param name="onScroll">滚动位置变化回调</param>
        /// <param name="alwaysShowVertical">是否总是显示垂直滚动条</param>
        /// <param name="alwaysShowHorizontal">是否总是显示水平滚动条</param>
        /// <param name="style">样式</param>
        /// <param name="children">子元素</param>
        public ScrollRect(Vector2 scrollPosition,
                         System.Action<Vector2> onScroll = null,
                         bool alwaysShowVertical = false, bool alwaysShowHorizontal = false,
                         IDeclStyle style = null,
                         params IElement[] children)
        {
            Key = null;
            _elements = null;
            _count = 0;
            _capacity = 0;
            Style = style;
            Events = new DeclEvent();
            ScrollPosition = scrollPosition;
            OnScroll = onScroll;
            AlwaysShowVertical = alwaysShowVertical;
            AlwaysShowHorizontal = alwaysShowHorizontal;

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
        /// <param name="scrollPosition">滚动位置</param>
        /// <param name="style">样式</param>
        /// <param name="onScroll">滚动位置变化回调</param>
        /// <param name="alwaysShowVertical">是否总是显示垂直滚动条</param>
        /// <param name="alwaysShowHorizontal">是否总是显示水平滚动条</param>
        /// <param name="children">子元素</param>
        public ScrollRect(Vector2 scrollPosition, IDeclStyle style,
                         System.Action<Vector2> onScroll = null,
                         bool alwaysShowVertical = false, bool alwaysShowHorizontal = false,
                         params IElement[] children) : this(scrollPosition, onScroll, alwaysShowVertical, alwaysShowHorizontal, style, children)
       {
       }

        /// <summary>
        /// 构造函数 - 使用集合
        /// </summary>
        /// <param name="scrollPosition">滚动位置</param>
        /// <param name="children">子元素集合</param>
        /// <param name="onScroll">滚动位置变化回调</param>
        /// <param name="alwaysShowVertical">是否总是显示垂直滚动条</param>
        /// <param name="alwaysShowHorizontal">是否总是显示水平滚动条</param>
        /// <param name="style">样式</param>
        public ScrollRect(Vector2 scrollPosition, IEnumerable<IElement> children,
                         System.Action<Vector2> onScroll = null,
                         bool alwaysShowVertical = false, bool alwaysShowHorizontal = false,
                         IDeclStyle style = null)
        {
            Key = null;
            _elements = null;
            _count = 0;
            _capacity = 0;
            Style = style;
            Events = new DeclEvent();
            ScrollPosition = scrollPosition;
            OnScroll = onScroll;
            AlwaysShowVertical = alwaysShowVertical;
            AlwaysShowHorizontal = alwaysShowHorizontal;

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
        /// <returns>UI元素</returns>
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
                System.Buffers.ArrayPool<IElement>.Shared.Return(_elements, true);
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
        /// 设置样式
        /// </summary>
        /// <param name="style">新样式</param>
        /// <returns>带样式的滚动视图容器</returns>
        public ScrollRect WithStyle(IDeclStyle style)
        {
            return new ScrollRect(ScrollPosition, OnScroll, AlwaysShowVertical, AlwaysShowHorizontal, style, GetElementsArray());
        }

        /// <summary>
        /// 设置滚动条显示选项
        /// </summary>
        /// <param name="showVertical">显示垂直滚动条</param>
        /// <param name="showHorizontal">显示水平滚动条</param>
        /// <returns>带新设置的滚动视图容器</returns>
        public ScrollRect WithScrollbars(bool showVertical, bool showHorizontal)
        {
            return new ScrollRect(ScrollPosition, OnScroll, showVertical, showHorizontal, Style, GetElementsArray());
        }
        
        /// <summary>
        /// 获取元素数组的副本
        /// </summary>
        private IElement[] GetElementsArray()
        {
            if (_count == 0) return new IElement[0];
            
            var result = new IElement[_count];
            Array.Copy(_elements, result, _count);
            return result;
        }
        
        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;
    }
}