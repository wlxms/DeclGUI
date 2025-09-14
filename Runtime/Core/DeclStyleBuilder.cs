using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// DeclStyle 构建器
    /// 提供流畅的API创建复杂样式
    /// </summary>
    public class DeclStyleBuilder
    {
        private IDeclStyle _style;
        
        public DeclStyleBuilder()
        {
            _style = new DeclStyle();
        }
        
        public DeclStyleBuilder WithColor(Color color)
        {
            _style = new DeclStyle(color: color).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithWidth(float width)
        {
            _style = new DeclStyle(width: width).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithHeight(float height)
        {
            _style = new DeclStyle(height: height).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithStyleSetId(string styleSetId)
        {
            _style = new DeclStyle(styleSetId: styleSetId).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithBackgroundColor(Color backgroundColor)
        {
            _style = new DeclStyle(backgroundColor: backgroundColor).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithBorderColor(Color borderColor)
        {
            _style = new DeclStyle(borderColor: borderColor).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithPadding(RectOffset padding)
        {
            _style = new DeclStyle(padding: padding).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithPadding(int left, int right, int top, int bottom)
        {
            return WithPadding(new RectOffset(left, right, top, bottom));
        }
        
        public DeclStyleBuilder WithMargin(RectOffset margin)
        {
            _style = new DeclStyle(margin: margin).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithFontSize(int fontSize)
        {
            _style = new DeclStyle(fontSize: fontSize).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithFontStyle(FontStyle fontStyle)
        {
            _style = new DeclStyle(fontStyle: fontStyle).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithAlignment(TextAnchor alignment)
        {
            _style = new DeclStyle(alignment: alignment).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithBorderWidth(float borderWidth)
        {
            _style = new DeclStyle(borderWidth: borderWidth).Merge(_style);
            return this;
        }
        
        public DeclStyleBuilder WithBorderRadius(float borderRadius)
        {
            _style = new DeclStyle(borderRadius: borderRadius).Merge(_style);
            return this;
        }
        
        public DeclStyle Build()
        {
            return (DeclStyle)_style;
        }
        
        // 隐式转换，方便使用
        public static implicit operator DeclStyle(DeclStyleBuilder builder)
        {
            return builder.Build();
        }
    }
}