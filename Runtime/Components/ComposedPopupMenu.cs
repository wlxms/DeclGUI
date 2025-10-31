using System;
using System.Collections.Generic;
using System.Linq;
using DeclGUI.Core;
using DeclGUI.Components.Advanced;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 组合式弹出菜单组件
    /// 使用控件组合模式实现，而不是独立的渲染器模式
    /// </summary>
    public struct ComposedPopupMenu : IElement<ComposedPopupMenuState>, IEventfulElement, IStylefulElement
    {
        public DeclEvent Events { get; set; }
        public string Key { get; set; }
        public IDeclStyle Style { get; }
        
        /// <summary>
        /// 所有菜单项
        /// </summary>
        public List<PopupMenuItem> Items { get; }
        
        /// <summary>
        /// 每页最大项目数
        /// </summary>
        public int PageSize { get; }
        
        /// <summary>
        /// 选中回调，返回选中索引
        /// </summary>
        public Action<int> OnItemSelected { get; }
        
        /// <summary>
        /// 自定义项渲染器
        /// </summary>
        public Func<PopupMenuItem, IElement> ItemRenderer { get; }
        
        /// <summary>
        /// 搜索框占位符文本
        /// </summary>
        public string SearchPlaceholder { get; }
        
        /// <summary>
        /// 是否显示分页控件
        /// </summary>
        public bool ShowPagination { get; }
        
        /// <summary>
        /// 是否显示搜索框
        /// </summary>
        public bool ShowSearch { get; }

        public ComposedPopupMenu(
            List<PopupMenuItem> items,
            int pageSize = 10,
            Action<int> onItemSelected = null,
            Func<PopupMenuItem, IElement> itemRenderer = null,
            string searchPlaceholder = "搜索...",
            bool showPagination = true,
            bool showSearch = true,
            IDeclStyle style = null)
        {
            Events = new DeclEvent();
            Key = null;
            Style = style;
            
            Items = items ?? new List<PopupMenuItem>();
            PageSize = pageSize;
            OnItemSelected = onItemSelected;
            ItemRenderer = itemRenderer;
            SearchPlaceholder = searchPlaceholder;
            ShowPagination = showPagination;
            ShowSearch = showSearch;
        }
        
        public ComposedPopupMenuState CreateState() => new ComposedPopupMenuState();
        
        /// <summary>
        /// 渲染方法 - 返回组合的UI元素
        /// </summary>
        public IElement Render(ComposedPopupMenuState state)
        {
            // 确保状态已初始化
            if (state.SearchResults == null || state.SearchResults.Count == 0)
            {
                state.UpdateSearchResults(Items);
                state.UpdateDisplayItems(PageSize);
            }

            var elements = new List<IElement>();

            // 添加搜索框（如果启用）
            if (ShowSearch)
            {
                elements.Add(CreateSearchBox(state));
            }

            // 添加菜单项列表
            elements.Add(CreateMenuItemsList(state));

            // 添加分页控件（如果启用且需要分页）
            if (ShowPagination && state.GetTotalPages(PageSize) > 1)
            {
                elements.Add(CreatePaginationControls(state));
            }

            // 使用Panel容器包装所有元素
            return new Panel(Style, elements.ToArray());
        }

        /// <summary>
        /// 创建搜索框组件
        /// </summary>
        private IElement CreateSearchBox(ComposedPopupMenuState state)
        {
            // 保存当前实例的Items和PageSize，避免在lambda中访问this
            var items = Items;
            var pageSize = PageSize;

            // 为TextField创建一个临时的onValueChanged处理
            Action<string> onSearchChanged = (newValue) =>
            {
                state.SearchKeyword = newValue;
                state.CurrentPage = 0; // 搜索时重置到第一页
                state.UpdateSearchResults(items);
                state.UpdateDisplayItems(pageSize);
            };

            var searchField = new TextField(
                state.SearchKeyword,
                onValueChanged: onSearchChanged
            );

            // 创建清除搜索按钮或占位空间
            IElement clearButton;
            if (!string.IsNullOrEmpty(state.SearchKeyword))
            {
                clearButton = new Button("×", () =>
                {
                    state.SearchKeyword = "";
                    state.CurrentPage = 0;
                    state.UpdateSearchResults(items);
                    state.UpdateDisplayItems(pageSize);
                }).WithStyle(new DeclStyle(width: 20, height: 20));
            }
            else
            {
                clearButton = null;
            }

            return new Hor(searchField, clearButton);
        }

        /// <summary>
        /// 创建菜单项列表
        /// </summary>
        private IElement CreateMenuItemsList(ComposedPopupMenuState state)
        {
            if (state.DisplayItems == null || state.DisplayItems.Count == 0)
            {
                return new Label("没有找到匹配的项目")
                    .WithStyle(new DeclStyle(alignment: UnityEngine.TextAnchor.MiddleCenter));
            }

            var menuItems = new List<IElement>();

            for (int i = 0; i < state.DisplayItems.Count; i++)
            {
                var menuItem = state.DisplayItems[i];
                var globalIndex = state.CurrentPage * PageSize + i;
                
                menuItems.Add(CreateMenuItem(menuItem, globalIndex, state));
            }

            return new Ver(menuItems.ToArray());
        }

        /// <summary>
        /// 创建单个菜单项
        /// </summary>
        private IElement CreateMenuItem(PopupMenuItem menuItem, int globalIndex, ComposedPopupMenuState state)
        {
            var isSelected = state.SelectedIndex == globalIndex;
            
            // 使用自定义渲染器或默认渲染
            if (ItemRenderer != null)
            {
                var customElement = ItemRenderer(menuItem);
                if (customElement != null)
                {
                    return WrapMenuItemWithEvents(customElement, globalIndex, state);
                }
            }

            // 默认菜单项渲染
            return CreateDefaultMenuItem(menuItem, globalIndex, state, isSelected);
        }

        /// <summary>
        /// 创建默认菜单项
        /// </summary>
        private IElement CreateDefaultMenuItem(PopupMenuItem menuItem, int globalIndex, ComposedPopupMenuState state, bool isSelected)
        {
            var displayName = menuItem.DisplayName ?? "Unknown";
            
            // 高亮搜索关键词
            if (!string.IsNullOrEmpty(state.SearchKeyword))
            {
                displayName = HighlightSearchKeyword(displayName, state.SearchKeyword);
            }

            // 保存当前实例的回调，避免在lambda中访问this
            var onItemSelected = OnItemSelected;
            var events = Events;

            var button = new Button(displayName, () =>
            {
                // 选中项目
                state.SelectedIndex = globalIndex;
                
                // 触发回调，返回选中索引
                onItemSelected?.Invoke(globalIndex);
                
                // 触发点击事件
                events.OnClick?.Invoke();
            });

            // 设置选中样式
            if (isSelected)
            {
                button = button.WithStyle(new DeclStyle(backgroundColor: new Color(0.2f, 0.4f, 0.8f, 0.3f)));
            }

            return button.WithStyle(new DeclStyle(alignment: UnityEngine.TextAnchor.MiddleLeft));
        }

        /// <summary>
        /// 包装菜单项并添加事件处理
        /// </summary>
        private IElement WrapMenuItemWithEvents(IElement element, int globalIndex, ComposedPopupMenuState state)
        {
            // 这里可以添加额外的事件处理逻辑
            // 目前直接返回原始元素，因为事件处理在Button中已经处理
            return element;
        }

        /// <summary>
        /// 创建分页控件
        /// </summary>
        private IElement CreatePaginationControls(ComposedPopupMenuState state)
        {
            var pageSize = PageSize;

            var totalPages = state.GetTotalPages(pageSize);
            
            return new Hor(
                new Button("◀", () =>
                {
                    if (state.CurrentPage > 0)
                    {
                        state.CurrentPage--;
                        state.UpdateDisplayItems(pageSize);
                    }
                }).WithStyle(new DeclStyle(width: 30, height: 20)),
                
                new Label($"第 {state.CurrentPage + 1} / {totalPages} 页")
                    .WithStyle(new DeclStyle(alignment: UnityEngine.TextAnchor.MiddleCenter)),
                
                new Button("▶", () =>
                {
                    if (state.CurrentPage < totalPages - 1)
                    {
                        state.CurrentPage++;
                        state.UpdateDisplayItems(pageSize);
                    }
                }).WithStyle(new DeclStyle(width: 30, height: 20))
            ).WithStyle(new DeclStyle(alignment: UnityEngine.TextAnchor.MiddleCenter));
        }

        /// <summary>
        /// 高亮搜索关键词
        /// </summary>
        private string HighlightSearchKeyword(string text, string keyword)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(keyword))
                return text;

            var lowerText = text.ToLower();
            var lowerKeyword = keyword.ToLower();
            var index = lowerText.IndexOf(lowerKeyword);
            
            if (index >= 0)
            {
                // 在DeclGUI中，我们使用富文本格式来高亮
                var highlighted = text.Substring(0, index) + 
                                 $"<b><color=yellow>{text.Substring(index, keyword.Length)}</color></b>" + 
                                 text.Substring(index + keyword.Length);
                return highlighted;
            }
            
            return text;
        }

        public void BindEvent(DeclEventType eventType, Action handler)
        {
            var events = Events;
            events.SetHandler(eventType, handler);
            Events = events;
        }

        public void UnbindEvent(DeclEventType eventType)
        {
            var events = Events;
            events.SetHandler(eventType, null);
            Events = events;
        }

        public IStylefulElement WithStyle(IDeclStyle style)
        {
            var element = this;
            // 由于Style是只读的，我们需要创建一个新的实例
            return new ComposedPopupMenu(
                element.Items,
                element.PageSize,
                element.OnItemSelected,
                element.ItemRenderer,
                element.SearchPlaceholder,
                element.ShowPagination,
                element.ShowSearch,
                style
            )
            {
                Key = element.Key,
                Events = element.Events
            };
        }
    }
}