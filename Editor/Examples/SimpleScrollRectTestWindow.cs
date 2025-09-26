using UnityEngine;
using UnityEditor;
using DeclGUI.Core;
using DeclGUI.Components;
using DeclGUI.Editor.Renderers;
using System;

namespace DeclGUI.Examples
{
    /// <summary>
    /// 简单的ScrollRect测试窗口
    /// 用于测试ScrollRect组件的基本功能
    /// </summary>
    public class SimpleScrollRectTestWindow : EditorWindow
    {
        private EditorRenderManager _renderManager;
        private Vector2 _scrollPosition;

        [MenuItem("Tools/DeclGUI/示例/ScrollRect测试")]
        public static void ShowWindow()
        {
            GetWindow<SimpleScrollRectTestWindow>("ScrollRect测试");
        }

        private void OnGUI()
        {
            _renderManager ??= new EditorRenderManager();

            // 使用ScrollRect包装内容

            var scrollRect = new ScrollRect(_scrollPosition, (pos) => { _scrollPosition = pos; })
            {
                new Ver
                {
                    // 添加多个标签来测试滚动
                    new Label("滚动项 1", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 2", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 3", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 4", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 5", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 6", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 7", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 8", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 9", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 10", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 11", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 12", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 13", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 14", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 15", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 16", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 17", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 18", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 19", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 20", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 21", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 22", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 23", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 24", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 25", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 26", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 27", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 28", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 29", DeclStyle.WithColor(Color.white)),
                    new Spc(10),
                    new Label("滚动项 30", DeclStyle.WithColor(Color.white)),
                }
            };

            // Rect backgroundRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            // var originalColor = GUI.backgroundColor;
            // GUI.backgroundColor = Color.red;
            // GUI.Box(backgroundRect, "", new GUIStyle(GUI.skin.box));
            // GUI.backgroundColor = originalColor;
            // Debug.Log($"backgroundRect: {backgroundRect}");
            // if (backgroundRect.height > 0 && backgroundRect.width > 0)
            // {
            //     GUILayout.BeginArea(backgroundRect);
                _renderManager.RenderDOM(scrollRect);
            //     GUILayout.EndArea();
            // }
            // if (backgroundRect.height > 0 && backgroundRect.width > 0)
            // {
            //     GUILayout.BeginArea(backgroundRect);


            // }

            // _renderManager.RenderDOM(scrollRect);

            // if (backgroundRect.height > 0 && backgroundRect.width > 0)
            // {


            //     GUILayout.EndArea();
            // }


            // if(GUILayout.Button("render"))
            // {
            //     Repaint();
            // }

        }

        private void RenderScrollTest(Action render)
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            render?.Invoke();
            EditorGUILayout.EndScrollView();
        }
    }
}
