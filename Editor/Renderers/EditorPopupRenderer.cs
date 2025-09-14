using DeclGUI.Components;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Popup组件的Editor渲染器
    /// </summary>
    public class EditorPopupRenderer : EditorElementRenderer<Popup>
    {
        /// <summary>
        /// 渲染Popup组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr,in Popup element)
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

                // 确保选项列表不为空
                var options = element.Options ?? new string[0];
                if (options.Length == 0)
                {
                    options = new[] { "No Options" };
                }

                // 确保选中索引在有效范围内
                var selectedIndex = Mathf.Clamp(element.SelectedIndex, 0, options.Length - 1);

                // 渲染下拉选择框
                var newIndex = EditorGUILayout.Popup(
                    selectedIndex,
                    options,
                    style,
                    GUILayout.Width(width > 0 ? width : 120)
                );

                // 检查选择是否变化并触发回调
                if (newIndex != element.SelectedIndex && element.OnSelectionChanged != null)
                {
                    element.OnSelectionChanged(newIndex);
                }
            }
            finally
            {
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
        }

        /// <summary>
        /// 计算Popup元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr,in Popup element,in DeclStyle? style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var guiStyle = editorMgr.ApplyStyle(style ?? element.Style, EditorStyles.popup);
            var width = editorMgr.GetStyleWidth(style ?? element.Style);
            
            // 确保选项列表不为空
            var options = element.Options ?? new string[0];
            if (options.Length == 0)
            {
                options = new[] { "No Options" };
            }

            // 找到最长的选项文本
            var longestOption = "";
            foreach (var option in options)
            {
                if (option.Length > longestOption.Length)
                {
                    longestOption = option;
                }
            }

            // 使用GUILayout来测量下拉框的大小
            if (width > 0)
            {
                return new Vector2(width, guiStyle.CalcHeight(new GUIContent(longestOption), width));
            }
            else
            {
                var size = guiStyle.CalcSize(new GUIContent(longestOption));
                size.y = guiStyle.CalcHeight(new GUIContent(longestOption), size.x);
                return size;
            }
        }
    }
}