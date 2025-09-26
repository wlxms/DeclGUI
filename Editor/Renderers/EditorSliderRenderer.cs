using DeclGUI.Components;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Slider组件的Editor渲染器
    /// </summary>
    public class EditorSliderRenderer : EditorElementRenderer<Slider>
    {
        /// <summary>
        /// 渲染Slider组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr, in Slider element, in IDeclStyle styleParam)
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

            // 保存原始颜色
            var originalBackgroundColor = GUI.backgroundColor;
            var originalColor = GUI.color;
            var originalContentColor = GUI.contentColor;



            try
            {
                var currentStyle = styleParam ?? element.Style;
                var style = editorMgr.ApplyStyle(currentStyle, EditorStyles.label);
                var width = editorMgr.GetStyleWidth(currentStyle);

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

                // 渲染滑块
                var newValue = EditorGUILayout.Slider(
                    element.Value,
                    element.MinValue,
                    element.MaxValue,
                    GUILayout.Width(width > 0 ? width : 200)
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
        /// 计算Slider元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in Slider element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var width = editorMgr.GetStyleWidth(style ?? element.Style);

            // 滑块的标准大小
            var standardSize = new Vector2(200, 16);

            if (width > 0)
            {
                return new Vector2(width, standardSize.y);
            }
            else
            {
                return standardSize;
            }
        }
    }
}
