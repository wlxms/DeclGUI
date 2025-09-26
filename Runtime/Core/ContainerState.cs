using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 容器状态实现
    /// 负责管理容器内子元素的状态，包含类型安全检查
    /// </summary>
    public class ContainerState : IStateManager
    {
        // 修改状态存储字典使用 IElementState 接口
        private readonly Dictionary<string, IElementState> _stateStorage = new Dictionary<string, IElementState>();
        private readonly Dictionary<Type, int> _typeCounters = new Dictionary<Type, int>();
        private readonly Dictionary<string, int> _stateLastUsedFrame = new Dictionary<string, int>();
        private int _currentFrame = 0;

        public IElementState GetOrCreateState(in IElementWithKey element)
        {
            string key = GetElementKey(in element);
            var elementType = element.GetType();

            if (_stateStorage.TryGetValue(key, out var existingState))
            {
                // 检查元素类型是否匹配，确保状态安全
                if (existingState.ElementType == elementType)
                {
                    _stateLastUsedFrame[key] = _currentFrame;
                    return existingState;
                }
                else
                {
                    // 类型不匹配，重建状态
                    _stateStorage.Remove(key);
                    _stateLastUsedFrame.Remove(key);
                }
            }

            // 创建新的状态对象
            var newState = new ElementState();
            newState.ElementType = elementType; // 设置元素类型

            if (element is IStatefulElement statefulElement)
            {
                // 如果元素是有状态的，使用元素自己的状态
                var elementState = statefulElement.CreateState();
                newState.State = elementState;
                
                // 更新状态标志（确保状态同步）
                newState.UpdateStateFlags();
            }

            _stateStorage[key] = newState;
            _stateLastUsedFrame[key] = _currentFrame;
            return newState;
        }

        public void UpdateState(in IStatefulElement element, object state)
        {
            string key = GetElementKey(element);
            if (_stateStorage.TryGetValue(key, out var existingState))
            {
                existingState.State = state;
                
                // 更新状态标志（确保状态同步）
                if (existingState is ElementState concreteState)
                {
                    concreteState.UpdateStateFlags();
                }
            }
            else
            {
                var newState = GetOrCreateState(element) as IElementState;
                newState.State = state;
                
                // 更新状态标志（确保状态同步）
                if (newState is ElementState concreteState)
                {
                    concreteState.UpdateStateFlags();
                }
            }
            _stateLastUsedFrame[key] = _currentFrame;
        }

        public void RemoveState(in IElementWithKey element)
        {
            string key = GetElementKey(in element);
            _stateStorage.Remove(key);
            _stateLastUsedFrame.Remove(key); // 同时清理帧记录
        }

        public bool HasState(in IElementWithKey element)
        {
            string key = GetElementKey(in element);
            return _stateStorage.ContainsKey(key) &&
                   _stateStorage[key].ElementType == element.GetType();
        }

        /// <summary>
        /// 重置所有类型计数器
        /// 在每帧渲染完成后调用
        /// </summary>
        public void ResetCounters()
        {
            _typeCounters.Clear();
            _currentFrame++;
        }

        /// <summary>
        /// 清理未使用的状态
        /// </summary>
        /// <param name="framesToKeep">保留的帧数</param>
        public void CleanupUnusedStates(int framesToKeep = 2)
        {
            var keysToRemove = new List<string>();

            foreach (var kvp in _stateLastUsedFrame)
            {
                if (_currentFrame - kvp.Value > framesToKeep)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _stateStorage.Remove(key);
                _stateLastUsedFrame.Remove(key);
            }
        }

        /// <summary>
        /// 获取元素的唯一键
        /// 使用元素已分配的Key或生成类型+索引的组合键
        /// </summary>
        private string GetElementKey(in IElementWithKey element)
        {
            var key = !string.IsNullOrEmpty(element.Key)
                ? element.Key
                : GenerateIndexedKey(element.GetType());

            element.Key = key;
            return key;
        }

        /// <summary>
        /// 生成类型+索引的组合键
        /// 每个状态管理器实例维护自己的计数器
        /// </summary>
        private string GenerateIndexedKey(Type elementType)
        {
            if (!_typeCounters.TryGetValue(elementType, out int count))
            {
                count = 0;
                _typeCounters[elementType] = count;
            }

            string key = $"{elementType.Name}_{count}";
            _typeCounters[elementType] = count + 1;
            return key;
        }
    }
}