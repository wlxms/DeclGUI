using UnityEngine;
using System;
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
}