using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;
using System.Linq;
using UnityEditor;
using PlasticGui.WebApi.Responses;
using System.Collections.Generic;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// FoldoutHeaderGroup组件的Editor渲染器
    /// 使用Unity的EditorGUILayout.BeginFoldoutHeaderGroup实现
    /// </summary>
    public class EditorFoldoutHeaderGroupRenderer : EditorElementRenderer<FoldoutHeaderGroup, FoldoutHeaderGroupState>
    {

        /// <summary>
        /// 渲染方法（有状态版本）
        /// </summary>
        public override void Render(RenderManager mgr, in FoldoutHeaderGroup element, FoldoutHeaderGroupState state, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            var elementState = editorMgr.GetElementState(element);
            if (elementState == null)
                return;

            var editorState = elementState.GetState<EditorElementState>();
            if (editorState == null)
                return;

            // 获取样式
            var currentStyle = style ?? element.Style;
            var headerStyle = mgr.ResolveStyleWithCache(element.HeaderStyle, elementState);
            var headerHeight = editorMgr.GetStyleHeight(headerStyle);


            // 创建用于标题的GUI样式
            var headerGuiStyle = editorMgr.ApplyStyle(headerStyle, EditorStyles.foldoutHeader);
            var headerLayoutGuiStyle = editorMgr.ApplyStyle(headerStyle);

            headerGuiStyle.fixedHeight = headerHeight;

            var originalBackgroundColor = GUI.backgroundColor;
            var originalColor = GUI.color;
            try
            {
                using (RenderWithBackgroundColor(currentStyle?.BackgroundColor ?? Color.clear))
                {
                    GUI.Box(editorState.RenderRect, GUIContent.none);
                }

                GUILayout.BeginVertical();

                var newExpandedState = state.IsExpanded;

                try
                {
                    // 应用背景颜色
                    var backgroundColor = headerStyle?.BackgroundColor;
                    if (backgroundColor.HasValue)
                    {
                        GUI.backgroundColor = backgroundColor.Value;
                    }
                    newExpandedState = EditorGUILayout.BeginFoldoutHeaderGroup(
                        state.IsExpanded,
                        GUIContent.none,
                        headerGuiStyle
                    );
                }
                finally
                {
                    // 恢复原始颜色
                    GUI.backgroundColor = originalBackgroundColor;
                    GUI.color = originalColor;
                }

                if (Event.current.type == EventType.Repaint)
                {
                    state.lastHeaderRect = GUILayoutUtility.GetLastRect();
                }
                GUILayout.BeginArea(state.lastHeaderRect, headerLayoutGuiStyle);
                GUILayout.BeginHorizontal();
                mgr.RenderElement(element.HeaderElement);
                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                // var splitRect = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true));
                // splitRect.x -= 10;
                // splitRect.width += 10;
                // GUI.Box(splitRect, GUIContent.none);
                DeclEditorGUI.DrawSplitLine(1, -10, bottomSpacing: 2);


                // 如果状态改变，更新状态
                if (newExpandedState != state.IsExpanded)
                {
                    state.IsExpanded = newExpandedState;
                }

                // 如果展开，渲染子元素
                if (state.IsExpanded)
                {
                    GUILayout.BeginVertical();
                    // 应用背景颜色
                    var contextColor = currentStyle?.Color;
                    // 保存当前GUI颜色
                    originalBackgroundColor = GUI.backgroundColor;
                    originalColor = GUI.color;
                    try
                    {
                        if (contextColor.HasValue)
                        {
                            // 设置背景颜色
                            GUI.backgroundColor = contextColor.Value;
                        }

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
                        GUILayout.EndVertical();
                    }
                }
            }
            finally
            {
                GUI.backgroundColor = originalBackgroundColor;
                GUI.color = originalColor;
                EditorGUILayout.EndFoldoutHeaderGroup();
                GUILayout.EndVertical();
                if (Event.current.type == EventType.Repaint)
                {
                    editorState.RenderRect = GUILayoutUtility.GetLastRect();
                }
            }
        }

        public override Vector2 CalculateSize(RenderManager mgr, in FoldoutHeaderGroup element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var headerStyle = element.HeaderStyle;

            // 计算标题大小
            Vector2 headerSize = mgr.CalculateElementSize(element.HeaderElement);

            // 计算子元素大小（如果展开）
            float totalHeight = headerSize.y + 5; // 标题高度 + 间距

            // 获取元素状态以确定是否展开
            IElementWithKey elementWithKey = element;
            var elementState = mgr.StateStack.CurrentStateManager.GetOrCreateState(elementWithKey);
            var foldoutState = elementState.State as FoldoutHeaderGroupState;
            if (foldoutState == null)
            {
                foldoutState = new FoldoutHeaderGroupState(element.InitialExpanded);
            }

            if (foldoutState.IsExpanded)
            {
                // 如果展开，需要计算子元素的总高度
                // 存储每个子元素的尺寸和margin
                var childSizes = new List<(Vector2 size, RectOffset margin)>();
                
                foreach (var childElement in element)
                {
                    if (childElement != null)
                    {
                        var childSize = mgr.CalculateElementSize(childElement, null);
                        RectOffset childMargin = null;

                        // 如果子元素有样式，获取margin但不直接加到尺寸上
                        if (childElement is IStylefulElement stylefulElement)
                        {
                            var childStyle = mgr.ResolveStyleWithCache(stylefulElement.Style, PseudoClass.Normal);
                            var guiStyleChild = editorMgr.ApplyStyle(childStyle, null);
                            if (guiStyleChild != null)
                            {
                                childMargin = guiStyleChild.margin;
                            }
                        }

                        childSizes.Add((childSize, childMargin));
                    }
                }

                // 计算垂直布局的总尺寸（FoldoutHeaderGroup是垂直布局）
                float childrenTotalHeight = 0;
                float maxChildWidth = 0;

                for (int i = 0; i < childSizes.Count; i++)
                {
                    var (childSize, childMargin) = childSizes[i];
                    
                    // 计算宽度：内容宽度 + 左右margin的最大值
                    float elementWidth = childSize.x;
                    if (childMargin != null)
                    {
                        elementWidth += childMargin.left + childMargin.right;
                    }
                    maxChildWidth = Mathf.Max(maxChildWidth, elementWidth);

                    // 计算高度：内容高度 + 垂直margin（考虑相邻元素margin重叠）
                    float elementHeight = childSize.y;
                    if (childMargin != null)
                    {
                        // 第一个元素：添加top margin
                        if (i == 0)
                        {
                            elementHeight += childMargin.top;
                        }
                        
                        // 最后一个元素：添加bottom margin
                        if (i == childSizes.Count - 1)
                        {
                            elementHeight += childMargin.bottom;
                        }
                        
                        // 中间元素：与下一个元素的margin取最大值
                        if (i < childSizes.Count - 1)
                        {
                            var (nextSize, nextMargin) = childSizes[i + 1];
                            if (childMargin != null && nextMargin != null)
                            {
                                elementHeight += Mathf.Max(childMargin.bottom, nextMargin.top);
                            }
                            else if (childMargin != null)
                            {
                                elementHeight += childMargin.bottom;
                            }
                            else if (nextMargin != null)
                            {
                                elementHeight += nextMargin.top;
                            }
                        }
                    }

                    childrenTotalHeight += elementHeight;
                }

                totalHeight += childrenTotalHeight;
            }

            float width = currentStyle?.Width ?? 0;
            if (width <= 0)
            {
                width = headerSize.x; // 默认使用标题的宽度
            }

            return new Vector2(width, totalHeight);
        }
    }
}