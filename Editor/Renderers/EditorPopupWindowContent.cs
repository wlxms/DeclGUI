using DeclGUI.Core;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Editor环境下的弹出窗口内容
    /// 用于在PopupWindow中渲染DeclGUI元素
    /// </summary>
    public class EditorPopupWindowContent : PopupWindowContent
    {
        private readonly IElement _element;
        private readonly EditorRenderManager _renderManager;
        private Vector2 _scrollPosition;

        /// <summary>
        /// 创建弹出窗口内容
        /// </summary>
        /// <param name="element">要渲染的元素</param>
        /// <param name="renderManager">渲染管理器</param>
        public EditorPopupWindowContent(IElement element, EditorRenderManager renderManager)
        {
            _element = element;
            _renderManager = renderManager ?? new EditorRenderManager();
        }

        /// <summary>
        /// 获取窗口大小
        /// </summary>
        public override Vector2 GetWindowSize()
        {
            // 计算元素期望大小
            var elementSize = _renderManager.CalculateElementSize(_element);
            
            // 添加一些边距
            return new Vector2(
                elementSize.x, // 最小宽度200
                elementSize.y  // 最小高度100
            );
        }

        /// <summary>
        /// 在GUI中渲染内容
        /// </summary>
        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);
            // 使用滚动视图来适应不同大小的内容
            // _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            try
            {
                // 使用EditorRenderManager渲染元素
                _renderManager.RenderDOM(_element);
            }
            finally
            {
                // EditorGUILayout.EndScrollView();
                GUILayout.EndArea();
            }
        }

        /// <summary>
        /// 当窗口打开时调用
        /// </summary>
        public override void OnOpen()
        {
            base.OnOpen();
            // 可以在这里进行初始化操作
        }

        /// <summary>
        /// 当窗口关闭时调用
        /// </summary>
        public override void OnClose()
        {
            base.OnClose();
            // 可以在这里进行清理操作
        }
    }
}