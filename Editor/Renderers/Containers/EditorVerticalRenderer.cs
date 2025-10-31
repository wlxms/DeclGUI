using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Vertical布局组件的Editor渲染器
    /// </summary>
    public class EditorVerRenderer : EditorElementRenderer<Ver>
    {
        public override void Render(RenderManager mgr, in Ver element, in IDeclStyle styleParam)
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
                // 开始垂直布局
                if (width > 0 && height > 0)
                {
                    GUILayout.BeginVertical(style, GUILayout.Width(width), GUILayout.Height(height));
                }
                else if (width > 0)
                {
                    GUILayout.BeginVertical(style, GUILayout.Width(width));
                }
                else if (height > 0)
                {
                    GUILayout.BeginVertical(style, GUILayout.Height(height));
                }
                else
                {
                    GUILayout.BeginVertical(style);
                }
            }

            try
            {
                // 渲染所有子元素（使用状态栈）
                foreach (var child in element)
                {
                    mgr.RenderElement(child);
                }
            }
            finally
            {
                GUILayout.EndVertical();
                // if (editorMgr.IsRendering<ComposedPopupMenu>())
                //     Debug.Log($"Ver元素渲染完成，宽度: {width}, 高度: {height}, rect: {GUILayoutUtility.GetLastRect()}");

            }
        }

        /// <summary>
        /// 计算Ver元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in Ver element, in IDeclStyle style)
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

            // 计算所有子元素的大小（这是垂直布局的正确计算方式）
            Vector2 contentSize = Vector2.zero;

            // 存储每个子元素的尺寸和margin
            var childSizes = new List<(Vector2 size, RectOffset margin)>();

            var lastChildBottomMargin = 0f;
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
                    contentSize.y += childSize.y;

                    var left = Mathf.Max(guiStyleChild.margin.left, guiStyleChild.padding.left);
                    var right = Mathf.Max(guiStyleChild.margin.right, guiStyleChild.padding.right);

                    contentSize.x = Mathf.Max(contentSize.x, (childSize.x - guiStyleChild.padding.left - guiStyleChild.padding.right) + left + right);

                    if (lastChildBottomMargin > 0)
                    {
                        contentSize.y += Mathf.Max(lastChildBottomMargin, guiStyleChild.margin.top);
                    }

                    if (nextChild != null)
                    {
                        lastChildBottomMargin = guiStyleChild.margin.bottom;
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
                float topSpace = Mathf.Max(guiStyle.padding.top, firstMargin.top);
                float bottomSpace = Mathf.Max(guiStyle.padding.bottom, lastMargin.bottom);
                totalSize.x += guiStyle.padding.left + guiStyle.padding.right;
                totalSize.y += topSpace + bottomSpace;
            }

            // if (editorMgr.IsCalculating<ComposedPopupMenu>())
            // {
            //     Debug.LogError($"Ver元素计算期望大小, padding: {guiStyle.padding}, margin: {guiStyle.margin}, 内容宽度: {contentSize.x}, 内容高度: {contentSize.y}, 总宽度: {totalSize.x}, 总高度: {totalSize.y}");
            // }


            return totalSize;
        }
    }
}