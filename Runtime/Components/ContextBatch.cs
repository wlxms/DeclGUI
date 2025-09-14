using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DeclGUI.Components;
using Unity.VisualScripting;

namespace DeclGUI.Core
{
    /// <summary>
    /// 批量上下文容器
    /// 用于同时提供多个上下文
    /// </summary>
    public struct ContextBatch : IContextProvider, IElement, IEnumerable<IElement>
    {
        private readonly IContextProvider[] _contexts;
        public IElement Child { get; private set; }

        public ContextBatch(params IContextProvider[] contexts)
        {
            _contexts = contexts ?? Array.Empty<IContextProvider>();
            Child = null;
        }

        public IElement Render()
        {
            if (_contexts.Length <= 0) return Child;

            var element = _contexts[0];
            for (int i = 1; i < _contexts.Length; i++)
            {
                var current = _contexts[i];
                element.Add(current);
                element = current;
            }

            return _contexts[0];
        }

        public void Add(IElement child)
        {
            Child = child;
        }
        
        public IEnumerator<IElement> GetEnumerator()
        {
            yield return Child;
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}