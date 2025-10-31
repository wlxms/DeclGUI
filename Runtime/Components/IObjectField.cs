using System;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// ObjectField接口
    /// 提供ObjectField组件的统一访问方式，避免反射
    /// </summary>
    public interface IObjectField : IElement, IStylefulElement
    {
        /// <summary>
        /// 当前对象引用
        /// </summary>
        UnityEngine.Object Value { get; }
        
        /// <summary>
        /// 对象类型
        /// </summary>
        Type ObjectType { get; }
        
        /// <summary>
        /// 是否允许场景对象
        /// </summary>
        bool AllowSceneObjects { get; }
        
        /// <summary>
        /// 标签文本
        /// </summary>
        string Label { get; }
        
        /// <summary>
        /// 通知对象值变化
        /// </summary>
        /// <param name="newValue">新对象值</param>
        void NotifyChanged(UnityEngine.Object newValue);
    }
}