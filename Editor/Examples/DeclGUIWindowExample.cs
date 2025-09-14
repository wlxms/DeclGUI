using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Examples
{
    /// <summary>
    /// 演示如何使用DeclGUIWindow作为控件
    /// </summary>
    public class DeclGUIWindowExample : DeclGUIWindow
    {
        private int _counter = 0;
        private string _message = "Hello from DeclGUIWindow!";
        private float _sliderValue = 0.5f;
        private bool _toggleValue = false;
        private int _intFieldValue = 42;
        private float _floatFieldValue = 3.14f;
        private int _popupIndex = 0;
        private string[] _popupOptions = { "Option 1", "Option 2", "Option 3" };
        private System.Enum _enumValue = UnityEngine.KeyCode.A;
        private Color _colorValue = Color.red;
        private Vector2 _vector2Value = new Vector2(1, 2);
        private Vector3 _vector3Value = new Vector3(1, 2, 3);
        private Vector4 _vector4Value = new Vector4(1, 2, 3, 4);
        private float _minValue = 0.2f;
        private float _maxValue = 0.8f;
        private int _layerValue = 0;
        private string _tagValue = "Untagged";
        private AnimationCurve _curveValue = AnimationCurve.Linear(0, 0, 1, 1);

        [MenuItem("Tools/DeclGUI/Window Example")]
        public static void ShowWindow()
        {
            ShowWindow<DeclGUIWindowExample>("DeclGUI Window Example");
        }

        /// <summary>
        /// 实现抽象渲染方法
        /// </summary>
        /// <returns>UI元素</returns>
        public override IElement Render()
        {
            return new Ver(
                new Label("DeclGUI Window Example", DeclStyle.WithColor(Color.blue)),
                new Spc(10),
                
                // 基本控件示例
                new Label("基本控件:", DeclStyle.WithColor(Color.gray)),
                new Hor(
                    new Button("Increment", () =>
                    {
                        _counter++;
                        Repaint();
                    }, DeclStyle.WithWidth(100)),
                    new Spc(10),
                    new Button("Decrement", () =>
                    {
                        _counter--;
                        Repaint();
                    }, DeclStyle.WithWidth(100))
                ),
                new Spc(5),
                new Label($"Counter: {_counter}"),
                new Spc(10),
                new TextField( _message, newValue =>
                {
                    _message = newValue;
                    Repaint();
                }, DeclStyle.WithWidth(200)),
                new Spc(5),
                new Label($"Current message: {_message}"),
                new Spc(15),
                
                // 新控件示例
                new Label("新控件演示:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                
                // Slider
                new Hor(
                    new Label("Slider:"),
                    new Spc(5),
                    new Label(_sliderValue.ToString("F2"))
                ),
                new Slider(_sliderValue, 0, 1, newValue =>
                {
                    _sliderValue = newValue;
                    Repaint();
                }, DeclStyle.WithWidth(150)),
                new Spc(5),
                
                // Toggle
                new Hor(
                    new Toggle(_toggleValue, newValue =>
                    {
                        _toggleValue = newValue;
                        Repaint();
                    }),
                    new Spc(5),
                    new Label($"Toggle: {_toggleValue}")
                ),
                new Spc(5),
                
                // IntField 和 FloatField
                new Hor(
                    new IntField(_intFieldValue, newValue =>
                    {
                        _intFieldValue = newValue;
                        Repaint();
                    }, DeclStyle.WithWidth(80)),
                    new Spc(10),
                    new FloatField(_floatFieldValue, newValue =>
                    {
                        _floatFieldValue = newValue;
                        Repaint();
                    }, DeclStyle.WithWidth(80))
                ),
                new Spc(5),
                
                // Popup
                new Popup(_popupIndex, _popupOptions, newIndex =>
                {
                    _popupIndex = newIndex;
                    Repaint();
                }, DeclStyle.WithWidth(120)),
                new Spc(5),
                
                // EnumPopup
                new EnumPopup(_enumValue, newValue =>
                {
                    _enumValue = newValue;
                    Repaint();
                }, DeclStyle.WithWidth(120)),
                new Spc(5),
                
                // ColorField
                new ColorField(_colorValue, true, newValue =>
                {
                    _colorValue = newValue;
                    Repaint();
                }, DeclStyle.WithWidth(60)),
                new Spc(5),
                
                // Vector字段
                new Vector2Field(_vector2Value, newValue =>
                {
                    _vector2Value = newValue;
                    Repaint();
                }, DeclStyle.WithWidth(120)),
                new Spc(5),
                new Vector3Field(_vector3Value, newValue =>
                {
                    _vector3Value = newValue;
                    Repaint();
                }, DeclStyle.WithWidth(150)),
                new Spc(5),
                new Vector4Field(_vector4Value, newValue =>
                {
                    _vector4Value = newValue;
                    Repaint();
                }, DeclStyle.WithWidth(180)),
                new Spc(5),
                
                // MinMaxSlider
                new Hor(
                    new Label($"Range: [{_minValue:F2}, {_maxValue:F2}]")
                ),
                new MinMaxSlider(_minValue, _maxValue, 0, 1, (min, max) =>
                {
                    _minValue = min;
                    _maxValue = max;
                    Repaint();
                }, DeclStyle.WithWidth(150)),
                new Spc(5),
                
                // LayerField
                new LayerField(_layerValue, newValue =>
                {
                    _layerValue = newValue;
                    Repaint();
                }, DeclStyle.WithWidth(120)),
                new Spc(5),
                
                // TagField
                new TagField(_tagValue, newValue =>
                {
                    _tagValue = newValue;
                    Repaint();
                }, DeclStyle.WithWidth(120)),
                new Spc(5),
                
                // CurveField
                new CurveField(_curveValue, newValue =>
                {
                    _curveValue = newValue;
                    Repaint();
                }, DeclStyle.WithWidth(150)),
                new Spc(15),
                
                new Button("Open Another Window", () =>
                {
                    NestedWindowExample.ShowWindow();
                }, DeclStyle.WithWidth(150))
            );
        }
    }

    /// <summary>
    /// 嵌套窗口示例
    /// </summary>
    public class NestedWindowExample : DeclGUIWindow
    {
        [MenuItem("Tools/DeclGUI/Nested Window Example")]
        public static void ShowWindow()
        {
            ShowWindow<NestedWindowExample>("Nested Window Example");
        }

        public override IElement Render()
        {
            return new Ver(
                new Label("Nested Window Example"),
                new Spc(15),
                // 这里演示如何使用自定义控件
                new CustomControl(),
                new Spc(15),
                new Button("Close", () => Close(), DeclStyle.WithWidth(80))
            );
        }
    }

    /// <summary>
    /// 自定义控件示例，实现IElement接口的可重用控件
    /// </summary>
    public class CustomControl : IElement
    {
        private float _sliderValue = 0.5f;

        /// <summary>
        /// 控件渲染方法
        /// </summary>
        /// <returns>UI元素</returns>
        public IElement Render()
        {
            return new Ver(
                new Label("Custom Control (IElement Implementation)"),
                new Spc(10),
                new Hor(
                    new Label("Slider:"),
                    new Spc(5),
                    new Label(_sliderValue.ToString("F2"))
                ),
                new Spc(10),
                new Hor(
                    new Button("-", () =>
                    {
                        _sliderValue = Mathf.Max(0, _sliderValue - 0.1f);
                    }, DeclStyle.WithWidth(30)),
                    new Spc(5),
                    new Button("+", () =>
                    {
                        _sliderValue = Mathf.Min(1, _sliderValue + 0.1f);
                    }, DeclStyle.WithWidth(30))
                )
            );
        }
    }
}