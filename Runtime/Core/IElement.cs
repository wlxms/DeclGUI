using System;

namespace DeclGUI.Core
{
    /// <summary>
    /// 声明式UI元素的核心接口
    /// 所有DeclGUI组件都必须实现此接口
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// 渲染方法，返回实际要渲染的元素
        /// 可以返回自身或其他元素
        /// </summary>
        /// <returns>要渲染的元素</returns>
        IElement Render();
    }

    public interface IElementWithKey : IElement
    {
        /// <summary>
        /// 元素的唯一键
        /// 用于在布局中识别和管理元素
        /// </summary>
        string Key { get; set; }
    }

    // 有状态元素接口
    public interface IStatefulElement : IElementWithKey
    {
        object CreateState(); // 返回 IElementState
        IElement Render(in object state); // 使用 IElementState

        IElement IElement.Render()
        {
            throw new InvalidOperationException("Stateful elements require state parameter");
        }

    }
    // 泛型有状态元素接口
    public interface IElement<TState> : IStatefulElement
    {
        new TState CreateState();
        IElement Render(TState state);

        object IStatefulElement.CreateState() => CreateState();
        IElement IStatefulElement.Render(in object state)
        {
            return Render((TState)state);
        }
    }
}