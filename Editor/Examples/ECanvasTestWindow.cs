using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Examples
{
    /// <summary>
    /// ECanvas和AbsolutePanel测试窗口
    /// 纯声明式实现
    /// </summary>
    public class ECanvasTestWindow : DeclGUIWindow
    {
        private Vector2 scrollPosition = Vector2.zero;
        private float sliderValue = 0.5f;
        private bool toggleValue = false;
        private int intFieldValue = 100;
        private float floatFieldValue = 50.5f;
        private Color colorValue = Color.red;
        private Vector2 vector2Value = new Vector2(10, 20);
        private Vector2 minMaxValue = new Vector2(0, 100);

        [MenuItem("Tools/DeclGUI/ECanvas Test Window")]
        public static void ShowWindow()
        {
            ShowWindow<ECanvasTestWindow>("ECanvas Test");
        }

        public override IElement Render()
        {
            return new ScrollRect(
                scrollPosition,
                new Ver(
                    // 标题
                    new Label("ECanvas和AbsolutePanel测试", DeclStyle.WithGUIStyle(EditorStyles.boldLabel)),
                    
                    new Spc(10),

                    // ECanvas示例 - 在自动布局中创建固定位置区域
                    new Label("1. ECanvas示例 - 固定位置面板", DeclStyle.WithGUIStyle(EditorStyles.boldLabel)),
                    
                    // 创建一个ECanvas，内部使用AbsolutePanel进行绝对定位
                    new ECanvas(
                        new AbsolutePanel(new Vector2(0, 0), 
                            new Ver(
                                new Label("固定在位置(200,50)的ECanvas内容"),
                                new Button("ECanvas中的按钮", () => Debug.Log("ECanvas按钮点击"))
                            ),
                            DeclStyle.WithColor(Color.cyan)
                        ),
                        new AbsolutePanel(new Vector2(400, 100),
                            new Hor(
                                new Label("另一个位置:"),
                                new Button("测试", () => Debug.Log("测试按钮点击"))
                            ),
                            DeclStyle.WithColor(Color.yellow)
                        )
                    ),

                    new Spc(20),

                    // AbsolutePanel示例 - 各种尺寸控制
                    new Label("2. AbsolutePanel尺寸控制示例", DeclStyle.WithGUIStyle(EditorStyles.boldLabel)),
                    
                    // 自动尺寸
                    new AbsolutePanel(new Vector2(20, 150),
                        new Button("自动尺寸按钮", () => Debug.Log("自动尺寸")),
                        null
                    ),

                    // 固定尺寸
                    new AbsolutePanel(new Vector2(200, 150),
                        new Button("固定尺寸", () => Debug.Log("固定尺寸")),
                        DeclStyle.WithSize(120, 40)
                    ),

                    // 最小尺寸限制
                    new AbsolutePanel(new Vector2(350, 150),
                        new Label("最小尺寸限制文本"),
                        DeclStyle.WithSize(80, 30),
                        100, 40, null, null // 最小宽度100，最小高度40
                    ),

                    // 最大尺寸限制
                    new AbsolutePanel(new Vector2(500, 150),
                        new Label("这是一个很长的文本内容用于测试最大尺寸限制"),
                        DeclStyle.WithSize(200, 30),
                        null, null, 150, 25 // 最大宽度150，最大高度25
                    ),

                    new Spc(20),

                    // 交互控件示例
                    new Label("3. 交互控件在AbsolutePanel中的使用", DeclStyle.WithGUIStyle(EditorStyles.boldLabel)),
                    
                    // 各种控件在AbsolutePanel中的使用
                    new AbsolutePanel(new Vector2(20, 250),
                        new Ver(
                            new Slider(sliderValue, 0, 1, v => { sliderValue = v; Repaint(); }),
                            new Toggle(toggleValue, v => { toggleValue = v; Repaint(); }),
                            new IntField(intFieldValue, v => { intFieldValue = v; Repaint(); }),
                            new FloatField(floatFieldValue, v => { floatFieldValue = v; Repaint(); })
                        ),
                        DeclStyle.WithSize(200, 120)
                    ),

                    new AbsolutePanel(new Vector2(250, 250),
                        new Ver(
                            new ColorField(colorValue, true, v => { colorValue = v; Repaint(); }),
                            new Vector2Field(vector2Value, v => { vector2Value = v; Repaint(); }),
                            new MinMaxSlider(minMaxValue.x, minMaxValue.y, 0, 200, 
                                (min, max) => { minMaxValue = new Vector2(min, max); Repaint(); })
                        ),
                        DeclStyle.WithSize(200, 120)
                    ),

                    new Spc(20),

                    // 复杂布局示例
                    new Label("4. 复杂布局组合示例", DeclStyle.WithGUIStyle(EditorStyles.boldLabel)),
                    
                    // ECanvas内部嵌套多个AbsolutePanel - 调整位置到可见范围内
                    new ECanvas(
                        DeclStyle.WithSize(500, 100), // 设置更大的ECanvas尺寸以容纳所有面板
                        new AbsolutePanel(new Vector2(20, 20),
                            new Ver(
                                new Label("面板1"),
                                new Button("操作1", () => Debug.Log("操作1")),
                                new Button("操作2", () => Debug.Log("操作2"))
                            ),
                            DeclStyle.WithColor(new Color(0.8f, 0.9f, 1.0f))
                        ),
                        new AbsolutePanel(new Vector2(150, 20),
                            new Ver(
                                new Label("面板2"),
                                new Slider(0.5f, 0, 1, v => {}),
                                new Toggle(false, v => {})
                            ),
                            DeclStyle.WithColor(new Color(1.0f, 0.9f, 0.8f))
                        ),
                        new AbsolutePanel(new Vector2(280, 20),
                            new Ver(
                                new Label("面板3"),
                                new IntField(50, v => {}),
                                new FloatField(25.5f, v => {})
                            ),
                            DeclStyle.WithColor(new Color(0.9f, 1.0f, 0.8f))
                        )
                    )
                ),
                pos => { scrollPosition = pos; Repaint(); }
            );
        }
    }
}