using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;
using System;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// 有状态按钮组件的Editor渲染器（类型安全版本）
    /// </summary>
    public class EditorStatefulButtonRenderer : EditorElementRenderer<StatefulButton, ButtonState>
    {
        /// <summary>
        /// 渲染有状态按钮（类型安全版本）
        /// </summary>
        public override void Render(RenderManager mgr, in StatefulButton element, ButtonState state)
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
                // 根据状态显示不同的按钮文本
                string buttonText = element.Text;
                if (state.ClickCount > 0)
                {
                    buttonText = $"{element.Text} (Clicked: {state.ClickCount})";
                }

                // 渲染按钮
                if (GUILayout.Button(buttonText))
                {
                    if (!isReadOnly)
                    {
                        // 更新状态
                        state.ClickCount++;
                        state.LastClickTime = DateTime.Now;
                        
                        // 触发点击事件
                        element.Events.OnClick?.Invoke();
                        
                        // 通知状态管理器状态已更新（使用状态栈）
                        mgr.StateStack.CurrentStateManager.UpdateState(element, state);
                    }
                }
            }
            finally
            {
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
        }

        public override Vector2 CalculateSize(RenderManager mgr, in StatefulButton element, in DeclStyle? style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            // 计算按钮大小
            var guiStyle = editorMgr.ApplyStyle(style, null);
            var content = new GUIContent(element.Text);
            
            if (guiStyle != null)
            {
                return guiStyle.CalcSize(content);
            }
            
            return GUI.skin.button.CalcSize(content);
        }
    }
}