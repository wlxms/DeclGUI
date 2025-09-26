using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 可序列化的声明式样式接口
    /// 继承自 IDeclStyle，添加可序列化字段支持
    /// 使用统一的StyleProperty<T>类型，移除GUIStyle字段
    /// </summary>
    public interface ISerializableDeclStyle : IDeclStyle
    {
        // 保持StyleSetId使用SerializableRefNullable以保持向后兼容
        new SerializableRefNullable<string> StyleSetId { get; set; }
        
        // 可序列化字段属性，使用StyleProperty类型
        new StyleProperty<Color> Color { get; set; }
        new StyleProperty<float> Width { get; set; }
        new StyleProperty<float> Height { get; set; }
        new StyleProperty<Color> BackgroundColor { get; set; }
        new StyleProperty<Color> BorderColor { get; set; }
        new StyleProperty<RectOffset> Padding { get; set; }
        new StyleProperty<RectOffset> Margin { get; set; }
        new StyleProperty<int> FontSize { get; set; }
        new StyleProperty<FontStyle> FontStyle { get; set; }
        new StyleProperty<TextAnchor> Alignment { get; set; }
        new StyleProperty<float> BorderWidth { get; set; }
        new StyleProperty<float> BorderRadius { get; set; }
    }
}