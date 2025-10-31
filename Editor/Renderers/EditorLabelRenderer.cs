using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Label组件的Editor渲染器
    /// </summary>
    public class EditorLabelRenderer : EditorElementRenderer<Label>
    {
        public override void Render(RenderManager mgr, in Label element, in IDeclStyle styleParam)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            var currentStyle = styleParam ?? element.Style;
            var style = editorMgr.ApplyStyle(currentStyle, GUI.skin.label);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 保存原始颜色
            var originalBackgroundColor = GUI.backgroundColor;
            var originalColor = GUI.color;
            var originalContentColor = GUI.contentColor;

            // 应用样式颜色
            if (currentStyle?.BackgroundColor != null)
            {
                GUI.backgroundColor = currentStyle.BackgroundColor.Value;
            }
            
            if (currentStyle?.Color != null)
            {
                GUI.color = currentStyle.Color.Value;
                // 同时设置contentColor以确保文字颜色正确
                GUI.contentColor = currentStyle.Color.Value;
            }

            try
            {
                if (width > 0 && height > 0)
                {
                    GUILayout.Label(element.Text, style, GUILayout.Width(width), GUILayout.Height(height));
                }
                else if (width > 0)
                {
                    GUILayout.Label(element.Text, style, GUILayout.Width(width));
                }
                else if (height > 0)
                {
                    GUILayout.Label(element.Text, style, GUILayout.Height(height));
                }
                else
                {
                    GUILayout.Label(element.Text, style);
                }
            }
            finally
            {
                // 恢复原始颜色
                GUI.backgroundColor = originalBackgroundColor;
                GUI.color = originalColor;
                GUI.contentColor = originalContentColor;
            }
        }

        /// <summary>
        /// 获取Label元素的屏幕区域
        /// 在Editor环境下，需要跟踪最后渲染的矩形区域
        /// </summary>
        /// <returns>Label的屏幕矩形</returns>
        public override Rect GetElementRect()
        {
            // 在Editor环境下，可以使用GUILayoutUtility.GetLastRect()获取最后渲染的矩形
            // 注意：这需要在渲染后立即调用才有效
            return GUILayoutUtility.GetLastRect();
        }

        /// <summary>
        /// 计算Label的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in Label element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var guiStyle = editorMgr.ApplyStyle(currentStyle, GUI.skin.label);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 使用 Unity 的标准方法计算标签尺寸
            var content = new GUIContent(element.Text);
            Vector2 totalSize;
            
            if (width > 0 && height > 0)
            {
                // 如果设置了固定宽度和高度，直接使用
                totalSize = new Vector2(width, height);
            }
            else if (width > 0)
            {
                // 如果设置了固定宽度，计算自适应高度
                totalSize = new Vector2(width, guiStyle.CalcHeight(content, width));
            }
            else if (height > 0)
            {
                // 如果设置了固定高度，计算自适应宽度
                totalSize = new Vector2(guiStyle.CalcSize(content).x, height);
            }
            else
            {
                // 完全自适应尺寸
                totalSize = guiStyle.CalcSize(content);
            }

            // 确保至少有最小尺寸
            if (guiStyle.fixedWidth > 0)
                totalSize.x = Mathf.Max(totalSize.x, guiStyle.fixedWidth);
            if (guiStyle.fixedHeight > 0)
                totalSize.y = Mathf.Max(totalSize.y, guiStyle.fixedHeight);

            return totalSize;
        }
    }
}