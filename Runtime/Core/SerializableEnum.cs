using System;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 可序列化的枚举包装器
    /// </summary>
    [Serializable]
    public struct SerializableEnum<T> where T : struct, Enum
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

        public SerializableEnum(T? value)
        {
            _hasValue = value.HasValue;
            _value = value.HasValue ? value.Value : default;
        }

        public static implicit operator SerializableEnum<T>(T? value)
        {
            return new SerializableEnum<T>(value);
        }

        public static implicit operator T?(SerializableEnum<T> serializableEnum)
        {
            return serializableEnum.Value;
        }

        public T GetValueOrDefault(T defaultValue = default)
        {
            return _hasValue ? _value : defaultValue;
        }
    }
}