using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;
using System.Linq;
using UnityEditor;

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
                // 如果展开，需要计算子元素的总高度
                foreach (var childElement in element)
                {
                    if (childElement != null)
                    {
                        var childSize = mgr.CalculateElementSize(childElement, null);
                        totalHeight += childSize.y;
                    }
                }
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
