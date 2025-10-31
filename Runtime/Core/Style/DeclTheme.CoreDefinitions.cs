using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// DeclTheme 的核心定义扩展
    /// 实现 ICoreThemeDefinition 和 ICorePropertyDefinition 接口
    /// </summary>
    public partial class DeclTheme : ICoreThemeDefinition, ICorePropertyDefinition
    {
        // ICoreThemeDefinition 接口实现 - 核心样式属性
        public IDeclStyle label
        {
            get => GetOrCreateStyle("label", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeNormal"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft)
            });
            set => RegisterStyleSet("label", value);
        }
        public IDeclStyle textField
        {
            get => GetOrCreateStyle("textField", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("textFieldPadding");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightNormal");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeNormal");
                return styleSet;
            });
            set => RegisterStyleSet("textField", value);
        }
        public IDeclStyle textArea
        {
            get => GetOrCreateStyle("textArea", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("textFieldPadding");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeNormal");
                styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Focus, new DeclStyle()
                {
                    BorderColor = StyleProperty<Color>.Ref("focusColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("textArea", value);
        }
        public IDeclStyle button
        {
            get => GetOrCreateStyle("button", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusNormal");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("buttonPadding");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightNormal");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeNormal");
                styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Focus, new DeclStyle()
                {
                    BorderColor = StyleProperty<Color>.Ref("focusColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Disabled, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("disabledColor"),
                    Color = StyleProperty<Color>.Ref("textSecondaryColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("button", value);
        }
        public IDeclStyle toggle
        {
            get => GetOrCreateStyle("toggle", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightSmall");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeSmall");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("toggle", value);
        }
        public IDeclStyle slider
        {
            get => GetOrCreateStyle("slider", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightSmall");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("slider", value);
        }
        public IDeclStyle sliderThumb
        {
            get => GetOrCreateStyle("sliderThumb", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("primaryColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusNormal");
                styleSet.Width = StyleProperty<float>.Direct(16f);
                styleSet.Height = StyleProperty<float>.Direct(16f);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("sliderThumb", value);
        }
        public IDeclStyle horizontalSlider
        {
            get => GetOrCreateStyle("horizontalSlider", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightSmall");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("horizontalSlider", value);
        }
        public IDeclStyle horizontalSliderThumb
        {
            get => GetOrCreateStyle("horizontalSliderThumb", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("primaryColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusNormal");
                styleSet.Width = StyleProperty<float>.Direct(16f);
                styleSet.Height = StyleProperty<float>.Direct(16f);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("horizontalSliderThumb", value);
        }
        public IDeclStyle verticalSlider
        {
            get => GetOrCreateStyle("verticalSlider", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Width = StyleProperty<float>.Ref("controlHeightSmall");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("verticalSlider", value);
        }
        public IDeclStyle verticalSliderThumb
        {
            get => GetOrCreateStyle("verticalSliderThumb", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("primaryColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusNormal");
                styleSet.Width = StyleProperty<float>.Direct(16f);
                styleSet.Height = StyleProperty<float>.Direct(16f);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("verticalSliderThumb", value);
        }
        public IDeclStyle scrollbar
        {
            get => GetOrCreateStyle("scrollbar", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Width = StyleProperty<float>.Direct(16f);
                
                return styleSet;
            });
            set => RegisterStyleSet("scrollbar", value);
        }
        public IDeclStyle scrollbarThumb
        {
            get => GetOrCreateStyle("scrollbarThumb", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("primaryColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("scrollbarThumb", value);
        }
        public IDeclStyle scrollbarUpButton
        {
            get => GetOrCreateStyle("scrollbarUpButton", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Height = StyleProperty<float>.Direct(16f);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("scrollbarUpButton", value);
        }
        public IDeclStyle scrollbarDownButton
        {
            get => GetOrCreateStyle("scrollbarDownButton", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Height = StyleProperty<float>.Direct(16f);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("scrollbarDownButton", value);
        }
        public IDeclStyle scrollbarLeftButton
        {
            get => GetOrCreateStyle("scrollbarLeftButton", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Width = StyleProperty<float>.Direct(16f);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("scrollbarLeftButton", value);
        }
        public IDeclStyle scrollbarRightButton
        {
            get => GetOrCreateStyle("scrollbarRightButton", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Width = StyleProperty<float>.Direct(16f);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("scrollbarRightButton", value);
        }
        public IDeclStyle window
        {
            get => GetOrCreateStyle("window", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusNormal");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingNormal");
                
                return styleSet;
            });
            set => RegisterStyleSet("window", value);
        }
        public IDeclStyle box
        {
            get => GetOrCreateStyle("box", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                
                return styleSet;
            });
            set => RegisterStyleSet("box", value);
        }
        public IDeclStyle helpBox
        {
            get => GetOrCreateStyle("helpBox", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("infoColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingNormal");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeNormal");
                
                return styleSet;
            });
            set => RegisterStyleSet("helpBox", value);
        }
        public IDeclStyle toolbar
        {
            get => GetOrCreateStyle("toolbar", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightNormal");
                
                return styleSet;
            });
            set => RegisterStyleSet("toolbar", value);
        }
        public IDeclStyle toolbarButton
        {
            get => GetOrCreateStyle("toolbarButton", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightSmall");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeSmall");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("toolbarButton", value);
        }
        public IDeclStyle toolbarDropDown
        {
            get => GetOrCreateStyle("toolbarDropDown", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightSmall");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeSmall");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("toolbarDropDown", value);
        }
        public IDeclStyle toolbarTextField
        {
            get => GetOrCreateStyle("toolbarTextField", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightSmall");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeSmall");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Focus, new DeclStyle()
                {
                    BorderColor = StyleProperty<Color>.Ref("focusColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("toolbarTextField", value);
        }
        public IDeclStyle toolbarSearchField
        {
            get => GetOrCreateStyle("toolbarSearchField", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightSmall");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeSmall");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Focus, new DeclStyle()
                {
                    BorderColor = StyleProperty<Color>.Ref("focusColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("toolbarSearchField", value);
        }
        public IDeclStyle popup
        {
            get => GetOrCreateStyle("popup", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusNormal");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingNormal");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeNormal");
                
                return styleSet;
            });
            set => RegisterStyleSet("popup", value);
        }
        
        // 特殊样式
        public IDeclStyle whiteLabel
        {
            get => GetOrCreateStyle("whiteLabel", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Direct(Color.white),
                FontSize = StyleProperty<int>.Ref("fontSizeNormal"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft)
            });
            set => RegisterStyleSet("whiteLabel", value);
        }
        public IDeclStyle whiteMiniLabel
        {
            get => GetOrCreateStyle("whiteMiniLabel", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Direct(Color.white),
                FontSize = StyleProperty<int>.Ref("fontSizeSmall"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft)
            });
            set => RegisterStyleSet("whiteMiniLabel", value);
        }
        public IDeclStyle miniLabel
        {
            get => GetOrCreateStyle("miniLabel", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeSmall"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft)
            });
            set => RegisterStyleSet("miniLabel", value);
        }
        public IDeclStyle boldLabel
        {
            get => GetOrCreateStyle("boldLabel", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeNormal"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft),
                FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold)
            });
            set => RegisterStyleSet("boldLabel", value);
        }
        public IDeclStyle largeLabel
        {
            get => GetOrCreateStyle("largeLabel", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeLarge"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft)
            });
            set => RegisterStyleSet("largeLabel", value);
        }
        public IDeclStyle centeredGreyMiniLabel
        {
            get => GetOrCreateStyle("centeredGreyMiniLabel", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textSecondaryColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeSmall"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter)
            });
            set => RegisterStyleSet("centeredGreyMiniLabel", value);
        }
        public IDeclStyle wordWrappedLabel
        {
            get => GetOrCreateStyle("wordWrappedLabel", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeNormal"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft)
            });
            set => RegisterStyleSet("wordWrappedLabel", value);
        }
        public IDeclStyle wordWrappedMiniLabel
        {
            get => GetOrCreateStyle("wordWrappedMiniLabel", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeSmall"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft)
            });
            set => RegisterStyleSet("wordWrappedMiniLabel", value);
        }
        public IDeclStyle textView
        {
            get => GetOrCreateStyle("textView", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("textFieldPadding");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeNormal");
                styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft);
                
                return styleSet;
            });
            set => RegisterStyleSet("textView", value);
        }
        
        // 编辑器专用样式
        public IDeclStyle colorField
        {
            get => GetOrCreateStyle("colorField", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightNormal");
                
                return styleSet;
            });
            set => RegisterStyleSet("colorField", value);
        }
        public IDeclStyle layerMaskField
        {
            get => GetOrCreateStyle("layerMaskField", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightNormal");
                
                return styleSet;
            });
            set => RegisterStyleSet("layerMaskField", value);
        }
        public IDeclStyle enumPopup
        {
            get => GetOrCreateStyle("enumPopup", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightNormal");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeNormal");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("enumPopup", value);
        }
        public IDeclStyle objectField
        {
            get => GetOrCreateStyle("objectField", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightNormal");
                
                return styleSet;
            });
            set => RegisterStyleSet("objectField", value);
        }
        public IDeclStyle objectFieldThumb
        {
            get => GetOrCreateStyle("objectFieldThumb", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("primaryColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Width = StyleProperty<float>.Direct(32f);
                styleSet.Height = StyleProperty<float>.Direct(32f);
                
                return styleSet;
            });
            set => RegisterStyleSet("objectFieldThumb", value);
        }
        public IDeclStyle curveField
        {
            get => GetOrCreateStyle("curveField", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightNormal");
                
                return styleSet;
            });
            set => RegisterStyleSet("curveField", value);
        }
        public IDeclStyle curveFieldBackground
        {
            get => GetOrCreateStyle("curveFieldBackground", () => new DeclStyle()
            {
                BackgroundColor = StyleProperty<Color>.Ref("surfaceColor"),
                BorderColor = StyleProperty<Color>.Ref("borderColor"),
                BorderWidth = StyleProperty<float>.Ref("borderWidthNormal"),
                BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall")
            });
            set => RegisterStyleSet("curveFieldBackground", value);
        }
        public IDeclStyle preLabel
        {
            get => GetOrCreateStyle("preLabel", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeNormal"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft)
            });
            set => RegisterStyleSet("preLabel", value);
        }
        public IDeclStyle preButton
        {
            get => GetOrCreateStyle("preButton", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusNormal");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("buttonPadding");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightNormal");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeNormal");
                styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("preButton", value);
        }
        public IDeclStyle preToolbar
        {
            get => GetOrCreateStyle("preToolbar", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightNormal");
                
                return styleSet;
            });
            set => RegisterStyleSet("preToolbar", value);
        }
        public IDeclStyle preSlider
        {
            get => GetOrCreateStyle("preSlider", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightSmall");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("preSlider", value);
        }
        public IDeclStyle preSliderThumb
        {
            get => GetOrCreateStyle("preSliderThumb", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("primaryColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusNormal");
                styleSet.Width = StyleProperty<float>.Direct(16f);
                styleSet.Height = StyleProperty<float>.Direct(16f);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("preSliderThumb", value);
        }
        public IDeclStyle preTextArea
        {
            get => GetOrCreateStyle("preTextArea", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("textFieldPadding");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeNormal");
                styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Focus, new DeclStyle()
                {
                    BorderColor = StyleProperty<Color>.Ref("focusColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("preTextArea", value);
        }
        
        // 布局和容器样式
        public IDeclStyle tab
        {
            get => GetOrCreateStyle("tab", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightNormal");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeNormal");
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("primaryColor"),
                    Color = StyleProperty<Color>.Direct(Color.white)
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("tab", value);
        }
        public IDeclStyle tabGroup
        {
            get => GetOrCreateStyle("tabGroup", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                
                return styleSet;
            });
            set => RegisterStyleSet("tabGroup", value);
        }
        public IDeclStyle tabContent
        {
            get => GetOrCreateStyle("tabContent", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("backgroundColor");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingNormal");
                
                return styleSet;
            });
            set => RegisterStyleSet("tabContent", value);
        }
        public IDeclStyle panel
        {
            get => GetOrCreateStyle("panel", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusNormal");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("panelPadding");
                
                return styleSet;
            });
            set => RegisterStyleSet("panel", value);
        }
        public IDeclStyle scrollGroup
        {
            get => GetOrCreateStyle("scrollGroup", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                
                return styleSet;
            });
            set => RegisterStyleSet("scrollGroup", value);
        }
        public IDeclStyle header
        {
            get => GetOrCreateStyle("header", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeLarge"),
                Padding = StyleProperty<RectOffset>.Ref("paddingHeading"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft),
                FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold)
            });
            set => RegisterStyleSet("header", value);
        }
        public IDeclStyle subHeader
        {
            get => GetOrCreateStyle("subHeader", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeNormal"),
                Padding = StyleProperty<RectOffset>.Ref("paddingHeading"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft),
                FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold)
            });
            set => RegisterStyleSet("subHeader", value);
        }
        public IDeclStyle separator
        {
            get => GetOrCreateStyle("separator", () => new DeclStyle()
            {
                BackgroundColor = StyleProperty<Color>.Ref("borderColor"),
                Height = StyleProperty<float>.Direct(1f),
                Margin = StyleProperty<RectOffset>.Ref("marginNormal")
            });
            set => RegisterStyleSet("separator", value);
        }
        public IDeclStyle space
        {
            get => GetOrCreateStyle("space", () => new DeclStyle()
            {
                Height = StyleProperty<float>.Direct(8f),
                Width = StyleProperty<float>.Direct(8f)
            });
            set => RegisterStyleSet("space", value);
        }
        
        // 交互元素样式
        public IDeclStyle link
        {
            get => GetOrCreateStyle("link", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("linkColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeNormal"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft)
            });
            set => RegisterStyleSet("link", value);
        }
        public IDeclStyle progressBar
        {
            get => GetOrCreateStyle("progressBar", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("primaryColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightSmall");
                
                return styleSet;
            });
            set => RegisterStyleSet("progressBar", value);
        }
        public IDeclStyle progressBarBack
        {
            get => GetOrCreateStyle("progressBarBack", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Height = StyleProperty<float>.Ref("controlHeightSmall");
                
                return styleSet;
            });
            set => RegisterStyleSet("progressBarBack", value);
        }
        public IDeclStyle selectionRect
        {
            get => GetOrCreateStyle("selectionRect", () => new DeclStyle()
            {
                BackgroundColor = StyleProperty<Color>.Ref("hoverColor"),
                BorderColor = StyleProperty<Color>.Ref("primaryColor"),
                BorderWidth = StyleProperty<float>.Ref("borderWidthNormal"),
                BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall")
            });
            set => RegisterStyleSet("selectionRect", value);
        }
        public IDeclStyle badge
        {
            get => GetOrCreateStyle("badge", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Direct(Color.white);
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("primaryColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusNormal");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeSmall");
                styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
                
                return styleSet;
            });
            set => RegisterStyleSet("badge", value);
        }
        public IDeclStyle notification
        {
            get => GetOrCreateStyle("notification", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("infoColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusNormal");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingNormal");
                styleSet.FontSize = StyleProperty<int>.Ref("fontSizeNormal");
                
                return styleSet;
            });
            set => RegisterStyleSet("notification", value);
        }
        
        // 图标和图像样式
        public IDeclStyle iconButton
        {
            get => GetOrCreateStyle("iconButton", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.Color = StyleProperty<Color>.Ref("textColor");
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                styleSet.Width = StyleProperty<float>.Direct(32f);
                styleSet.Height = StyleProperty<float>.Direct(32f);
                styleSet.Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleCenter);
                
                // 添加伪类样式
                styleSet.AddStyle(PseudoClass.Hover, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("hoverColor")
                });
                    
                styleSet.AddStyle(PseudoClass.Active, new DeclStyle()
                {
                    BackgroundColor = StyleProperty<Color>.Ref("activeColor")
                });
                    
                return styleSet;
            });
            set => RegisterStyleSet("iconButton", value);
        }
        public IDeclStyle imageView
        {
            get => GetOrCreateStyle("imageView", () =>
            {
                var styleSet = ScriptableObject.CreateInstance<DeclStyleSet>();
                styleSet.BackgroundColor = StyleProperty<Color>.Ref("surfaceColor");
                styleSet.BorderColor = StyleProperty<Color>.Ref("borderColor");
                styleSet.BorderWidth = StyleProperty<float>.Ref("borderWidthNormal");
                styleSet.BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall");
                styleSet.Padding = StyleProperty<RectOffset>.Ref("paddingSmall");
                
                return styleSet;
            });
            set => RegisterStyleSet("imageView", value);
        }
        public IDeclStyle image
        {
            get => GetOrCreateStyle("image", () => new DeclStyle()
            {
                BackgroundColor = StyleProperty<Color>.Ref("surfaceColor"),
                BorderColor = StyleProperty<Color>.Ref("borderColor"),
                BorderWidth = StyleProperty<float>.Ref("borderWidthNormal"),
                BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall")
            });
            set => RegisterStyleSet("image", value);
        }
        
        // Markdown样式
        public IDeclStyle H1
        {
            get => GetOrCreateStyle("H1", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeXXXLarge"),
                Padding = StyleProperty<RectOffset>.Ref("paddingHeading"),
                Margin = StyleProperty<RectOffset>.Ref("marginHeading"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft),
                FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold)
            });
            set => RegisterStyleSet("H1", value);
        }
        public IDeclStyle H2
        {
            get => GetOrCreateStyle("H2", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeXXLarge"),
                Padding = StyleProperty<RectOffset>.Ref("paddingHeading"),
                Margin = StyleProperty<RectOffset>.Ref("marginHeading"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft),
                FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold)
            });
            set => RegisterStyleSet("H2", value);
        }
        public IDeclStyle H3
        {
            get => GetOrCreateStyle("H3", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeXLarge"),
                Padding = StyleProperty<RectOffset>.Ref("paddingHeading"),
                Margin = StyleProperty<RectOffset>.Ref("marginHeading"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft),
                FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold)
            });
            set => RegisterStyleSet("H3", value);
        }
        public IDeclStyle H4
        {
            get => GetOrCreateStyle("H4", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeLarge"),
                Padding = StyleProperty<RectOffset>.Ref("paddingHeading"),
                Margin = StyleProperty<RectOffset>.Ref("marginHeading"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft),
                FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold)
            });
            set => RegisterStyleSet("H4", value);
        }
        public IDeclStyle H5
        {
            get => GetOrCreateStyle("H5", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeNormal"),
                Padding = StyleProperty<RectOffset>.Ref("paddingHeading"),
                Margin = StyleProperty<RectOffset>.Ref("marginHeading"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft),
                FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold)
            });
            set => RegisterStyleSet("H5", value);
        }
        public IDeclStyle H6
        {
            get => GetOrCreateStyle("H6", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeSmall"),
                Padding = StyleProperty<RectOffset>.Ref("paddingHeading"),
                Margin = StyleProperty<RectOffset>.Ref("marginHeading"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.MiddleLeft),
                FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Bold)
            });
            set => RegisterStyleSet("H6", value);
        }
        public IDeclStyle paragraph
        {
            get => GetOrCreateStyle("paragraph", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                FontSize = StyleProperty<int>.Ref("fontSizeNormal"),
                Padding = StyleProperty<RectOffset>.Ref("paddingParagraph"),
                Margin = StyleProperty<RectOffset>.Ref("marginParagraph"),
                Alignment = StyleProperty<TextAnchor>.Direct(TextAnchor.UpperLeft)
            });
            set => RegisterStyleSet("paragraph", value);
        }
        public IDeclStyle code
        {
            get => GetOrCreateStyle("code", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textColor"),
                BackgroundColor = StyleProperty<Color>.Ref("surfaceColor"),
                BorderColor = StyleProperty<Color>.Ref("borderColor"),
                BorderWidth = StyleProperty<float>.Ref("borderWidthNormal"),
                BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall"),
                Padding = StyleProperty<RectOffset>.Ref("paddingSmall"),
                FontSize = StyleProperty<int>.Ref("fontSizeSmall"),
                FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Normal)
            });
            set => RegisterStyleSet("code", value);
        }
        public IDeclStyle quote
        {
            get => GetOrCreateStyle("quote", () => new DeclStyle()
            {
                Color = StyleProperty<Color>.Ref("textSecondaryColor"),
                BackgroundColor = StyleProperty<Color>.Ref("surfaceColor"),
                BorderColor = StyleProperty<Color>.Ref("borderColor"),
                BorderWidth = StyleProperty<float>.Ref("borderWidthNormal"),
                BorderRadius = StyleProperty<float>.Ref("borderRadiusSmall"),
                Padding = StyleProperty<RectOffset>.Ref("paddingNormal"),
                Margin = StyleProperty<RectOffset>.Ref("marginQuote"),
                FontSize = StyleProperty<int>.Ref("fontSizeNormal"),
                FontStyle = StyleProperty<FontStyle>.Direct(FontStyle.Italic)
            });
            set => RegisterStyleSet("quote", value);
        }

        // ICorePropertyDefinition 接口实现 - 核心属性
        public DeclPropertyTemplate.TemplateProperty backgroundColor
        {
            get => CreateTemplateProperty("backgroundColor", StyleConfig.BackgroundColor, "基础背景颜色");
            set => SetTemplateProperty("backgroundColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty surfaceColor
        {
            get => CreateTemplateProperty("surfaceColor", StyleConfig.SurfaceColor, "表面颜色");
            set => SetTemplateProperty("surfaceColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty primaryColor
        {
            get => CreateTemplateProperty("primaryColor", StyleConfig.PrimaryColor, "主色调");
            set => SetTemplateProperty("primaryColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty secondaryColor
        {
            get => CreateTemplateProperty("secondaryColor", StyleConfig.SecondaryColor, "次要色调");
            set => SetTemplateProperty("secondaryColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty textColor
        {
            get => CreateTemplateProperty("textColor", StyleConfig.TextColor, "基础文本颜色");
            set => SetTemplateProperty("textColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty textSecondaryColor
        {
            get => CreateTemplateProperty("textSecondaryColor", StyleConfig.TextSecondaryColor, "次要文本颜色");
            set => SetTemplateProperty("textSecondaryColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty borderColor
        {
            get => CreateTemplateProperty("borderColor", StyleConfig.BorderColor, "基础边框颜色");
            set => SetTemplateProperty("borderColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty hoverColor
        {
            get => CreateTemplateProperty("hoverColor", StyleConfig.HoverColor, "鼠标悬停状态的颜色");
            set => SetTemplateProperty("hoverColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty activeColor
        {
            get => CreateTemplateProperty("activeColor", StyleConfig.ActiveColor, "激活/按下状态的颜色");
            set => SetTemplateProperty("activeColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty focusColor
        {
            get => CreateTemplateProperty("focusColor", StyleConfig.FocusColor, "获得焦点状态的颜色");
            set => SetTemplateProperty("focusColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty disabledColor
        {
            get => CreateTemplateProperty("disabledColor", StyleConfig.DisabledColor, "禁用状态的颜色");
            set => SetTemplateProperty("disabledColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty successColor
        {
            get => CreateTemplateProperty("successColor", StyleConfig.SuccessColor, "成功状态颜色");
            set => SetTemplateProperty("successColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty warningColor
        {
            get => CreateTemplateProperty("warningColor", StyleConfig.WarningColor, "警告状态颜色");
            set => SetTemplateProperty("warningColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty errorColor
        {
            get => CreateTemplateProperty("errorColor", StyleConfig.ErrorColor, "错误状态颜色");
            set => SetTemplateProperty("errorColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty infoColor
        {
            get => CreateTemplateProperty("infoColor", StyleConfig.InfoColor, "信息状态颜色");
            set => SetTemplateProperty("infoColor", value);
        }
        
        // 字体大小属性
        public DeclPropertyTemplate.TemplateProperty fontSizeSmall
        {
            get => CreateTemplateProperty("fontSizeSmall", StyleConfig.FontSizeSmall, "小号字体大小");
            set => SetTemplateProperty("fontSizeSmall", value);
        }
        public DeclPropertyTemplate.TemplateProperty fontSizeNormal
        {
            get => CreateTemplateProperty("fontSizeNormal", StyleConfig.FontSizeNormal, "标准字体大小");
            set => SetTemplateProperty("fontSizeNormal", value);
        }
        public DeclPropertyTemplate.TemplateProperty fontSizeLarge
        {
            get => CreateTemplateProperty("fontSizeLarge", StyleConfig.FontSizeLarge, "大号字体大小");
            set => SetTemplateProperty("fontSizeLarge", value);
        }
        public DeclPropertyTemplate.TemplateProperty fontSizeXLarge
        {
            get => CreateTemplateProperty("fontSizeXLarge", StyleConfig.FontSizeXLarge, "超大号字体大小");
            set => SetTemplateProperty("fontSizeXLarge", value);
        }
        public DeclPropertyTemplate.TemplateProperty fontSizeXXLarge
        {
            get => CreateTemplateProperty("fontSizeXXLarge", StyleConfig.FontSizeXXLarge, "特大号字体大小");
            set => SetTemplateProperty("fontSizeXXLarge", value);
        }
        public DeclPropertyTemplate.TemplateProperty fontSizeXXXLarge
        {
            get => CreateTemplateProperty("fontSizeXXXLarge", StyleConfig.FontSizeXXXLarge, "最大号字体大小");
            set => SetTemplateProperty("fontSizeXXXLarge", value);
        }
        
        // 间距属性
        public DeclPropertyTemplate.TemplateProperty paddingSmall
        {
            get => CreateTemplateProperty("paddingSmall", StyleConfig.PaddingSmall, "小内边距");
            set => SetTemplateProperty("paddingSmall", value);
        }
        public DeclPropertyTemplate.TemplateProperty paddingNormal
        {
            get => CreateTemplateProperty("paddingNormal", StyleConfig.PaddingNormal, "标准内边距");
            set => SetTemplateProperty("paddingNormal", value);
        }
        public DeclPropertyTemplate.TemplateProperty paddingLarge
        {
            get => CreateTemplateProperty("paddingLarge", StyleConfig.PaddingLarge, "大内边距");
            set => SetTemplateProperty("paddingLarge", value);
        }
        public DeclPropertyTemplate.TemplateProperty marginSmall
        {
            get => CreateTemplateProperty("marginSmall", StyleConfig.MarginSmall, "小外边距");
            set => SetTemplateProperty("marginSmall", value);
        }
        public DeclPropertyTemplate.TemplateProperty marginNormal
        {
            get => CreateTemplateProperty("marginNormal", StyleConfig.MarginNormal, "标准外边距");
            set => SetTemplateProperty("marginNormal", value);
        }
        public DeclPropertyTemplate.TemplateProperty marginLarge
        {
            get => CreateTemplateProperty("marginLarge", StyleConfig.MarginLarge, "大外边距");
            set => SetTemplateProperty("marginLarge", value);
        }
        public DeclPropertyTemplate.TemplateProperty paddingHeading
        {
            get => CreateTemplateProperty("paddingHeading", StyleConfig.PaddingHeading, "标题内边距");
            set => SetTemplateProperty("paddingHeading", value);
        }
        public DeclPropertyTemplate.TemplateProperty marginHeading
        {
            get => CreateTemplateProperty("marginHeading", StyleConfig.MarginHeading, "标题外边距");
            set => SetTemplateProperty("marginHeading", value);
        }
        public DeclPropertyTemplate.TemplateProperty paddingParagraph
        {
            get => CreateTemplateProperty("paddingParagraph", StyleConfig.PaddingParagraph, "段落内边距");
            set => SetTemplateProperty("paddingParagraph", value);
        }
        public DeclPropertyTemplate.TemplateProperty marginParagraph
        {
            get => CreateTemplateProperty("marginParagraph", StyleConfig.MarginParagraph, "段落外边距");
            set => SetTemplateProperty("marginParagraph", value);
        }
        public DeclPropertyTemplate.TemplateProperty marginQuote
        {
            get => CreateTemplateProperty("marginQuote", StyleConfig.MarginQuote, "引用外边距");
            set => SetTemplateProperty("marginQuote", value);
        }
        
        // 边框属性
        public DeclPropertyTemplate.TemplateProperty borderWidthNormal
        {
            get => CreateTemplateProperty("borderWidthNormal", StyleConfig.BorderWidthNormal, "标准边框宽度");
            set => SetTemplateProperty("borderWidthNormal", value);
        }
        public DeclPropertyTemplate.TemplateProperty borderWidthThick
        {
            get => CreateTemplateProperty("borderWidthThick", StyleConfig.BorderWidthThick, "粗边框宽度");
            set => SetTemplateProperty("borderWidthThick", value);
        }
        public DeclPropertyTemplate.TemplateProperty borderRadiusSmall
        {
            get => CreateTemplateProperty("borderRadiusSmall", StyleConfig.BorderRadiusSmall, "小圆角半径");
            set => SetTemplateProperty("borderRadiusSmall", value);
        }
        public DeclPropertyTemplate.TemplateProperty borderRadiusNormal
        {
            get => CreateTemplateProperty("borderRadiusNormal", StyleConfig.BorderRadiusNormal, "标准圆角半径");
            set => SetTemplateProperty("borderRadiusNormal", value);
        }
        public DeclPropertyTemplate.TemplateProperty borderRadiusLarge
        {
            get => CreateTemplateProperty("borderRadiusLarge", StyleConfig.BorderRadiusLarge, "大圆角半径");
            set => SetTemplateProperty("borderRadiusLarge", value);
        }
        
        // 控件高度属性
        public DeclPropertyTemplate.TemplateProperty controlHeightSmall
        {
            get => CreateTemplateProperty("controlHeightSmall", StyleConfig.ControlHeightSmall, "小控件高度");
            set => SetTemplateProperty("controlHeightSmall", value);
        }
        public DeclPropertyTemplate.TemplateProperty controlHeightNormal
        {
            get => CreateTemplateProperty("controlHeightNormal", StyleConfig.ControlHeightNormal, "标准控件高度");
            set => SetTemplateProperty("controlHeightNormal", value);
        }
        public DeclPropertyTemplate.TemplateProperty controlHeightLarge
        {
            get => CreateTemplateProperty("controlHeightLarge", StyleConfig.ControlHeightLarge, "大控件高度");
            set => SetTemplateProperty("controlHeightLarge", value);
        }
        
        // Markdown专用属性
        public DeclPropertyTemplate.TemplateProperty linkColor
        {
            get => CreateTemplateProperty("linkColor", StyleConfig.LinkColor, "链接颜色");
            set => SetTemplateProperty("linkColor", value);
        }
        public DeclPropertyTemplate.TemplateProperty linkHoverColor
        {
            get => CreateTemplateProperty("linkHoverColor", StyleConfig.LinkHoverColor, "链接悬停颜色");
            set => SetTemplateProperty("linkHoverColor", value);
        }
        
        // 按钮专用内边距属性
        public DeclPropertyTemplate.TemplateProperty buttonPadding
        {
            get => CreateTemplateProperty("buttonPadding", StyleConfig.ButtonPadding, "按钮内边距");
            set => SetTemplateProperty("buttonPadding", value);
        }
        public DeclPropertyTemplate.TemplateProperty smallButtonPadding
        {
            get => CreateTemplateProperty("smallButtonPadding", StyleConfig.SmallButtonPadding, "小按钮内边距");
            set => SetTemplateProperty("smallButtonPadding", value);
        }
        public DeclPropertyTemplate.TemplateProperty largeButtonPadding
        {
            get => CreateTemplateProperty("largeButtonPadding", StyleConfig.LargeButtonPadding, "大按钮内边距");
            set => SetTemplateProperty("largeButtonPadding", value);
        }
        
        // 面板专用内边距属性
        public DeclPropertyTemplate.TemplateProperty panelPadding
        {
            get => CreateTemplateProperty("panelPadding", StyleConfig.PanelPadding, "面板内边距");
            set => SetTemplateProperty("panelPadding", value);
        }
        
        // 输入框专用内边距属性
        public DeclPropertyTemplate.TemplateProperty textFieldPadding
        {
            get => CreateTemplateProperty("textFieldPadding", StyleConfig.TextFieldPadding, "文本输入框内边距");
            set => SetTemplateProperty("textFieldPadding", value);
        }
        public DeclPropertyTemplate.TemplateProperty subtleTextFieldPadding
        {
            get => CreateTemplateProperty("subtleTextFieldPadding", StyleConfig.SubtleTextFieldPadding, "柔和文本输入框内边距");
            set => SetTemplateProperty("subtleTextFieldPadding", value);
        }
        
        // 标签专用内边距属性
        public DeclPropertyTemplate.TemplateProperty labelPadding
        {
            get => CreateTemplateProperty("labelPadding", StyleConfig.LabelPadding, "标签内边距");
            set => SetTemplateProperty("labelPadding", value);
        }

        // 辅助方法：获取或创建样式
        private IDeclStyle GetOrCreateStyle(string key, Func<IDeclStyle> defaultStyleCreator)
        {
            var existingStyle = GetStyleSet(key);
            if (existingStyle != null)
            {
                return existingStyle;
            }

            // 创建默认样式并注册
            var defaultStyle = defaultStyleCreator();
            RegisterStyleSet(key, defaultStyle);
            return defaultStyle;
        }

        // 辅助方法：基于泛型的模板属性创建
        private DeclPropertyTemplate.TemplateProperty CreateTemplateProperty<T>(string name, T defaultValue, string description)
        {
            var existingProp = themeProperties.Find(p => p.Name == name);
            if (existingProp != null)
            {
                // 如果找到了，创建对应的TemplateProperty
                var templateProp = new DeclPropertyTemplate.TemplateProperty
                {
                    Name = name,
                    Type = existingProp.Type,
                    Description = description
                };
                
                // 根据类型设置值
                switch (existingProp.Type)
                {
                    case PropertyType.Float:
                        templateProp.FloatValue = existingProp.FloatValue;
                        break;
                    case PropertyType.Int:
                        templateProp.IntValue = existingProp.IntValue;
                        break;
                    case PropertyType.Color:
                        templateProp.ColorValue = existingProp.ColorValue;
                        break;
                    case PropertyType.String:
                        templateProp.StringValue = existingProp.StringValue;
                        break;
                    case PropertyType.Boolean:
                        templateProp.BoolValue = existingProp.BoolValue;
                        break;
                    case PropertyType.Vector2:
                        templateProp.Vector2Value = existingProp.Vector2Value;
                        break;
                    case PropertyType.Vector3:
                        templateProp.Vector3Value = existingProp.Vector3Value;
                        break;
                    case PropertyType.RectOffset:
                        templateProp.RectOffsetValue = existingProp.RectOffsetValue;
                        break;
                }
                
                return templateProp;
            }
            else
            {
                // 如果没有找到，创建一个新的TemplateProperty并添加到主题属性中
                var templateProp = new DeclPropertyTemplate.TemplateProperty
                {
                    Name = name,
                    Description = description
                };

                // 根据类型设置值和类型
                switch (defaultValue)
                {
                    case float floatValue:
                        templateProp.Type = PropertyType.Float;
                        templateProp.FloatValue = floatValue;
                        break;
                    case int intValue:
                        templateProp.Type = PropertyType.Int;
                        templateProp.IntValue = intValue;
                        break;
                    case Color colorValue:
                        templateProp.Type = PropertyType.Color;
                        templateProp.ColorValue = colorValue;
                        break;
                    case string stringValue:
                        templateProp.Type = PropertyType.String;
                        templateProp.StringValue = stringValue;
                        break;
                    case bool boolValue:
                        templateProp.Type = PropertyType.Boolean;
                        templateProp.BoolValue = boolValue;
                        break;
                    case Vector2 vector2Value:
                        templateProp.Type = PropertyType.Vector2;
                        templateProp.Vector2Value = vector2Value;
                        break;
                    case Vector3 vector3Value:
                        templateProp.Type = PropertyType.Vector3;
                        templateProp.Vector3Value = vector3Value;
                        break;
                    case RectOffset rectOffsetValue:
                        templateProp.Type = PropertyType.RectOffset;
                        templateProp.RectOffsetValue = rectOffsetValue;
                        break;
                }

                // 添加到主题属性中
                var themeProp = new ThemeProperty { Name = name, Type = templateProp.Type };
                themeProp.SetValue(defaultValue);
                themeProperties.Add(themeProp);

                return templateProp;
            }
        }

        // 辅助方法
        private DeclPropertyTemplate.TemplateProperty GetOrCreateTemplateProperty(string name, PropertyType type, object defaultValue, string description)
        {
            // 尝试从当前主题属性中获取
            var existingProp = themeProperties.Find(p => p.Name == name);
            if (existingProp != null)
            {
                // 如果找到了，创建对应的TemplateProperty
                var templateProp = new DeclPropertyTemplate.TemplateProperty
                {
                    Name = name,
                    Type = existingProp.Type,
                    Description = description
                };
                
                // 根据类型设置值
                switch (existingProp.Type)
                {
                    case PropertyType.Float:
                        templateProp.FloatValue = existingProp.FloatValue;
                        break;
                    case PropertyType.Int:
                        templateProp.IntValue = existingProp.IntValue;
                        break;
                    case PropertyType.Color:
                        templateProp.ColorValue = existingProp.ColorValue;
                        break;
                    case PropertyType.String:
                        templateProp.StringValue = existingProp.StringValue;
                        break;
                    case PropertyType.Boolean:
                        templateProp.BoolValue = existingProp.BoolValue;
                        break;
                    case PropertyType.Vector2:
                        templateProp.Vector2Value = existingProp.Vector2Value;
                        break;
                    case PropertyType.Vector3:
                        templateProp.Vector3Value = existingProp.Vector3Value;
                        break;
                    case PropertyType.RectOffset:
                        templateProp.RectOffsetValue = existingProp.RectOffsetValue;
                        break;
                }
                
                return templateProp;
            }
            else
            {
                // 如果没有找到，创建一个新的TemplateProperty并添加到主题属性中
                var templateProp = new DeclPropertyTemplate.TemplateProperty
                {
                    Name = name,
                    Type = type,
                    Description = description
                };

                switch (type)
                {
                    case PropertyType.Float:
                        templateProp.FloatValue = (float)defaultValue;
                        break;
                    case PropertyType.Int:
                        templateProp.IntValue = (int)defaultValue;
                        break;
                    case PropertyType.Color:
                        templateProp.ColorValue = (Color)defaultValue;
                        break;
                    case PropertyType.String:
                        templateProp.StringValue = (string)defaultValue;
                        break;
                    case PropertyType.Boolean:
                        templateProp.BoolValue = (bool)defaultValue;
                        break;
                    case PropertyType.Vector2:
                        templateProp.Vector2Value = (Vector2)defaultValue;
                        break;
                    case PropertyType.Vector3:
                        templateProp.Vector3Value = (Vector3)defaultValue;
                        break;
                    case PropertyType.RectOffset:
                        templateProp.RectOffsetValue = (RectOffset)defaultValue;
                        break;
                }

                // 添加到主题属性中
                var themeProp = new ThemeProperty { Name = name, Type = type };
                themeProp.SetValue(defaultValue);
                themeProperties.Add(themeProp);

                return templateProp;
            }
        }

        private void SetTemplateProperty(string name, DeclPropertyTemplate.TemplateProperty templateProperty)
        {
            if (templateProperty != null && templateProperty.Name == name)
            {
                // 更新主题属性
                var existingProp = themeProperties.Find(p => p.Name == name);
                if (existingProp != null)
                {
                    themeProperties.Remove(existingProp);
                }

                var newThemeProp = new ThemeProperty { Name = templateProperty.Name, Type = templateProperty.Type };
                
                switch (templateProperty.Type)
                {
                    case PropertyType.Float:
                        newThemeProp.FloatValue = templateProperty.FloatValue;
                        break;
                    case PropertyType.Int:
                        newThemeProp.IntValue = templateProperty.IntValue;
                        break;
                    case PropertyType.Color:
                        newThemeProp.ColorValue = templateProperty.ColorValue;
                        break;
                    case PropertyType.String:
                        newThemeProp.StringValue = templateProperty.StringValue;
                        break;
                    case PropertyType.Boolean:
                        newThemeProp.BoolValue = templateProperty.BoolValue;
                        break;
                    case PropertyType.Vector2:
                        newThemeProp.Vector2Value = templateProperty.Vector2Value;
                        break;
                    case PropertyType.Vector3:
                        newThemeProp.Vector3Value = templateProperty.Vector3Value;
                        break;
                    case PropertyType.RectOffset:
                        newThemeProp.RectOffsetValue = templateProperty.RectOffsetValue;
                        break;
                }

                themeProperties.Add(newThemeProp);
            }
        }

        // ICoreThemeDefinition 接口方法实现
        public IEnumerable<string> GetCoreStyleNames()
        {
            var interfaceType = typeof(ICoreThemeDefinition);
            var properties = interfaceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties.Where(p => p.PropertyType == typeof(IDeclStyle))
                           .Select(p => p.Name);
        }

        public bool IsCoreStyle(string styleName)
        {
            return GetCoreStyleNames().Contains(styleName);
        }

        // ICorePropertyDefinition 接口方法实现
        public IEnumerable<string> GetCorePropertyNames()
        {
            var interfaceType = typeof(ICorePropertyDefinition);
            var properties = interfaceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties.Where(p => p.PropertyType == typeof(DeclPropertyTemplate.TemplateProperty))
                           .Select(p => p.Name);
        }

        public bool IsCoreProperty(string propertyName)
        {
            return GetCorePropertyNames().Contains(propertyName);
        }
    }
}