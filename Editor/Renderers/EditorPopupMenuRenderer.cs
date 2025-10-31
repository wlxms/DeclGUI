using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using PopupMenuItem = DeclGUI.Components.PopupMenuItem;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Editor环境下的PopupMenu渲染器
    /// 实现带搜索、分页功能的菜单渲染
    /// </summary>
    public class EditorPopupMenuRenderer : EditorElementRenderer<PopupMenu, MenuState>
    {
        /// <summary>
        /// 渲染有状态菜单
        /// </summary>
        public override void Render(RenderManager mgr, in PopupMenu element, MenuState state, in IDeclStyle style)
        {
            // 确保状态已初始化
            if (state.SearchResults == null || state.SearchResults.Count == 0)
            {
                element.UpdateSearchResults(state);
                element.UpdateDisplayItems(state);
            }

            // 渲染搜索框
            RenderSearchBox(element, state);
            
            // 渲染菜单项
            RenderMenuItems(mgr, element, state, style);
            
            // 渲染分页控件
            RenderPagination(element, state);
        }

        /// <summary>
        /// 渲染搜索框
        /// </summary>
        private void RenderSearchBox(PopupMenu element, MenuState state)
        {
            EditorGUILayout.BeginHorizontal();
            
            // 搜索框
            var newSearchKeyword = EditorGUILayout.TextField(state.SearchKeyword, EditorStyles.toolbarSearchField);
            if (newSearchKeyword != state.SearchKeyword)
            {
                state.SearchKeyword = newSearchKeyword;
                state.CurrentPage = 0; // 搜索时重置到第一页
                element.UpdateSearchResults(state);
                element.UpdateDisplayItems(state);
            }
            
            // 清除搜索按钮
            if (!string.IsNullOrEmpty(state.SearchKeyword))
            {
                if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    state.SearchKeyword = "";
                    state.CurrentPage = 0;
                    element.UpdateSearchResults(state);
                    element.UpdateDisplayItems(state);
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            // 搜索结果统计
            if (!string.IsNullOrEmpty(state.SearchKeyword))
            {
                EditorGUILayout.LabelField($"找到 {state.SearchResults.Count} 个结果", EditorStyles.miniLabel);
            }
        }

        /// <summary>
        /// 渲染菜单项
        /// </summary>
        private void RenderMenuItems(RenderManager mgr, PopupMenu element, MenuState state, IDeclStyle style)
        {
            if (state.DisplayItems == null || state.DisplayItems.Count == 0)
            {
                EditorGUILayout.LabelField("没有找到匹配的项目", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            // 使用滚动视图
            var scrollViewStyle = new GUIStyle(GUI.skin.box)
            {
                margin = new RectOffset(2, 2, 2, 2),
                padding = new RectOffset(2, 2, 2, 2)
            };

            EditorGUILayout.BeginVertical(scrollViewStyle, GUILayout.ExpandHeight(true));
            
            for (int i = 0; i < state.DisplayItems.Count; i++)
            {
                var menuItem = state.DisplayItems[i];
                var globalIndex = state.CurrentPage * element.PageSize + i;
                
                RenderMenuItem(mgr, element, state, menuItem, globalIndex, i, style);
            }
            
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 渲染单个菜单项
        /// </summary>
        private void RenderMenuItem(RenderManager mgr, PopupMenu element, MenuState state, 
            PopupMenuItem menuItem, int globalIndex, int pageIndex, IDeclStyle style)
        {
            var isSelected = state.SelectedIndex == globalIndex;
            var itemStyle = isSelected ? EditorStyles.whiteLabel : EditorStyles.label;
            
            // 使用自定义渲染器或默认渲染
            if (element.ItemRenderer != null)
            {
                var customElement = element.ItemRenderer(menuItem);
                if (customElement != null)
                {
                    // 渲染自定义元素
                    mgr.RenderElement(customElement);
                }
                else
                {
                    RenderDefaultMenuItem(menuItem, itemStyle, globalIndex, pageIndex, element, state);
                }
            }
            else
            {
                RenderDefaultMenuItem(menuItem, itemStyle, globalIndex, pageIndex, element, state);
            }
        }

        /// <summary>
        /// 渲染默认菜单项
        /// </summary>
        private void RenderDefaultMenuItem(PopupMenuItem menuItem, GUIStyle itemStyle, 
            int globalIndex, int pageIndex, PopupMenu element, MenuState state)
        {
            var displayName = menuItem.DisplayName ?? "Unknown";
            
            // 高亮搜索关键词
            if (!string.IsNullOrEmpty(state.SearchKeyword))
            {
                displayName = HighlightSearchKeyword(displayName, state.SearchKeyword);
            }

            // 渲染菜单项按钮
            var buttonStyle = new GUIStyle(EditorStyles.toolbarButton)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(8, 8, 4, 4),
                richText = true
            };

            if (state.SelectedIndex == globalIndex)
            {
                buttonStyle.normal = buttonStyle.active;
            }

            if (GUILayout.Button(displayName, buttonStyle))
            {
                // 选中项目
                state.SelectedIndex = globalIndex;
                
                // 触发回调，返回选中索引
                element.OnItemSelected?.Invoke(globalIndex);
                
                // 触发点击事件
                element.Events.OnClick?.Invoke();
            }
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
                var highlighted = text.Substring(0, index) + 
                                 $"<b><color=yellow>{text.Substring(index, keyword.Length)}</color></b>" + 
                                 text.Substring(index + keyword.Length);
                return highlighted;
            }
            
            return text;
        }

        /// <summary>
        /// 渲染分页控件
        /// </summary>
        private void RenderPagination(PopupMenu element, MenuState state)
        {
            var totalPages = element.GetTotalPages(state);
            if (totalPages <= 1) return;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // 上一页按钮
            var prevEnabled = state.CurrentPage > 0;
            GUI.enabled = prevEnabled;
            if (GUILayout.Button("◀", EditorStyles.miniButton, GUILayout.Width(30)))
            {
                state.CurrentPage--;
                element.UpdateDisplayItems(state);
            }

            // 页码显示
            var pageInfo = $"第 {state.CurrentPage + 1} / {totalPages} 页";
            GUILayout.Label(pageInfo, EditorStyles.miniLabel, GUILayout.ExpandWidth(false));

            // 下一页按钮
            var nextEnabled = state.CurrentPage < totalPages - 1;
            GUI.enabled = nextEnabled;
            if (GUILayout.Button("▶", EditorStyles.miniButton, GUILayout.Width(30)))
            {
                state.CurrentPage++;
                element.UpdateDisplayItems(state);
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 计算菜单大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in PopupMenu element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 如果设置了固定尺寸，直接返回
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            // 使用实际的菜单项样式来计算尺寸
            var menuItemStyle = new GUIStyle(EditorStyles.toolbarButton)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(8, 8, 4, 4)
            };

            // 计算菜单项的最大宽度和总高度
            float maxItemWidth = 200f; // 默认最小宽度
            float totalContentHeight = 0f;
            
            // 计算搜索框高度
            var searchFieldStyle = EditorStyles.toolbarSearchField;
            var searchFieldSize = CalculateTextSize("搜索...", searchFieldStyle, 0);
            float searchHeight = searchFieldSize.y + searchFieldStyle.padding.vertical;
            
            // 计算统计信息高度（仅在搜索时显示）
            var miniLabelStyle = EditorStyles.miniLabel;
            var statsSize = CalculateTextSize("找到 0 个结果", miniLabelStyle, 0);
            float statsHeight = statsSize.y + miniLabelStyle.padding.vertical;
            
            // 计算分页控件高度
            var miniButtonStyle = EditorStyles.miniButton;
            var pageSize = CalculateTextSize("第 1 / 1 页", miniLabelStyle, 0);
            float paginationHeight = pageSize.y + miniButtonStyle.padding.vertical;

            // 如果有菜单项，计算实际的最大宽度和总高度
            if (element.Items != null && element.Items.Count > 0)
            {
                // 计算单个菜单项的标准高度
                var sampleItemHeight = menuItemStyle.CalcHeight(new GUIContent("测试"), maxItemWidth);
                
                // 限制显示的项目数量
                int displayCount = Mathf.Min(element.PageSize, element.Items.Count);
                totalContentHeight = displayCount * sampleItemHeight;

                // 计算所有菜单项的最大宽度
                foreach (var item in element.Items)
                {
                    var displayName = item.DisplayName ?? "Unknown";
                    var textSize = CalculateTextSize(displayName, menuItemStyle, 0);
                    
                    // 正确计算包含padding的菜单项宽度
                    float itemWidth = textSize.x + menuItemStyle.padding.horizontal;
                    maxItemWidth = Mathf.Max(maxItemWidth, itemWidth);
                }
            }
            else
            {
                // 没有项目时的默认高度
                var emptyLabelStyle = EditorStyles.centeredGreyMiniLabel;
                var emptySize = CalculateTextSize("没有找到匹配的项目", emptyLabelStyle, 0);
                totalContentHeight = emptySize.y + emptyLabelStyle.padding.vertical;
            }

            // 计算总高度：搜索框 + 统计信息 + 内容 + 分页
            float totalHeight = searchHeight + totalContentHeight + paginationHeight;
            
            // 只有在有搜索关键词时才添加统计信息高度
            if (!string.IsNullOrEmpty(element.SearchPlaceholder))
            {
                totalHeight += statsHeight;
            }

            // 应用样式约束
            Vector2 totalSize = new Vector2(
                width > 0 ? width : Mathf.Max(maxItemWidth, 250f), // 确保最小宽度
                height > 0 ? height : Mathf.Max(totalHeight, 200f) // 确保最小高度
            );
            
            // 使用 Unity 的标准方法计算包含样式的尺寸
            var containerStyle = editorMgr.ApplyStyle(currentStyle, GUI.skin.box);
            if (containerStyle != null)
            {
                // 对于容器，使用 GUIStyle 的 CalcSize 来正确计算包含 padding 的尺寸
                // 但不包括 border，因为 border 不影响布局计算
                totalSize.x += containerStyle.padding.horizontal;
                totalSize.y += containerStyle.padding.vertical;
            }
            
            return totalSize;
        }

        /// <summary>
        /// 获取元素矩形（用于事件处理）
        /// </summary>
        public override Rect GetElementRect()
        {
            return GUILayoutUtility.GetLastRect();
        }
    }
}