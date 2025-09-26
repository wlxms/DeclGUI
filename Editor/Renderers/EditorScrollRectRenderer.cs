using DeclGUI.Components;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// ScrollRect组件的Editor渲染器
    /// </summary>
    public class EditorScrollRectRenderer : EditorElementRenderer<ScrollRect>
    {
        /// <summary>
        /// 渲染ScrollRect组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr, in ScrollRect element, in IDeclStyle styleParam)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            var currentStyle = styleParam ?? element.Style;

            var style = editorMgr.ApplyStyle(currentStyle, GUI.skin.scrollView);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 设置滚动条显示选项
            GUIStyle verticalScrollbar = element.AlwaysShowVertical ? GUI.skin.verticalScrollbar : GUIStyle.none;
            GUIStyle horizontalScrollbar = element.AlwaysShowHorizontal ? GUI.skin.horizontalScrollbar : GUIStyle.none;

            // 渲染滚动视图
            Vector2 newScrollPosition;

            newScrollPosition = EditorGUILayout.BeginScrollView(
                    element.ScrollPosition,
                    element.AlwaysShowVertical,
                    element.AlwaysShowHorizontal,
                    GUILayout.Width(width),
                    GUILayout.Height(height)
                    );

            try
            {
                foreach (var child in element)
                {
                    mgr.RenderElement(child);
                }
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }

            if (currentStyle?.BackgroundColor != null)
            {
                // 保存原始颜色
                var originalBackgroundColor = GUI.backgroundColor;
                var originalColor = GUI.color;
                var originalContentColor = GUI.contentColor;

                try
                {
                    GUI.backgroundColor = currentStyle.BackgroundColor.Value;
                    var lastRect = GUILayoutUtility.GetLastRect();
                    GUI.Box(lastRect, "");
                }
                finally
                {
                    // 恢复原始颜色
                    GUI.backgroundColor = originalBackgroundColor;
                    GUI.color = originalColor;
                    GUI.contentColor = originalContentColor;
                }
            }

            // 检查滚动位置是否变化并触发回调
            if (newScrollPosition != element.ScrollPosition && element.OnScroll != null)
            {
                element.OnScroll(newScrollPosition);
            }
        }

        /// <summary>
        /// 计算ScrollRect元素的期望大小
        /// ScrollRect的大小基于给定的宽高或样式中的宽高，而不是内容的总大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in ScrollRect element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // ScrollRect的大小就是其设定的宽高
            // 如果没有设定宽高，返回0，让GUILayout系统决定合适的大小
            float finalWidth = width > 0 ? width : 0;
            float finalHeight = height > 0 ? height : 0;

            // 添加滚动条的空间（如果总是显示的话）
            if (element.AlwaysShowVertical)
            {
                finalWidth += GUI.skin.verticalScrollbar.fixedWidth;
            }
            if (element.AlwaysShowHorizontal)
            {
                finalHeight += GUI.skin.horizontalScrollbar.fixedHeight;
            }

            return new Vector2(finalWidth, finalHeight);
        }
    }
}