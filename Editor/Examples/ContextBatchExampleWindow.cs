using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Renderers;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Examples
{
    /// <summary>
    /// ContextBatch示例窗口
    /// 展示如何使用DeclGUI的上下文机制实现声明式上下文传递
    /// </summary>
    public class ContextBatchExampleWindow : EditorWindow
    {
        private EditorRenderManager _renderManager;
        private string _userName = "John Doe";
        private bool _isReadOnly = false;
        private int _clickCount = 0;

        [MenuItem("Tools/DeclGUI/ContextBatch Example")]
        public static void ShowWindow()
        {
            GetWindow<ContextBatchExampleWindow>("ContextBatch Example");
        }

        void OnGUI()
        {
            _renderManager ??= new EditorRenderManager();

            // 使用ContextBatch提供多个上下文
            var ui = new ContextBatch(new DisableContext(_isReadOnly), new UserName(_userName)) {
                BuildUI()
            };

            // 渲染整个UI树
            _renderManager.RenderDOM(ui);
        }

        private IElement BuildUI()
        {
            return new Ver {
                new Label("ContextBatch 示例", DeclStyle.WithColor(Color.blue)),
                new Spc(15),
                
                // 控制面板 - 设置上下文值
                new Label("上下文控制:", DeclStyle.WithColor(Color.green)),
                new Spc(10),
                new Hor {
                    new Label("用户名:"),
                    new Spc(5),
                    new TextField(_userName, newName => {
                        _userName = newName;
                        Repaint();
                    }, DeclStyle.WithWidth(150)),
                    new Spc(10),
                    new Label("只读模式:"),
                    new Spc(5),
                    new Toggle(_isReadOnly, isReadOnly => {
                        _isReadOnly = isReadOnly;
                        Repaint();
                    })
                },
                new Spc(20),

                // 上下文消费者示例
                new Label("上下文消费者演示:", DeclStyle.WithColor(Color.magenta)),
                new Spc(10),
                
                // 消费ReadOnly和UserName上下文
                new ContextConsumer(context => {
                    bool isReadOnly = context.Get<DisableContext>().Value;
                    string userName = context.Get<UserName>().Value;
                    
                    return new Ver {
                        new Label($"当前用户: {userName}"),
                        new Label($"只读模式: {isReadOnly}"),
                        new Spc(10),
                        new Button(isReadOnly ? "只读按钮 (已禁用)" : "点击我!", () => {
                            _clickCount++;
                            Debug.Log($"按钮被点击! 计数: {_clickCount}");
                            Repaint();
                        }),
                        new Spc(5),
                        new Label($"点击次数: {_clickCount}")
                    };
                }),

                new Spc(20),
                
                // 嵌套上下文示例
                new Label("嵌套上下文演示:", DeclStyle.WithColor(Color.cyan)),
                new Spc(10),
                
                // 外层上下文 - 只读模式
                new DisableContext(true) {
                    new UserName("Admin") {
                        new ContextConsumer(innerContext => {
                            bool outerReadOnly = innerContext.Get<DisableContext>().Value;
                            string innerUserName = innerContext.Get<UserName>().Value;
                            
                            return new Ver {
                                new Label($"嵌套上下文 - 用户: {innerUserName}"),
                                new Label($"外层只读: {outerReadOnly}"),
                                new Spc(5),
                                new Button("嵌套按钮", () => Debug.Log("嵌套按钮点击"))
                            };
                        })
                    }
                },

                new Spc(20),
                
                // 条件上下文示例
                new Label("条件上下文演示:", DeclStyle.WithColor(Color.yellow)),
                new Spc(10),
                
                // 根据条件提供不同的上下文
                _userName.Contains("Admin")
                    ? new UserName("管理员模式") { BuildAdminUI() }
                    : new UserName("普通用户模式") { BuildUserUI() }
            };
        }

        private IElement BuildAdminUI()
        {
            return new Ver {
                new Label("管理员特权界面", DeclStyle.WithColor(Color.red)),
                new Spc(10),
                new Button("管理功能1", () => Debug.Log("管理员功能1")),
                new Spc(5),
                new Button("管理功能2", () => Debug.Log("管理员功能2")),
                new Spc(5),
                new Button("重置数据", () => {
                    _clickCount = 0;
                    Repaint();
                })
            };
        }

        private IElement BuildUserUI()
        {
            return new Ver {
                new Label("普通用户界面", DeclStyle.WithColor(Color.gray)),
                new Spc(10),
                new Label("您没有管理员权限"),
                new Spc(5),
                new Button("基本功能", () => Debug.Log("基本功能"))
            };
        }
    }
}