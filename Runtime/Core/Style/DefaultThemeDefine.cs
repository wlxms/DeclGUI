using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 默认主题定义类
    /// 提供所有核心样式和属性的默认实现
    /// 基于DeclThemeCreator中的设计
    /// </summary>
    public static class DefaultThemeDefine
    {
        // 静态字段存储默认样式
        private static IDeclStyle _label;
        private static IDeclStyle _textField;
        private static IDeclStyle _textArea;
        private static IDeclStyle _button;
        private static IDeclStyle _toggle;
        private static IDeclStyle _slider;
        private static IDeclStyle _sliderThumb;
        private static IDeclStyle _horizontalSlider;
        private static IDeclStyle _horizontalSliderThumb;
        private static IDeclStyle _verticalSlider;
        private static IDeclStyle _verticalSliderThumb;
        private static IDeclStyle _scrollbar;
        private static IDeclStyle _scrollbarThumb;
        private static IDeclStyle _scrollbarUpButton;
        private static IDeclStyle _scrollbarDownButton;
        private static IDeclStyle _scrollbarLeftButton;
        private static IDeclStyle _scrollbarRightButton;
        private static IDeclStyle _window;
        private static IDeclStyle _box;
        private static IDeclStyle _helpBox;
        private static IDeclStyle _toolbar;
        private static IDeclStyle _toolbarButton;
        private static IDeclStyle _toolbarDropDown;
        private static IDeclStyle _toolbarTextField;
        private static IDeclStyle _toolbarSearchField;
        private static IDeclStyle _popup;
        
        // 特殊样式
        private static IDeclStyle _whiteLabel;
        private static IDeclStyle _whiteMiniLabel;
        private static IDeclStyle _miniLabel;
        private static IDeclStyle _boldLabel;
        private static IDeclStyle _largeLabel;
        private static IDeclStyle _centeredGreyMiniLabel;
        private static IDeclStyle _wordWrappedLabel;
        private static IDeclStyle _wordWrappedMiniLabel;
        private static IDeclStyle _textView;
        
        // 编辑器专用样式
        private static IDeclStyle _colorField;
        private static IDeclStyle _layerMaskField;
        private static IDeclStyle _enumPopup;
        private static IDeclStyle _objectField;
        private static IDeclStyle _objectFieldThumb;
        private static IDeclStyle _curveField;
        private static IDeclStyle _curveFieldBackground;
        private static IDeclStyle _preLabel;
        private static IDeclStyle _preButton;
        private static IDeclStyle _preToolbar;
        private static IDeclStyle _preSlider;
        private static IDeclStyle _preSliderThumb;
        private static IDeclStyle _preTextArea;
        
        // 布局和容器样式
        private static IDeclStyle _tab;
        private static IDeclStyle _tabGroup;
        private static IDeclStyle _tabContent;
        private static IDeclStyle _panel;
        private static IDeclStyle _scrollGroup;
        private static IDeclStyle _header;
        private static IDeclStyle _subHeader;
        private static IDeclStyle _separator;
        private static IDeclStyle _space;
        
        // 交互元素样式
        private static IDeclStyle _link;
        private static IDeclStyle _progressBar;
        private static IDeclStyle _progressBarBack;
        private static IDeclStyle _selectionRect;
        private static IDeclStyle _badge;
        private static IDeclStyle _notification;
        
        // 图标和图像样式
        private static IDeclStyle _iconButton;
        private static IDeclStyle _imageView;
        private static IDeclStyle _image;
        
        // Markdown样式
        private static IDeclStyle _H1;
        private static IDeclStyle _H2;
        private static IDeclStyle _H3;
        private static IDeclStyle _H4;
        private static IDeclStyle _H5;
        private static IDeclStyle _H6;
        private static IDeclStyle _paragraph;
        private static IDeclStyle _code;
        private static IDeclStyle _quote;

        // 静态字段存储默认属性
        private static DeclPropertyTemplate.TemplateProperty _backgroundColor;
        private static DeclPropertyTemplate.TemplateProperty _surfaceColor;
        private static DeclPropertyTemplate.TemplateProperty _primaryColor;
        private static DeclPropertyTemplate.TemplateProperty _secondaryColor;
        private static DeclPropertyTemplate.TemplateProperty _textColor;
        private static DeclPropertyTemplate.TemplateProperty _textSecondaryColor;
        private static DeclPropertyTemplate.TemplateProperty _borderColor;
        private static DeclPropertyTemplate.TemplateProperty _hoverColor;
        private static DeclPropertyTemplate.TemplateProperty _activeColor;
        private static DeclPropertyTemplate.TemplateProperty _focusColor;
        private static DeclPropertyTemplate.TemplateProperty _disabledColor;
        private static DeclPropertyTemplate.TemplateProperty _successColor;
        private static DeclPropertyTemplate.TemplateProperty _warningColor;
        private static DeclPropertyTemplate.TemplateProperty _errorColor;
        private static DeclPropertyTemplate.TemplateProperty _infoColor;
        
        // 字体大小属性
        private static DeclPropertyTemplate.TemplateProperty _fontSizeSmall;
        private static DeclPropertyTemplate.TemplateProperty _fontSizeNormal;
        private static DeclPropertyTemplate.TemplateProperty _fontSizeLarge;
        private static DeclPropertyTemplate.TemplateProperty _fontSizeXLarge;
        private static DeclPropertyTemplate.TemplateProperty _fontSizeXXLarge;
        private static DeclPropertyTemplate.TemplateProperty _fontSizeXXXLarge;
        
        // 间距属性
        private static DeclPropertyTemplate.TemplateProperty _paddingSmall;
        private static DeclPropertyTemplate.TemplateProperty _paddingNormal;
        private static DeclPropertyTemplate.TemplateProperty _paddingLarge;
        private static DeclPropertyTemplate.TemplateProperty _marginSmall;
        private static DeclPropertyTemplate.TemplateProperty _marginNormal;
        private static DeclPropertyTemplate.TemplateProperty _marginLarge;
        private static DeclPropertyTemplate.TemplateProperty _paddingHeading;
        private static DeclPropertyTemplate.TemplateProperty _marginHeading;
        private static DeclPropertyTemplate.TemplateProperty _paddingParagraph;
        private static DeclPropertyTemplate.TemplateProperty _marginParagraph;
        private static DeclPropertyTemplate.TemplateProperty _marginQuote;
        
        // 边框属性
        private static DeclPropertyTemplate.TemplateProperty _borderWidthNormal;
        private static DeclPropertyTemplate.TemplateProperty _borderWidthThick;
        private static DeclPropertyTemplate.TemplateProperty _borderRadiusSmall;
        private static DeclPropertyTemplate.TemplateProperty _borderRadiusNormal;
        private static DeclPropertyTemplate.TemplateProperty _borderRadiusLarge;
        
        // 控件高度属性
        private static DeclPropertyTemplate.TemplateProperty _controlHeightSmall;
        private static DeclPropertyTemplate.TemplateProperty _controlHeightNormal;
        private static DeclPropertyTemplate.TemplateProperty _controlHeightLarge;
        
        // Markdown专用属性
        private static DeclPropertyTemplate.TemplateProperty _linkColor;
        private static DeclPropertyTemplate.TemplateProperty _linkHoverColor;
        
        // 按钮专用内边距属性
        private static DeclPropertyTemplate.TemplateProperty _buttonPadding;
        private static DeclPropertyTemplate.TemplateProperty _smallButtonPadding;
        private static DeclPropertyTemplate.TemplateProperty _largeButtonPadding;
        
        // 面板专用内边距属性
        private static DeclPropertyTemplate.TemplateProperty _panelPadding;
        
        // 输入框专用内边距属性
        private static DeclPropertyTemplate.TemplateProperty _textFieldPadding;
        private static DeclPropertyTemplate.TemplateProperty _subtleTextFieldPadding;
        
        // 标签专用内边距属性
        private static DeclPropertyTemplate.TemplateProperty _labelPadding;

        // 静态构造函数初始化所有默认值
        static DefaultThemeDefine()
        {
            InitializeDefaultValues();
        }

        private static void InitializeDefaultValues()
        {
            // 初始化默认样式
            InitializeDefaultStyles();
            // 初始化默认属性
            InitializeDefaultProperties();
        }

        private static void InitializeDefaultStyles()
        {
            // 基础控件样式 - 基于DeclThemeCreator中的设计
            _label = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetPadding(StyleConfig.PaddingSmall)
                .SetAlignment(TextAnchor.MiddleLeft);

            _textField = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.TextFieldPadding)
                .SetHeight(StyleConfig.ControlHeightNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal);

            _textArea = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.TextFieldPadding)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal);

            _button = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetPadding(StyleConfig.ButtonPadding)
                .SetHeight(StyleConfig.ControlHeightNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetAlignment(TextAnchor.MiddleCenter);

            _toggle = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.PaddingSmall)
                .SetHeight(StyleConfig.ControlHeightSmall)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeSmall);

            _slider = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetHeight(StyleConfig.ControlHeightSmall);

            _sliderThumb = new DeclStyle()
                .SetBackgroundColor(StyleConfig.PrimaryColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetWidth(16f)
                .SetHeight(20f);

            _horizontalSlider = _slider;
            _horizontalSliderThumb = _sliderThumb;
            _verticalSlider = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetWidth(StyleConfig.ControlHeightSmall);
            _verticalSliderThumb = new DeclStyle()
                .SetBackgroundColor(StyleConfig.PrimaryColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetWidth(20f)
                .SetHeight(16f);

            _scrollbar = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall);
            _scrollbarThumb = new DeclStyle()
                .SetBackgroundColor(StyleConfig.HoverColor)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal);
            _scrollbarUpButton = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetHeight(StyleConfig.ControlHeightSmall);
            _scrollbarDownButton = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetHeight(StyleConfig.ControlHeightSmall);
            _scrollbarLeftButton = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetWidth(StyleConfig.ControlHeightSmall);
            _scrollbarRightButton = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetWidth(StyleConfig.ControlHeightSmall);

            _window = new DeclStyle()
                .SetBackgroundColor(StyleConfig.BackgroundColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetPadding(StyleConfig.PaddingNormal);

            _box = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetPadding(StyleConfig.PaddingNormal);

            _helpBox = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.PaddingLarge);

            _toolbar = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetHeight(StyleConfig.ControlHeightLarge);

            _toolbarButton = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.ButtonPadding)
                .SetHeight(StyleConfig.ControlHeightNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetAlignment(TextAnchor.MiddleCenter);

            _toolbarDropDown = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.ButtonPadding)
                .SetHeight(StyleConfig.ControlHeightNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal);

            _toolbarTextField = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.TextFieldPadding)
                .SetHeight(StyleConfig.ControlHeightNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal);

            _toolbarSearchField = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusLarge)
                .SetPadding(new RectOffset(8, 24, 3, 3)) // 留空间给搜索图标
                .SetHeight(StyleConfig.ControlHeightSmall)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeSmall);

            _popup = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.ButtonPadding)
                .SetHeight(StyleConfig.ControlHeightNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal);

            // 特殊样式
            _whiteLabel = new DeclStyle()
                .SetColor(Color.white)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetAlignment(TextAnchor.MiddleLeft);

            _whiteMiniLabel = new DeclStyle()
                .SetColor(Color.white)
                .SetFontSize(StyleConfig.FontSizeSmall)
                .SetAlignment(TextAnchor.MiddleLeft);

            _miniLabel = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeSmall)
                .SetPadding(StyleConfig.LabelPadding)
                .SetAlignment(TextAnchor.MiddleLeft);

            _boldLabel = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeLarge)
                .SetFontStyle(FontStyle.Bold)
                .SetAlignment(TextAnchor.MiddleLeft);

            _largeLabel = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeLarge)
                .SetAlignment(TextAnchor.MiddleLeft);

            _centeredGreyMiniLabel = new DeclStyle()
                .SetColor(StyleConfig.TextSecondaryColor)
                .SetFontSize(StyleConfig.FontSizeSmall)
                .SetAlignment(TextAnchor.MiddleCenter);

            _wordWrappedLabel = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetAlignment(TextAnchor.MiddleLeft);

            _wordWrappedMiniLabel = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeSmall)
                .SetAlignment(TextAnchor.MiddleLeft);

            _textView = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetPadding(StyleConfig.PaddingSmall)
                .SetAlignment(TextAnchor.UpperLeft);

            // 编辑器专用样式
            _colorField = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetWidth(60f)
                .SetHeight(StyleConfig.ControlHeightNormal);

            _layerMaskField = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetPadding(StyleConfig.ButtonPadding)
                .SetHeight(StyleConfig.ControlHeightNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal);

            _enumPopup = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.ButtonPadding)
                .SetHeight(StyleConfig.ControlHeightNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal);

            _objectField = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.ButtonPadding)
                .SetHeight(StyleConfig.ControlHeightNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal);

            _objectFieldThumb = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetWidth(64f)
                .SetHeight(32f);

            _curveField = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetWidth(60f)
                .SetHeight(16f);

            _curveFieldBackground = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal);

            _preLabel = new DeclStyle()
                .SetColor(StyleConfig.TextSecondaryColor)
                .SetFontSize(StyleConfig.FontSizeSmall)
                .SetAlignment(TextAnchor.MiddleLeft);

            _preButton = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.ButtonPadding)
                .SetHeight(StyleConfig.ControlHeightNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal);

            _preToolbar = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetHeight(StyleConfig.ControlHeightLarge);

            _preSlider = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetHeight(StyleConfig.ControlHeightSmall);

            _preSliderThumb = new DeclStyle()
                .SetBackgroundColor(StyleConfig.PrimaryColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetWidth(16f)
                .SetHeight(20f);

            _preTextArea = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.PaddingNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal);

            // 布局和容器样式
            _tab = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetPadding(StyleConfig.ButtonPadding)
                .SetHeight(StyleConfig.ControlHeightNormal)
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetAlignment(TextAnchor.MiddleCenter);

            _tabGroup = new DeclStyle()
                .SetBackgroundColor(StyleConfig.BackgroundColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetPadding(StyleConfig.PaddingNormal);

            _tabContent = new DeclStyle()
                .SetBackgroundColor(StyleConfig.BackgroundColor)
                .SetPadding(StyleConfig.PaddingNormal);

            _panel = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetPadding(StyleConfig.PanelPadding);

            _scrollGroup = new DeclStyle()
                .SetBackgroundColor(StyleConfig.BackgroundColor);

            _header = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeLarge)
                .SetFontStyle(FontStyle.Bold)
                .SetPadding(StyleConfig.PaddingNormal)
                .SetHeight(StyleConfig.ControlHeightLarge);

            _subHeader = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetFontStyle(FontStyle.Bold)
                .SetPadding(StyleConfig.PaddingSmall)
                .SetHeight(StyleConfig.ControlHeightNormal);

            _separator = new DeclStyle()
                .SetBackgroundColor(StyleConfig.BorderColor)
                .SetHeight(1f);

            _space = new DeclStyle()
                .SetHeight(StyleConfig.MarginNormal.vertical);

            // 交互元素样式
            _link = new DeclStyle()
                .SetColor(StyleConfig.PrimaryColor)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetFontStyle(FontStyle.Normal);

            _progressBar = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetHeight(StyleConfig.ControlHeightSmall);

            _progressBarBack = new DeclStyle()
                .SetBackgroundColor(StyleConfig.HoverColor)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal);

            _selectionRect = new DeclStyle()
                .SetBackgroundColor(new Color(0.0f, 0.5f, 1.0f, 0.2f))
                .SetBorderColor(new Color(0.0f, 0.5f, 1.0f, 0.8f))
                .SetBorderWidth(1f);

            _badge = new DeclStyle()
                .SetBackgroundColor(StyleConfig.ErrorColor)
                .SetColor(Color.white)
                .SetFontSize(StyleConfig.FontSizeSmall)
                .SetAlignment(TextAnchor.MiddleCenter)
                .SetWidth(16f)
                .SetHeight(16f)
                .SetBorderRadius(8f);

            _notification = new DeclStyle()
                .SetBackgroundColor(StyleConfig.WarningColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetPadding(StyleConfig.PaddingNormal);

            // 图标和图像样式
            _iconButton = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusNormal)
                .SetPadding(StyleConfig.SmallButtonPadding)
                .SetWidth(StyleConfig.ControlHeightSmall)
                .SetHeight(StyleConfig.ControlHeightSmall)
                .SetColor(StyleConfig.TextColor)
                .SetAlignment(TextAnchor.MiddleCenter);

            _imageView = new DeclStyle()
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall);

            _image = new DeclStyle()
                .SetBackgroundColor(Color.clear);

            // Markdown样式
            _H1 = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeXXXLarge)
                .SetFontStyle(FontStyle.Bold)
                .SetPadding(new RectOffset(0, 0, 4, 4))
                .SetMargin(new RectOffset(0, 0, 8, 4))
                .SetAlignment(TextAnchor.UpperLeft);

            _H2 = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeXXLarge)
                .SetFontStyle(FontStyle.Bold)
                .SetPadding(new RectOffset(0, 0, 4, 4))
                .SetMargin(new RectOffset(0, 0, 8, 4))
                .SetAlignment(TextAnchor.UpperLeft);

            _H3 = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeXLarge)
                .SetFontStyle(FontStyle.Bold)
                .SetPadding(new RectOffset(0, 0, 4, 4))
                .SetMargin(new RectOffset(0, 0, 8, 4))
                .SetAlignment(TextAnchor.UpperLeft);

            _H4 = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeLarge)
                .SetFontStyle(FontStyle.Bold)
                .SetPadding(new RectOffset(0, 0, 4, 4))
                .SetMargin(new RectOffset(0, 0, 8, 4))
                .SetAlignment(TextAnchor.UpperLeft);

            _H5 = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetFontStyle(FontStyle.Bold)
                .SetPadding(new RectOffset(0, 0, 4, 4))
                .SetMargin(new RectOffset(0, 0, 8, 4))
                .SetAlignment(TextAnchor.UpperLeft);

            _H6 = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeSmall)
                .SetFontStyle(FontStyle.Bold)
                .SetPadding(new RectOffset(0, 0, 4, 4))
                .SetMargin(new RectOffset(0, 0, 8, 4))
                .SetAlignment(TextAnchor.UpperLeft);

            _paragraph = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetPadding(new RectOffset(0, 0, 2, 2))
                .SetMargin(new RectOffset(0, 0, 4, 4))
                .SetAlignment(TextAnchor.UpperLeft);

            _code = new DeclStyle()
                .SetColor(StyleConfig.TextColor)
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.PaddingSmall)
                .SetFontSize(StyleConfig.FontSizeSmall)
                .SetFontStyle(FontStyle.Normal)
                .SetAlignment(TextAnchor.UpperLeft);

            _quote = new DeclStyle()
                .SetColor(StyleConfig.TextSecondaryColor)
                .SetBackgroundColor(StyleConfig.SurfaceColor)
                .SetBorderColor(StyleConfig.BorderColor)
                .SetBorderWidth(StyleConfig.BorderWidthNormal)
                .SetBorderRadius(StyleConfig.BorderRadiusSmall)
                .SetPadding(StyleConfig.PaddingNormal)
                .SetFontSize(StyleConfig.FontSizeNormal)
                .SetFontStyle(FontStyle.Italic)
                .SetAlignment(TextAnchor.UpperLeft)
                .SetMargin(new RectOffset(8, 0, 4, 4));
        }

        private static void InitializeDefaultProperties()
        {
            // 颜色类核心属性 - 基于DeclThemeCreator中的颜色配置
            _backgroundColor = CreateTemplateProperty("backgroundColor", PropertyType.Color, StyleConfig.BackgroundColor, "基础背景颜色");
            _surfaceColor = CreateTemplateProperty("surfaceColor", PropertyType.Color, StyleConfig.SurfaceColor, "表面颜色");
            _primaryColor = CreateTemplateProperty("primaryColor", PropertyType.Color, StyleConfig.PrimaryColor, "主色调");
            _secondaryColor = CreateTemplateProperty("secondaryColor", PropertyType.Color, StyleConfig.SecondaryColor, "次要色调");
            _textColor = CreateTemplateProperty("textColor", PropertyType.Color, StyleConfig.TextColor, "基础文本颜色");
            _textSecondaryColor = CreateTemplateProperty("textSecondaryColor", PropertyType.Color, StyleConfig.TextSecondaryColor, "次要文本颜色");
            _borderColor = CreateTemplateProperty("borderColor", PropertyType.Color, StyleConfig.BorderColor, "基础边框颜色");
            _hoverColor = CreateTemplateProperty("hoverColor", PropertyType.Color, StyleConfig.HoverColor, "鼠标悬停状态的颜色");
            _activeColor = CreateTemplateProperty("activeColor", PropertyType.Color, StyleConfig.ActiveColor, "激活/按下状态的颜色");
            _focusColor = CreateTemplateProperty("focusColor", PropertyType.Color, StyleConfig.FocusColor, "获得焦点状态的颜色");
            _disabledColor = CreateTemplateProperty("disabledColor", PropertyType.Color, StyleConfig.DisabledColor, "禁用状态的颜色");
            _successColor = CreateTemplateProperty("successColor", PropertyType.Color, StyleConfig.SuccessColor, "成功状态颜色");
            _warningColor = CreateTemplateProperty("warningColor", PropertyType.Color, StyleConfig.WarningColor, "警告状态颜色");
            _errorColor = CreateTemplateProperty("errorColor", PropertyType.Color, StyleConfig.ErrorColor, "错误状态颜色");
            _infoColor = CreateTemplateProperty("infoColor", PropertyType.Color, StyleConfig.InfoColor, "信息状态颜色");
            
            // 字体大小属性
            _fontSizeSmall = CreateTemplateProperty("fontSizeSmall", PropertyType.Int, StyleConfig.FontSizeSmall, "小号字体大小");
            _fontSizeNormal = CreateTemplateProperty("fontSizeNormal", PropertyType.Int, StyleConfig.FontSizeNormal, "标准字体大小");
            _fontSizeLarge = CreateTemplateProperty("fontSizeLarge", PropertyType.Int, StyleConfig.FontSizeLarge, "大号字体大小");
            _fontSizeXLarge = CreateTemplateProperty("fontSizeXLarge", PropertyType.Int, StyleConfig.FontSizeXLarge, "超大号字体大小");
            _fontSizeXXLarge = CreateTemplateProperty("fontSizeXXLarge", PropertyType.Int, StyleConfig.FontSizeXXLarge, "特大号字体大小");
            _fontSizeXXXLarge = CreateTemplateProperty("fontSizeXXXLarge", PropertyType.Int, StyleConfig.FontSizeXXXLarge, "最大号字体大小");
            
            // 间距属性
            _paddingSmall = CreateTemplateProperty("paddingSmall", PropertyType.RectOffset, StyleConfig.PaddingSmall, "小内边距");
            _paddingNormal = CreateTemplateProperty("paddingNormal", PropertyType.RectOffset, StyleConfig.PaddingNormal, "标准内边距");
            _paddingLarge = CreateTemplateProperty("paddingLarge", PropertyType.RectOffset, StyleConfig.PaddingLarge, "大内边距");
            _marginSmall = CreateTemplateProperty("marginSmall", PropertyType.RectOffset, StyleConfig.MarginSmall, "小外边距");
            _marginNormal = CreateTemplateProperty("marginNormal", PropertyType.RectOffset, StyleConfig.MarginNormal, "标准外边距");
            _marginLarge = CreateTemplateProperty("marginLarge", PropertyType.RectOffset, StyleConfig.MarginLarge, "大外边距");
            _paddingHeading = CreateTemplateProperty("paddingHeading", PropertyType.RectOffset, new RectOffset(0, 0, 4, 4), "标题内边距");
            _marginHeading = CreateTemplateProperty("marginHeading", PropertyType.RectOffset, new RectOffset(0, 0, 8, 4), "标题外边距");
            _paddingParagraph = CreateTemplateProperty("paddingParagraph", PropertyType.RectOffset, new RectOffset(0, 0, 2, 2), "段落内边距");
            _marginParagraph = CreateTemplateProperty("marginParagraph", PropertyType.RectOffset, new RectOffset(0, 0, 4, 4), "段落外边距");
            _marginQuote = CreateTemplateProperty("marginQuote", PropertyType.RectOffset, new RectOffset(8, 0, 4, 4), "引用外边距");
            
            // 边框属性
            _borderWidthNormal = CreateTemplateProperty("borderWidthNormal", PropertyType.Float, StyleConfig.BorderWidthNormal, "标准边框宽度");
            _borderWidthThick = CreateTemplateProperty("borderWidthThick", PropertyType.Float, StyleConfig.BorderWidthThick, "粗边框宽度");
            _borderRadiusSmall = CreateTemplateProperty("borderRadiusSmall", PropertyType.Float, StyleConfig.BorderRadiusSmall, "小圆角半径");
            _borderRadiusNormal = CreateTemplateProperty("borderRadiusNormal", PropertyType.Float, StyleConfig.BorderRadiusNormal, "标准圆角半径");
            _borderRadiusLarge = CreateTemplateProperty("borderRadiusLarge", PropertyType.Float, StyleConfig.BorderRadiusLarge, "大圆角半径");
            
            // 控件高度属性
            _controlHeightSmall = CreateTemplateProperty("controlHeightSmall", PropertyType.Float, StyleConfig.ControlHeightSmall, "小控件高度");
            _controlHeightNormal = CreateTemplateProperty("controlHeightNormal", PropertyType.Float, StyleConfig.ControlHeightNormal, "标准控件高度");
            _controlHeightLarge = CreateTemplateProperty("controlHeightLarge", PropertyType.Float, StyleConfig.ControlHeightLarge, "大控件高度");
            
            // Markdown专用属性
            _linkColor = CreateTemplateProperty("linkColor", PropertyType.Color, StyleConfig.LinkColor, "链接颜色");
            _linkHoverColor = CreateTemplateProperty("linkHoverColor", PropertyType.Color, StyleConfig.LinkHoverColor, "链接悬停颜色");
            
            // 按钮专用内边距属性
            _buttonPadding = CreateTemplateProperty("buttonPadding", PropertyType.RectOffset, StyleConfig.ButtonPadding, "按钮内边距");
            _smallButtonPadding = CreateTemplateProperty("smallButtonPadding", PropertyType.RectOffset, StyleConfig.SmallButtonPadding, "小按钮内边距");
            _largeButtonPadding = CreateTemplateProperty("largeButtonPadding", PropertyType.RectOffset, StyleConfig.LargeButtonPadding, "大按钮内边距");
            
            // 面板专用内边距属性
            _panelPadding = CreateTemplateProperty("panelPadding", PropertyType.RectOffset, StyleConfig.PanelPadding, "面板内边距");
            
            // 输入框专用内边距属性
            _textFieldPadding = CreateTemplateProperty("textFieldPadding", PropertyType.RectOffset, StyleConfig.TextFieldPadding, "文本输入框内边距");
            _subtleTextFieldPadding = CreateTemplateProperty("subtleTextFieldPadding", PropertyType.RectOffset, StyleConfig.SubtleTextFieldPadding, "柔和文本输入框内边距");
            
            // 标签专用内边距属性
            _labelPadding = CreateTemplateProperty("labelPadding", PropertyType.RectOffset, StyleConfig.LabelPadding, "标签内边距");
        }

        private static DeclPropertyTemplate.TemplateProperty CreateTemplateProperty(string name, PropertyType type, object defaultValue, string description)
        {
            var prop = new DeclPropertyTemplate.TemplateProperty
            {
                Name = name,
                Type = type,
                Description = description
            };

            switch (type)
            {
                case PropertyType.Float:
                    prop.FloatValue = (float)defaultValue;
                    break;
                case PropertyType.Int:
                    prop.IntValue = (int)defaultValue;
                    break;
                case PropertyType.Color:
                    prop.ColorValue = (Color)defaultValue;
                    break;
                case PropertyType.String:
                    prop.StringValue = (string)defaultValue;
                    break;
                case PropertyType.Boolean:
                    prop.BoolValue = (bool)defaultValue;
                    break;
                case PropertyType.Vector2:
                    prop.Vector2Value = (Vector2)defaultValue;
                    break;
                case PropertyType.Vector3:
                    prop.Vector3Value = (Vector3)defaultValue;
                    break;
                case PropertyType.RectOffset:
                    prop.RectOffsetValue = (RectOffset)defaultValue;
                    break;
            }

            return prop;
        }

        // 静态方法提供默认样式和属性
        public static IDeclStyle GetLabelStyle() => _label;
        public static IDeclStyle GetTextFieldStyle() => _textField;
        public static IDeclStyle GetTextAreaStyle() => _textArea;
        public static IDeclStyle GetButtonStyle() => _button;
        public static IDeclStyle GetToggleStyle() => _toggle;
        public static IDeclStyle GetSliderStyle() => _slider;
        public static IDeclStyle GetSliderThumbStyle() => _sliderThumb;
        public static IDeclStyle GetHorizontalSliderStyle() => _horizontalSlider;
        public static IDeclStyle GetHorizontalSliderThumbStyle() => _horizontalSliderThumb;
        public static IDeclStyle GetVerticalSliderStyle() => _verticalSlider;
        public static IDeclStyle GetVerticalSliderThumbStyle() => _verticalSliderThumb;
        public static IDeclStyle GetScrollbarStyle() => _scrollbar;
        public static IDeclStyle GetScrollbarThumbStyle() => _scrollbarThumb;
        public static IDeclStyle GetScrollbarUpButtonStyle() => _scrollbarUpButton;
        public static IDeclStyle GetScrollbarDownButtonStyle() => _scrollbarDownButton;
        public static IDeclStyle GetScrollbarLeftButtonStyle() => _scrollbarLeftButton;
        public static IDeclStyle GetScrollbarRightButtonStyle() => _scrollbarRightButton;
        public static IDeclStyle GetWindowStyle() => _window;
        public static IDeclStyle GetBoxStyle() => _box;
        public static IDeclStyle GetHelpBoxStyle() => _helpBox;
        public static IDeclStyle GetToolbarStyle() => _toolbar;
        public static IDeclStyle GetToolbarButtonStyle() => _toolbarButton;
        public static IDeclStyle GetToolbarDropDownStyle() => _toolbarDropDown;
        public static IDeclStyle GetToolbarTextFieldStyle() => _toolbarTextField;
        public static IDeclStyle GetToolbarSearchFieldStyle() => _toolbarSearchField;
        public static IDeclStyle GetPopupStyle() => _popup;
        
        // 特殊样式
        public static IDeclStyle GetWhiteLabelStyle() => _whiteLabel;
        public static IDeclStyle GetWhiteMiniLabelStyle() => _whiteMiniLabel;
        public static IDeclStyle GetMiniLabelStyle() => _miniLabel;
        public static IDeclStyle GetBoldLabelStyle() => _boldLabel;
        public static IDeclStyle GetLargeLabelStyle() => _largeLabel;
        public static IDeclStyle GetCenteredGreyMiniLabelStyle() => _centeredGreyMiniLabel;
        public static IDeclStyle GetWordWrappedLabelStyle() => _wordWrappedLabel;
        public static IDeclStyle GetWordWrappedMiniLabelStyle() => _wordWrappedMiniLabel;
        public static IDeclStyle GetTextViewStyle() => _textView;
        
        // 编辑器专用样式
        public static IDeclStyle GetColorFieldStyle() => _colorField;
        public static IDeclStyle GetLayerMaskFieldStyle() => _layerMaskField;
        public static IDeclStyle GetEnumPopupStyle() => _enumPopup;
        public static IDeclStyle GetObjectFieldStyle() => _objectField;
        public static IDeclStyle GetObjectFieldThumbStyle() => _objectFieldThumb;
        public static IDeclStyle GetCurveFieldStyle() => _curveField;
        public static IDeclStyle GetCurveFieldBackgroundStyle() => _curveFieldBackground;
        public static IDeclStyle GetPreLabelStyle() => _preLabel;
        public static IDeclStyle GetPreButtonStyle() => _preButton;
        public static IDeclStyle GetPreToolbarStyle() => _preToolbar;
        public static IDeclStyle GetPreSliderStyle() => _preSlider;
        public static IDeclStyle GetPreSliderThumbStyle() => _preSliderThumb;
        public static IDeclStyle GetPreTextAreaStyle() => _preTextArea;
        
        // 布局和容器样式
        public static IDeclStyle GetTabStyle() => _tab;
        public static IDeclStyle GetTabGroupStyle() => _tabGroup;
        public static IDeclStyle GetTabContentStyle() => _tabContent;
        public static IDeclStyle GetPanelStyle() => _panel;
        public static IDeclStyle GetScrollGroupStyle() => _scrollGroup;
        public static IDeclStyle GetHeaderStyle() => _header;
        public static IDeclStyle GetSubHeaderStyle() => _subHeader;
        public static IDeclStyle GetSeparatorStyle() => _separator;
        public static IDeclStyle GetSpaceStyle() => _space;
        
        // 交互元素样式
        public static IDeclStyle GetLinkStyle() => _link;
        public static IDeclStyle GetProgressBarStyle() => _progressBar;
        public static IDeclStyle GetProgressBarBackStyle() => _progressBarBack;
        public static IDeclStyle GetSelectionRectStyle() => _selectionRect;
        public static IDeclStyle GetBadgeStyle() => _badge;
        public static IDeclStyle GetNotificationStyle() => _notification;
        
        // 图标和图像样式
        public static IDeclStyle GetIconButtonStyle() => _iconButton;
        public static IDeclStyle GetImageViewStyle() => _imageView;
        public static IDeclStyle GetImageStyle() => _image;
        
        // Markdown样式
        public static IDeclStyle GetH1Style() => _H1;
        public static IDeclStyle GetH2Style() => _H2;
        public static IDeclStyle GetH3Style() => _H3;
        public static IDeclStyle GetH4Style() => _H4;
        public static IDeclStyle GetH5Style() => _H5;
        public static IDeclStyle GetH6Style() => _H6;
        public static IDeclStyle GetParagraphStyle() => _paragraph;
        public static IDeclStyle GetCodeStyle() => _code;
        public static IDeclStyle GetQuoteStyle() => _quote;

        // 静态方法提供默认属性
        public static DeclPropertyTemplate.TemplateProperty GetBackgroundColorProperty() => _backgroundColor;
        public static DeclPropertyTemplate.TemplateProperty GetSurfaceColorProperty() => _surfaceColor;
        public static DeclPropertyTemplate.TemplateProperty GetPrimaryColorProperty() => _primaryColor;
        public static DeclPropertyTemplate.TemplateProperty GetSecondaryColorProperty() => _secondaryColor;
        public static DeclPropertyTemplate.TemplateProperty GetTextColorProperty() => _textColor;
        public static DeclPropertyTemplate.TemplateProperty GetTextSecondaryColorProperty() => _textSecondaryColor;
        public static DeclPropertyTemplate.TemplateProperty GetBorderColorProperty() => _borderColor;
        public static DeclPropertyTemplate.TemplateProperty GetHoverColorProperty() => _hoverColor;
        public static DeclPropertyTemplate.TemplateProperty GetActiveColorProperty() => _activeColor;
        public static DeclPropertyTemplate.TemplateProperty GetFocusColorProperty() => _focusColor;
        public static DeclPropertyTemplate.TemplateProperty GetDisabledColorProperty() => _disabledColor;
        public static DeclPropertyTemplate.TemplateProperty GetSuccessColorProperty() => _successColor;
        public static DeclPropertyTemplate.TemplateProperty GetWarningColorProperty() => _warningColor;
        public static DeclPropertyTemplate.TemplateProperty GetErrorColorProperty() => _errorColor;
        public static DeclPropertyTemplate.TemplateProperty GetInfoColorProperty() => _infoColor;
        
        // 字体大小属性
        public static DeclPropertyTemplate.TemplateProperty GetFontSizeSmallProperty() => _fontSizeSmall;
        public static DeclPropertyTemplate.TemplateProperty GetFontSizeNormalProperty() => _fontSizeNormal;
        public static DeclPropertyTemplate.TemplateProperty GetFontSizeLargeProperty() => _fontSizeLarge;
        public static DeclPropertyTemplate.TemplateProperty GetFontSizeXLargeProperty() => _fontSizeXLarge;
        public static DeclPropertyTemplate.TemplateProperty GetFontSizeXXLargeProperty() => _fontSizeXXLarge;
        public static DeclPropertyTemplate.TemplateProperty GetFontSizeXXXLargeProperty() => _fontSizeXXXLarge;
        
        // 间距属性
        public static DeclPropertyTemplate.TemplateProperty GetPaddingSmallProperty() => _paddingSmall;
        public static DeclPropertyTemplate.TemplateProperty GetPaddingNormalProperty() => _paddingNormal;
        public static DeclPropertyTemplate.TemplateProperty GetPaddingLargeProperty() => _paddingLarge;
        public static DeclPropertyTemplate.TemplateProperty GetMarginSmallProperty() => _marginSmall;
        public static DeclPropertyTemplate.TemplateProperty GetMarginNormalProperty() => _marginNormal;
        public static DeclPropertyTemplate.TemplateProperty GetMarginLargeProperty() => _marginLarge;
        public static DeclPropertyTemplate.TemplateProperty GetPaddingHeadingProperty() => _paddingHeading;
        public static DeclPropertyTemplate.TemplateProperty GetMarginHeadingProperty() => _marginHeading;
        public static DeclPropertyTemplate.TemplateProperty GetPaddingParagraphProperty() => _paddingParagraph;
        public static DeclPropertyTemplate.TemplateProperty GetMarginParagraphProperty() => _marginParagraph;
        public static DeclPropertyTemplate.TemplateProperty GetMarginQuoteProperty() => _marginQuote;
        
        // 边框属性
        public static DeclPropertyTemplate.TemplateProperty GetBorderWidthNormalProperty() => _borderWidthNormal;
        public static DeclPropertyTemplate.TemplateProperty GetBorderWidthThickProperty() => _borderWidthThick;
        public static DeclPropertyTemplate.TemplateProperty GetBorderRadiusSmallProperty() => _borderRadiusSmall;
        public static DeclPropertyTemplate.TemplateProperty GetBorderRadiusNormalProperty() => _borderRadiusNormal;
        public static DeclPropertyTemplate.TemplateProperty GetBorderRadiusLargeProperty() => _borderRadiusLarge;
        
        // 控件高度属性
        public static DeclPropertyTemplate.TemplateProperty GetControlHeightSmallProperty() => _controlHeightSmall;
        public static DeclPropertyTemplate.TemplateProperty GetControlHeightNormalProperty() => _controlHeightNormal;
        public static DeclPropertyTemplate.TemplateProperty GetControlHeightLargeProperty() => _controlHeightLarge;
        
        // Markdown专用属性
        public static DeclPropertyTemplate.TemplateProperty GetLinkColorProperty() => _linkColor;
        public static DeclPropertyTemplate.TemplateProperty GetLinkHoverColorProperty() => _linkHoverColor;
        
        // 按钮专用内边距属性
        public static DeclPropertyTemplate.TemplateProperty GetButtonPaddingProperty() => _buttonPadding;
        public static DeclPropertyTemplate.TemplateProperty GetSmallButtonPaddingProperty() => _smallButtonPadding;
        public static DeclPropertyTemplate.TemplateProperty GetLargeButtonPaddingProperty() => _largeButtonPadding;
        
        // 面板专用内边距属性
        public static DeclPropertyTemplate.TemplateProperty GetPanelPaddingProperty() => _panelPadding;
        
        // 输入框专用内边距属性
        public static DeclPropertyTemplate.TemplateProperty GetTextFieldPaddingProperty() => _textFieldPadding;
        public static DeclPropertyTemplate.TemplateProperty GetSubtleTextFieldPaddingProperty() => _subtleTextFieldPadding;
        
        // 标签专用内边距属性
        public static DeclPropertyTemplate.TemplateProperty GetLabelPaddingProperty() => _labelPadding;
    }
}