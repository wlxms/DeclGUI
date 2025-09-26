using UnityEngine;
using UnityEditor;
using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Renderers;

namespace DeclGUI.Examples
{
    /// <summary>
    /// DeclGUI 扩展样式示例窗口
    /// 展示所有新增的控件样式
    /// </summary>
    public class ExampleWindowExtendedStyles : EditorWindow
    {
        private EditorRenderManager _renderManager;
        private int _clickCount = 0;
        private bool _showDebugInfo = true;

        private Vector2 _scrollPosition;

        [MenuItem("Tools/DeclGUI/示例/扩展样式示例")]
        public static void ShowWindow()
        {
            GetWindow<ExampleWindowExtendedStyles>("扩展样式示例");
        }

        private void OnGUI()
        {
            _renderManager ??= new EditorRenderManager();

            // 显示调试信息
            if (_showDebugInfo)
            {
                EditorGUILayout.HelpBox("DeclGUI 扩展样式示例窗口\n- 展示所有新增的控件样式\n- 包括按钮、面板、输入框等", MessageType.Info);
            }

            // 创建UI组件树
            var ui =

            // 使用ScrollRect替代EditorGUILayout.BeginScrollView
            new ScrollRect(_scrollPosition, (pos) => { _scrollPosition = pos; })
            {
                new Ver
                {
                    // 标题
                    new Label("DeclGUI 扩展样式示例", DeclStyle.WithColor(Color.white).SetFontSize(18)),
                    new Spc(20),

                    // 按钮样式示例
                    CreateButtonStylesSection(),

                    new Spc(15),

                    // 面板样式示例
                    CreatePanelStylesSection(),

                    new Spc(15),

                    // 输入框样式示例
                    CreateInputFieldStylesSection(),

                    new Spc(15),

                    // 标签样式示例
                    CreateLabelStylesSection(),

                    new Spc(15),

                    // 工具栏样式示例
                    CreateToolbarStylesSection(),

                    new Spc(15),

                    // ScrollRect 示例
                    CreateScrollRectExample(),

                    new Spc(15),

                    // DisableGroup 示例
                    CreateDisableGroupExample(),

                    new Spc(15),

                    // 交互控件
                    new Label($"按钮点击次数: {_clickCount}", DeclStyle.WithColor(Color.gray)),
                    new Spc(10),
                    new Button("重置计数", () => { _clickCount = 0; Repaint(); }, DeclStyle.WithSize(100, 25)),
                    new Spc(10),
                    new Toggle(_showDebugInfo, value => { _showDebugInfo = value; Repaint(); }, new DeclStyle("Toggle"))
                }
            };

            // 渲染整个UI树
            _renderManager.RenderDOM(ui);
        }

