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
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            // 检查ReadOnly上下文
            bool isReadOnly = false;
            if (mgr.ContextStack.TryGet<ReadOnly>(out var readOnlyContext))
            {
                isReadOnly = readOnlyContext.Value;
            }

            // 保存当前GUI enabled状态
            bool originalGUIEnabled = GUI.enabled;
            
            // 在只读状态下禁用GUI
            GUI.enabled = !isReadOnly;

            var currentStyle = styleParam ?? element.Style;
            var style = editorMgr.ApplyStyle(currentStyle, GUI.skin.button);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

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
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }

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