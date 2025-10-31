using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Core;
using UnityEditor;
using UnityEngine;
using PopupMenuItem = DeclGUI.Components.PopupMenuItem;

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
        private int _selectedMenuIndex = -1;
        private string _selectedMenuItem = "未选择";
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
                new TextField(_message, newValue =>
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
                }, DeclStyle.WithWidth(150)),
                new Spc(15),

                // 菜单测试
                new Label("菜单系统测试:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Button("打开测试菜单", () =>
                {
                    var menuItems = CreateTestMenuItems();
                    var popupMenu = new PopupMenu(
                        menuItems,
                        pageSize: 8,
                        onItemSelected: OnMenuSelected,
                        searchPlaceholder: "搜索菜单项..."
                    );
                    
                    // 使用RenderManager显示弹出菜单
                    var buttonRect = GUILayoutUtility.GetLastRect();
                    var screenPos = GUIUtility.GUIToScreenPoint(buttonRect.position);
                    var popupRect = new Rect(screenPos.x, screenPos.y + buttonRect.height, 300, 400);
                    
                    RenderManager.ShowPopup(popupMenu, popupRect);
                }, DeclStyle.WithWidth(120)),
                new Spc(5),
                new Label($"选中项: {_selectedMenuItem} (索引: {_selectedMenuIndex})")
            );
        }

        private void OnMenuItemSelected(string selectedItem)
        {
            Debug.Log($"选中了菜单项: {selectedItem}");
            // 这里可以处理选中后的逻辑
        }

        /// <summary>
        /// 创建测试菜单项
        /// </summary>
        private System.Collections.Generic.List<PopupMenuItem> CreateTestMenuItems()
        {
            var menuItems = new System.Collections.Generic.List<PopupMenuItem>();
            
            // 添加大量测试菜单项，展示搜索和分页功能
            for (int i = 1; i <= 50; i++)
            {
                var displayName = $"菜单项 {i:D2} - 测试数据";
                menuItems.Add(new PopupMenuItem(displayName));
            }
            
            // 添加一些特殊菜单项
            menuItems.Add(new PopupMenuItem("特殊选项 - 重要功能"));
            menuItems.Add(new PopupMenuItem("特殊选项 - 高级设置"));
            menuItems.Add(new PopupMenuItem("特殊选项 - 系统配置"));
            menuItems.Add(new PopupMenuItem("特殊选项 - 用户管理"));
            menuItems.Add(new PopupMenuItem("特殊选项 - 权限控制"));
            
            return menuItems;
        }

        /// <summary>
        /// 菜单项选中回调
        /// </summary>
        private void OnMenuSelected(int selectedIndex)
        {
            _selectedMenuIndex = selectedIndex;
            
            var menuItems = CreateTestMenuItems();
            if (selectedIndex >= 0 && selectedIndex < menuItems.Count)
            {
                _selectedMenuItem = menuItems[selectedIndex].DisplayName;
                Debug.Log($"选中了菜单项: {_selectedMenuItem} (索引: {selectedIndex})");
            }
            else
            {
                _selectedMenuItem = "无效选择";
                Debug.LogWarning($"无效的菜单索引: {selectedIndex}");
            }
            
            Repaint();
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