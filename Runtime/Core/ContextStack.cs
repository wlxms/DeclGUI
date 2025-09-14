using System;
using System.Collections.Generic;

namespace DeclGUI.Core
{
    /// <summary>
    /// 上下文栈管理器
    /// 负责管理不同类型上下文的栈结构
    /// </summary>
    public class ContextStack : IContextReader
    {
        private readonly Dictionary<Type, Stack<IContextProvider>> _contextStacks = new Dictionary<Type, Stack<IContextProvider>>();
        
        /// <summary>
        /// 压入上下文
        /// </summary>
        public void Push<T>(in T context) where T : struct, IContextProvider
        {
            Type contextType = typeof(T);
            if (!_contextStacks.TryGetValue(contextType, out var stack))
            {
                stack = new Stack<IContextProvider>();
                _contextStacks[contextType] = stack;
            }
            stack.Push(context);
        }
        
        /// <summary>
        /// 获取指定类型的上下文
        /// </summary>
        public T Get<T>() where T : struct, IContextProvider
        {
            if (_contextStacks.TryGetValue(typeof(T), out var stack) && stack.Count > 0)
            {
                return (T)stack.Peek();
            }
            return default;
        }
        
        /// <summary>
        /// 尝试获取指定类型的上下文
        /// </summary>
        public bool TryGet<T>(out T value) where T : struct, IContextProvider
        {
            value = default;
            if (_contextStacks.TryGetValue(typeof(T), out var stack) && stack.Count > 0)
            {
                value = (T)stack.Peek();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 检查是否存在指定类型的上下文
        /// </summary>
        public bool Has<T>() where T : struct, IContextProvider
        {
            return _contextStacks.TryGetValue(typeof(T), out var stack) && stack.Count > 0;
        }
        
        /// <summary>
        /// 弹出指定类型的上下文
        /// </summary>
        public void Pop<T>() where T : struct, IContextProvider
        {
            Pop(typeof(T));
        }
        
        /// <summary>
        /// 弹出指定类型的上下文（非泛型版本）
        /// </summary>
        public void Pop(Type contextType)
        {
            if (_contextStacks.TryGetValue(contextType, out var stack) && stack.Count > 0)
            {
                stack.Pop();
                if (stack.Count == 0)
                {
                    _contextStacks.Remove(contextType);
                }
            }
        }
        
        /// <summary>
        /// 压入上下文（非泛型版本）
        /// </summary>
        public void Push(Type contextType, in IContextProvider context)
        {
            if (!_contextStacks.TryGetValue(contextType, out var stack))
            {
                stack = new Stack<IContextProvider>();
                _contextStacks[contextType] = stack;
            }
            stack.Push(context);
        }
    }
}