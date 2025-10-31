using System;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 元素渲染器基础接口
    /// </summary>
    public interface IElementRenderer
    {
        /// <summary>
        /// 渲染指定元素
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">要渲染的元素</param>
        void Render(RenderManager mgr, in IElement element, in IDeclStyle style);

        /// <summary>
        /// 计算元素的期望大小
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">要计算大小的元素</param>
        /// <param name="style">应用的样式</param>
        /// <returns>元素的期望大小</returns>
        Vector2 CalculateSize(RenderManager mgr, in IElement element, in IDeclStyle style);

        /// <summary>
        /// 渲染元素时发生异常的回退方法
        /// </summary>
        /// <param name="ex">捕获的异常</param>
        /// <param name="element">发生异常的元素</param>
        void RenderFallback(Exception ex, in IElement element);
    }

    /// <summary>
    /// 泛型元素渲染器接口
    /// 提供类型安全的渲染方法
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    public interface IElementRenderer<T> : IElementRenderer where T : IElement
    {
        /// <summary>
        /// 渲染指定类型的元素
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">要渲染的元素</param>
        void Render(RenderManager mgr, in T element, in IDeclStyle style);

        /// <summary>
        /// 计算元素的期望大小
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">要计算大小的元素</param>
        /// <param name="style">应用的样式</param>
        /// <returns>元素的期望大小</returns>
        Vector2 CalculateSize(RenderManager mgr, in T element, in IDeclStyle style);
    }
}