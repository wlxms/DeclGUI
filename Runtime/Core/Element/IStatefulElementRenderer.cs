namespace DeclGUI.Core
{
    /// <summary>
    /// 有状态元素渲染器接口
    /// 支持带状态参数的渲染
    /// </summary>
    public interface IStatefulElementRenderer : IElementRenderer
    {
        /// <summary>
        /// 渲染有状态元素
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">有状态元素</param>
        /// <param name="state">元素状态</param>
        void Render(RenderManager mgr, in IStatefulElement element, object state, in IDeclStyle style);
    }

    /// <summary>
    /// 泛型有状态元素渲染器接口
    /// </summary>
    public interface IStatefulElementRenderer<TElement, TState> : IStatefulElementRenderer
        where TElement : IElement<TState>
    {
        /// <summary>
        /// 渲染有状态元素
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">有状态元素</param>
        /// <param name="state">元素状态</param>
        void Render(RenderManager mgr,in TElement element, TState state, in IDeclStyle style);
    }
}