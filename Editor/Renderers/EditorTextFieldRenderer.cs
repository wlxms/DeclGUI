using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// TextField组件的Editor渲染器
    /// </summary>
    public class EditorTextFieldRenderer : EditorElementRenderer<TextField>
    {
        public override void Render(RenderManager mgr,in TextField element, in IDeclStyle styleParam)
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
            var style = editorMgr.ApplyStyle(currentStyle, GUI.skin.textField);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            try
            {
                string currentValue = element.Value;

                if (width > 0 && height > 0)
                {
                    var newValue = GUILayout.TextField(currentValue, style, GUILayout.Width(width), GUILayout.Height(height));
                    CheckValueChanged(element, currentValue, newValue);
                }
                else if (width > 0)
                {
                    var newValue = GUILayout.TextField(currentValue, style, GUILayout.Width(width));
                    CheckValueChanged(element, currentValue, newValue);
                }
                else if (height > 0)
                {
                    var newValue = GUILayout.TextField(currentValue, style, GUILayout.Height(height));
                    CheckValueChanged(element, currentValue, newValue);
                }
                else
                {
                    var newValue = GUILayout.TextField(currentValue, style);
                    CheckValueChanged(element, currentValue, newValue);
                }
            }
            finally
            {
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
        }

        private void CheckValueChanged(TextField element, string oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                element.OnValueChanged?.Invoke(newValue);
            }
        }

        public override Vector2 CalculateSize(RenderManager mgr,in TextField element,in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var guiStyle = editorMgr.ApplyStyle(currentStyle, GUI.skin.textField);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 如果设置了固定尺寸，直接返回
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            // 计算文本字段的大小
            Vector2 textSize = CalculateTextSize(element.Value ?? "", guiStyle, width > 0 ? width : 0);
            
            // 添加文本字段的padding和border
            if (guiStyle != null)
            {
                textSize.x += guiStyle.padding.left + guiStyle.padding.right;
                textSize.y += guiStyle.padding.top + guiStyle.padding.bottom;
                textSize.x += guiStyle.border.left + guiStyle.border.right;
                textSize.y += guiStyle.border.top + guiStyle.border.bottom;
                
                // 确保至少有最小尺寸
                textSize.x = Mathf.Max(textSize.x, guiStyle.fixedWidth > 0 ? guiStyle.fixedWidth : 0);
                textSize.y = Mathf.Max(textSize.y, guiStyle.fixedHeight > 0 ? guiStyle.fixedHeight : 0);
            }

            // 应用尺寸约束
            if (width > 0) textSize.x = width;
            if (height > 0) textSize.y = height;

            return textSize;
        }
    }
}