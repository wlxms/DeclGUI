using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Gap组件的Editor渲染器
    /// </summary>
    public class EditorSpcRenderer : EditorElementRenderer<Spc>
    {
        public override void Render(RenderManager mgr, in Spc element)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            // Gap组件只需要显示间距，不需要样式
            GUILayout.Space(element.Size);
        }

        /// <summary>
        /// 计算Spc元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in Spc element, in DeclStyle? style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            // Spc组件的大小就是指定的间距大小
            return new Vector2(0, element.Size);
        }
    }
}