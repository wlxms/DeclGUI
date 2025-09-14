using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Horizontal布局组件的Editor渲染器
    /// </summary>
    public class EditorHorizontalRenderer : EditorElementRenderer<Hor>
    {
        public override void Render(RenderManager mgr, in Hor element)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            var style = editorMgr.ApplyStyle(element.Style, null);
            var width = editorMgr.GetStyleWidth(element.Style);
            var height = editorMgr.GetStyleHeight(element.Style);

            if (width > 0 && height > 0)
            {
                GUILayout.BeginHorizontal(style, GUILayout.Width(width), GUILayout.Height(height));
            }
            else if (width > 0)
            {
                GUILayout.BeginHorizontal(style, GUILayout.Width(width));
            }
            else if (height > 0)
            {
                GUILayout.BeginHorizontal(style, GUILayout.Height(height));
            }
            else
            {
                GUILayout.BeginHorizontal(style);
            }

            // 渲染所有子元素（使用状态栈）
            foreach (var child in element)
            {
                mgr.RenderElement(child);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 获取Horizontal布局元素的屏幕区域
        /// </summary>
        /// <returns>Horizontal布局的屏幕矩形</returns>
        public override Rect GetElementRect()
        {
            // 对于布局容器，返回最后渲染的矩形区域
            return GUILayoutUtility.GetLastRect();
        }
    
        public override Vector2 CalculateSize(RenderManager mgr, in Hor element, in DeclStyle? style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var guiStyle = editorMgr.ApplyStyle(currentStyle, null);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 如果设置了固定尺寸，直接返回
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            // 计算所有子元素的大小
            Vector2 contentSize = Vector2.zero;
            float spacing = 0; // GUILayout默认没有spacing，可以考虑从样式中获取
            
            int index = 0;
            foreach (var child in element)
            {
                var childSize = mgr.CalculateElementSize(child, null); // 子元素使用自己的样式
                
                contentSize.x += childSize.x;
                contentSize.y = Mathf.Max(contentSize.y, childSize.y);
                
                // 添加元素间距（除了最后一个元素）
                if (index < element.Count - 1)
                {
                    contentSize.x += spacing;
                }
                index++;
            }
            
            // 添加容器的padding
            Vector2 totalSize = contentSize;
            if (guiStyle != null)
            {
                totalSize.x += guiStyle.padding.left + guiStyle.padding.right;
                totalSize.y += guiStyle.padding.top + guiStyle.padding.bottom;
                
                // 添加border
                totalSize.x += guiStyle.border.left + guiStyle.border.right;
                totalSize.y += guiStyle.border.top + guiStyle.border.bottom;
            }

            // 应用尺寸约束
            if (width > 0) totalSize.x = width;
            if (height > 0) totalSize.y = height;

            return totalSize;
        }
    }
}