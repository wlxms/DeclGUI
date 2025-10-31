using System;
using System.Collections.Generic;
using System.Linq;

namespace DeclGUI.Components
{
    /// <summary>
    /// 组合式弹出菜单状态类
    /// 用于处理ComposedPopupMenu的交互逻辑
    /// </summary>
    public class ComposedPopupMenuState
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
        
        public ComposedPopupMenuState()
        {
            SearchResults = new List<PopupMenuItem>();
            DisplayItems = new List<PopupMenuItem>();
            CurrentPageItems = new List<PopupMenuItem>();
        }
        
        /// <summary>
        /// 更新搜索结果
        /// </summary>
        public void UpdateSearchResults(List<PopupMenuItem> items)
        {
            if (string.IsNullOrEmpty(SearchKeyword))
            {
                SearchResults = new List<PopupMenuItem>(items);
            }
            else
            {
                SearchResults = items.Where(item =>
                    item.DisplayName?.ToLower().Contains(SearchKeyword.ToLower()) == true)
                    .ToList();
            }
        }

        /// <summary>
        /// 更新显示项目
        /// </summary>
        public void UpdateDisplayItems(int pageSize)
        {
            var startIndex = CurrentPage * pageSize;
            var endIndex = Math.Min(startIndex + pageSize, SearchResults.Count);
            
            DisplayItems = new List<PopupMenuItem>();
            if (startIndex < SearchResults.Count)
            {
                DisplayItems = SearchResults.GetRange(startIndex, endIndex - startIndex);
            }
            
            // 更新当前页项目
            CurrentPageItems = new List<PopupMenuItem>(DisplayItems);
        }

        /// <summary>
        /// 获取总页数
        /// </summary>
        public int GetTotalPages(int pageSize)
        {
            if (SearchResults == null || SearchResults.Count == 0)
                return 1;
                
            return (int)Math.Ceiling((double)SearchResults.Count / pageSize);
        }
    }
}