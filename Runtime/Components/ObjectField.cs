using System;
using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 对象字段组件
    /// 原子化的对象选择控件，只负责选择和显示Unity对象引用
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public struct ObjectField<T> : IObjectField where T : UnityEngine.Object
    {
        /// <summary>
        /// 元素唯一标识符
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 当前对象引用
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// 对象变化回调
        /// </summary>
        public System.Action<T> OnValueChanged { get; }

        /// <summary>
        /// 是否允许场景对象
        /// </summary>
        public bool AllowSceneObjects { get; }

        /// <summary>
        /// 样式
        /// </summary>
        public IDeclStyle Style { get; }

        public string Label;

        /// <summary>
        /// 创建对象字段组件
        /// </summary>
        /// <param name="value">当前对象引用</param>
        /// <param name="onValueChanged">对象变化回调</param>
        /// <param name="allowSceneObjects">是否允许场景对象</param>
        /// <param name="style">样式</param>
        public ObjectField(T value, System.Action<T> onValueChanged = null,
                          bool allowSceneObjects = false, IDeclStyle style = null, string label = null)
        {
            Key = null;
            Value = value;
            OnValueChanged = onValueChanged;
            AllowSceneObjects = allowSceneObjects;
            Style = style;
            Label = label;
        }

        /// <summary>
        /// 渲染方法
        /// </summary>
        /// <returns>UI元素</returns>
        public IElement Render() => null;

        /// <summary>
        /// 设置样式
        /// </summary>
        /// <param name="style">新样式</param>
        /// <returns>带样式的对象字段组件</returns>
        public ObjectField<T> WithStyle(IDeclStyle style)
        {
            return new ObjectField<T>(Value, OnValueChanged, AllowSceneObjects, style);
        }

        /// <summary>
        /// IStylefulElement 接口实现 - 设置样式
        /// </summary>
        /// <param name="style">新样式</param>
        /// <returns>带样式的元素</returns>
        IStylefulElement IStylefulElement.WithStyle(IDeclStyle style)
        {
            return WithStyle(style);
        }

        /// <summary>
        /// 设置是否允许场景对象
        /// </summary>
        /// <param name="allow">是否允许</param>
        /// <returns>带新设置的对象字段组件</returns>
        public ObjectField<T> WithAllowSceneObjects(bool allow)
        {
            return new ObjectField<T>(Value, OnValueChanged, allow, Style);
        }
        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="label">标签文本</param>
        /// <returns>带新标签的对象字段组件</returns>
        public ObjectField<T> WithLabel(string label)
        {
            return new ObjectField<T>(Value, OnValueChanged, AllowSceneObjects, Style, label);
        }

        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;

        /// <summary>
        /// IObjectField 接口实现 - 当前对象引用
        /// </summary>
        UnityEngine.Object IObjectField.Value => Value;

        /// <summary>
        /// IObjectField 接口实现 - 对象类型
        /// </summary>
        Type IObjectField.ObjectType => typeof(T);

        /// <summary>
        /// IObjectField 接口实现 - 是否允许场景对象
        /// </summary>
        bool IObjectField.AllowSceneObjects => AllowSceneObjects;

        /// <summary>
        /// IObjectField 接口实现 - 标签文本
        /// </summary>
        string IObjectField.Label => Label;

        /// <summary>
        /// IObjectField 接口实现 - 通知对象值变化
        /// </summary>
        void IObjectField.NotifyChanged(UnityEngine.Object newValue)
        {
            OnValueChanged?.Invoke(newValue as T);
        }
    }

    /// <summary>
    /// 非泛型版本的对象字段组件
    /// 支持传入 Type 类型
    /// </summary>
    public struct ObjectField : IObjectField
    {
        /// <summary>
        /// 元素唯一标识符
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 当前对象引用
        /// </summary>
        public UnityEngine.Object Value { get; }

        /// <summary>
        /// 对象变化回调
        /// </summary>
        public System.Action<UnityEngine.Object> OnValueChanged { get; }

        /// <summary>
        /// 对象类型
        /// </summary>
        public Type ObjectType { get; }

        /// <summary>
        /// 是否允许场景对象
        /// </summary>
        public bool AllowSceneObjects { get; }

        /// <summary>
        /// 样式
        /// </summary>
        public IDeclStyle Style { get; }

        /// <summary>
        /// 标签文本
        /// </summary>
        public string Label;

        /// <summary>
        /// 创建非泛型对象字段组件
        /// </summary>
        /// <param name="value">当前对象引用</param>
        /// <param name="objectType">对象类型</param>
        /// <param name="onValueChanged">对象变化回调</param>
        /// <param name="allowSceneObjects">是否允许场景对象</param>
        /// <param name="style">样式</param>
        public ObjectField(UnityEngine.Object value, Type objectType, System.Action<UnityEngine.Object> onValueChanged = null,
                          bool allowSceneObjects = false, IDeclStyle style = null, string label = null)
        {
            Key = null;
            Value = value;
            ObjectType = objectType ?? typeof(UnityEngine.Object);
            OnValueChanged = onValueChanged;
            AllowSceneObjects = allowSceneObjects;
            Style = style;
            Label = label;
        }

        /// <summary>
        /// 渲染方法
        /// </summary>
        /// <returns>UI元素</returns>
        public IElement Render() => null;

        /// <summary>
        /// 设置样式
        /// </summary>
        /// <param name="style">新样式</param>
        /// <returns>带样式的对象字段组件</returns>
        public ObjectField WithStyle(IDeclStyle style)
        {
            return new ObjectField(Value, ObjectType, OnValueChanged, AllowSceneObjects, style);
        }

        /// <summary>
        /// IStylefulElement 接口实现 - 设置样式
        /// </summary>
        /// <param name="style">新样式</param>
        /// <returns>带样式的元素</returns>
        IStylefulElement IStylefulElement.WithStyle(IDeclStyle style)
        {
            return WithStyle(style);
        }

        /// <summary>
        /// 设置是否允许场景对象
        /// </summary>
        /// <param name="allow">是否允许</param>
        /// <returns>带新设置的对象字段组件</returns>
        public ObjectField WithAllowSceneObjects(bool allow)
        {
            return new ObjectField(Value, ObjectType, OnValueChanged, allow, Style);
        }
        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="label">标签文本</param>
        /// <returns>带新标签的对象字段组件</returns>
        public ObjectField WithLabel(string label)
        {
            return new ObjectField(Value, ObjectType, OnValueChanged, AllowSceneObjects, Style, label);
        }

        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;

        /// <summary>
        /// IObjectField 接口实现 - 当前对象引用
        /// </summary>
        UnityEngine.Object IObjectField.Value => Value;

        /// <summary>
        /// IObjectField 接口实现 - 对象类型
        /// </summary>
        Type IObjectField.ObjectType => ObjectType;

        /// <summary>
        /// IObjectField 接口实现 - 是否允许场景对象
        /// </summary>
        bool IObjectField.AllowSceneObjects => AllowSceneObjects;

        /// <summary>
        /// IObjectField 接口实现 - 标签文本
        /// </summary>
        string IObjectField.Label => Label;

        /// <summary>
        /// IObjectField 接口实现 - 通知对象值变化
        /// </summary>
        void IObjectField.NotifyChanged(UnityEngine.Object newValue)
        {
            OnValueChanged?.Invoke(newValue);
        }
    }

}