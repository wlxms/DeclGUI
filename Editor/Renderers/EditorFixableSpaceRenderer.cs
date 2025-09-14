using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// FixableSpace组件的Editor渲染器
    /// </summary>
    public class EditorFixableSpaceRenderer : EditorElementRenderer<FixableSpace>
    {
        public override void Render(RenderManager mgr, in FixableSpace element)
        {
            // 创建一个可伸缩的空白区域
            GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// 计算FixableSpace元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in FixableSpace element, in DeclStyle? style)
        {
            // FixableSpace组件是可伸缩的，不占用固定空间
            return Vector2.zero;
        }
    }
}