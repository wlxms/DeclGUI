using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// AbsolutePanel组件的Editor渲染器
    /// 不参与自动布局，但可以使用自动布局系统渲染内容物
    /// </summary>
    public class EditorAbsolutePanelRenderer : EditorElementRenderer<AbsolutePanel>
    {
        public override void Render(RenderManager mgr, in AbsolutePanel element, in IDeclStyle styleParam)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            var currentStyle = styleParam ?? element.Style;
            var style = editorMgr.ApplyStyle(currentStyle, null);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 保存当前GUI状态
            var matrix = GUI.matrix;
            var color = GUI.color;

            try
            {
                // 创建一个临时区域来计算子元素的大小
                Rect calculatedRect = new Rect(element.Position.x, element.Position.y, 0, 0);

                // 如果指定了固定尺寸，使用固定尺寸
                if (width > 0 && height > 0)
                {
                    calculatedRect.width = width;
                    calculatedRect.height = height;
                }
                else
                {
                    // 计算子元素的期望大小
                    var childSize = CalculateChildSize(mgr, element.Child, style,
                        element.MinWidth, element.MinHeight, element.MaxWidth, element.MaxHeight);

                    calculatedRect.width = childSize.x;
                    calculatedRect.height = childSize.y;
                }

                // 应用尺寸限制
                if (element.MinWidth.HasValue && calculatedRect.width < element.MinWidth.Value)
                    calculatedRect.width = element.MinWidth.Value;
                if (element.MinHeight.HasValue && calculatedRect.height < element.MinHeight.Value)
                    calculatedRect.height = element.MinHeight.Value;
                if (element.MaxWidth.HasValue && calculatedRect.width > element.MaxWidth.Value)
                    calculatedRect.width = element.MaxWidth.Value;
                if (element.MaxHeight.HasValue && calculatedRect.height > element.MaxHeight.Value)
                    calculatedRect.height = element.MaxHeight.Value;

                // 开始绝对定位区域
                GUILayout.BeginArea(calculatedRect, style);

                // 在绝对定位区域内使用自动布局渲染子元素
                if (element.Child != null)
                {
                    mgr.RenderElement(element.Child);
                }

                GUILayout.EndArea();
            }
            finally
            {
                // 恢复GUI状态
                GUI.matrix = matrix;
                GUI.color = color;
            }
        }

        /// <summary>
        /// 计算子元素的期望大小
        /// 使用渲染管理器的CalculateElementSize方法来准确计算大小
        /// </summary>
        private Vector2 CalculateChildSize(RenderManager mgr, IElement child, GUIStyle style,
                                          float? minWidth, float? minHeight, float? maxWidth, float? maxHeight)
        {
            if (child == null)
                return Vector2.zero;

            // 使用渲染管理器计算子元素的期望大小
            Vector2 size = mgr.CalculateElementSize(child, null);

            // 应用尺寸限制
            if (minWidth.HasValue && size.x < minWidth.Value) size.x = minWidth.Value;
            if (minHeight.HasValue && size.y < minHeight.Value) size.y = minHeight.Value;
            if (maxWidth.HasValue && size.x > maxWidth.Value) size.x = maxWidth.Value;
            if (maxHeight.HasValue && size.y > maxHeight.Value) size.y = maxHeight.Value;

            return size;
        }

        public override Vector2 CalculateSize(RenderManager mgr, in AbsolutePanel element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var guiStyle = editorMgr.ApplyStyle(currentStyle, null);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 如果指定了固定尺寸，使用固定尺寸
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            // 计算子元素的期望大小
            var childSize = CalculateChildSize(mgr, element.Child, guiStyle,
                element.MinWidth, element.MinHeight, element.MaxWidth, element.MaxHeight);

            // 应用尺寸限制
            return ApplySizeConstraints(childSize,
                element.MinWidth, element.MinHeight,
                element.MaxWidth, element.MaxHeight);
        }
    }
}