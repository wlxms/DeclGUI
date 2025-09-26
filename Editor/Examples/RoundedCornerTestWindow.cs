using UnityEngine;
using UnityEditor;
using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Renderers;
using System;

namespace DeclGUI.Examples
{
    /// <summary>
    /// 圆角背景渲染测试窗口
    /// 用于测试圆角贴图缓存和控件圆角背景渲染功能
    /// </summary>
    public class RoundedCornerTestWindow : EditorWindow
    {
        private EditorRenderManager _renderManager;
        private float _borderRadius = 8f;
        private Color _backgroundColor = new Color(0.2f, 0.4f, 0.8f, 1f);
        private int _cacheCount = 0;
        private Vector2 _scrollPosition;

        [MenuItem("Tools/DeclGUI/示例/圆角背景测试")]
        public static void ShowWindow()
        {
            GetWindow<RoundedCornerTestWindow>("圆角背景测试");
        }

        private void OnEnable()
        {
            _renderManager = new EditorRenderManager();
            UpdateCacheCount();
        }

        private void OnGUI()
        {
            _renderManager ??= new EditorRenderManager();

            // 创建完整的UI树
            var ui = CreateUITree();
            
            // 渲染整个UI树
            _renderManager.RenderDOM(ui);
        }

        /// <summary>
        /// 创建完整的UI树
        /// </summary>
        private IElement CreateUITree()
        {
            return new ScrollRect(_scrollPosition, (pos) => { _scrollPosition = pos; })
            {
                new Ver
                {
                    // 标题
                    new Label("圆角背景渲染测试", DeclStyle.WithColor(Color.white).SetFontSize(18)),
                    new Spc(20),

                    // 控制面板
                    CreateControlPanel(),

                    // 分隔线
                    new Label("", DeclStyle.WithSize(0, 1).SetBackgroundColor(Color.gray)),
                    new Spc(20),

                    // 测试内容
                    CreateTestContent()
                }
            };
        }

