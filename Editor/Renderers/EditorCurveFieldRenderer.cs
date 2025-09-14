using DeclGUI.Components;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// CurveField组件的Editor渲染器
    /// </summary>
    public class EditorCurveFieldRenderer : EditorElementRenderer<CurveField>
    {
        /// <summary>
        /// 渲染CurveField组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr, in CurveField element, in IDeclStyle styleParam)
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
                var style = editorMgr.ApplyStyle(styleParam ?? element.Style, EditorStyles.objectField);
                var width = editorMgr.GetStyleWidth(styleParam ?? element.Style);

                // 渲染曲线编辑器
                var newValue = EditorGUILayout.CurveField(
                    element.Value,
                    GUILayout.Width(width > 0 ? width : 150),
                    GUILayout.Height(editorMgr.GetStyleHeight(styleParam ?? element.Style) > 0 ?
                        editorMgr.GetStyleHeight(styleParam ?? element.Style) : 60)
                );

                // 检查值是否变化并触发回调
                if (newValue != element.Value && element.OnValueChanged != null)
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
        /// 计算CurveField元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in CurveField element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var width = editorMgr.GetStyleWidth(style ?? element.Style);
            var height = editorMgr.GetStyleHeight(style ?? element.Style);

            // 曲线字段的标准大小
            var standardSize = new Vector2(150, 60);

            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }
            else if (width > 0)
            {
                return new Vector2(width, standardSize.y);
            }
            else if (height > 0)
            {
                return new Vector2(standardSize.x, height);
            }
            else
            {
                return standardSize;
            }
        }
    }
}