using System.Collections;
using System.Collections.Generic;

namespace DeclGUI.Core
{
    public interface IContainerElement : IElement, IEnumerable<IElement>
    {
        public string Key { get; set; }
    }
    public interface IContainerElement<TState> : IContainerElement, IElement<TState>
    {
        // 不再需要单独的Children属性，通过IEnumerable<IElement>接口提供遍历功能
    }
}