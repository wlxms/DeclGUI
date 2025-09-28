using System;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// FoldoutGroup状态类
    /// </summary>
    public class FoldoutGroupState
    {
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public FoldoutGroupState()
        {
            IsExpanded = true;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isExpanded">初始展开状态</param>
        public FoldoutGroupState(bool isExpanded)
        {
            IsExpanded = isExpanded;
        }
    }
}