using DeclGUI.Components;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// FloatField组件的Editor渲染器
    /// </summary>
    public class EditorFloatFieldRenderer : EditorElementRenderer<FloatField>
    {
        /// <summary>
        /// 渲染FloatField组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr, in FloatField element, in IDeclStyle styleParam)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            // 检查ReadOnly上下文
            bool isReadOnly = false;
            if (mgr.ContextStack.TryGet<DisableContext>(out var readOnlyContext))
            {
                isReadOnly = readOnlyContext.Value;
            }

            // 保存当前GUI enabled状态
            bool originalGUIEnabled = GUI.enabled;
            
            // 在只读状态下禁用GUI
            GUI.enabled = !isReadOnly;

            var currentStyle = styleParam ?? element.Style;
            
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
                var style = editorMgr.ApplyStyle(styleParam ?? element.Style, EditorStyles.numberField);
                var width = editorMgr.GetStyleWidth(styleParam ?? element.Style);

                // 渲染浮点数字段
                var newValue = EditorGUILayout.FloatField(
                    element.Value,
                    style,
                    GUILayout.Width(width > 0 ? width : 100)
                );

                // 检查值是否变化并触发回调
                if (!Mathf.Approximately(newValue, element.Value) && element.OnValueChanged != null)
                {
                    element.OnValueChanged(newValue);
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
        }

        /// <summary>
        /// 计算FloatField元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in FloatField element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var guiStyle = editorMgr.ApplyStyle(currentStyle, EditorStyles.numberField);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 如果设置了固定尺寸，直接返回
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            // 计算文本内容大小
            Vector2 textSize = CalculateTextSize(element.Value.ToString("F2"), guiStyle, width > 0 ? width : 0);
            
            // 添加字段的padding和border
            Vector2 totalSize = textSize;
            if (guiStyle != null)
            {
                totalSize.x += guiStyle.padding.left + guiStyle.padding.right;
                totalSize.y += guiStyle.padding.top + guiStyle.padding.bottom;
                
                totalSize.x += guiStyle.border.left + guiStyle.border.right;
                totalSize.y += guiStyle.border.top + guiStyle.border.bottom;
                
                // 确保至少有最小尺寸
                totalSize.x = Mathf.Max(totalSize.x, guiStyle.fixedWidth > 0 ? guiStyle.fixedWidth : 100);
                totalSize.y = Mathf.Max(totalSize.y, guiStyle.fixedHeight > 0 ? guiStyle.fixedHeight : 0);
            }

            // 应用尺寸约束
            if (width > 0) totalSize.x = width;
            if (height > 0) totalSize.y = height;

            return totalSize;
        }
    }
}