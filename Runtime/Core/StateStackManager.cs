using System;
using System.Collections.Generic;

namespace DeclGUI.Core
{
    /// <summary>
    /// 状态栈管理器
    /// 负责管理状态存储器的栈结构
    /// </summary>
    public class StateStackManager
    {
        private readonly Stack<IStateManagerStorage> _storageStack = new Stack<IStateManagerStorage>();
        
        /// <summary>
        /// 当前栈顶的状态存储器存储
        /// </summary>
        public IStateManagerStorage CurrentStorage => _storageStack.Count > 0 ? _storageStack.Peek() : null;
        
        /// <summary>
        /// 当前活跃的状态管理器（栈顶存储器的当前状态管理器）
        /// </summary>
        public IStateManager CurrentStateManager => CurrentStorage?.CurrentStateManager;
        
        /// <summary>
        /// 栈深度
        /// </summary>
        public int Count => _storageStack.Count;
        
        /// <summary>
        /// 压入新的状态存储器存储
        /// </summary>
        /// <param name="storage">状态存储器存储实例</param>
        public void Push(IStateManagerStorage storage)
        {
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));
                
            _storageStack.Push(storage);
        }
        
        /// <summary>
        /// 弹出栈顶的状态存储器存储
        /// </summary>
        public IStateManagerStorage Pop()
        {
            if (_storageStack.Count == 0)
                throw new InvalidOperationException("State stack is empty");
                
            return _storageStack.Pop();
        }
        
        /// <summary>
        /// 清空状态栈
        /// </summary>
        public void Clear()
        {
            _storageStack.Clear();
        }
        
        /// <summary>
        /// 检查状态栈是否为空
        /// </summary>
        public bool IsEmpty()
        {
            return _storageStack.Count == 0;
        }
    }
}