        private Ver CreateButtonStylesSection()
        {
            return new Ver
            {
                new Label("按钮样式", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
 
                // Primary Button
                new Label("Primary Button", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Hor
                {
                    new Button("Primary Button", OnButtonClicked, new DeclStyle("PrimaryButton")),
                    new Spc(5),
                    new DisableGroup(true)
                    {
                        new Button("Primary Disabled", OnButtonClicked, new DeclStyle("PrimaryButton"))
                    }
                },
                new Spc(10),
 
                // Secondary Button
                new Label("Secondary Button", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Hor
                {
                    new Button("Secondary Button", OnButtonClicked, new DeclStyle("SecondaryButton")),
                    new Spc(5),
                    new DisableGroup(true)
                    {
                        new Button("Secondary Disabled", OnButtonClicked, new DeclStyle("SecondaryButton"))
                    }
                },
                new Spc(10),
 
                // Tertiary Button
                new Label("Tertiary Button", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Hor
                {
                    new Button("Tertiary Button", OnButtonClicked, new DeclStyle("TertiaryButton")),
                    new Spc(5),
                    new DisableGroup(true)
                    {
                        new Button("Tertiary Disabled", OnButtonClicked, new DeclStyle("TertiaryButton"))
                    }
                },
                new Spc(10),
 
                // Success Button
                new Label("Success Button", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Hor
                {
                    new Button("Success Button", OnButtonClicked, new DeclStyle("SuccessButton")),
                    new Spc(5),
                    new DisableGroup(true)
                    {
                        new Button("Success Disabled", OnButtonClicked, new DeclStyle("SuccessButton"))
                    }
                },
                new Spc(10),
 
                // Warning Button
                new Label("Warning Button", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Hor
                {
                    new Button("Warning Button", OnButtonClicked, new DeclStyle("WarningButton")),
                    new Spc(5),
                    new DisableGroup(true)
                    {
                        new Button("Warning Disabled", OnButtonClicked, new DeclStyle("WarningButton"))
                    }
                },
                new Spc(10),
 
                // Danger Button
                new Label("Danger Button", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Hor
                {
                    new Button("Danger Button", OnButtonClicked, new DeclStyle("DangerButton")),
                    new Spc(5),
                    new DisableGroup(true)
                    {
                        new Button("Danger Disabled", OnButtonClicked, new DeclStyle("DangerButton"))
                    }
                },
                new Spc(10),
 
                // Info Button
                new Label("Info Button", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Hor
                {
                    new Button("Info Button", OnButtonClicked, new DeclStyle("InfoButton")),
                    new Spc(5),
                    new DisableGroup(true)
                    {
                        new Button("Info Disabled", OnButtonClicked, new DeclStyle("InfoButton"))
                    }
                },
                new Spc(10),
 
                // Icon Button
                new Label("Icon Button", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Hor
                {
                    new Button("🔍 Search", OnButtonClicked, new DeclStyle("IconButton")),
                    new Spc(5),
                    new Button("⚙️ Settings", OnButtonClicked, new DeclStyle("IconButton"))
                },
                new Spc(10),
 
                // Small Button
                new Label("Small Button", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Hor
                {
                    new Button("Small", OnButtonClicked, new DeclStyle("SmallButton")),
                    new Spc(5),
                    new Button("Small", OnButtonClicked, new DeclStyle("SmallButton")),
                    new Spc(5),
                    new Button("Small", OnButtonClicked, new DeclStyle("SmallButton"))
                },
                new Spc(10),
 
                // Large Button
                new Label("Large Button", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Button("Large Button", OnButtonClicked, new DeclStyle("LargeButton"))
            };
        }

        private Ver CreatePanelStylesSection()
        {
            return new Ver
            {
                new Label("面板样式", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
 
                // Bordered Panel - 使用带样式的Ver组件代替AbsolutePanel
                new Label("Bordered Panel", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Ver(new DeclStyle("BorderedPanel"))
                {
                    new Label("这是一个带边框的面板", new DeclStyle("Label")),
                    new Spc(5),
                    new Button("面板内的按钮", OnButtonClicked, new DeclStyle("Button"))
                },
                new Spc(10),
 
                // Filled Panel - 使用带样式的Ver组件代替AbsolutePanel
                new Label("Filled Panel", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Ver(new DeclStyle("FilledPanel"))
                {
                    new Label("这是一个带背景填充的面板", new DeclStyle("Label")),
                    new Spc(5),
                    new Button("面板内的按钮", OnButtonClicked, new DeclStyle("Button"))
                }
            };
        }

        private Ver CreateInputFieldStylesSection()
        {
            return new Ver
            {
                new Label("输入框样式", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
 
                // Subtle TextField
                new Label("Subtle TextField", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new TextField("柔和的文本输入框", (text) => { }, new DeclStyle("SubtleTextField")),
                new Spc(5),
                new TextField("这是文本区域示例", (text) => { }, new DeclStyle("SubtleTextField"))
            };
        }

        private Ver CreateLabelStylesSection()
        {
            return new Ver
            {
                new Label("标签样式", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
 
                // Bold Label
                new Label("Bold Label", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Label("粗体标签 - 用于标题或强调文本", new DeclStyle("BoldLabel")),
                new Spc(5),
 
                // HelpBox Text
                new Label("HelpBox Text", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Label("这是帮助框文本样式，用于辅助说明文字", new DeclStyle("HelpBoxText")),
                new Spc(5),
 
                // Mini Label
                new Label("Mini Label", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new Label("迷你标签 - 用于辅助说明文字", new DeclStyle("MiniLabel"))
            };
        }

        private Ver CreateToolbarStylesSection()
        {
            return new Ver
            {
                new Label("工具栏样式", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
 
                // ToolbarSearchField
                new Label("Toolbar Search Field", DeclStyle.WithColor(Color.gray).SetFontSize(12)),
                new TextField("搜索...", (text) => { }, new DeclStyle("ToolbarSearchField"))
            };
        }

        private Ver CreateScrollRectExample()
        {
            return new Ver
            {
                new Label("ScrollRect 示例", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
                new Label("这是一个可滚动的区域，包含多个子元素", DeclStyle.WithColor(Color.gray)),
                new ScrollRect(_scrollPosition, (pos) => { }, true, false, DeclStyle.WithSize(300, 100))
                {
                    // 使用集合初始化语法添加多个子元素
                    new Ver
                    {
                        // 添加20个滚动项
                        new Hor(new Label("滚动项 1", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 1", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 2", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 2", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 3", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 3", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 4", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 4", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 5", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 5", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 6", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 6", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 7", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 7", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 8", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 8", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 9", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 9", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 10", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 10", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 11", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 11", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 12", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 12", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 13", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 13", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 14", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 14", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 15", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 15", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 16", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 16", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 17", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 17", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 18", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 18", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 19", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 19", OnButtonClicked, new DeclStyle("SmallButton"))),
                        new Spc(5),
                        new Hor(new Label("滚动项 20", DeclStyle.WithColor(Color.white)), new Spc(10), new Button("按钮 20", OnButtonClicked, new DeclStyle("SmallButton")))
                    }
                }
            };
        }

        private Ver CreateDisableGroupExample()
        {
            bool isDisabled = _clickCount % 2 == 0; // 根据点击次数切换禁用状态

            return new Ver
            {
                new Label("DisableGroup 示例", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
                new Label($"当前状态: {(isDisabled ? "禁用" : "启用")}", DeclStyle.WithColor(isDisabled ? Color.red : Color.green)),
                new Spc(5),
                new DisableGroup(isDisabled)
                {
                    new Button("可能被禁用的按钮", OnButtonClicked, new DeclStyle("SecondaryButton")),
                    new Spc(5),
                    new TextField("可能被禁用的输入框", (text) => { }, new DeclStyle("SubtleTextField")),
                    new Spc(5),
                    new Toggle(true, (value) => { }, new DeclStyle("Toggle"))
                },
                new Spc(5),
                new Button("切换禁用状态", () => { _clickCount++; Repaint(); }, new DeclStyle("SmallButton"))
            };
        }

        private void OnButtonClicked()
        {
            _clickCount++;
            Debug.Log($"按钮点击! 次数: {_clickCount}");
            Repaint();
        }
    }
}
