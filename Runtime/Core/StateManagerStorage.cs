using System;
using System.Collections.Generic;

namespace DeclGUI.Core
{
    /// <summary>
    /// 默认状态存储器存储实现
    /// </summary>
    public class StateManagerStorage : IStateManagerStorage
    {
        private readonly IStateManager _currentStateManager;
        private readonly Dictionary<string, IStateManagerStorage> _childStateManagers = new Dictionary<string, IStateManagerStorage>();
        private readonly Dictionary<Type, int> _containerTypeCounters = new Dictionary<Type, int>();
        
        public IStateManager CurrentStateManager => _currentStateManager;
        public IReadOnlyDictionary<string, IStateManagerStorage> ChildStateManagers => _childStateManagers;
        
        public StateManagerStorage(IStateManager currentStateManager)
        {
            _currentStateManager = currentStateManager ?? throw new ArgumentNullException(nameof(currentStateManager));
        }
        
        public IStateManagerStorage GetOrCreateChildStateManagerStorage(string key, Func<IStateManagerStorage> createStateManager)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
                
            if (createStateManager == null)
                throw new ArgumentNullException(nameof(createStateManager));
                
            if (!_childStateManagers.TryGetValue(key, out var stateManager))
            {
                stateManager = createStateManager();
                _childStateManagers[key] = stateManager;
            }
            return stateManager;
        }
        
        public IStateManagerStorage GetChildStateManagerStorage(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
                
            _childStateManagers.TryGetValue(key, out var stateManager);
            return stateManager;
        }
        
        public bool HasChildStateManager(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
                
            return _childStateManagers.ContainsKey(key);
        }

        /// <summary>
        /// 生成容器状态的键（类型+索引的组合键）
        /// 使用稳定的基于容器实例的键生成机制
        /// </summary>
        public string GenerateContainerStateKey(Type containerType)
        {
            if (!_containerTypeCounters.TryGetValue(containerType, out int count))
            {
                count = 0;
                _containerTypeCounters[containerType] = count;
            }

            string key = $"{containerType.Name}_{count}";
            _containerTypeCounters[containerType] = count + 1;
            return key;
        }
        
        /// <summary>
        /// 重置所有容器类型计数器
        /// 在每帧渲染完成后调用，确保下一帧键生成的一致性
        /// </summary>
        public void ResetCounters()
        {
            _containerTypeCounters.Clear();
        }

        public void ResetAndCleanupUnuseState(int frameThreshold)
        {
            ResetCounters();
            
            _currentStateManager.ResetCounters();
            _currentStateManager.CleanupUnusedStates(frameThreshold);
            foreach (var child in _childStateManagers.Values)
            {
                child.ResetAndCleanupUnuseState(frameThreshold);
            }
        }
    }
}