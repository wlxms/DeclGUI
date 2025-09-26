using System;
using UnityEngine;
using DeclGUI.Components;
using DeclGUI.Core;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// DisableGroup组件的Editor渲染器
    /// </summary>
    public class DisableGroupRenderer : EditorElementRenderer<DisableGroup>
    {
        public override void Render(RenderManager mgr, in DisableGroup element, in IDeclStyle styleParam)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            // 保存当前GUI enabled状态
            bool originalGUIEnabled = GUI.enabled;

            // 根据DisableGroup的IsDisabled属性创建ReadOnly上下文
            var readOnlyContext = new DisableContext(element.IsDisabled);

            // 推送ReadOnly上下文
            mgr.PushContext(readOnlyContext);

            try
            {
                // 根据是否禁用来设置GUI.enabled状态
                if (element.IsDisabled)
                {
                    GUI.enabled = false;
                }

                // 应用样式（如果提供）
                var currentStyle = styleParam ?? element.Style;
                var style = editorMgr.ApplyStyle(currentStyle, GUIStyle.none);
                
                // 渲染子元素
                foreach (var child in element)
                {
                    if (child != null)
                    {
                        mgr.RenderElement(child);
                    }
                }
            }
            finally
            {
                // 弹出ReadOnly上下文
                mgr.PopContext<DisableContext>();
                
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
        }

        public override Vector2 CalculateSize(RenderManager mgr, in DisableGroup element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var guiStyle = editorMgr.ApplyStyle(currentStyle, GUIStyle.none);
            
            // DisableGroup本身不占用额外空间，只计算子元素的大小
            Vector2 totalSize = Vector2.zero;
            
            foreach (var child in element)
            {
                if (child != null)
                {
                    Vector2 childSize = mgr.CalculateElementSize(child, style);
                    totalSize.x = Mathf.Max(totalSize.x, childSize.x);
                    totalSize.y += childSize.y;
                }
            }

            return totalSize;
        }
    }
}