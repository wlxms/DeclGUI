using System;
using System.Collections;
using System.Collections.Generic;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 缩进上下文，由Indent/DeIndent组件共同维护
    /// </summary>
    public struct IndentContext : IContextProvider
    {
        public int Level { get; }
        public float Size { get; }
        public IElement Child { get; private set; }
        
        public IndentContext(int level, float size, IElement child = null)
        {
            Level = level;
            Size = size;
            Child = child;
        }
        
        public IElement Render() => null;
        
        public IEnumerator<IElement> GetEnumerator()
        {
            if (Child != null)
                yield return Child;
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IElement child)
        {
            Child = child;
        }
    }
}