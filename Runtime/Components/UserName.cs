using System;
using System.Collections;
using System.Collections.Generic;

namespace DeclGUI.Core
{
    /// <summary>
    /// 用户名上下文
    /// 用于提供用户名信息
    /// </summary>
    public struct UserName : IContextProvider
    {
        public string Value { get; }
        public IElement Child { get; private set; }

        public UserName(string value, IElement child=null)
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