using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Horizontal布局组件的Editor渲染器
    /// </summary>
    public class EditorHorizontalRenderer : EditorElementRenderer<Hor>
    {
        public override void Render(RenderManager mgr, in Hor element, in IDeclStyle styleParam)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            var currentStyle = styleParam ?? element.Style;
            var backgroundColor = currentStyle?.BackgroundColor;
            GUIStyle defaultStyle = null;
            switch (element.BoxSkin)
            {
                case BoxSkin.HelpBox:
                    defaultStyle = EditorStyles.helpBox;
                    break;
                case BoxSkin.Box:
                    defaultStyle = GUI.skin.box;
                    break;
                default:
                    defaultStyle = backgroundColor.HasValue ? GUI.skin.box : null;
                    break;
            }

            var style = editorMgr.ApplyStyle(currentStyle, defaultStyle);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);
            using (DeclEditorGUI.BeginBackgroundColor(backgroundColor))
            {
                // 开始水平布局
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
            }

            var oldIndentLevel = EditorGUI.indentLevel;
            try
            {
                EditorGUI.indentLevel = 0;
                // 渲染所有子元素（使用状态栈）
                foreach (var child in element)
                {
                    mgr.RenderElement(child);
                }
            }
            finally
            {
                EditorGUI.indentLevel = oldIndentLevel;
                GUILayout.EndHorizontal();
            }
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

        public override Vector2 CalculateSize(RenderManager mgr, in Hor element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var guiStyle = editorMgr.ApplyStyle(currentStyle, null);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 如果设置了固定尺寸，使用固定尺寸
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            RectOffset firstMargin = new RectOffset(0, 0, 0, 0);
            RectOffset lastMargin = new RectOffset(0, 0, 0, 0);

            // 计算所有子元素的大小（这是水平布局的正确计算方式）
            Vector2 contentSize = Vector2.zero;
            var lastChildRightMargin = 0f;

            for (int index = 0; index < element.Count; index++)
            {
                var child = element[index];
                var nextChild = index < element.Count - 1 ? element[index + 1] : null;

                IDeclStyle targetStyle = null;
                GUIStyle guiStyleChild = null;
                if (child is IStylefulElement stylefulElement)
                {
                    var childStyle = mgr.ResolveStyleWithCache(stylefulElement.Style, PseudoClass.Normal);
                    guiStyleChild = editorMgr.ApplyStyle(childStyle, null);
                }
                var childSize = mgr.CalculateElementSize(child, targetStyle); // 子元素使用自己的样式

                if (guiStyleChild != null)
                {
                    if (index == 0)
                    {
                        firstMargin = guiStyleChild.margin;
                    }
                    contentSize.x += childSize.x;

                    var top = Mathf.Max(guiStyleChild.margin.top, guiStyleChild.padding.top);
                    var bottom = Mathf.Max(guiStyleChild.margin.bottom, guiStyleChild.padding.bottom);

                    contentSize.y = Mathf.Max(contentSize.y, (childSize.y - guiStyleChild.padding.top - guiStyleChild.padding.bottom) + top + bottom);

                    if (lastChildRightMargin > 0)
                    {
                        contentSize.x += Mathf.Max(lastChildRightMargin, guiStyleChild.margin.left);
                    }

                    if (nextChild != null)
                    {
                        lastChildRightMargin = guiStyleChild.margin.right;
                    }

                    if (nextChild == null)
                    {
                        lastMargin = guiStyleChild.margin;
                    }
                }
            }


            // 添加容器的padding
            Vector2 totalSize = contentSize;
            if (guiStyle != null)
            {
                float leftSpace = Mathf.Max(guiStyle.padding.left, firstMargin.left);
                float rightSpace = Mathf.Max(guiStyle.padding.right, lastMargin.right);
                totalSize.x += leftSpace + rightSpace;
                totalSize.y += guiStyle.padding.top + guiStyle.padding.bottom;
            }

            return totalSize;
        }
    }
}