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
        public override void Render(RenderManager mgr,in ScrollRect element, in IDeclStyle styleParam)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            var style = editorMgr.ApplyStyle(styleParam ?? element.Style, GUI.skin.scrollView);
            var width = editorMgr.GetStyleWidth(styleParam ?? element.Style);
            var height = editorMgr.GetStyleHeight(styleParam ?? element.Style);

            // 设置滚动条显示选项
            GUIStyle verticalScrollbar = element.AlwaysShowVertical ? GUI.skin.verticalScrollbar : GUIStyle.none;
            GUIStyle horizontalScrollbar = element.AlwaysShowHorizontal ? GUI.skin.horizontalScrollbar : GUIStyle.none;

            // 渲染滚动视图
            var newScrollPosition = EditorGUILayout.BeginScrollView(
                element.ScrollPosition,
                verticalScrollbar,
                horizontalScrollbar,
                GUILayout.Width(width > 0 ? width : 0),
                GUILayout.Height(height > 0 ? height : 0)
            );

            // 渲染内容
            if (element.Content != null)
            {
                mgr.RenderElement(element.Content);
            }

            EditorGUILayout.EndScrollView();

            // 检查滚动位置是否变化并触发回调
            if (newScrollPosition != element.ScrollPosition && element.OnScroll != null)
            {
                element.OnScroll(newScrollPosition);
            }
        }

        /// <summary>
        /// 计算ScrollRect元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr,in ScrollRect element,in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var width = editorMgr.GetStyleWidth(style ?? element.Style);
            var height = editorMgr.GetStyleHeight(style ?? element.Style);
            
            // 如果设置了固定尺寸，使用固定尺寸
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }
            
            // 如果没有设置固定尺寸，计算内容的大小
            if (element.Content != null)
            {
                var contentSize = mgr.CalculateElementSize(element.Content, style ?? element.Style);
                
                // 添加滚动条的空间
                if (element.AlwaysShowVertical)
                {
                    contentSize.x += GUI.skin.verticalScrollbar.fixedWidth;
                }
                if (element.AlwaysShowHorizontal)
                {
                    contentSize.y += GUI.skin.horizontalScrollbar.fixedHeight;
                }
                
                // 应用样式约束
                if (width > 0) contentSize.x = width;
                if (height > 0) contentSize.y = height;
                
                return contentSize;
            }
            
            // 默认大小
            return new Vector2(
                width > 0 ? width : 200,
                height > 0 ? height : 150
            );
        }
    }
}