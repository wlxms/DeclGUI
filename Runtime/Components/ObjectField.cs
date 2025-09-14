using DeclGUI.Core;
using UnityEngine;

namespace DeclGUI.Components
{
    /// <summary>
    /// 对象字段组件
    /// 原子化的对象选择控件，只负责选择和显示Unity对象引用
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public struct ObjectField<T> : IElement, IStylefulElement where T : Object
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

        /// <summary>
        /// 创建对象字段组件
        /// </summary>
        /// <param name="value">当前对象引用</param>
        /// <param name="onValueChanged">对象变化回调</param>
        /// <param name="allowSceneObjects">是否允许场景对象</param>
        /// <param name="style">样式</param>
        public ObjectField(T value, System.Action<T> onValueChanged = null,
                          bool allowSceneObjects = false, IDeclStyle style = null)
        {
            Key = null;
            Value = value;
            OnValueChanged = onValueChanged;
            AllowSceneObjects = allowSceneObjects;
            Style = style;
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
        /// 设置是否允许场景对象
        /// </summary>
        /// <param name="allow">是否允许</param>
        /// <returns>带新设置的对象字段组件</returns>
        public ObjectField<T> WithAllowSceneObjects(bool allow)
        {
            return new ObjectField<T>(Value, OnValueChanged, allow, Style);
        }
        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;
    }

}