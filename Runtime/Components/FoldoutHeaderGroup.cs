using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Buffers;
using DeclGUI.Core;
using static DeclGUI.Core.ArrayPoolHelper;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 使用Unity的EditorGUILayout.BeginFoldoutHeaderGroup实现的折叠头部组控件
    /// 支持Header的样式定义以及背景颜色
    /// </summary>
    public struct FoldoutHeaderGroup : IElement<FoldoutHeaderGroupState>, IContainerElement, IEventfulElement, IStylefulElement
    {
        public string Key { get; set; }
        private IElement[] _elements;
        private int _count;
        private int _capacity;
        private BoxSkin _boxSkin;

        /// <summary>
        /// 标题样式
        /// </summary>
        public IDeclStyle HeaderStyle { get; }

        /// <summary>
        /// 背景样式
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
        /// 初始展开状态
        /// </summary>
        public bool InitialExpanded { get; }

        public IElement HeaderElement { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="header">标题文本</param>
        /// <param name="initialExpanded">初始是否展开</param>
        /// <param name="headerStyle">标题样式</param>
        /// <param name="style">背景样式</param>
        /// <param name="children">子元素</param>
        public FoldoutHeaderGroup(IElement headerElement, bool initialExpanded = true, IDeclStyle headerStyle = null, IDeclStyle style = null, params IElement[] children) : this(headerElement, initialExpanded, headerStyle, BoxSkin.Auto, style, children)
        {
        }
        
        /// <summary>
        /// 构造函数 - 使用盒模型皮肤
        /// </summary>
        /// <param name="headerElement">标题元素</param>
        /// <param name="initialExpanded">初始是否展开</param>
        /// <param name="headerStyle">标题样式</param>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="style">背景样式</param>
        /// <param name="children">子元素</param>
        public FoldoutHeaderGroup(IElement headerElement, bool initialExpanded, IDeclStyle headerStyle, BoxSkin boxSkin, IDeclStyle style = null, params IElement[] children)
        {
            Key = null;
            _elements = null;
            _count = 0;
            _capacity = 0;
            HeaderElement = headerElement ?? new Label();
            HeaderStyle = headerStyle?? new DeclStyle("FoldoutHeader");;
            Style = style ?? new DeclStyle("FoldoutGroup");
            Events = new DeclEvent();
            InitialExpanded = initialExpanded;
            _boxSkin = boxSkin;

            if (children != null && children.Length > 0)
            {
                var validChildren = children.Where(c => c != null).ToArray();
                if (validChildren.Length > 0)
                {
                    InitializeFromArray(ref _elements, ref _capacity, ref _count, validChildren);
                }
            }
        }
        public FoldoutHeaderGroup(string header, bool initialExpanded = true, IDeclStyle headerStyle = null, IDeclStyle style = null, params IElement[] children) : this(header, initialExpanded, headerStyle, BoxSkin.Auto, style, children)
        {
        }
        
        /// <summary>
        /// 构造函数 - 使用盒模型皮肤
        /// </summary>
        /// <param name="header">标题文本</param>
        /// <param name="initialExpanded">初始是否展开</param>
        /// <param name="headerStyle">标题样式</param>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="style">背景样式</param>
        /// <param name="children">子元素</param>
        public FoldoutHeaderGroup(string header, bool initialExpanded, IDeclStyle headerStyle, BoxSkin boxSkin, IDeclStyle style = null, params IElement[] children)
        {
            Key = null;
            _elements = null;
            _count = 0;
            _capacity = 0;
            HeaderElement = new Label(header, new DeclStyle(fontStyle: FontStyle.Bold));
            HeaderStyle = headerStyle?? new DeclStyle("FoldoutHeader");;
            Style = style ?? new DeclStyle("FoldoutGroup");
            Events = new DeclEvent();
            InitialExpanded = initialExpanded;
            _boxSkin = boxSkin;

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
        /// 构造函数 - 使用集合
        /// </summary>
        /// <param name="header">标题文本</param>
        /// <param name="initialExpanded">初始是否展开</param>
        /// <param name="headerStyle">标题样式</param>
        /// <param name="style">背景样式</param>
        /// <param name="children">子元素集合</param>
        public FoldoutHeaderGroup(string header, bool initialExpanded, IDeclStyle headerStyle, IDeclStyle style, IEnumerable<IElement> children) : this(header, initialExpanded, headerStyle, BoxSkin.Auto, style, children)
        {
        }
        
        /// <summary>
        /// 构造函数 - 使用盒模型皮肤和集合
        /// </summary>
        /// <param name="header">标题文本</param>
        /// <param name="initialExpanded">初始是否展开</param>
        /// <param name="headerStyle">标题样式</param>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <param name="style">背景样式</param>
        /// <param name="children">子元素集合</param>
        public FoldoutHeaderGroup(string header, bool initialExpanded, IDeclStyle headerStyle, BoxSkin boxSkin, IDeclStyle style, IEnumerable<IElement> children)
        {
            Key = null;
            _elements = null;
            _count = 0;
            _capacity = 0;
            HeaderElement = new Label(header);
            HeaderStyle = headerStyle;
            Style = style;
            Events = new DeclEvent();
            InitialExpanded = initialExpanded;
            _boxSkin = boxSkin;

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
        /// 渲染方法
        /// </summary>
        public IElement Render()
        {
            return null;
        }

        /// <summary>
        /// 渲染方法（有状态版本）
        /// </summary>
        public IElement Render(FoldoutHeaderGroupState state)
        {
            // 返回自身，实际渲染由渲染器完成
            return null;
        }

        /// <summary>
        /// 创建状态
        /// </summary>
        public FoldoutHeaderGroupState CreateState()
        {
            return new FoldoutHeaderGroupState(InitialExpanded);
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

        IStylefulElement IStylefulElement.WithStyle(IDeclStyle style)
        {
            return WithStyle(style);
        }

        /// <summary>
        /// 便于链式调用的 WithStyle，返回 FoldoutHeaderGroup 类型
        /// </summary>
        public FoldoutHeaderGroup WithStyle(IDeclStyle style)
        {
            // 仅替换背景样式，其他字段保持原值
            var newGroup = new FoldoutHeaderGroup(HeaderElement, InitialExpanded, HeaderStyle, style);
            newGroup.Key = Key;
            newGroup.Events = Events;
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newGroup.Add(_elements[i]);
                }
            }
            return newGroup;
        }
        
        /// <summary>
        /// 设置盒模型皮肤样式
        /// </summary>
        /// <param name="boxSkin">盒模型皮肤样式</param>
        /// <returns>带有新盒模型皮肤的FoldoutHeaderGroup实例</returns>
        public FoldoutHeaderGroup WithBoxSkin(BoxSkin boxSkin)
        {
            // 仅替换背景样式，其他字段保持原值
            var newGroup = new FoldoutHeaderGroup(HeaderElement, InitialExpanded, HeaderStyle, boxSkin, Style);
            newGroup.Key = Key;
            newGroup.Events = Events;
            if (_count > 0 && _elements != null)
            {
                for (int i = 0; i < _count; i++)
                {
                    newGroup.Add(_elements[i]);
                }
            }
            return newGroup;
        }

        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;
    }
}