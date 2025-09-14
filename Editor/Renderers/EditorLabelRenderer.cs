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
        public override void Render(RenderManager mgr,in Label element, in IDeclStyle styleParam)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            var currentStyle = styleParam ?? element.Style;
            var style = editorMgr.ApplyStyle(currentStyle, GUI.skin.label);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

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
public override Vector2 CalculateSize(RenderManager mgr,in Label element,in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var guiStyle = editorMgr.ApplyStyle(currentStyle, GUI.skin.label);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 如果设置了固定尺寸，直接返回
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            // 计算文本内容大小
            Vector2 textSize = CalculateTextSize(element.Text, guiStyle, width > 0 ? width : 0);
            
            // 添加标签的padding和border
            Vector2 totalSize = textSize;
            if (guiStyle != null)
            {
                totalSize.x += guiStyle.padding.left + guiStyle.padding.right;
                totalSize.y += guiStyle.padding.top + guiStyle.padding.bottom;
                
                totalSize.x += guiStyle.border.left + guiStyle.border.right;
                totalSize.y += guiStyle.border.top + guiStyle.border.bottom;
                
                // 确保至少有最小尺寸
                totalSize.x = Mathf.Max(totalSize.x, guiStyle.fixedWidth > 0 ? guiStyle.fixedWidth : 0);
                totalSize.y = Mathf.Max(totalSize.y, guiStyle.fixedHeight > 0 ? guiStyle.fixedHeight : 0);
            }

            // 应用尺寸约束
            if (width > 0) totalSize.x = width;
            if (height > 0) totalSize.y = height;

            return totalSize;
        }
    }
}