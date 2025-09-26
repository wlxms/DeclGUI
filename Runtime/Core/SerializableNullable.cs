using System;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 可序列化的可空值类型包装器
    /// </summary>
    [Serializable]
    public struct SerializableNullable<T> where T : struct
    {
        [SerializeField] private T _value;
        [SerializeField] private bool _hasValue;

        public T? Value
        {
            get => _hasValue ? _value : (T?)null;
            set
            {
                _hasValue = value.HasValue;
                if (value.HasValue)
                {
                    _value = value.Value;
                }
            }
        }

        public bool HasValue => _hasValue;

        public SerializableNullable(T? value)
        {
            _hasValue = value.HasValue;
            _value = value.HasValue ? value.Value : default;
        }

        public static implicit operator SerializableNullable<T>(T? value)
        {
            return new SerializableNullable<T>(value);
        }

        public static implicit operator T?(SerializableNullable<T> serializableNullable)
        {
            return serializableNullable.Value;
        }

        public T GetValueOrDefault(T defaultValue = default)
        {
            return _hasValue ? _value : defaultValue;
        }
    }

    /// <summary>
    /// 可序列化的可空引用类型包装器
    /// </summary>
    [Serializable]
    public struct SerializableRefNullable<T> where T : class
    {
        [SerializeField] private T _value;
        [SerializeField] private bool _hasValue;

        public T Value
        {
            get => _hasValue ? _value : null;
            set
            {
                _hasValue = value != null;
                _value = value;
            }
        }

        public bool HasValue => _hasValue;

        public SerializableRefNullable(T value)
        {
            _hasValue = value != null;
            _value = value;
        }

        public static implicit operator SerializableRefNullable<T>(T value)
        {
            return new SerializableRefNullable<T>(value);
        }

        public static implicit operator T(SerializableRefNullable<T> serializableNullable)
        {
            return serializableNullable.Value;
        }
    }
}