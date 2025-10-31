using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 样式配置 - 从DeclThemeCreator中提取的配置
    /// </summary>
    public static class StyleConfig
    {
        // 字体大小
        public static int FontSizeSmall = 10;
        public static int FontSizeNormal = 12;
        public static int FontSizeLarge = 14;
        public static int FontSizeXLarge = 16;
        public static int FontSizeXXLarge = 18;
        public static int FontSizeXXXLarge = 24;

        // 间距
        public static RectOffset PaddingSmall = new RectOffset(4, 4, 1, 1);
        public static RectOffset PaddingNormal = new RectOffset(8, 8, 2, 2);
        public static RectOffset PaddingLarge = new RectOffset(12, 12, 4, 4);
        
        public static RectOffset MarginSmall = new RectOffset(2, 2, 2, 2);
        public static RectOffset MarginNormal = new RectOffset(4, 4, 4, 4);
        public static RectOffset MarginLarge = new RectOffset(8, 8, 8, 8);

        // 边框
        public static float BorderWidthNormal = 1f;
        public static float BorderWidthThick = 2f;
        public static float BorderRadiusSmall = 2f;
        public static float BorderRadiusNormal = 4f;
        public static float BorderRadiusLarge = 8f;

        // 控件高度
        public static float ControlHeightSmall = 20f;
        public static float ControlHeightNormal = 24f;
        public static float ControlHeightLarge = 32f;
        
        // 特殊控件内边距
        public static RectOffset ButtonPadding = new RectOffset(8, 8, 4, 4);
        public static RectOffset SmallButtonPadding = new RectOffset(4, 4, 2, 2);
        public static RectOffset LargeButtonPadding = new RectOffset(12, 12, 6, 6);
        public static RectOffset PanelPadding = new RectOffset(8, 8, 8, 8);
        public static RectOffset TextFieldPadding = new RectOffset(6, 6, 4, 4);
        public static RectOffset SubtleTextFieldPadding = new RectOffset(4, 4, 3, 3);
        public static RectOffset LabelPadding = new RectOffset(2, 2, 1, 1);

        // 补充缺失的间距字段
        public static RectOffset PaddingHeading = new RectOffset(0, 0, 4, 4);
        public static RectOffset MarginHeading = new RectOffset(0, 0, 8, 8);
        public static RectOffset PaddingParagraph = new RectOffset(0, 0, 2, 2);
        public static RectOffset MarginParagraph = new RectOffset(0, 0, 4, 4);
        public static RectOffset MarginQuote = new RectOffset(8, 0, 4, 4);

        // 颜色 - 使用深色主题默认值
        public static Color BackgroundColor = new Color(0.12f, 0.12f, 0.12f);
        public static Color SurfaceColor = new Color(0.18f, 0.18f, 0.18f);
        public static Color PrimaryColor = new Color(0.0f, 0.6f, 1.0f);
        public static Color SecondaryColor = new Color(0.7f, 0.7f, 0.7f);
        public static Color TextColor = new Color(0.9f, 0.9f, 0.9f);
        public static Color TextSecondaryColor = new Color(0.6f, 0.6f, 0.6f);
        public static Color BorderColor = new Color(0.3f, 0.3f, 0.3f);
        public static Color HoverColor = new Color(0.25f, 0.25f, 0.25f);
        public static Color ActiveColor = new Color(0.35f, 0.35f, 0.35f);
        public static Color FocusColor = new Color(0.2f, 0.4f, 0.6f);
        public static Color DisabledColor = new Color(0.4f, 0.4f, 0.4f);
        public static Color SuccessColor = new Color(0.3f, 0.8f, 0.4f);
        public static Color WarningColor = new Color(1.0f, 0.9f, 0.3f);
        public static Color ErrorColor = new Color(1.0f, 0.4f, 0.4f);
        public static Color InfoColor = new Color(0.3f, 0.7f, 1.0f);
        public static Color LinkColor = new Color(0.0f, 0.6f, 1.0f);
        public static Color LinkHoverColor = new Color(0.8f, 0.8f, 0.8f);
    }
}