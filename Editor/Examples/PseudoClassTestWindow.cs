using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Renderers;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Examples
{
    /// <summary>
    /// 伪类状态与过渡效果测试窗口
    /// </summary>
    public class PseudoClassTestWindow : EditorWindow
    {
        private EditorRenderManager _renderManager;
        private int _clickCount = 0;
        private bool _showDebugInfo = true;

        [MenuItem("Tools/DeclGUI/PseudoClass Test Window")]
        public static void ShowWindow()
        {
            GetWindow<PseudoClassTestWindow>("PseudoClass Test");
        }

        void OnGUI()
        {
            _renderManager ??= new EditorRenderManager();

            // 显示调试信息
            if (_showDebugInfo)
            {
                EditorGUILayout.HelpBox("伪类状态测试窗口\n- 检查悬停效果\n- 检查过渡动画\n- 检查样式集解析", MessageType.Info);
            }

            // 测试基础Button样式集
            var ui = new Ver(
                new Label("基础Button样式集测试:", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
                new Hor(
                    new Button("Button样式", OnButtonClicked, new DeclStyle("Button")),
                    new Spc(5),
                    new Button("悬停测试", OnButtonClicked, new DeclStyle("Button")).WithHoverEnter(() => {
                        Debug.Log("悬停进入");
                        Repaint();
                    }).WithHoverExit(() => {
                        Debug.Log("悬停退出");
                        Repaint();
                    })
                ),
                new Spc(15),
                new Label("自定义样式测试:", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
                new Hor(
                    new Button("蓝色按钮", OnButtonClicked,
                        DeclStyle.WithSize(120, 30)
                            .SetBackgroundColor(Color.blue)
                            .SetColor(Color.white)
                            .SetBorderRadius(5)
                    ),
                    new Spc(5),
                    new Button("绿色按钮", OnButtonClicked,
                        DeclStyle.WithSize(120, 30)
                            .SetBackgroundColor(Color.green)
                            .SetColor(Color.white)
                    )
                ),
                new Spc(15),
                new Label($"按钮点击次数: {_clickCount}", DeclStyle.WithColor(Color.gray)),
                new Spc(10),
                new Button("重置计数", () => { _clickCount = 0; Repaint(); }, DeclStyle.WithSize(100, 25)),
                new Spc(10),
                new Toggle(_showDebugInfo, value => { _showDebugInfo = value; Repaint(); })
            );

            // 渲染整个UI树
            _renderManager.RenderDOM(ui);
        }

        private void OnButtonClicked()
        {
            _clickCount++;
            Debug.Log($"按钮点击! 次数: {_clickCount}");
            Repaint();
        }
    }
}