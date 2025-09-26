using DeclGUI.Core;
using DeclGUI.Components;
using UnityEngine;
using DeclGUI.Editor.Renderers;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// GUIStyle对象池，用于重用GUIStyle实例以减少GC
    /// </summary>
    internal class GUIStylePool
    {
        private Dictionary<(int styleHashCode, string defaultStyleName), GUIStyle> _stylePool = new Dictionary<(int styleHashCode, string defaultStyleName), GUIStyle>();
        private Dictionary<int, Texture2D> _texturePool = new Dictionary<int, Texture2D>(); // 纹理缓存池
        private List<GUIStyle> _availableStyles = new List<GUIStyle>(); // 可用的GUIStyle对象池
        
        /// <summary>
        /// 获取或创建GUIStyle实例
        /// </summary>
        public GUIStyle GetOrCreateStyle((int styleHashCode, string defaultStyleName) key, GUIStyle defaultStyle)
        {
            // 首先尝试从池中获取
            if (_stylePool.TryGetValue(key, out GUIStyle existingStyle))
            {
                return existingStyle;
            }
            
            // 如果没有找到，创建新的GUIStyle
            GUIStyle newStyle = CreateNewStyle(defaultStyle);
            _stylePool[key] = newStyle;
            return newStyle;
        }
        
        public GUIStyle GetExistedStyle((int styleHashCode, string defaultStyleName) key)
        {
            return _stylePool.TryGetValue(key, out GUIStyle existingStyle) ? existingStyle : null;
        }

        public GUIStyle GetPoolStyle((int styleHashCode, string defaultStyleName) key, GUIStyle defaultStyle)
        { 
            // 如果没有找到，创建新的GUIStyle
            GUIStyle newStyle = CreateNewStyle(defaultStyle);
            _stylePool[key] = newStyle;
            return newStyle;
        }
        
        /// <summary>
        /// 纹理缓存键，包含颜色、边框宽度和圆角信息
        /// </summary>
        private struct TextureCacheKey : IEquatable<TextureCacheKey>
        {
            public int ColorHash;
            public int? BorderWidth;
            public int? BorderRadius;

            public TextureCacheKey(Color color, int? borderWidth, int? borderRadius)
            {
                ColorHash = color.GetHashCode();
                BorderWidth = borderWidth;
                BorderRadius = borderRadius;
            }

            public bool Equals(TextureCacheKey other)
            {
                return ColorHash == other.ColorHash
                    && BorderWidth == other.BorderWidth
                    && BorderRadius == other.BorderRadius;
            }

            public override bool Equals(object obj)
            {
                return obj is TextureCacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + ColorHash;
                    hash = hash * 23 + BorderWidth.GetHashCode();
                    hash = hash * 23 + BorderRadius.GetHashCode();
                    return hash;
                }
            }
        }
        
        /// <summary>
        /// 获取或创建颜色纹理
        /// </summary>
        public Texture2D GetOrCreateColorTexture(Color color, int? borderWidth = null, int? borderRadius = null)
        {
            // 使用颜色、边框宽度和圆角的组合哈希值作为键
            var key = new TextureCacheKey(color, borderWidth, borderRadius);
            
            if (_texturePool.TryGetValue(key.GetHashCode(), out Texture2D existingTexture))
            {
                return existingTexture;
            }
            
            // 创建新的纹理
            Texture2D newTexture = CreateColorTexture(color, borderWidth, borderRadius);
            _texturePool[key.GetHashCode()] = newTexture;
            return newTexture;
        }
        
        /// <summary>
        /// 创建颜色纹理
        /// </summary>
        private Texture2D CreateColorTexture(Color color, int? borderWidth, int? borderRadius)
        {
            // 创建纹理，边框宽度和圆角主要通过GUIStyle的border属性控制
            // 但我们仍然为不同的参数组合创建不同的纹理以确保一致性
            var texture = new Texture2D(2, 2);
            Color[] pixels = new Color[4];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
        
        /// <summary>
        /// 创建新的GUIStyle实例
        /// </summary>
        private GUIStyle CreateNewStyle(GUIStyle defaultStyle)
        {
            // 从可用池中获取或创建新实例
            GUIStyle style;
            if (_availableStyles.Count > 0)
            {
                style = _availableStyles[_availableStyles.Count - 1];
                _availableStyles.RemoveAt(_availableStyles.Count - 1);
                // 重置样式属性
                style.normal = defaultStyle.normal;
                style.hover = defaultStyle.hover;
                style.active = defaultStyle.active;
                style.focused = defaultStyle.focused;
                style.onNormal = defaultStyle.onNormal;
                style.onHover = defaultStyle.onHover;
                style.onActive = defaultStyle.onActive;
                style.onFocused = defaultStyle.onFocused;
                style.border = defaultStyle.border;
                style.margin = defaultStyle.margin;
                style.padding = defaultStyle.padding;
                style.overflow = defaultStyle.overflow;
                style.font = defaultStyle.font;
                style.fontSize = defaultStyle.fontSize;
                style.fontStyle = defaultStyle.fontStyle;
                style.alignment = defaultStyle.alignment;
                style.wordWrap = defaultStyle.wordWrap;
                style.clipping = defaultStyle.clipping;
                style.imagePosition = defaultStyle.imagePosition;
                style.contentOffset = defaultStyle.contentOffset;
                style.fixedWidth = defaultStyle.fixedWidth;
                style.fixedHeight = defaultStyle.fixedHeight;
                style.stretchWidth = defaultStyle.stretchWidth;
                style.stretchHeight = defaultStyle.stretchHeight;
            }
            else
            {
                style = new GUIStyle(defaultStyle);
            }
            
            return style;
        }
        
        /// <summary>
        /// 清理对象池（保留GUIStyle实例以供重用，仅重置映射）
        /// </summary>
        public void Clear()
        {
            _stylePool.Clear();
            // 注意：不清理纹理池，因为纹理可以被多个样式共享
        }
        
        /// <summary>
        /// 释放所有资源（在适当的时候调用，如场景切换）
        /// </summary>
        public void Dispose()
        {
            _stylePool.Clear();
            // 销毁纹理资源
            foreach (var texture in _texturePool.Values)
            {
                if (texture != null)
                {
                    UnityEngine.Object.DestroyImmediate(texture);
                }
            }
            _texturePool.Clear();
            _availableStyles.Clear();
        }
    }


    /// <summary>
    /// Editor环境渲染管理器
    /// </summary>
    public class EditorRenderManager : RenderManager
    {
        // GUIStyle对象池 - 重用GUIStyle对象以减少GC
        private GUIStylePool _guiStylePool = new GUIStylePool();

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
            
            // 注册新布局组件渲染器
            RegisterRenderer<ECanvas>(new EditorECanvasRenderer());
            RegisterRenderer<AbsolutePanel>(new EditorAbsolutePanelRenderer());
            
            // 注册有状态组件渲染器
            RegisterRenderer<StatefulButton>(new EditorStatefulButtonRenderer());
            RegisterRenderer<LongPressButton>(new EditorLongPressButtonRenderer());
            
            // 注册DisableGroup渲染器
            RegisterRenderer<DisableGroup>(new DisableGroupRenderer());
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
            if(existingStyle != null)
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
            
            // 不再设置背景纹理以避免性能问题
            // 如果需要背景色，应在渲染器中使用GUI.backgroundColor处理
            // if (backgroundColor != null)
            // {
            //     var bgTexture = CreateOrGetTexture(backgroundColor.Value, (int?)borderWidth, (int?)borderRadius);
            //     guiStyle.normal.background = bgTexture;
            //     guiStyle.hover.background = bgTexture;
            //     guiStyle.active.background = bgTexture;
            //     guiStyle.focused.background = bgTexture;
            // }
        
            
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
        /// 创建或获取纹理（缓存纹理以避免重复创建）
        /// </summary>
        private Texture2D CreateOrGetTexture(Color color, int? borderWidth = null, int? borderRadius = null)
        {
            return _guiStylePool.GetOrCreateColorTexture(color, borderWidth, borderRadius);
        }
        
        /// <summary>
        /// 清理缓存（在每帧渲染结束后调用）
        /// </summary>
        private void CleanupCaches()
        {
            _guiStylePool.Clear();
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
    }
}