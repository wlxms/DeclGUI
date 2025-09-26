using DeclGUI.Components;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// EnumPopup组件的Editor渲染器
    /// </summary>
    public class EditorEnumPopupRenderer : EditorElementRenderer<EnumPopup>
    {
        /// <summary>
        /// 渲染EnumPopup组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr, in EnumPopup element, in IDeclStyle styleParam)
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
                var currentStyle = styleParam ?? element.Style;
                var style = editorMgr.ApplyStyle(currentStyle, EditorStyles.popup);
                var width = editorMgr.GetStyleWidth(currentStyle);

                // 渲染枚举下拉选择框
                var newValue = EditorGUILayout.EnumPopup(
                    element.Value,
                    GUILayout.Width(width > 0 ? width : 120)
                );

                // 检查值是否变化并触发回调
                if (!Equals(newValue, element.Value) && element.OnValueChanged != null)
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
        /// 计算EnumPopup元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in EnumPopup element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var guiStyle = editorMgr.ApplyStyle(style ?? element.Style, EditorStyles.popup);
            var width = editorMgr.GetStyleWidth(style ?? element.Style);
            
            // 获取枚举值的显示名称
            var enumValue = element.Value;
            var displayName = enumValue?.ToString() ?? "None";

            // 使用GUILayout来测量枚举下拉框的大小
            if (width > 0)
            {
                return new Vector2(width, guiStyle.CalcHeight(new GUIContent(displayName), width));
            }
            else
            {
                var size = guiStyle.CalcSize(new GUIContent(displayName));
                size.y = guiStyle.CalcHeight(new GUIContent(displayName), size.x);
                return size;
            }
        }
    }
}