using System;
using System.Collections;
using System.Collections.Generic;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 取消缩进组件，减少缩进级别
    /// </summary>
    public struct DeIndent : IContextProvider, ISpecialContext
    {
        public int Levels { get; }
        public IElement Child { get; private set; }

        public DeIndent(int levels = 1, IElement child = null)
        {
            Levels = levels;
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