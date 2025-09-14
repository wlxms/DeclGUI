using System;
using System.Linq;
using DeclGUI.Core;
using UnityEngine;
using UnityEditor;

namespace DeclGUI.Editor.Renderers
{
    public abstract class EditorElementRenderer : IElementRenderer, IElementRectProvider
    {
        public abstract Vector2 CalculateSize(RenderManager mgr, in IElement element, in DeclStyle? style);
        public abstract void Render(RenderManager mgr, in IElement element);
        
        /// <summary>
        /// 获取元素的屏幕区域
        /// 默认实现返回空矩形，子类需要重写此方法提供具体的位置信息
        /// </summary>
        /// <returns>元素的屏幕矩形</returns>
        public virtual Rect GetElementRect()
        {
            // 默认返回空矩形，子类需要重写此方法
            return GUILayoutUtility.GetLastRect();
        }

        /// <summary>
        /// 计算文本元素的大小
        /// </summary>
        protected Vector2 CalculateTextSize(string text, GUIStyle style, float maxWidth = 0)
        {
            var content = new GUIContent(text);

            if (maxWidth > 0)
            {
                return new Vector2(
                    Mathf.Min(style.CalcSize(content).x, maxWidth),
                    style.CalcHeight(content, maxWidth)
                );
            }
            else
            {
                var size = style.CalcSize(content);
                size.y = style.CalcHeight(content, size.x);
                return size;
            }
        }

        /// <summary>
        /// 计算带约束的尺寸
        /// </summary>
        protected Vector2 ApplySizeConstraints(Vector2 size, float? minWidth, float? minHeight, float? maxWidth, float? maxHeight)
        {
            if (minWidth.HasValue && size.x < minWidth.Value) size.x = minWidth.Value;
            if (minHeight.HasValue && size.y < minHeight.Value) size.y = minHeight.Value;
            if (maxWidth.HasValue && size.x > maxWidth.Value) size.x = maxWidth.Value;
            if (maxHeight.HasValue && size.y > maxHeight.Value) size.y = maxHeight.Value;

            return size;
        }

        /// <summary>
        /// 渲染回退方法，在出现异常时显示错误信息
        /// </summary>
        /// <param name="exception">发生的异常</param>
        /// <param name="element">发生异常的元素</param>
        public void RenderFallback(Exception exception, in IElement element)
        {
            EditorRenderManager.RenderFallbackStatic(exception, element);
        }
    }

    /// <summary>
    /// Editor元素渲染器基类
    /// 提供通用的CalculateSize实现
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    public abstract class EditorElementRenderer<T> : EditorElementRenderer, IElementRenderer<T> where T : IElement
    {
        /// <summary>
        /// 渲染元素
        /// </summary>
        public abstract void Render(RenderManager mgr, in T element);

        /// <summary>
        /// 渲染元素（非泛型版本）
        /// </summary>
        public override void Render(RenderManager mgr, in IElement element)
        {
            if (element is T typedElement)
            {
                Render(mgr, typedElement);
            }
        }

        /// <summary>
        /// 计算元素的期望大小
        /// 默认实现使用GUILayout来测量元素大小
        /// </summary>
        public abstract Vector2 CalculateSize(RenderManager mgr, in T element, in DeclStyle? style);

        public override Vector2 CalculateSize(RenderManager mgr, in IElement element, in DeclStyle? style)
        {
            return CalculateSize(mgr, (T)element, style);
        }
    }

    /// <summary>
    /// Editor有状态元素渲染器基类
    /// 提供类型安全的状态渲染
    /// </summary>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <typeparam name="TState">状态类型</typeparam>
    public abstract class EditorElementRenderer<TElement, TState> : EditorElementRenderer<TElement>, IStatefulElementRenderer<TElement, TState>
        where TElement : IElement<TState>
    {
        /// <summary>
        /// 渲染有状态元素（类型安全版本）
        /// </summary>
        public abstract void Render(RenderManager mgr, in TElement element, TState state);

        /// <summary>
        /// 渲染有状态元素（非泛型版本）
        /// </summary>
        public void Render(RenderManager mgr, in IStatefulElement element, object state)
        {
            if (element is TElement typedElement && state is TState typedState)
            {
                Render(mgr, typedElement, typedState);
            }
        }

        /// <summary>
        /// 渲染元素（重写基类方法，禁止无状态调用）
        /// </summary>
        public override void Render(RenderManager mgr, in TElement element)
        {
            throw new InvalidOperationException($"Stateful element {typeof(TElement).Name} requires state parameter. Use Render(RenderManager, TElement, TState) instead.");
        }
    }
}