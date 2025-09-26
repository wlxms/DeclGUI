using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Button组件的Editor渲染器
    /// </summary>
    public class EditorButtonRenderer : EditorElementRenderer<Button>
    {
        public override void Render(RenderManager mgr, in Button element, in IDeclStyle styleParam)
        {
            float timeStartRender = Time.time;
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            // 检查元素状态中的禁用状态
            bool isDisabled = false;
            var elementState = mgr.GetElementState(element);
                isDisabled = elementState.HasState(ElementStateFlags.Disabled);

            // 保存当前GUI enabled状态
            bool originalGUIEnabled = GUI.enabled;

            // 在禁用状态下禁用GUI（上下文禁用或元素状态禁用任一为真）
            GUI.enabled = !(isDisabled);

            var currentStyle = styleParam ?? element.Style;
            var style = editorMgr.ApplyStyle(currentStyle, GUI.skin.button);
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
                    if (GUILayout.Button(element.Text, style, GUILayout.Width(width), GUILayout.Height(height)))
                    {
                        element.Events.OnClick?.Invoke();
                    }
                }
                else if (width > 0)
                {
                    if (GUILayout.Button(element.Text, style, GUILayout.Width(width)))
                    {
                        element.Events.OnClick?.Invoke();
                    }
                }
                else if (height > 0)
                {
                    if (GUILayout.Button(element.Text, style, GUILayout.Height(height)))
                    {
                        element.Events.OnClick?.Invoke();
                    }
                }
                else
                {
                    if (GUILayout.Button(element.Text, style))
                    {
                        element.Events.OnClick?.Invoke();
                    }
                }
            }
            finally
            {
                // 恢复原始颜色
                GUI.backgroundColor = originalBackgroundColor;
                GUI.color = originalColor;
                GUI.contentColor = originalContentColor;
                
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
            // 记录渲染时间
            float timeEndRender = Time.time;
            float timeCostRender = timeEndRender - timeStartRender;

        }
        public override Vector2 CalculateSize(RenderManager mgr, in Button element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var guiStyle = editorMgr.ApplyStyle(currentStyle, GUI.skin.button);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 如果设置了固定尺寸，直接返回
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            // 计算文本内容大小
            Vector2 textSize = CalculateTextSize(element.Text, guiStyle, width > 0 ? width : 0);

            // 添加按钮的padding和border
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