using System;
using UnityEngine;
using DeclGUI.Components;
using DeclGUI.Core;
using System.Collections.Generic;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// DisableGroup组件的Editor渲染器
    /// </summary>
    public class DisableGroupRenderer : EditorElementRenderer<DisableGroup>
    {
        public override void Render(RenderManager mgr, in DisableGroup element, in IDeclStyle styleParam)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            // 保存当前GUI enabled状态
            bool originalGUIEnabled = GUI.enabled;

            // 根据DisableGroup的IsDisabled属性创建ReadOnly上下文
            var readOnlyContext = new DisableContext(element.IsDisabled);

            // 推送ReadOnly上下文
            mgr.PushContext(readOnlyContext);

            try
            {
                // 根据是否禁用来设置GUI.enabled状态
                if (element.IsDisabled)
                {
                    GUI.enabled = false;
                }

                // 应用样式（如果提供）
                var currentStyle = styleParam ?? element.Style;
                var style = editorMgr.ApplyStyle(currentStyle, GUIStyle.none);

                // 渲染子元素
                foreach (var child in element)
                {
                    if (child != null)
                    {
                        mgr.RenderElement(child);
                    }
                }
            }
            finally
            {
                // 弹出ReadOnly上下文
                mgr.PopContext<DisableContext>();

                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
        }

        public override Vector2 CalculateSize(RenderManager mgr, in DisableGroup element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var guiStyle = editorMgr.ApplyStyle(currentStyle, GUIStyle.none);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 如果设置了固定尺寸，使用固定尺寸
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            RectOffset firstMargin = new RectOffset(0, 0, 0, 0);
            RectOffset lastMargin = new RectOffset(0, 0, 0, 0);

            // 计算所有子元素的大小（基于EditorVerticalRenderer的逻辑）
            Vector2 contentSize = Vector2.zero;



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

            return totalSize;
        }
    }
}