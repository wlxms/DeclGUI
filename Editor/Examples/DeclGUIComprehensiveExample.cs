using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Core;
using UnityEditor;
using UnityEngine;
using PopupMenuItem = DeclGUI.Components.PopupMenuItem;

namespace DeclGUI.Editor.Examples
{
    /// <summary>
    /// DeclGUI 综合示例窗口
    /// 统一展示所有核心功能，按功能分类分页
    /// </summary>
    public class DeclGUIComprehensiveExample : DeclGUIWindow
    {
        // 分页枚举
        private enum TabType
        {
            BasicControls,
            LayoutContainers,
            AdvancedFeatures,
            StyleSystem
        }

        // 状态变量
        private TabType _currentTab = TabType.BasicControls;
        private Vector2 _scrollPosition = Vector2.zero;

        // 基本控件状态
        private int _counter = 0;
        private string _textValue = "Hello DeclGUI";
        private float _sliderValue = 0.5f;
        private bool _toggleValue = false;
        private int _intValue = 42;
        private float _floatValue = 3.14f;
        private int _popupIndex = 0;
        private string[] _popupOptions = { "选项1", "选项2", "选项3" };
        private System.Enum _enumValue = KeyCode.A;
        private Color _colorValue = Color.red;
        private Vector2 _vector2Value = new Vector2(1, 2);
        private Vector3 _vector3Value = new Vector3(1, 2, 3);
        private Vector4 _vector4Value = new Vector4(1, 2, 3, 4);
        private float _minValue = 0.2f;
        private float _maxValue = 0.8f;
        private int _layerValue = 0;
        private string _tagValue = "Untagged";
        private AnimationCurve _curveValue = AnimationCurve.Linear(0, 0, 1, 1);
        private Texture2D _objectValue;

        // 高级功能状态
        private int _selectedMenuIndex = -1;
        private string _selectedMenuItem = "未选择";
        private string _contextUserName = "John Doe";
        private bool _contextReadOnly = false;
        private int _contextClickCount = 0;

        [MenuItem("Tools/DeclGUI/综合示例窗口")]
        public static void ShowWindow()
        {
            ShowWindow<DeclGUIComprehensiveExample>("DeclGUI 综合示例");
        }

        public override IElement Render()
        {
            return new Ver
            {
                // 标题和分页导航
                new Label("DeclGUI 综合示例", DeclStyle.WithGUIStyle(EditorStyles.boldLabel)),
                new Spc(10),
                CreateTabNavigation(),
                new Spc(15),
                
                // 内容区域
                new ScrollRect(_scrollPosition, pos => { _scrollPosition = pos; Repaint(); })
                {
                    CreateCurrentTabContent(),
                }
            };
        }

        /// <summary>
        /// 创建分页导航
        /// </summary>
        private IElement CreateTabNavigation()
        {
            return new Hor
            {
                CreateTabButton("基本控件", TabType.BasicControls),
                new Spc(5),
                CreateTabButton("布局容器", TabType.LayoutContainers),
                new Spc(5),
                CreateTabButton("高级功能", TabType.AdvancedFeatures),
                new Spc(5),
                CreateTabButton("样式系统", TabType.StyleSystem)
            };
        }

        /// <summary>
        /// 创建分页按钮
        /// </summary>
        private IElement CreateTabButton(string label, TabType tabType)
        {
            bool isActive = _currentTab == tabType;
            var style = isActive
                ? DeclStyle.WithColor(Color.blue)
                : default;

            return new Button(label, () =>
            {
                _currentTab = tabType;
                Repaint();
            }, style);
        }

        /// <summary>
        /// 创建当前分页内容
        /// </summary>
        private IElement CreateCurrentTabContent()
        {
            return _currentTab switch
            {
                TabType.BasicControls => CreateBasicControlsTab(),
                TabType.LayoutContainers => CreateLayoutContainersTab(),
                TabType.AdvancedFeatures => CreateAdvancedFeaturesTab(),
                TabType.StyleSystem => CreateStyleSystemTab(),
                _ => new Label("未知分页")
            };
        }

