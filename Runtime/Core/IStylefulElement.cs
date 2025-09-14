namespace DeclGUI.Core
{
    /// <summary>
    /// 具有样式的元素接口
    /// </summary>
    public interface IStylefulElement : IElementWithKey
    {
        /// <summary>
        /// 元素样式
        /// </summary>
        IDeclStyle Style { get; }
    }
}