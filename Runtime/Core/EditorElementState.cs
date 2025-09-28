using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 编辑器渲染器特定的元素状态
    /// 包含编辑器渲染相关的状态信息，如最后渲染的矩形区域
    /// </summary>
    public class EditorElementState
    {
        /// <summary>
        /// 最后渲染的矩形区域
        /// </summary>
        public Rect RenderRect { get; set; }
        
        /// <summary>
        /// 元素是否可见
        /// </summary>
        public bool IsVisible { get; set; } = true;
        
        /// <summary>
        /// 元素是否被选中
        /// </summary>
        public bool IsSelected { get; set; } = false;
        
        /// <summary>
        /// 元素的层级索引
        /// </summary>
        public int ZIndex { get; set; } = 0;
        
        /// <summary>
        /// 元素的渲染透明度
        /// </summary>
        public float Alpha { get; set; } = 1.0f;
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EditorElementState()
        {
            RenderRect = new Rect(0, 0, 0, 0);
        }
        
        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="rect">最后渲染的矩形区域</param>
        public EditorElementState(Rect rect)
        {
            RenderRect = rect;
        }
    }
}