using System;
using System.Collections.Generic;

namespace DeclGUI.Core
{
    /// <summary>
    /// 状态存储器存储接口
    /// 负责管理状态管理器的层次结构
    /// </summary>
    public interface IStateManagerStorage
    {
        /// <summary>
        /// 当前层级的状态管理器
        /// </summary>
        IStateManager CurrentStateManager { get; }

        /// <summary>
        /// 子节点状态管理器字典
        /// </summary>
        IReadOnlyDictionary<string, IStateManagerStorage> ChildStateManagers { get; }

        /// <summary>
        /// 获取或创建子节点状态管理器
        /// </summary>
        /// <param name="key">子节点键</param>
        /// <param name="createStateManager">状态管理器创建函数</param>
        IStateManagerStorage GetOrCreateChildStateManagerStorage(string key, Func<IStateManagerStorage> createStateManager);

        /// <summary>
        /// 获取子节点状态管理器
        /// </summary>
        IStateManagerStorage GetChildStateManagerStorage(string key);

        /// <summary>
        /// 检查子节点状态管理器是否存在
        /// </summary>
        bool HasChildStateManager(string key);

        /// <summary>
        /// 生成容器状态的键（类型+索引的组合键）
        /// </summary>
        string GenerateContainerStateKey(Type containerType);
        
        void ResetAndCleanupUnuseState(int frameThreshold);
    }
}