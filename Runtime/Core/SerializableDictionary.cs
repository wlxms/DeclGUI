using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 可序列化字典基类
    /// </summary>
    [Serializable]
    public abstract class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> _keys = new List<TKey>();
        [SerializeField] private List<TValue> _values = new List<TValue>();
        
        protected Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        
        public Dictionary<TKey, TValue> Dictionary => _dictionary;
        
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            
            foreach (var kvp in _dictionary)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }
        
        public void OnAfterDeserialize()
        {
            _dictionary.Clear();
            
            for (int i = 0; i < Mathf.Min(_keys.Count, _values.Count); i++)
            {
                if (_keys[i] != null)
                {
                    _dictionary[_keys[i]] = _values[i];
                }
            }
        }
        
        // 字典操作方法
        public void Add(TKey key, TValue value) => _dictionary.Add(key, value);
        public bool Remove(TKey key) => _dictionary.Remove(key);
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
        public void Clear() => _dictionary.Clear();
        public int Count => _dictionary.Count;
        
        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }
        
        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
    }
    
    /// <summary>
    /// 伪类到样式的可序列化字典
    /// </summary>
    [Serializable]
    public class PseudoClassStyleDictionary : SerializableDictionary<PseudoClass, DeclStyle>
    {
        // 使用 DeclStyle 作为值类型，避免接口序列化问题
    }
}