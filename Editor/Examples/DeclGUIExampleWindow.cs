using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Renderers;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Examples
{
    /// <summary>
    /// DeclGUI示例窗口
    /// 展示如何使用DeclGUI框架创建声明式UI
    /// </summary>
    public class DeclGUIExampleWindow : EditorWindow
    {
        private EditorRenderManager _renderManager;
        private string _inputText = "Hello DeclGUI";
        private int _clickCount = 0;
        private float _sliderValue = 0.5f;
        private Texture2D _selectedTexture;

        [MenuItem("Tools/DeclGUI/Example Window")]
        public static void ShowWindow()
        {
            GetWindow<DeclGUIExampleWindow>("DeclGUI Example");
        }

        void OnGUI()
        {
            _renderManager ??= new EditorRenderManager();

            // 使用DeclGUI创建声明式UI
            var ui = new Ver(
                new Label("Welcome to DeclGUI", DeclStyle.WithColor(Color.blue)),
                new Spc(10),
                new Hor(
                    new Button("Click Me", OnButtonClicked,
                        DeclStyle.WithSize(100, 30).SetColor(Color.white)),
                    new Spc(10),
                    new Button("Reset", OnResetClicked,
                        DeclStyle.WithSize(80, 30).SetColor(Color.red))
                ),
                new Spc(15),
                new Label($"Button clicked {_clickCount} times"),
                new Spc(15),
                new TextField(_inputText, OnInputChanged,
                    DeclStyle.WithWidth(200)),
                new Spc(10),
                new Label($"Current input: {_inputText}"),
                new Spc(20),
                new Hor(
                    new Label("Layout Demo:"),
                    new Spc(5),
                    new Button("A", () => Debug.Log("A clicked")),
                    new Spc(5),
                    new Button("B", () => Debug.Log("B clicked")),
                    new Spc(5),
                    new Button("C", () => Debug.Log("C clicked"))
                ),
                new Spc(20),
                new Label("New Components Demo:", DeclStyle.WithColor(Color.green)),
                new Spc(10),
                new Hor(
                    new Label("Test Slider:"),
                    new Spc(5),
                    new Slider(_sliderValue, 0f, 1f, newValue =>
                    {
                        _sliderValue = newValue;
                        Debug.Log($"Slider value: {_sliderValue:F2}");
                        Repaint();
                    }, DeclStyle.WithWidth(200)),
                    new Spc(5),
                    new Label($"{_sliderValue:F2}")
                ),
                new Spc(10),
                new Hor(
                    new Label("Select Texture:"),
                    new Spc(5),
                    new ObjectField<Texture2D>(_selectedTexture, newTexture =>
                    {
                        _selectedTexture = newTexture;
                        Debug.Log($"Selected texture: {newTexture?.name ?? "None"}");
                        Repaint();
                    }, true, DeclStyle.WithWidth(200))
                ),
                new Spc(20),
                new Label("Collection Initializer Demo:", DeclStyle.WithColor(Color.magenta)),
                new Spc(10),
                // 测试集合初始化语法
                new Hor {
                    new Label("Collection Init Test:"),
                    new Spc(5),
                    new Button("Test1", () => Debug.Log("Test1 clicked from collection init")),
                    new Spc(5),
                    new Button("Test2", () => Debug.Log("Test2 clicked from collection init"))
                },
                new Spc(20),
                new Label("State Management Demo:", DeclStyle.WithColor(Color.cyan)),
                new Spc(10),
                new Hor {
                    new Label("Stateful Components:"),
                    new Spc(5),
                    // 有状态按钮示例
                    new StatefulButton("Stateful Button").WithClick(() => {
                        Debug.Log("Stateful button clicked!");
                        Repaint();
                    }),
                    new Spc(5),
                    // 长按按钮示例
                    new LongPressButton("Long Press").WithLongPress(() => {
                        Debug.Log("Long press detected!");
                        Repaint();
                    }, 1.5f)
                },
                new Spc(10),
                new Label("Stateful Container Demo:"),
                new Spc(5),
                // 有状态容器示例 - 使用集合初始化语法
                new Hor {
                    new StatefulButton("Button A").WithClick(() => Debug.Log("Button A clicked")),
                    new Spc(5),
                    new StatefulButton("Button B").WithClick(() => Debug.Log("Button B clicked")),
                    new Spc(5),
                    new StatefulButton("Button C").WithClick(() => Debug.Log("Button C clicked"))
                },
                new Spc(20),
                new Label("FixableSpace Demo:", DeclStyle.WithColor(Color.yellow)),
                new Spc(10),
                // FixableSpace示例
                new Hor {
                    new Button("Left", () => Debug.Log("Left button clicked")),
                    new FixableSpace(),
                    new Button("Right", () => Debug.Log("Right button clicked"))
                },
                new Spc(20),
                new Label("Hover Event Demo:", DeclStyle.WithColor(Color.gray)),
                new Spc(10),
                new Hor {
                    new Button("Hover Me").WithHoverEnter(() => {
                        Debug.Log("Button hover entered!");
                        Repaint();
                    }).WithHoverExit(() => {
                        Debug.Log("Button hover exited!");
                        Repaint();
                    }),
                    new Spc(5),
                    new StatefulButton("Hover Stateful").WithHoverEnter(() => {
                        Debug.Log("Stateful button hover entered!");
                        Repaint();
                    }).WithHoverExit(() => {
                        Debug.Log("Stateful button hover exited!");
                        Repaint();
                    })
                }
            );

            // 渲染整个UI树
            _renderManager.RenderDOM(ui);
        }

        private void OnButtonClicked()
        {
            _clickCount++;
            Debug.Log($"Button clicked! Count: {_clickCount}");
            Repaint();
        }

        private void OnResetClicked()
        {
            _clickCount = 0;
            _inputText = "Hello DeclGUI";
            Debug.Log("Reset clicked");
            Repaint();
        }

        private void OnInputChanged(string newValue)
        {
            _inputText = newValue;
            Debug.Log($"Input changed to: {newValue}");
            Repaint();
        }
    }
}