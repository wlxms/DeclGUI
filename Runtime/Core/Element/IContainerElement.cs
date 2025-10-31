using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace DeclGUI.Core
{
    public interface IContainerElement : IElement, IEnumerable<IElement>
    {
        public string Key { get; set; }

        /// <summary>
        /// 获取容器的上下文参数列表
        /// </summary>
        IReadOnlyList<IContextProvider> ContextParams { get; }

        /// <summary>
        /// 添加上下文参数
        /// </summary>
        /// <param name="contextParams">上下文参数</param>
        /// <returns>带有上下文参数的新容器实例</returns>
        IContainerElement WithContext(params IContextProvider[] contextParams);

        IElement this[int index] { get; }
        
        /// <summary>
        /// 获取容器的子元素数量
        /// </summary>
        int Count { get; }
    }
    public interface IContainerElement<TState> : IContainerElement, IElement<TState>
    {
        // 不再需要单独的Children属性，通过IEnumerable<IElement>接口提供遍历功能
    }

}
