using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 声明式样式接口
    /// 不直接操作 GUIStyle，由渲染器负责样式应用
    /// 移除GUIStyle属性
    /// </summary>
    public interface IDeclStyle : System.IEquatable<IDeclStyle>
    {
        // 可序列化字段属性（移除GUIStyle）
        Color? Color { get; set; }
        float? Width { get; set; }
        float? Height { get; set; }
        string? StyleSetId { get; set; }
        Color? BackgroundColor { get; set; }
        Color? BorderColor { get; set; }
        RectOffset? Padding { get; set; }
        RectOffset? Margin { get; set; }
        int? FontSize { get; set; }
        FontStyle? FontStyle { get; set; }
        TextAnchor? Alignment { get; set; }
        float? BorderWidth { get; set; }
        float? BorderRadius { get; set; }
        
        /// <summary>
        /// 根据元素状态获取对应的样式
        /// </summary>
        IDeclStyle GetStyleForState(IElementState elementState);
        
        /// <summary>
        /// 合并另一个样式（返回新实例）
        /// </summary>
        IDeclStyle Merge(IDeclStyle other);

        /// <summary>
        /// 获取样式内容的哈希码
        /// </summary>
        int GetContentHashCode();

        // IEquatable<IDeclStyle>.Equals(IDeclStyle other) 由接口继承，无需重复声明
    }
}