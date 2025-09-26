using DeclGUI.Components;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Vector2Field组件的Editor渲染器
    /// </summary>
    public class EditorVector2FieldRenderer : EditorElementRenderer<Vector2Field>
    {
        /// <summary>
        /// 渲染Vector2Field组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr,in Vector2Field element, in IDeclStyle styleParam)
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

            try
            {
                var style = editorMgr.ApplyStyle(styleParam ?? element.Style, EditorStyles.numberField);
                var width = editorMgr.GetStyleWidth(styleParam ?? element.Style);

                // 渲染Vector2输入字段
                var newValue = EditorGUILayout.Vector2Field(
                    string.Empty,
                    element.Value,
                    GUILayout.Width(width > 0 ? width : 120)
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
        /// 计算Vector2Field元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr,in Vector2Field element,in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var width = editorMgr.GetStyleWidth(style ?? element.Style);
            
            // Vector2字段的标准大小
            var standardSize = new Vector2(120, 36);
            
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