using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 状态管理器接口
    /// 负责管理和维护元素的状态
    /// </summary>
    public interface IStateManager
    {
        /// <summary>
        /// 获取或创建元素的状态
        /// </summary>
        /// <param name="element">有状态元素</param>
        /// <returns>元素的状态对象</returns>
        IElementState GetOrCreateState(in IElementWithKey element);
        
        /// <summary>
        /// 更新元素的状态
        /// </summary>
        /// <param name="element">有状态元素</param>
        /// <param name="state">新的状态对象</param>
        void UpdateState(in IStatefulElement element, object state);
        
        /// <summary>
        /// 移除元素的状态
        /// </summary>
        /// <param name="element">有状态元素</param>
        void RemoveState(in IElementWithKey element);
        
        /// <summary>
        /// 检查元素是否有状态
        /// </summary>
        /// <param name="element">有状态元素</param>
        /// <returns>如果元素有状态则返回true</returns>
        bool HasState(in IElementWithKey element);

        /// <summary>
        /// 重置所有类型计数器
        /// 在每帧渲染完成后调用
        /// </summary>
        void ResetCounters();
        
        /// <summary>
        /// 清理未使用的状态
        /// </summary>
        /// <param name="framesToKeep">保留的帧数</param>
        void CleanupUnusedStates(int framesToKeep = 2);
    }
}