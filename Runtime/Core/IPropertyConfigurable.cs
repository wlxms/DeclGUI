using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 支持属性配置的接口
    /// </summary>
    public interface IPropertyConfigurable
    {
        /// <summary>
        /// 获取所有可配置的属性信息
        /// </summary>
        IEnumerable<PropertyInfo> GetConfigurableProperties();
        
        /// <summary>
        /// 设置属性值
        /// </summary>
        void SetProperty(string propertyName, object value);
        
        /// <summary>
        /// 获取属性值
        /// </summary>
        object GetProperty(string propertyName);
    }
    
    /// <summary>
    /// 属性信息
    /// </summary>
    [Serializable]
    public class PropertyInfo
    {
        public string Name;
        public PropertyType Type;
        public object Value;
        public string DisplayName;
        public string Description;
        
        public PropertyInfo(string name, PropertyType type, object value = null, string displayName = null, string description = null)
        {
            Name = name;
            Type = type;
            Value = value;
            DisplayName = displayName ?? name;
            Description = description ?? string.Empty;
        }
    }
    
    /// <summary>
    /// 属性类型枚举
    /// </summary>
    public enum PropertyType
    {
        Float,
        Int,
        Color,
        String,
        Boolean,
        Vector2,
        Vector3,
        RectOffset
    }
}