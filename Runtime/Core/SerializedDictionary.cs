using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SerializedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private Dictionary<TKey, TValue> m_dictRuntime;

    public List<TKey> keys = new List<TKey>();
    public List<TValue> values = new List<TValue>();


    public SerializedDictionary()
    {
        
    }

    public SerializedDictionary(IDictionary<TKey,TValue> dict)
    {
        foreach (var (key,value) in dict)
        {
            this[key] = value;
        }
    }

    private void InitializeEnumerator(bool reset = false)
    {
        if (m_dictRuntime != null && reset == false) return;

        m_dictRuntime = new Dictionary<TKey, TValue>();

        for (var index = 0; index < keys.Count; index++)
        {
            var key = keys[index];
            var value = values[index];
            m_dictRuntime[key] = value;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        InitializeEnumerator();
        return m_dictRuntime.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        var index = keys.IndexOf(item.Key);

        if (index != -1)
        {
            keys.RemoveAt(index);
            values.RemoveAt(index);
        }

        keys.Add(item.Key);
        values.Add(item.Value);
        if (m_dictRuntime == null) return;

        m_dictRuntime[item.Key] = item.Value;
    }

    public void Clear()
    {
        m_dictRuntime?.Clear();

        keys.Clear();
        values.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        InitializeEnumerator();

        if (m_dictRuntime.ContainsKey(item.Key) == false)
            return false;

        return m_dictRuntime[item.Key].Equals(item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException("The array cannot be null.");
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
        if (Count > array.Length - arrayIndex)
            throw new ArgumentException("The destination array has fewer elements than the collection.");

        if (keys.Count < arrayIndex)
            throw new ArgumentException("Not enough space in the array;");

        for (int i = 0; i < arrayIndex; i++)
        {
            array[i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var index = keys.IndexOf(item.Key);

        if (index == -1)
            return false;

        keys.RemoveAt(index);
        values.RemoveAt(index);

        if (m_dictRuntime != null)
            return m_dictRuntime.Remove(item.Key);

        return true;
    }

    
    public int Count => keys.Count;
    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        this.Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public bool ContainsKey(TKey key)
    {
        if (m_dictRuntime != null)
            return m_dictRuntime.ContainsKey(key);

        return keys.Contains(key);
    }

    public bool Remove(TKey key)
    {
        var index = keys.IndexOf(key);

        if (index == -1)
            return false;

        keys.RemoveAt(index);
        values.RemoveAt(index);

        if (m_dictRuntime != null)
            return m_dictRuntime.Remove(key);

        return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        InitializeEnumerator();

        return m_dictRuntime.TryGetValue(key, out value);
    }

    public TValue this[TKey key]
    {
        get
        {
            InitializeEnumerator();
            return m_dictRuntime[key];
        }
        set
        {
            InitializeEnumerator();
            // m_dictRuntime[key] = value;
            Add(key, value);
        }
    }

    public ICollection<TKey> Keys => keys;
    public ICollection<TValue> Values => values;
}