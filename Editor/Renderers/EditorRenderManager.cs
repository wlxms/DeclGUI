using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;
using DeclGUI.Editor.Renderers;
using System;
using UnityEditor;
using System.Linq;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Editor环境渲染管理器
    /// </summary>
    public class EditorRenderManager : RenderManager
    {
        // GUIStyle对象池 - 重用GUIStyle对象以减少GC
        private GUIStylePool _guiStylePool = new GUIStylePool();

        private IElement _currentRenderElement;

        public Rect GetCurrentRenderRect()
        {
            if (_currentRenderElement == null)
                return Rect.zero;

            if (_currentRenderElement is IElementWithKey elementWithKey)
            {
                var elementState = GetElementState(elementWithKey);
                if (elementState != null)
                {
                    var elementRenderState = elementState.GetState<EditorElementState>();
                    return elementRenderState.RenderRect;
                }
            }

            return GUILayoutUtility.GetLastRect();
        }

        protected void CacheElementState(in IElement element)
        {
            if (element == null || Event.current.type != EventType.Repaint)
                return;

            if (element is IElementWithKey elementWithKey)
            {
                var elementState = GetElementState(elementWithKey);
                if (elementState != null)
                {
                    var elementRenderState = elementState.GetState<EditorElementState>();
                    var rect = GUILayoutUtility.GetLastRect();
                    // Debug.Log($"CacheElementState: {elementWithKey.Key} - {rect} - {Event.current.type}");
                    if (rect.width > 0 && rect.height > 0)
                        elementRenderState.RenderRect = rect;
                }
            }
        }


        /// <summary>
        /// 重写RenderElement方法，应用缩进效果
        /// </summary>
        public override void RenderElement(in IElement element)
        {
            // 检查是否有缩进上下文
            bool hasIndent = ContextStack.TryGet<IndentContext>(out var indentContext);

            if (hasIndent)
            {
                EditorGUI.indentLevel += indentContext.Level;
                PushContext(new IndentContext(0, indentContext.Size));
            }
            _currentRenderElement = element;
            base.RenderElement(element);
            CacheElementState(element);

            if (hasIndent)
            {
                PopContext<IndentContext>();
                EditorGUI.indentLevel -= indentContext.Level;
            }

        }

        /// <summary>
        /// 发现和注册Editor专用的渲染器
        /// </summary>
        protected override void DiscoverRenderers()
        {
            // 注册基本组件渲染器
            RegisterRenderer<Button>(new EditorButtonRenderer());
            RegisterRenderer<Label>(new EditorLabelRenderer());
            RegisterRenderer<Hor>(new EditorHorizontalRenderer());
            RegisterRenderer<Ver>(new EditorVerRenderer());
            RegisterRenderer<Spc>(new EditorSpcRenderer());
            RegisterRenderer<TextField>(new EditorTextFieldRenderer());

            // 注册新组件渲染器
            RegisterRenderer<FixableSpace>(new EditorFixableSpaceRenderer());
            RegisterRenderer<Slider>(new EditorSliderRenderer());
            RegisterRenderer<ScrollRect>(new EditorScrollRectRenderer());
            RegisterRenderer<Toggle>(new EditorToggleRenderer());
            RegisterRenderer<IntField>(new EditorIntFieldRenderer());
            RegisterRenderer<FloatField>(new EditorFloatFieldRenderer());
            RegisterRenderer<Popup>(new EditorPopupRenderer());
            RegisterRenderer<StringPopup>(new EditorStringPopupRenderer());
            RegisterRenderer<EnumPopup>(new EditorEnumPopupRenderer());
            RegisterRenderer<ColorField>(new EditorColorFieldRenderer());
            RegisterRenderer<Vector2Field>(new EditorVector2FieldRenderer());
            RegisterRenderer<Vector3Field>(new EditorVector3FieldRenderer());
            RegisterRenderer<Vector4Field>(new EditorVector4FieldRenderer());
            RegisterRenderer<MinMaxSlider>(new EditorMinMaxSliderRenderer());
            RegisterRenderer<LayerField>(new EditorLayerFieldRenderer());
            RegisterRenderer<TagField>(new EditorTagFieldRenderer());

            // 注册ObjectField渲染器（处理所有ObjectField<T>类型）
            RegisterRenderer(typeof(DeclGUI.Components.ObjectField<>), new EditorObjectFieldRenderer());

            // 注册非泛型ObjectField渲染器
            RegisterRenderer<ObjectField>(new EditorNonGenericObjectFieldRenderer());

            // 注册新布局组件渲染器
            RegisterRenderer<ECanvas>(new EditorECanvasRenderer());
            RegisterRenderer<AbsolutePanel>(new EditorAbsolutePanelRenderer());

            // 注册有状态组件渲染器
            RegisterRenderer<StatefulButton>(new EditorStatefulButtonRenderer());
            RegisterRenderer<LongPressButton>(new EditorLongPressButtonRenderer());

            // 注册DisableGroup渲染器
            RegisterRenderer<DisableGroup>(new DisableGroupRenderer());
            // 注册FoldoutGroup渲染器
            RegisterRenderer<FoldoutGroup>(new EditorFoldoutGroupRenderer());
            RegisterRenderer<FoldoutHeaderGroup>(new EditorFoldoutHeaderGroupRenderer());

            // 注册Image渲染器
            RegisterRenderer<Image>(new EditorImageRenderer());

            // 注册PopupMenu渲染器
            RegisterRenderer<PopupMenu>(new EditorPopupMenuRenderer());
        }

        /// <summary>
        /// 应用样式到GUI
        /// </summary>
        /// <param name="style">样式</param>
        /// <param name="defaultStyle">默认样式类型</param>
        /// <returns>应用样式后的GUI状态</returns>
        public GUIStyle ApplyStyle(IDeclStyle style, GUIStyle defaultStyle = null)
        {
            if (style == null)
                return defaultStyle ?? GUI.skin.label; // 默认返回标签样式

            // 创建缓存键
            var cacheKey = (style.GetContentHashCode(), defaultStyle?.name ?? "label");

            // 从对象池获取或创建GUIStyle
            // var guiStyle = _guiStylePool.GetOrCreateStyle(cacheKey, defaultStyle ?? GUI.skin.label);
            var existingStyle = _guiStylePool.GetExistedStyle(cacheKey);
            if (existingStyle != null)
                return existingStyle;

            // 如果没有找到，创建新的GUIStyle
            var guiStyle = _guiStylePool.GetPoolStyle(cacheKey, defaultStyle ?? GUI.skin.label);

            // 使用IDeclStyle接口的方法获取样式属性
            var color = style.Color;
            var backgroundColor = style.BackgroundColor;
            var borderColor = style.BorderColor;
            var fontSize = style.FontSize;
            var fontStyle = style.FontStyle;
            var alignment = style.Alignment;
            var padding = style.Padding;
            var margin = style.Margin;
            var borderWidth = style.BorderWidth;  // 新增：边框宽度
            var borderRadius = style.BorderRadius;  // 新增：圆角半径

            // 重置样式属性为默认值
            var defaultStyleToUse = defaultStyle ?? GUI.skin.label;
            guiStyle.normal = defaultStyleToUse.normal;
            guiStyle.hover = defaultStyleToUse.hover;
            guiStyle.active = defaultStyleToUse.active;
            guiStyle.focused = defaultStyleToUse.focused;
            guiStyle.onNormal = defaultStyleToUse.onNormal;
            guiStyle.onHover = defaultStyleToUse.onHover;
            guiStyle.onActive = defaultStyleToUse.onActive;
            guiStyle.onFocused = defaultStyleToUse.onFocused;
            guiStyle.border = defaultStyleToUse.border;
            guiStyle.margin = defaultStyleToUse.margin;
            guiStyle.padding = defaultStyleToUse.padding;
            guiStyle.overflow = defaultStyleToUse.overflow;
            guiStyle.font = defaultStyleToUse.font;
            guiStyle.fontSize = defaultStyleToUse.fontSize;
            guiStyle.fontStyle = defaultStyleToUse.fontStyle;
            guiStyle.alignment = defaultStyleToUse.alignment;
            guiStyle.wordWrap = defaultStyleToUse.wordWrap;
            guiStyle.clipping = defaultStyleToUse.clipping;
            guiStyle.imagePosition = defaultStyleToUse.imagePosition;
            guiStyle.contentOffset = defaultStyleToUse.contentOffset;
            guiStyle.fixedWidth = defaultStyleToUse.fixedWidth;
            guiStyle.fixedHeight = defaultStyleToUse.fixedHeight;
            guiStyle.stretchWidth = defaultStyleToUse.stretchWidth;
            guiStyle.stretchHeight = defaultStyleToUse.stretchHeight;

            // 应用边框宽度到GUIStyle的border属性
            if (borderWidth != null && borderWidth > 0)
            {
                // 将边框宽度应用到GUIStyle的border属性
                int borderWidthValue = (int)borderWidth.Value;
                guiStyle.border = new RectOffset(
                    borderWidthValue,
                    borderWidthValue,
                    borderWidthValue,
                    borderWidthValue
                );
            }

            // 应用所有样式属性
            if (color != null)
            {
                guiStyle.normal.textColor = color.Value;
                guiStyle.hover.textColor = color.Value;
                guiStyle.active.textColor = color.Value;
                guiStyle.focused.textColor = color.Value;
            }

            if (fontSize != null && fontSize > 0)
            {
                guiStyle.fontSize = fontSize.Value;
            }

            if (fontStyle != null && fontStyle != FontStyle.Normal)
            {
                guiStyle.fontStyle = fontStyle.Value;
            }

            if (alignment != null && alignment != TextAnchor.UpperLeft)
            {
                guiStyle.alignment = alignment.Value;
            }

            if (padding != null)
            {
                guiStyle.padding = padding;
            }

            if (margin != null)
            {
                guiStyle.margin = margin;
            }

            return guiStyle;
        }

        /// <summary>
        /// 获取样式宽度
        /// </summary>
        /// <param name="style">样式</param>
        /// <returns>宽度值，如果没有设置则返回0</returns>
        public float GetStyleWidth(IDeclStyle style)
        {
            return style?.Width ?? 0;
        }

        /// <summary>
        /// 获取样式高度
        /// </summary>
        /// <param name="style">样式</param>
        /// <returns>高度值，如果没有设置则返回0</returns>
        public float GetStyleHeight(IDeclStyle style)
        {
            return style?.Height ?? 0;
        }

        /// <summary>
        /// 获取样式margin的水平总和（left + right）
        /// </summary>
        /// <param name="style">样式</param>
        /// <returns>margin水平总和</returns>
        public float GetStyleMarginHorizontal(IDeclStyle style)
        {
            if (style?.Margin == null) return 0;
            return style.Margin.left + style.Margin.right;
        }

        /// <summary>
        /// 获取样式margin的垂直总和（top + bottom）
        /// </summary>
        /// <param name="style">样式</param>
        /// <returns>margin垂直总和</returns>
        public float GetStyleMarginVertical(IDeclStyle style)
        {
            if (style?.Margin == null) return 0;
            return style.Margin.top + style.Margin.bottom;
        }

        /// <summary>
        /// 应用margin到尺寸计算
        /// </summary>
        /// <param name="size">原始尺寸</param>
        /// <param name="style">样式</param>
        /// <returns>包含margin的尺寸</returns>
        public Vector2 ApplyMarginToSize(Vector2 size, IDeclStyle style)
        {
            if (style?.Margin == null) return size;
            
            return new Vector2(
                size.x + style.Margin.left + style.Margin.right,
                size.y + style.Margin.top + style.Margin.bottom
            );
        }

        /// <summary>
        /// 静态的Fallback渲染方法（供EditorElementRenderer调用）
        /// </summary>
        /// <param name="exception">发生的异常</param>
        /// <param name="element">发生异常的元素</param>
        public static void RenderFallbackStatic(Exception exception, IElement element)
        {
            string elementType = element?.GetType().Name ?? "Unknown";

            // 记录完整的错误信息（包含可跳转的堆栈）
            Debug.LogError($"DeclGUI Render Error in {elementType}: {exception.Message}\n{exception.StackTrace}");

            // 创建红色错误样式（使用更醒目的深红色）
            var errorStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                normal = { textColor = new Color(0.8f, 0.1f, 0.1f) }, // 深红色
                richText = true
            };

            var boldErrorStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = new Color(0.8f, 0.1f, 0.1f) }, // 深红色
                richText = true
            };

            // 在UI中显示红色错误信息，使用更醒目的红色背景
            GUILayout.BeginVertical(new GUIStyle(GUI.skin.box)
            {
                normal = { background = MakeTex(2, 2, new Color(1f, 0.9f, 0.9f)) } // 浅红色背景
            });

            // 使用深红色显示标题
            GUILayout.Label($"<color=#CC000>⚠️ Render Error: {elementType}</color>", boldErrorStyle);

            // 使用深红色显示错误消息
            GUILayout.Label($"<color=#CC000>Error: {exception.Message}</color>", errorStyle);

            // 显示简化的堆栈信息（第一行）
            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                var stackLines = exception.StackTrace.Split('\n');
                var firstStackLine = stackLines.Length > 0 ? stackLines[0].Trim() : null;
                if (!string.IsNullOrEmpty(firstStackLine))
                {
                    GUILayout.Label($"<color=#CC0000>At: {firstStackLine}</color>", errorStyle);
                }
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 创建纯色纹理（用于背景）
        /// </summary>
        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        /// <summary>
        /// 统一的Fallback渲染方法
        /// </summary>
        /// <param name="exception">发生的异常</param>
        /// <param name="element">发生异常的元素</param>
        protected override void RenderFallback(Exception exception, IElement element)
        {
            RenderFallbackStatic(exception, element);
        }

        /// <summary>
        /// 获取元素的屏幕区域
        /// Editor环境下的实现，需要根据GUI布局计算元素位置
        /// </summary>
        /// <param name="element">要获取位置的元素</param>
        /// <returns>元素的屏幕矩形</returns>
        protected override Rect GetElementScreenRect(IElement element)
        {
            // 在Editor环境下，元素位置检测需要渲染器提供
            // 这里使用一个简化的实现，实际项目中需要更精确的位置计算

            // 使用基类的GetRenderer方法获取渲染器
            var renderer = GetRenderer(element);

            // 如果渲染器实现了提供位置的方法
            if (renderer is IElementRectProvider rectProvider)
            {
                return rectProvider.GetElementRect();
            }

            // 默认返回空矩形（需要渲染器具体实现）
            return new Rect();
        }

        protected override void OnElementStateCreated(IElementState elementState)
        {
            base.OnElementStateCreated(elementState);
            elementState.AddState(new EditorElementState());
        }

        /// <summary>
        /// 显示弹出窗口（Editor环境实现）
        /// </summary>
        /// <param name="element">要显示的元素</param>
        /// <param name="position">弹出窗口位置</param>
        public void ShowPopup(IElement element, Rect position)
        {
            if (element == null)
            {
                Debug.LogWarning("Cannot show popup with null element");
                return;
            }

            try
            {
                // 创建弹出窗口内容
                var popupContent = new EditorPopupWindowContent(element, null);

                // 显示弹出窗口
                PopupWindow.Show(position, popupContent);
            }
            catch (System.Exception)
            {

                
            }


        }
    }
}