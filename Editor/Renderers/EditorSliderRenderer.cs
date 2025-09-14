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
        public override void Render(RenderManager mgr,in Slider element)
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

            try
            {
                var style = editorMgr.ApplyStyle(element.Style, EditorStyles.label);
                var width = editorMgr.GetStyleWidth(element.Style);
                
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
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
        }

        /// <summary>
        /// 计算Slider元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr,in Slider element,in DeclStyle? style)
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