        /// <summary>
        /// 创建控制面板
        /// </summary>
        private Ver CreateControlPanel()
        {
            return new Ver
            {
                new Label("控制面板", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
                
                // 圆角半径控制
                new Hor
                {
                    new Label("圆角半径:", DeclStyle.WithColor(Color.white).SetWidth(80)),
                    new Slider(_borderRadius, 0f, 20f, (value) => { _borderRadius = value; Repaint(); }, null)
                },
                new Spc(5),
                
                // 背景颜色控制
                new Hor
                {
                    new Label("背景颜色:", DeclStyle.WithColor(Color.white).SetWidth(80)),
                    new ColorField(_backgroundColor, true, (color) => { _backgroundColor = color; Repaint(); }, default(DeclStyle))
                },
                new Spc(10),
                
                // 缓存信息
                new Hor
                {
                    new Label($"缓存贴图数量: {_cacheCount}", DeclStyle.WithColor(Color.gray)),
                    new Spc(10),
                    new Button("刷新缓存计数", () => { UpdateCacheCount(); Repaint(); }, DeclStyle.WithSize(120, 20)),
                    new Spc(5),
                    new Button("清理缓存", () => { EditorTextureCache.ClearCache(); UpdateCacheCount(); Repaint(); }, DeclStyle.WithSize(80, 20))
                },
                new Spc(10)
            };
        }

        /// <summary>
        /// 创建测试内容
        /// </summary>
        private Ver CreateTestContent()
        {
            return new Ver
            {
                // 测试1: 垂直布局容器
                new Label("垂直布局容器测试", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
                CreateVerticalTest(),
                new Spc(20),

                // 测试2: 水平布局容器
                new Label("水平布局容器测试", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
                CreateHorizontalTest(),
                new Spc(20),

                // 测试3: 不同圆角半径对比
                new Label("不同圆角半径对比", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
                CreateRadiusComparison(),
                new Spc(20),

                // 测试4: 不同背景颜色
                new Label("不同背景颜色测试", DeclStyle.WithColor(Color.white).SetFontSize(14)),
                new Spc(10),
                CreateColorComparison()
            };
        }

        /// <summary>
        /// 创建垂直布局测试
        /// </summary>
        private Ver CreateVerticalTest()
        {
            var roundedStyle = new DeclStyle(
                backgroundColor: _backgroundColor,
                borderRadius: _borderRadius,
                padding: new RectOffset(10, 10, 10, 10)
            );

            return new Ver(roundedStyle)
            {
                new Label("这是一个带圆角背景的垂直布局容器", DeclStyle.WithColor(Color.white)),
                new Spc(5),
                new Label($"圆角半径: {_borderRadius}", DeclStyle.WithColor(Color.white)),
                new Spc(5),
                new Button("测试按钮", OnButtonClicked, DeclStyle.WithColor(Color.white))
            };
        }

        /// <summary>
        /// 创建水平布局测试
        /// </summary>
        private Hor CreateHorizontalTest()
        {
            var roundedStyle = new DeclStyle(
                backgroundColor: _backgroundColor,
                borderRadius: _borderRadius,
                padding: new RectOffset(10, 10, 10, 10)
            );

            return new Hor(roundedStyle)
            {
                new Label("水平布局", DeclStyle.WithColor(Color.white)),
                new Spc(10),
                new Button("按钮1", OnButtonClicked, DeclStyle.WithColor(Color.white)),
                new Spc(5),
                new Button("按钮2", OnButtonClicked, DeclStyle.WithColor(Color.white))
            };
        }

        /// <summary>
        /// 创建圆角半径对比测试
        /// </summary>
        private Ver CreateRadiusComparison()
        {
            var radii = new float[] { 0f, 4f, 8f, 12f, 16f };
            var colors = new Color[] 
            {
                new Color(0.8f, 0.3f, 0.3f, 1f),
                new Color(0.3f, 0.8f, 0.3f, 1f),
                new Color(0.3f, 0.3f, 0.8f, 1f),
                new Color(0.8f, 0.8f, 0.3f, 1f),
                new Color(0.8f, 0.3f, 0.8f, 1f)
            };

            var comparison = new Ver();
            
            for (int i = 0; i < radii.Length; i++)
            {
                var style = new DeclStyle(
                    backgroundColor: colors[i],
                    borderRadius: radii[i],
                    padding: new RectOffset(8, 8, 8, 8)
                );

                comparison.Add(new Hor(style)
                {
                    new Label($"半径: {radii[i]}", DeclStyle.WithColor(Color.white))
                });
                
                if (i < radii.Length - 1)
                {
                    comparison.Add(new Spc(5));
                }
            }
            
            return comparison;
        }

        /// <summary>
        /// 创建颜色对比测试
        /// </summary>
        private Ver CreateColorComparison()
        {
            var colors = new Color[] 
            {
                new Color(0.2f, 0.6f, 1f, 1f),    // 蓝色
                new Color(0.9f, 0.3f, 0.3f, 1f),  // 红色
                new Color(0.3f, 0.8f, 0.3f, 1f),  // 绿色
                new Color(1f, 0.8f, 0.2f, 1f),    // 黄色
                new Color(0.7f, 0.3f, 0.9f, 1f)   // 紫色
            };

            var comparison = new Ver();
            
            foreach (var color in colors)
            {
                var style = new DeclStyle(
                    backgroundColor: color,
                    borderRadius: _borderRadius,
                    padding: new RectOffset(8, 8, 8, 8)
                );

                comparison.Add(new Ver(style)
                {
                    new Label($"颜色: R{color.r:F1} G{color.g:F1} B{color.b:F1}", DeclStyle.WithColor(Color.white)),
                    new Spc(2),
                    new Label($"圆角半径: {_borderRadius}", DeclStyle.WithColor(Color.white).SetFontSize(10))
                });
                
                comparison.Add(new Spc(5));
            }
            
            return comparison;
        }

        /// <summary>
        /// 按钮点击事件处理
        /// </summary>
        private void OnButtonClicked()
        {
            Debug.Log("按钮被点击了！");
            UpdateCacheCount();
            Repaint();
        }

        /// <summary>
        /// 更新缓存计数
        /// </summary>
        private void UpdateCacheCount()
        {
            _cacheCount = EditorTextureCache.GetCacheCount();
        }

        private void OnInspectorUpdate()
        {
            // 定期更新缓存计数显示
            UpdateCacheCount();
            Repaint();
        }
    }
}