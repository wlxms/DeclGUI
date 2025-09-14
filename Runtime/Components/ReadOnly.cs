using System;
using System.Collections;
using System.Collections.Generic;

namespace DeclGUI.Core
{
    /// <summary>
    /// 只读上下文
    /// 用于控制子元素的只读状态
    /// </summary>
    public struct ReadOnly : IContextProvider
    {
        public bool Value { get; }
        public IElement Child { get; private set; }

        public ReadOnly(bool value, IElement child = null)
        {
            Value = value;
            Child = child;
        }

        public IElement Render() => null;
        
        public IEnumerator<IElement> GetEnumerator() { yield return Child; }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IElement child)
        {
            Child = child;
        }
    }
}