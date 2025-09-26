using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 属性值类型枚举
    /// </summary>
    public enum PropertyValueType
    {
        /// <summary>
        /// 空值/未设置
        /// </summary>
        None,

        /// <summary>
        /// 直接值
        /// </summary>
        Direct,

        /// <summary>
        /// 属性引用
        /// </summary>
        PropertyRef
    }

    /// <summary>
    /// 统一的样式属性结构体
    /// 封装属性值的存储和解析逻辑，支持空值状态
    /// </summary>
    [Serializable]
    public struct StyleProperty<T> : IEquatable<StyleProperty<T>>
    {
        public bool Equals(StyleProperty<T> other)
        {
            if (_valueType != other._valueType)
                return false;
            if (_valueType == PropertyValueType.Direct)
            {
                if (_directValue == null && other._directValue == null) return true;
                if (_directValue == null || other._directValue == null) return false;
                return AreDirectValuesEqual(_directValue, other._directValue);
            }
            if (_valueType == PropertyValueType.PropertyRef)
            {
                return string.Equals(_propertyRef, other._propertyRef);
            }
            return true; // None
        }

        /// <summary>
        /// 比较直接值是否相等，特别处理某些类型以确保基于内部值比较
        /// </summary>
        private bool AreDirectValuesEqual(T value1, T value2)
        {
            // 特殊处理RectOffset类型，确保基于其内部值比较而不是引用
            if (value1 is RectOffset rectOffset1 && value2 is RectOffset rectOffset2)
            {
                return rectOffset1.left == rectOffset2.left &&
                       rectOffset1.right == rectOffset2.right &&
                       rectOffset1.top == rectOffset2.top &&
                       rectOffset1.bottom == rectOffset2.bottom;
            }
            // 特殊处理其他需要考虑内部值的引用类型
            else if (value1 is UnityEngine.Object unityObject1 && value2 is UnityEngine.Object unityObject2)
            {
                return unityObject1.GetInstanceID() == unityObject2.GetInstanceID();
            }
            else
            {
                // 对于其他类型，使用默认比较器
                return EqualityComparer<T>.Default.Equals(value1, value2);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is StyleProperty<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + _valueType.GetHashCode();
            if (_valueType == PropertyValueType.Direct && _directValue != null)
            {
                // 对于引用类型，如果类型实现了自定义GetHashCode，使用该方法
                // 但对于RectOffset等Unity类型，我们可能需要特殊处理
                hash = hash * 31 + GetDirectValueHashCode(_directValue);
            }
            if (_valueType == PropertyValueType.PropertyRef && _propertyRef != null)
                hash = hash * 31 + _propertyRef.GetHashCode();
            return hash;
        }

        /// <summary>
        /// 获取直接值的哈希码，特别处理某些类型以确保值变化时哈希码也会变化
        /// </summary>
        private int GetDirectValueHashCode(T value)
        {
            // 特殊处理RectOffset类型，确保其内部值变化时哈希码也会变化
            if (value is RectOffset rectOffset)
            {
                return CalculateRectOffsetHashCode(rectOffset);
            }
            // 特殊处理其他需要考虑内部值的引用类型
            else if (value is UnityEngine.Object unityObject)
            {
                // Unity对象使用其引用ID或名称来生成哈希码
                return unityObject.GetInstanceID();
            }
            else
            {
                // 对于其他类型，使用默认比较器
                return EqualityComparer<T>.Default.GetHashCode(value);
            }
        }

        /// <summary>
        /// 计算RectOffset的哈希码，基于其内部值而不是引用
        /// </summary>
        private int CalculateRectOffsetHashCode(RectOffset rectOffset)
        {
            if (rectOffset == null) return 0;
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + rectOffset.left.GetHashCode();
                hash = hash * 23 + rectOffset.right.GetHashCode();
                hash = hash * 23 + rectOffset.top.GetHashCode();
                hash = hash * 23 + rectOffset.bottom.GetHashCode();
                return hash;
            }
        }

        [SerializeField] private PropertyValueType _valueType;
        [SerializeField] private T _directValue;
        [SerializeField] private string _propertyRef;

        /// <summary>
        /// 属性值类型
        /// </summary>
        public PropertyValueType ValueType
        {
            get => _valueType;
            set => _valueType = value;
        }

        /// <summary>
        /// 直接值
        /// </summary>
        public T DirectValue
        {
            get => _directValue;
            set => _directValue = value;
        }

        /// <summary>
        /// 属性引用
        /// </summary>
        public string PropertyRef
        {
            get => _propertyRef;
            set => _propertyRef = value;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public StyleProperty(PropertyValueType valueType = PropertyValueType.None, T directValue = default, string propertyRef = null)
        {
            _valueType = valueType;
            _directValue = directValue;
            _propertyRef = propertyRef;
        }

        /// <summary>
        /// 创建空值属性
        /// </summary>
        public static StyleProperty<T> None()
        {
            return new StyleProperty<T>(PropertyValueType.None, default, null);
        }

        /// <summary>
        /// 创建直接值属性
        /// </summary>
        public static StyleProperty<T> Direct(T value)
        {
            return new StyleProperty<T>(PropertyValueType.Direct, value, null);
        }

        /// <summary>
        /// 创建属性引用
        /// </summary>
        public static StyleProperty<T> Ref(string propertyRef)
        {
            return new StyleProperty<T>(PropertyValueType.PropertyRef, default, propertyRef);
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        public T GetValue(DeclTheme theme, T defaultValue = default)
        {
            switch (_valueType)
            {
                case PropertyValueType.Direct:
                    return _directValue;
                case PropertyValueType.PropertyRef:
                    if (theme != null && !string.IsNullOrEmpty(_propertyRef))
                    {
                        return theme.GetThemeProperty(_propertyRef, defaultValue);
                    }
                    return defaultValue;
                case PropertyValueType.None:
                default:
                    return defaultValue;
            }
        }

        /// <summary>
        /// 隐式转换：从直接值创建StyleProperty
        /// </summary>
        public static implicit operator StyleProperty<T>(T value)
        {
            return Direct(value);
        }

        /// <summary>
        /// 隐式转换：从字符串引用创建StyleProperty
        /// </summary>
        public static implicit operator StyleProperty<T>(string propertyRef)
        {
            return Ref(propertyRef);
        }

        /// <summary>
        /// 检查是否有值（非空）
        /// </summary>
        public bool HasValue => _valueType != PropertyValueType.None;

        /// <summary>
        /// 检查是否是空值
        /// </summary>
        public bool IsNone => _valueType == PropertyValueType.None;

        /// <summary>
        /// 检查是否是直接值
        /// </summary>
        public bool IsDirectValue => _valueType == PropertyValueType.Direct;

        /// <summary>
        /// 检查是否是属性引用
        /// </summary>
        public bool IsPropertyRef => _valueType == PropertyValueType.PropertyRef && !string.IsNullOrEmpty(_propertyRef);
    }
}