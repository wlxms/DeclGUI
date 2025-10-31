using System;
using System.Collections.Generic;
using System.Linq;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 菜单项数据结构
    /// </summary>
    public struct PopupMenuItem
    {
        public string DisplayName { get; set; }
        public Func<int, IElement> CustomRenderer { get; set; }
        
        public PopupMenuItem(string displayName, Func<int, IElement> customRenderer = null)
        {
            DisplayName = displayName;
            CustomRenderer = customRenderer;
        }
    }

    /// <summary>
    /// 菜单状态类
    /// </summary>
    public class MenuState
    {
        /// <summary>
        /// 当前页码（从0开始）
        /// </summary>
        public int CurrentPage { get; set; } = 0;
        
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string SearchKeyword { get; set; } = "";
        
        /// <summary>
        /// 选中项目的索引
        /// </summary>
        public int SelectedIndex { get; set; } = -1;
        
        /// <summary>
        /// 搜索结果缓存
        /// </summary>
        public List<PopupMenuItem> SearchResults { get; set; }
        
        /// <summary>
        /// 当前显示的项目列表
        /// </summary>
        public List<PopupMenuItem> DisplayItems { get; set; }
        
        /// <summary>
        /// 当前页的项目列表
        /// </summary>
        public List<PopupMenuItem> CurrentPageItems { get; set; }
        
        public MenuState()
        {
            SearchResults = new List<PopupMenuItem>();
            DisplayItems = new List<PopupMenuItem>();
            CurrentPageItems = new List<PopupMenuItem>();
        }
    }

    /// <summary>
    /// 带状态的菜单组件
    /// 支持搜索、分页和自定义渲染
    /// </summary>
    public struct PopupMenu : IElement<MenuState>, IEventfulElement, IStylefulElement
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
        
        public PopupMenu(
            List<PopupMenuItem> items,
            int pageSize = 10,
            Action<int> onItemSelected = null,
            Func<PopupMenuItem, IElement> itemRenderer = null,
            string searchPlaceholder = "搜索...",
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
        }
        
        public MenuState CreateState() => new MenuState();
        
        public IElement Render(MenuState state)
        {
            return null;
        }

        /// <summary>
        /// 更新搜索结果
        /// </summary>
        public void UpdateSearchResults(MenuState state)
        {
            if (string.IsNullOrEmpty(state.SearchKeyword))
            {
                state.SearchResults = new List<PopupMenuItem>(Items);
            }
            else
            {
                state.SearchResults = Items.Where(item =>
                    item.DisplayName?.ToLower().Contains(state.SearchKeyword.ToLower()) == true)
                    .ToList();
            }
        }

        /// <summary>
        /// 更新显示项目
        /// </summary>
        public void UpdateDisplayItems(MenuState state)
        {
            var startIndex = state.CurrentPage * PageSize;
            var endIndex = Math.Min(startIndex + PageSize, state.SearchResults.Count);
            
            state.DisplayItems = new List<PopupMenuItem>();
            if (startIndex < state.SearchResults.Count)
            {
                state.DisplayItems = state.SearchResults.GetRange(startIndex, endIndex - startIndex);
            }
            
            // 更新当前页项目
            state.CurrentPageItems = new List<PopupMenuItem>(state.DisplayItems);
        }

        /// <summary>
        /// 获取总页数
        /// </summary>
        public int GetTotalPages(MenuState state)
        {
            if (state.SearchResults == null || state.SearchResults.Count == 0)
                return 1;
                
            return (int)Math.Ceiling((double)state.SearchResults.Count / PageSize);
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
            // element.Style = style;
            return element;
        }
    }
}