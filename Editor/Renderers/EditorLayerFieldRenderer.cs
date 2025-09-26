using DeclGUI.Components;
using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// LayerField组件的Editor渲染器
    /// </summary>
    public class EditorLayerFieldRenderer : EditorElementRenderer<LayerField>
    {
        /// <summary>
        /// 渲染LayerField组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr, in LayerField element, in IDeclStyle styleParam)
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
                var style = editorMgr.ApplyStyle(styleParam ?? element.Style, EditorStyles.popup);
                var width = editorMgr.GetStyleWidth(styleParam ?? element.Style);

                // 渲染图层选择器
                var newLayerIndex = EditorGUILayout.LayerField(
                    element.LayerIndex,
                    GUILayout.Width(width > 0 ? width : 120)
                );

                // 检查值是否变化并触发回调
                if (newLayerIndex != element.LayerIndex && element.OnValueChanged != null)
                {
                    element.OnValueChanged(newLayerIndex);
                }
            }
            finally
            {
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
        }

        /// <summary>
        /// 计算LayerField元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in LayerField element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var guiStyle = editorMgr.ApplyStyle(style ?? element.Style, EditorStyles.popup);
            var width = editorMgr.GetStyleWidth(style ?? element.Style);
            
            // 获取图层名称
            var layerName = LayerMask.LayerToName(element.LayerIndex);
            if (string.IsNullOrEmpty(layerName))
            {
                layerName = "Default";
            }

            // 使用GUILayout来测量图层选择器的大小
            if (width > 0)
            {
                return new Vector2(width, guiStyle.CalcHeight(new GUIContent(layerName), width));
            }
            else
            {
                var size = guiStyle.CalcSize(new GUIContent(layerName));
                size.y = guiStyle.CalcHeight(new GUIContent(layerName), size.x);
                return size;
            }
        }
    }
}