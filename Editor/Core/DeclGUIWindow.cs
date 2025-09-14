using DeclGUI.Core;
using DeclGUI.Components;
using UnityEditor;
using UnityEngine;
using DeclGUI.Editor.Renderers;

namespace DeclGUI.Editor.Core
{
    /// <summary>
    /// 声明式GUI窗口基类
    /// 继承自EditorWindow并实现IElement接口
    /// 可以作为独立窗口使用，也可以作为控件嵌入其他UI
    /// </summary>
    public abstract class DeclGUIWindow : EditorWindow, IElement
    {
        private EditorRenderManager _renderManager;

        /// <summary>
        /// 渲染管理器
        /// </summary>
        protected EditorRenderManager RenderManager
        {
            get
            {
                _renderManager ??= new EditorRenderManager();
                return _renderManager;
            }
        }

        /// <summary>
        /// 窗口的GUI渲染方法
        /// </summary>
        protected virtual void OnGUI()
        {
            var ui = Render();
            if (ui != null)
            {
                RenderManager.RenderDOM(ui);
            }
        }

        /// <summary>
        /// 抽象渲染方法，子类必须实现
        /// 返回要渲染的UI元素
        /// </summary>
        /// <returns>UI元素</returns>
        public abstract IElement Render();

        /// <summary>
        /// 创建并显示窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="title">窗口标题</param>
        /// <returns>窗口实例</returns>
        public static T ShowWindow<T>(string title = null) where T : DeclGUIWindow
        {
            var window = GetWindow<T>();
            if (!string.IsNullOrEmpty(title))
            {
                window.titleContent = new GUIContent(title);
            }
            window.Show();
            return window;
        }

        /// <summary>
        /// 作为控件渲染时的入口点
        /// 当这个窗口被作为控件使用时调用
        /// </summary>
        /// <returns>当前窗口实例（作为IElement）</returns>
        IElement IElement.Render()
        {
            return this;
        }
    }
}