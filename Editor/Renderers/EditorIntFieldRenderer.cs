using DeclGUI.Components;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// IntField组件的Editor渲染器
    /// </summary>
    public class EditorIntFieldRenderer : EditorElementRenderer<IntField>
    {
        /// <summary>
        /// 渲染IntField组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr, in IntField element, in IDeclStyle styleParam)
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
                
                var newValue = element.Value;

                if (width > 0)
                {
                    // 渲染整数字段
                    newValue = EditorGUILayout.IntField(
                        element.Value,
                        style,
                        GUILayout.Width(width)
                    );
                }
                else
                {
                    // 渲染整数字段
                    newValue = EditorGUILayout.IntField(
                        element.Value,
                        style
                    );
                }



                // 检查值是否变化并触发回调
                if (newValue != element.Value && element.OnValueChanged != null)
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
        /// 计算IntField元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in IntField element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var guiStyle = editorMgr.ApplyStyle(currentStyle, EditorStyles.numberField);
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 使用 Unity 的标准方法计算整数字段尺寸
            var content = new GUIContent(element.Value.ToString());
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