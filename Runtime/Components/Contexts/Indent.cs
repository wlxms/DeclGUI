using System;
using System.Collections;
using System.Collections.Generic;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 缩进组件，增加缩进级别
    /// </summary>
    public struct Indent : IContextProvider, ISpecialContext
    {
        public float? CustomSize { get; }
        public IElement Child { get; private set; }

        public Indent(float? customSize = null, IElement child = null)
        {
            CustomSize = customSize;
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