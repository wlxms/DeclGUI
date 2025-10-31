using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 元素位置提供者接口
    /// 渲染器实现此接口以提供元素的位置信息
    /// </summary>
    public interface IElementRectProvider
    {
        /// <summary>
        /// 获取元素的屏幕区域
        /// </summary>
        /// <returns>元素的屏幕矩形</returns>
        Rect GetElementRect();
    }
}