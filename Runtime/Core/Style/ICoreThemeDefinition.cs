namespace DeclGUI.Core
{
    /// <summary>
    /// 核心主题定义接口
    /// 定义所有主题必须包含的核心样式
    /// </summary>
    public interface ICoreThemeDefinition
    {
        // 基础控件样式
        IDeclStyle label { get; set; }
        IDeclStyle textField { get; set; }
        IDeclStyle textArea { get; set; }
        IDeclStyle button { get; set; }
        IDeclStyle toggle { get; set; }
        IDeclStyle slider { get; set; }
        IDeclStyle sliderThumb { get; set; }
        IDeclStyle horizontalSlider { get; set; }
        IDeclStyle horizontalSliderThumb { get; set; }
        IDeclStyle verticalSlider { get; set; }
        IDeclStyle verticalSliderThumb { get; set; }
        IDeclStyle scrollbar { get; set; }
        IDeclStyle scrollbarThumb { get; set; }
        IDeclStyle scrollbarUpButton { get; set; }
        IDeclStyle scrollbarDownButton { get; set; }
        IDeclStyle scrollbarLeftButton { get; set; }
        IDeclStyle scrollbarRightButton { get; set; }
        IDeclStyle window { get; set; }
        IDeclStyle box { get; set; }
        IDeclStyle helpBox { get; set; }
        IDeclStyle toolbar { get; set; }
        IDeclStyle toolbarButton { get; set; }
        IDeclStyle toolbarDropDown { get; set; }
        IDeclStyle toolbarTextField { get; set; }
        IDeclStyle toolbarSearchField { get; set; }
        IDeclStyle popup { get; set; }
        
        // 特殊样式
        IDeclStyle whiteLabel { get; set; }
        IDeclStyle whiteMiniLabel { get; set; }
        IDeclStyle miniLabel { get; set; }
        IDeclStyle boldLabel { get; set; }
        IDeclStyle largeLabel { get; set; }
        IDeclStyle centeredGreyMiniLabel { get; set; }
        IDeclStyle wordWrappedLabel { get; set; }
        IDeclStyle wordWrappedMiniLabel { get; set; }
        IDeclStyle textView { get; set; }
        
        // 编辑器专用样式
        IDeclStyle colorField { get; set; }
        IDeclStyle layerMaskField { get; set; }
        IDeclStyle enumPopup { get; set; }
        IDeclStyle objectField { get; set; }
        IDeclStyle objectFieldThumb { get; set; }
        IDeclStyle curveField { get; set; }
        IDeclStyle curveFieldBackground { get; set; }
        IDeclStyle preLabel { get; set; }
        IDeclStyle preButton { get; set; }
        IDeclStyle preToolbar { get; set; }
        IDeclStyle preSlider { get; set; }
        IDeclStyle preSliderThumb { get; set; }
        IDeclStyle preTextArea { get; set; }
        
        // 布局和容器样式
        IDeclStyle tab { get; set; }
        IDeclStyle tabGroup { get; set; }
        IDeclStyle tabContent { get; set; }
        IDeclStyle panel { get; set; }
        IDeclStyle scrollGroup { get; set; }
        IDeclStyle header { get; set; }
        IDeclStyle subHeader { get; set; }
        IDeclStyle separator { get; set; }
        IDeclStyle space { get; set; }
        
        // 交互元素样式
        IDeclStyle link { get; set; }
        IDeclStyle progressBar { get; set; }
        IDeclStyle progressBarBack { get; set; }
        IDeclStyle selectionRect { get; set; }
        IDeclStyle badge { get; set; }
        IDeclStyle notification { get; set; }
        
        // 图标和图像样式
        IDeclStyle iconButton { get; set; }
        IDeclStyle imageView { get; set; }
        IDeclStyle image { get; set; }
        
        // Markdown样式
        IDeclStyle H1 { get; set; }
        IDeclStyle H2 { get; set; }
        IDeclStyle H3 { get; set; }
        IDeclStyle H4 { get; set; }
        IDeclStyle H5 { get; set; }
        IDeclStyle H6 { get; set; }
        IDeclStyle paragraph { get; set; }
        IDeclStyle code { get; set; }
        IDeclStyle quote { get; set; }
    }
}