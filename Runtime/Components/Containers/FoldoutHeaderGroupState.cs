using System;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// FoldoutHeaderGroup状态类
    /// </summary>
    public class FoldoutHeaderGroupState
    {
        public Rect lastHeaderRect;
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public FoldoutHeaderGroupState()
        {
            IsExpanded = true;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isExpanded">初始展开状态</param>
        public FoldoutHeaderGroupState(bool isExpanded)
        {
            IsExpanded = isExpanded;
        }
    }
}