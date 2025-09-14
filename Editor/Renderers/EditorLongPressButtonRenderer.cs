using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;
using System;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// 长按按钮组件的Editor渲染器
    /// </summary>
    public class EditorLongPressButtonRenderer : EditorElementRenderer<LongPressButton, LongPressButtonState>
    {
        public override void Render(RenderManager mgr, in LongPressButton element, in IDeclStyle style)
        {
            // 对于无状态调用，尝试从状态管理器获取状态
            if (!mgr.StateStack.IsEmpty())
            {
                // 将LongPressButton转换为IElementWithKey来获取状态
                IElementWithKey elementWithKey = element;
                var elementState = mgr.StateStack.CurrentStateManager.GetOrCreateState(elementWithKey);
                if (elementState != null && elementState.State is LongPressButtonState buttonState)
                {
                    Render(mgr, element, buttonState, style);
                    return;
                }
            }
            
            // 如果没有状态或状态栈为空，使用默认状态渲染
            Render(mgr, element, new LongPressButtonState(), style);
        }

        public override void Render(RenderManager mgr, in LongPressButton element, LongPressButtonState state, in IDeclStyle style)
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
                // 根据按压状态显示不同的按钮文本和外观
                string buttonText = element.Text;
                if (state.IsPressing)
                {
                    float progress = state.PressTime / element.LongPressDuration;
                    buttonText = $"{element.Text} ({progress:P0})";
                }
                else if (state.WasLongPressed)
                {
                    buttonText = $"{element.Text} (Long Pressed!)";
                }

                // 处理按钮事件
                var currentEvent = Event.current;
                var buttonRect = GUILayoutUtility.GetRect(new GUIContent(buttonText), GUI.skin.button);

                // 检测鼠标/触摸事件
                if (!isReadOnly)
                {
                    switch (currentEvent.type)
                    {
                        case EventType.MouseDown:
                            if (buttonRect.Contains(currentEvent.mousePosition))
                            {
                                state.IsPressing = true;
                                state.PressTime = 0f;
                                currentEvent.Use();
                            }
                            break;

                        case EventType.MouseUp:
                            if (state.IsPressing)
                            {
                                if (state.PressTime >= element.LongPressDuration)
                                {
                                    // 长按完成
                                    state.WasLongPressed = true;
                                    element.OnLongPress?.Invoke();
                                }
                                else
                                {
                                    // 短按或取消
                                    state.WasLongPressed = false;
                                }
                                state.IsPressing = false;
                                currentEvent.Use();
                            }
                            break;

                        case EventType.MouseDrag:
                            if (state.IsPressing && !buttonRect.Contains(currentEvent.mousePosition))
                            {
                                // 拖出按钮区域，取消按压
                                state.IsPressing = false;
                                state.WasLongPressed = false;
                                currentEvent.Use();
                            }
                            break;
                    }

                    // 更新按压时间
                    if (state.IsPressing)
                    {
                        state.PressTime += Time.deltaTime;
                        if (state.PressTime >= element.LongPressDuration && !state.WasLongPressed)
                        {
                            // 达到长按时间阈值
                            state.WasLongPressed = true;
                            element.OnLongPress?.Invoke();
                        }
                    }
                }

                // 渲染按钮
                if (GUI.Button(buttonRect, buttonText))
                {
                    // 处理普通点击（如果没有处理长按）
                    if (!isReadOnly && !state.WasLongPressed && !state.IsPressing)
                    {
                        // 普通点击逻辑
                        state.LastPressTime = DateTime.Now;
                    }
                }
            }
            finally
            {
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
        }

        public override Vector2 CalculateSize(RenderManager mgr, in LongPressButton element, in IDeclStyle style)
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