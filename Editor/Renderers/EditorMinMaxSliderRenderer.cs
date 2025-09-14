using DeclGUI.Components;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// MinMaxSlider组件的Editor渲染器
    /// </summary>
    public class EditorMinMaxSliderRenderer : EditorElementRenderer<MinMaxSlider>
    {
        /// <summary>
        /// 渲染MinMaxSlider组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr,in MinMaxSlider element, in IDeclStyle styleParam)
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
                var style = editorMgr.ApplyStyle(styleParam ?? element.Style, EditorStyles.numberField);
                var width = editorMgr.GetStyleWidth(styleParam ?? element.Style);

                // 复制值用于修改
                float minValue = element.MinValue;
                float maxValue = element.MaxValue;

                // 渲染最小-最大范围滑块
                EditorGUILayout.MinMaxSlider(
                    ref minValue,
                    ref maxValue,
                    element.MinLimit,
                    element.MaxLimit,
                    GUILayout.Width(width > 0 ? width : 120)
                );

                // 检查值是否变化并触发回调
                if ((minValue != element.MinValue || maxValue != element.MaxValue) && element.OnValueChanged != null)
                {
                    element.OnValueChanged(minValue, maxValue);
                }
            }
            finally
            {
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
        }

        /// <summary>
        /// 计算MinMaxSlider元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr,in MinMaxSlider element,in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var width = editorMgr.GetStyleWidth(style ?? element.Style);
            
            // 最小-最大滑块的标准大小
            var standardSize = new Vector2(120, 16);
            
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