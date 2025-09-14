using DeclGUI.Components;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// TagField组件的Editor渲染器
    /// </summary>
    public class EditorTagFieldRenderer : EditorElementRenderer<TagField>
    {
        /// <summary>
        /// 渲染TagField组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr,in TagField element)
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
                var style = editorMgr.ApplyStyle(element.Style, EditorStyles.popup);
                var width = editorMgr.GetStyleWidth(element.Style);

                // 渲染标签选择器
                var newTag = EditorGUILayout.TagField(
                    element.Tag,
                    style,
                    GUILayout.Width(width > 0 ? width : 120)
                );

                // 检查值是否变化并触发回调
                if (newTag != element.Tag && element.OnValueChanged != null)
                {
                    element.OnValueChanged(newTag);
                }
            }
            finally
            {
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
        }

        /// <summary>
        /// 计算TagField元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr,in TagField element,in DeclStyle? style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var guiStyle = editorMgr.ApplyStyle(style ?? element.Style, EditorStyles.popup);
            var width = editorMgr.GetStyleWidth(style ?? element.Style);
            
            // 使用GUILayout来测量标签选择器的大小
            if (width > 0)
            {
                return new Vector2(width, guiStyle.CalcHeight(new GUIContent(element.Tag), width));
            }
            else
            {
                var size = guiStyle.CalcSize(new GUIContent(element.Tag));
                size.y = guiStyle.CalcHeight(new GUIContent(element.Tag), size.x);
                return size;
            }
        }
    }
}