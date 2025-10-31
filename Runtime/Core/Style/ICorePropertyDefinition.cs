namespace DeclGUI.Core
{
    /// <summary>
    /// 核心属性定义接口
    /// 定义所有主题必须包含的核心属性
    /// 基于DeclThemeCreator中创建的属性模板
    /// </summary>
    public interface ICorePropertyDefinition
    {
        // 颜色属性
        DeclPropertyTemplate.TemplateProperty backgroundColor { get; set; }
        DeclPropertyTemplate.TemplateProperty surfaceColor { get; set; }
        DeclPropertyTemplate.TemplateProperty primaryColor { get; set; }
        DeclPropertyTemplate.TemplateProperty secondaryColor { get; set; }
        DeclPropertyTemplate.TemplateProperty textColor { get; set; }
        DeclPropertyTemplate.TemplateProperty textSecondaryColor { get; set; }
        DeclPropertyTemplate.TemplateProperty borderColor { get; set; }
        DeclPropertyTemplate.TemplateProperty hoverColor { get; set; }
        DeclPropertyTemplate.TemplateProperty activeColor { get; set; }
        DeclPropertyTemplate.TemplateProperty focusColor { get; set; }
        DeclPropertyTemplate.TemplateProperty disabledColor { get; set; }
        DeclPropertyTemplate.TemplateProperty successColor { get; set; }
        DeclPropertyTemplate.TemplateProperty warningColor { get; set; }
        DeclPropertyTemplate.TemplateProperty errorColor { get; set; }
        DeclPropertyTemplate.TemplateProperty infoColor { get; set; }
        
        // 字体大小属性
        DeclPropertyTemplate.TemplateProperty fontSizeSmall { get; set; }
        DeclPropertyTemplate.TemplateProperty fontSizeNormal { get; set; }
        DeclPropertyTemplate.TemplateProperty fontSizeLarge { get; set; }
        DeclPropertyTemplate.TemplateProperty fontSizeXLarge { get; set; }
        DeclPropertyTemplate.TemplateProperty fontSizeXXLarge { get; set; }
        DeclPropertyTemplate.TemplateProperty fontSizeXXXLarge { get; set; }
        
        // 间距属性
        DeclPropertyTemplate.TemplateProperty paddingSmall { get; set; }
        DeclPropertyTemplate.TemplateProperty paddingNormal { get; set; }
        DeclPropertyTemplate.TemplateProperty paddingLarge { get; set; }
        DeclPropertyTemplate.TemplateProperty marginSmall { get; set; }
        DeclPropertyTemplate.TemplateProperty marginNormal { get; set; }
        DeclPropertyTemplate.TemplateProperty marginLarge { get; set; }
        DeclPropertyTemplate.TemplateProperty paddingHeading { get; set; }
        DeclPropertyTemplate.TemplateProperty marginHeading { get; set; }
        DeclPropertyTemplate.TemplateProperty paddingParagraph { get; set; }
        DeclPropertyTemplate.TemplateProperty marginParagraph { get; set; }
        DeclPropertyTemplate.TemplateProperty marginQuote { get; set; }
        
        // 边框属性
        DeclPropertyTemplate.TemplateProperty borderWidthNormal { get; set; }
        DeclPropertyTemplate.TemplateProperty borderWidthThick { get; set; }
        DeclPropertyTemplate.TemplateProperty borderRadiusSmall { get; set; }
        DeclPropertyTemplate.TemplateProperty borderRadiusNormal { get; set; }
        DeclPropertyTemplate.TemplateProperty borderRadiusLarge { get; set; }
        
        // 控件高度属性
        DeclPropertyTemplate.TemplateProperty controlHeightSmall { get; set; }
        DeclPropertyTemplate.TemplateProperty controlHeightNormal { get; set; }
        DeclPropertyTemplate.TemplateProperty controlHeightLarge { get; set; }
        
        // Markdown专用属性
        DeclPropertyTemplate.TemplateProperty linkColor { get; set; }
        DeclPropertyTemplate.TemplateProperty linkHoverColor { get; set; }
        
        // 按钮专用内边距属性
        DeclPropertyTemplate.TemplateProperty buttonPadding { get; set; }
        DeclPropertyTemplate.TemplateProperty smallButtonPadding { get; set; }
        DeclPropertyTemplate.TemplateProperty largeButtonPadding { get; set; }
        
        // 面板专用内边距属性
        DeclPropertyTemplate.TemplateProperty panelPadding { get; set; }
        
        // 输入框专用内边距属性
        DeclPropertyTemplate.TemplateProperty textFieldPadding { get; set; }
        DeclPropertyTemplate.TemplateProperty subtleTextFieldPadding { get; set; }
        
        // 标签专用内边距属性
        DeclPropertyTemplate.TemplateProperty labelPadding { get; set; }
    }
}