using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// FoldoutGroup组件的Editor渲染器
    /// </summary>
    public class EditorFoldoutGroupRenderer : EditorElementRenderer<FoldoutGroup, FoldoutGroupState>
    {
        /// <summary>
        /// 渲染方法（有状态版本）
        /// </summary>
        public override void Render(RenderManager mgr, in FoldoutGroup element, FoldoutGroupState state, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            // 获取样式
            var currentStyle = style ?? element.Style;
            var headerStyle = element.HeaderStyle;

            // 创建用于标题的GUI样式
            var headerGuiStyle = editorMgr.ApplyStyle(headerStyle, GUI.skin.button);

            // 使用Unity的内置Foldout控件
            bool newExpandedState = EditorGUI.Foldout(
                EditorGUILayout.GetControlRect(),
                state.IsExpanded,
                string.IsNullOrEmpty(element.Header) ? GUIContent.none : new GUIContent(element.Header),
                true
            );


            // 如果状态改变，更新状态
            if (newExpandedState != state.IsExpanded)
            {
                state.IsExpanded = newExpandedState;
            }

            // 如果展开，渲染子元素
            if (state.IsExpanded)
            {
                // 应用背景颜色
                var backgroundColor = currentStyle?.BackgroundColor;
                if (backgroundColor.HasValue)
                {
                    // 保存当前GUI颜色
                    var originalBackgroundColor = GUI.backgroundColor;
                    var originalColor = GUI.color;

                    try
                    {
                        // 设置背景颜色
                        GUI.backgroundColor = backgroundColor.Value;

                        // 渲染子元素
                        foreach (var childElement in element)
                        {
                            if (childElement != null)
                            {
                                mgr.RenderElement(childElement);
                            }
                        }
                    }
                    finally
                    {
                        // 恢复原始颜色
                        GUI.backgroundColor = originalBackgroundColor;
                        GUI.color = originalColor;
                    }
                }
                else
                {
                    // 没有背景颜色时，直接渲染子元素
                    foreach (var childElement in element)
                    {
                        if (childElement != null)
                        {
                            mgr.RenderElement(childElement);
                        }
                    }
                }
            }
        }

        public override Vector2 CalculateSize(RenderManager mgr, in FoldoutGroup element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var headerStyle = element.HeaderStyle;
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 如果设置了固定尺寸，使用固定尺寸
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            // 计算标题大小
            var headerGuiStyle = editorMgr.ApplyStyle(headerStyle, GUI.skin.button);
            Vector2 headerSize = headerGuiStyle.CalcSize(new GUIContent(element.Header));

            // 计算子元素大小（如果展开）
            float totalHeight = headerSize.y + 5; // 标题高度 + 间距

            // 获取元素状态以确定是否展开
            FoldoutGroupState state = null;
            if (!mgr.StateStack.IsEmpty())
            {
                IElementWithKey elementWithKey = element;
                var elementState = mgr.StateStack.CurrentStateManager.GetOrCreateState(elementWithKey);
                if (elementState != null && elementState.State is FoldoutGroupState foldoutState)
                {
                    state = foldoutState;
                }
            }

            // 如果没有状态，使用初始展开状态
            if (state == null)
            {
                state = new FoldoutGroupState(element.InitialExpanded);
            }

            if (state.IsExpanded)
            {
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

                float topSpace = Mathf.Max(0, firstMargin.top);
                float bottomSpace = Mathf.Max(0, lastMargin.bottom);

                // 添加容器的padding
                Vector2 totalSize = contentSize;
                totalSize.y += topSpace + bottomSpace;

                totalHeight += totalSize.y;
            }

            // 应用宽度约束
            if (width <= 0)
            {
                width = headerSize.x; // 默认使用标题的宽度
            }

            return new Vector2(width, totalHeight);
        }
    }
}
