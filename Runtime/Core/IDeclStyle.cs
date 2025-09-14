using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 声明式样式接口
    /// 不直接操作 GUIStyle，由渲染器负责样式应用
    /// </summary>
    public interface IDeclStyle
    {
        /// <summary>
        /// 根据元素状态获取对应的样式
        /// </summary>
        IDeclStyle GetStyleForState(IElementState elementState);
        
        /// <summary>
        /// 合并另一个样式（返回新实例）
        /// </summary>
        IDeclStyle Merge(IDeclStyle other);
        
        // 尺寸属性
        float GetWidth();
        float GetHeight();
        
        // 颜色属性
        Color? GetColor();
        Color? GetBackgroundColor();
        Color? GetBorderColor();
        
        // 布局属性
        RectOffset GetPadding();
        RectOffset GetMargin();
        
        // 文本属性
        int? GetFontSize();
        FontStyle? GetFontStyle();
        TextAnchor? GetAlignment();
        
        // 边框属性
        float? GetBorderWidth();
        float? GetBorderRadius();
        
        // 样式集引用
        string GetStyleSetId();
    }
}