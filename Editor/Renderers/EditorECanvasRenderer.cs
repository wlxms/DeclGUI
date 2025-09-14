using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// ECanvas组件的Editor渲染器
    /// 画布本身参与自动布局，但内容物使用绝对定位
    /// </summary>
    public class EditorECanvasRenderer : EditorElementRenderer<ECanvas>
    {
        public override void Render(RenderManager mgr, in ECanvas element)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            var style = editorMgr.ApplyStyle(element.Style, null);
            var width = editorMgr.GetStyleWidth(element.Style);
            var height = editorMgr.GetStyleHeight(element.Style);

            // 保存当前GUI状态
            var matrix = GUI.matrix;
            var color = GUI.color;

            GUILayout.BeginScrollView(Vector2.zero);

            // 画布本身参与自动布局
            if (width > 0 && height > 0)
            {
                GUILayout.BeginVertical(style, GUILayout.Width(width), GUILayout.Height(height));
            }
            else if (width > 0)
            {
                GUILayout.BeginVertical(style, GUILayout.Width(width));
            }
            else if (height > 0)
            {
                GUILayout.BeginVertical(style, GUILayout.Height(height));
            }
            else
            {
                GUILayout.BeginVertical(style);
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // 在绝对定位区域内渲染子元素
            foreach (var child in element)
            {
                try
                {
                    mgr.RenderElement(child);
                }
                finally
                {
                    // 恢复GUI状态
                    GUI.matrix = matrix;
                    GUI.color = color;
                }
            }

            // 使用FlexibleSpace作为占位符来确保布局正确
            GUILayout.FlexibleSpace();

            // 结束自动布局区域
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
        public override Vector2 CalculateSize(RenderManager mgr, in ECanvas element, in DeclStyle? style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var guiStyle = editorMgr.ApplyStyle(element.Style, null);
            var width = editorMgr.GetStyleWidth(element.Style);
            var height = editorMgr.GetStyleHeight(element.Style);

            // 如果指定了固定尺寸，使用固定尺寸
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            // 对于ECanvas，默认返回一个合理的大小
            // 在实际应用中，可能需要计算所有子元素的最大边界
            return new Vector2(200, 150); // 默认大小
        }
    }
}