        /// <summary>
        /// 基本控件分页
        /// </summary>
        private IElement CreateBasicControlsTab()
        {
            return new Ver
            {
                new Label("基本控件演示", DeclStyle.WithGUIStyle(EditorStyles.largeLabel)),
                new Spc(15),

                // 按钮和文本
                new Label("按钮和文本控件:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Hor
                {
                    new Button("增加", () => { _counter++; Repaint(); }, DeclStyle.WithWidth(80)),
                    new Spc(5),
                    new Button("减少", () => { _counter--; Repaint(); }, DeclStyle.WithWidth(80))
                },
                new Spc(5),
                new Label($"计数器: {_counter}"),
                new Spc(10),
                new TextField(_textValue, v => { _textValue = v; Repaint(); }, DeclStyle.WithWidth(200)),
                new Spc(5),
                new Label($"当前文本: {_textValue}"),
                new Spc(15),

                // 数值控件
                new Label("数值控件:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Hor
                {
                    new Label("滑块:"),
                    new Spc(5),
                    new Label(_sliderValue.ToString("F2"))
                },
                new Slider(_sliderValue, 0, 1, v => { _sliderValue = v; Repaint(); }, DeclStyle.WithWidth(150)),
                new Spc(5),
                new Hor
                {
                    new Toggle(_toggleValue, v => { _toggleValue = v; Repaint(); }),
                    new Spc(5),
                    new Label($"开关: {_toggleValue}")
                },
                new Spc(5),
                new Hor
                {
                    new IntField(_intValue, v => { _intValue = v; Repaint(); }, DeclStyle.WithWidth(80)),
                    new Spc(10),
                    new FloatField(_floatValue, v => { _floatValue = v; Repaint(); }, DeclStyle.WithWidth(80))
                },
                new Spc(15),

                // 选择控件
                new Label("选择控件:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Popup(_popupIndex, _popupOptions, v => { _popupIndex = v; Repaint(); }, DeclStyle.WithWidth(120)),
                new Spc(5),
                new EnumPopup(_enumValue, v => { _enumValue = v; Repaint(); }, DeclStyle.WithWidth(120)),
                new Spc(5),
                new ColorField(_colorValue, true, v => { _colorValue = v; Repaint(); }, DeclStyle.WithWidth(60)),
                new Spc(15),

                // 向量和范围控件
                new Label("向量和范围控件:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Vector2Field(_vector2Value, v => { _vector2Value = v; Repaint(); }, DeclStyle.WithWidth(120)),
                new Spc(5),
                new Vector3Field(_vector3Value, v => { _vector3Value = v; Repaint(); }, DeclStyle.WithWidth(150)),
                new Spc(5),
                new Hor
                {
                    new Label($"范围: [{_minValue:F2}, {_maxValue:F2}]")
                },
                new MinMaxSlider(_minValue, _maxValue, 0, 1, (min, max) =>
                {
                    _minValue = min;
                    _maxValue = max;
                    Repaint();
                }, DeclStyle.WithWidth(150)),
                new Spc(15),

                // Unity专用控件
                new Label("Unity专用控件:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new LayerField(_layerValue, v => { _layerValue = v; Repaint(); }, DeclStyle.WithWidth(120)),
                new Spc(5),
                new TagField(_tagValue, v => { _tagValue = v; Repaint(); }, DeclStyle.WithWidth(120)),
                new Spc(5),
                new CurveField(_curveValue, v => { _curveValue = v; Repaint(); }, DeclStyle.WithWidth(150)),
                new Spc(5),
                new ObjectField<Texture2D>(_objectValue, v => { _objectValue = v; Repaint(); }, true, DeclStyle.WithWidth(150))
            };
        }

        /// <summary>
        /// 布局容器分页
        /// </summary>
        private IElement CreateLayoutContainersTab()
        {
            return new Ver
            {
                new Label("布局容器演示", DeclStyle.WithGUIStyle(EditorStyles.largeLabel)),
                new Spc(15),

                // 基本布局
                new Label("基本布局容器:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Label("水平布局 (Hor):"),
                new Hor
                {
                    new Button("按钮A", () => Debug.Log("A点击")),
                    new Spc(5),
                    new Button("按钮B", () => Debug.Log("B点击")),
                    new Spc(5),
                    new Button("按钮C", () => Debug.Log("C点击"))
                },
                new Spc(10),
                new Label("垂直布局 (Ver):"),
                new Ver
                {
                    new Button("顶部按钮", () => Debug.Log("顶部点击")),
                    new Spc(5),
                    new Button("中间按钮", () => Debug.Log("中间点击")),
                    new Spc(5),
                    new Button("底部按钮", () => Debug.Log("底部点击"))
                },
                new Spc(15),

                // 弹性空间
                new Label("弹性空间 (FixableSpace):", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Hor
                {
                    new Button("左侧", () => Debug.Log("左侧点击")),
                    new FixableSpace(),
                    new Button("右侧", () => Debug.Log("右侧点击"))
                },
                new Spc(15),

                // 滚动容器
                new Label("滚动容器 (ScrollRect):", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new ScrollRect(Vector2.zero,pos => { Repaint(); })
                {

                    new Ver
                    {
                        new Label("滚动区域内容"),
                        new Spc(10),
                        new Button("项目1", () => Debug.Log("项目1")),
                        new Button("项目2", () => Debug.Log("项目2")),
                        new Button("项目3", () => Debug.Log("项目3")),
                        new Button("项目4", () => Debug.Log("项目4")),
                        new Button("项目5", () => Debug.Log("项目5"))
                    },

                }
            };
        }

        /// <summary>
        /// 高级功能分页
        /// </summary>
        private IElement CreateAdvancedFeaturesTab()
        {
            return new Ver
            {
                new Label("高级功能演示", DeclStyle.WithGUIStyle(EditorStyles.largeLabel)),
                new Spc(15),

                // 弹出菜单
                new Label("弹出菜单系统:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Button("打开测试菜单", () =>
                {
                    var menuItems = CreateTestMenuItems();
                    var popupMenu = new ComposedPopupMenu(
                        menuItems,
                        pageSize: 8,
                        onItemSelected: OnMenuSelected,
                        searchPlaceholder: "搜索菜单项..."
                    );

                    var popupLabel = new Label("测试弹出菜单");
                

                    // 使用RenderManager显示弹出菜单
                    var buttonRect = RenderManager.GetCurrentRenderRect();
                    Debug.Log($"按钮位置: {buttonRect}");
                    var popupRect = new Rect(buttonRect.x, buttonRect.y, 0, 0);

                    RenderManager.ShowPopup(popupMenu, buttonRect);
                }, DeclStyle.WithWidth(120)),
                new Spc(5),
                new Label($"选中项: {_selectedMenuItem} (索引: {_selectedMenuIndex})"),
                new Spc(15),

                // 上下文系统
                new Label("上下文系统:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Hor
                {
                    new Label("用户名:"),
                    new Spc(5),
                    new TextField(_contextUserName, OnContextUserNameChanged, DeclStyle.WithWidth(150)),
                    new Spc(10),
                    new Label("只读模式:"),
                    new Spc(5),
                    new Toggle(_contextReadOnly, OnContextReadOnlyChanged)
                },
                new Spc(10),
                new Label($"当前用户: {_contextUserName}"),
                new Label($"只读模式: {_contextReadOnly}"),
                new Spc(10),
                new Button(_contextReadOnly ? "只读按钮 (已禁用)" : "点击我!", OnContextButtonClick),
                new Spc(5),
                new Label($"点击次数: {_contextClickCount}"),
                new Spc(15),

                new Button("打开测试菜单2", () =>
                {
                    var menuItems = CreateTestMenuItems();
                    var popupMenu = new PopupMenu(
                        menuItems,
                        pageSize: 8,
                        onItemSelected: OnMenuSelected,
                        searchPlaceholder: "搜索菜单项..."
                    );

                    var popupLabel = new Label("测试弹出菜单");
                    // 使用RenderManager显示弹出菜单
                    var buttonRect = RenderManager.GetCurrentRenderRect();
                    Debug.Log($"按钮位置: {buttonRect}");
                    var popupRect = new Rect(buttonRect.x, buttonRect.y, 0, 0);

                    RenderManager.ShowPopup(popupLabel, buttonRect);
                }, DeclStyle.WithWidth(120)),

                // 自定义控件
                new Label("自定义控件 (IElement实现):", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new CustomControlExample()
            };
        }


        /// <summary>
        /// 上下文用户名改变
        /// </summary>
        private void OnContextUserNameChanged(string value)
        {
            _contextUserName = value;
            Repaint();
        }

        /// <summary>
        /// 上下文只读模式改变
        /// </summary>
        private void OnContextReadOnlyChanged(bool value)
        {
            _contextReadOnly = value;
            Repaint();
        }

        /// <summary>
        /// 上下文按钮点击事件
        /// </summary>
        private void OnContextButtonClick()
        {
            _contextClickCount++;
            Debug.Log($"按钮被点击! 计数: {_contextClickCount}");
            Repaint();
        }

        /// <summary>
        /// 样式系统分页
        /// </summary>
        private IElement CreateStyleSystemTab()
        {
            return new Ver
            {
                new Label("样式系统演示", DeclStyle.WithGUIStyle(EditorStyles.largeLabel)),
                new Spc(15),

                // 基础样式
                new Label("基础样式应用:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Hor
                {
                    new Button("默认样式", OnStyleButtonClick),
                    new Spc(5),
                    new Button("蓝色按钮", OnStyleButtonClick, DeclStyle.WithColor(Color.blue)),
                    new Spc(5),
                    new Button("红色按钮", OnStyleButtonClick, DeclStyle.WithColor(Color.red)),
                    new Spc(5),
                    new Button("绿色按钮", OnStyleButtonClick, DeclStyle.WithColor(Color.green))
                },
                new Spc(10),
                new Hor
                {
                    new Label("小标签", DeclStyle.WithWidth(80)),
                    new Label("中等标签", DeclStyle.WithWidth(100).SetFontSize(14)),
                    new Label("大标签", DeclStyle.WithWidth(120).SetColor(Color.blue))
                },
                new Spc(15),

                // 尺寸控制
                new Label("尺寸控制:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Hor
                {
                    new Button("小", OnStyleButtonClick, DeclStyle.WithSize(60, 25)),
                    new Spc(5),
                    new Button("中", OnStyleButtonClick, DeclStyle.WithSize(80, 30)),
                    new Spc(5),
                    new Button("大", OnStyleButtonClick, DeclStyle.WithSize(100, 35))
                },
                new Spc(15),

                // 样式集演示
                new Label("样式集 (StyleSet) 演示:", DeclStyle.WithColor(Color.gray)),
                new Spc(5),
                new Label("注: 样式集功能需要配置样式主题和样式定义"),
                new Spc(5),
                new Label("核心样式字段:", DeclStyle.WithColor(Color.cyan)),
                new Spc(5),
                new Label("- Color: 前景色"),
                new Label("- BackgroundColor: 背景色"),
                new Label("- Width/Height: 尺寸"),
                new Label("- FontSize/FontStyle: 字体"),
                new Label("- Padding/Margin: 间距"),
                new Label("- BorderColor/BorderWidth: 边框"),
                new Spc(5),
                new Label("使用 DeclStyle.WithXXX() 方法链式设置样式")
            };
        }

        /// <summary>
        /// 样式按钮点击事件
        /// </summary>
        private void OnStyleButtonClick()
        {
            Debug.Log("样式按钮点击");
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
    /// 自定义控件示例
    /// </summary>
    public class CustomControlExample : IElement
    {
        private float _sliderValue = 0.5f;

        public IElement Render()
        {
            return new Ver
            {
                new Label("自定义控件 (IElement实现)"),
                new Spc(10),
                new Hor
                {
                    new Label("滑块:"),
                    new Spc(5),
                    new Label(_sliderValue.ToString("F2"))
                },
                new Spc(10),
                new Hor
                {
                    new Button("-", () =>
                    {
                        _sliderValue = Mathf.Max(0, _sliderValue - 0.1f);
                    }, DeclStyle.WithWidth(30)),
                    new Spc(5),
                    new Button("+", () =>
                    {
                        _sliderValue = Mathf.Min(1, _sliderValue + 0.1f);
                    }, DeclStyle.WithWidth(30))
                }
            };
        }
    }
}