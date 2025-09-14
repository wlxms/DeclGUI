using System;
using System.Collections;
using System.Collections.Generic;

namespace DeclGUI.Core
{
    /// <summary>
    /// 上下文提供者接口
    /// 用于声明式地提供上下文信息
    /// </summary>
    public interface IContextProvider : IElement, IEnumerable<IElement>
    {
        /// <summary>
        /// 子元素
        /// </summary>
        IElement Child { get; }

        /// <summary>
        /// 添加子元素
        /// </summary>
        /// <param name="child">子元素</param>
        void Add(IElement child);
    }
}