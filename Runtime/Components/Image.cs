using System;
using UnityEngine;
using DeclGUI.Core;

namespace DeclGUI.Components
{
    /// <summary>
    /// 图像组件 - 支持多种贴图格式的渲染
    /// </summary>
    public struct Image : IEventfulElement, IStylefulElement
    {
        /// <summary>
        /// 元素唯一标识符
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 要显示的纹理
        /// </summary>
        public Texture Texture { get; }

        /// <summary>
        /// 图像样式
        /// </summary>
        public IDeclStyle Style { get; }

        /// <summary>
        /// 图像显示模式（拉伸、适应、填充等）
        /// </summary>
        public ScaleMode ScaleMode { get; }
        
        /// <summary>
        /// 九宫格边距（用于九宫格切片）
        /// </summary>
        public RectOffset SlicedBorder { get; }
        
        /// <summary>
        /// 是否使用九宫格切片模式
        /// </summary>
        public bool UseSlicedMode { get; }

        /// <summary>
        /// 图像透明度
        /// </summary>
        public float Alpha { get; }

        /// <summary>
        /// 图像色调
        /// </summary>
        public Color TintColor { get; }

        /// <summary>
        /// UV坐标（用于纹理映射）
        /// </summary>
        public Rect UVRect { get; }

        /// <summary>
        /// 事件注册器
        /// </summary>
        public DeclEvent Events { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="texture">要显示的纹理</param>
        /// <param name="scaleMode">缩放模式</param>
        /// <param name="alpha">透明度</param>
        /// <param name="tintColor">色调</param>
        /// <param name="uvRect">UV坐标</param>
        /// <param name="slicedBorder">九宫格边距</param>
        /// <param name="useSlicedMode">是否使用九宫格切片模式</param>
        /// <param name="style">样式</param>
        public Image(
            Texture texture,
            ScaleMode scaleMode = ScaleMode.ScaleToFit,
            float alpha = 1.0f,
            Color? tintColor = null,
            Rect? uvRect = null,
            RectOffset slicedBorder = null,
            bool useSlicedMode = false,
            IDeclStyle? style = null)
        {
            Key = null;
            Texture = texture;
            ScaleMode = scaleMode;
            Alpha = alpha;
            TintColor = tintColor ?? Color.white;
            UVRect = uvRect ?? new Rect(0, 0, 1, 1);
            SlicedBorder = slicedBorder ?? new RectOffset();
            UseSlicedMode = useSlicedMode;
            Style = style;
            Events = new DeclEvent();
        }

        /// <summary>
        /// 构造函数（使用事件系统）
        /// </summary>
        /// <param name="texture">要显示的纹理</param>
        /// <param name="events">事件注册器</param>
        /// <param name="scaleMode">缩放模式</param>
        /// <param name="alpha">透明度</param>
        /// <param name="tintColor">色调</param>
        /// <param name="uvRect">UV坐标</param>
        /// <param name="slicedBorder">九宫格边距</param>
        /// <param name="useSlicedMode">是否使用九宫格切片模式</param>
        /// <param name="style">样式</param>
        public Image(
            Texture texture,
            DeclEvent events = default,
            ScaleMode scaleMode = ScaleMode.ScaleToFit,
            float alpha = 1.0f,
            Color? tintColor = null,
            Rect? uvRect = null,
            RectOffset slicedBorder = null,
            bool useSlicedMode = false,
            IDeclStyle? style = null)
        {
            Key = null;
            Texture = texture;
            ScaleMode = scaleMode;
            Alpha = alpha;
            TintColor = tintColor ?? Color.white;
            UVRect = uvRect ?? new Rect(0, 0, 1, 1);
            SlicedBorder = slicedBorder ?? new RectOffset();
            UseSlicedMode = useSlicedMode;
            Style = style;
            Events = events;
        }

        /// <summary>
        /// 渲染方法，返回自身
        /// </summary>
        /// <returns>当前图像实例</returns>
        public IElement Render() => null;

        /// <summary>
        /// 绑定事件处理器
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="handler">事件处理器</param>
        public void BindEvent(DeclEventType eventType, Action handler)
        {
            var events = Events;
            events.SetHandler(eventType, handler);
            Events = events;
        }

        /// <summary>
        /// 解绑事件处理器
        /// </summary>
        /// <param name="eventType">事件类型</param>
        public void UnbindEvent(DeclEventType eventType)
        {
            var events = Events;
            events.SetHandler(eventType, null);
            Events = events;
        }

        /// <summary>
        /// IStylefulElement 接口实现
        /// </summary>
        IDeclStyle IStylefulElement.Style => Style;

        /// <summary>
        /// IStylefulElement 显式实现，返回 IStylefulElement
        /// </summary>
        IStylefulElement IStylefulElement.WithStyle(IDeclStyle style)
        {
            return WithStyle(style);
        }

        /// <summary>
        /// 便于链式调用的 WithStyle，返回 Image 类型
        /// </summary>
        public Image WithStyle(IDeclStyle style)
        {
            var newImage = new Image(Texture, Events, ScaleMode, Alpha, TintColor, UVRect, SlicedBorder, UseSlicedMode, style);
            newImage.Key = Key;
            return newImage;
        }

        /// <summary>
        /// 使用新的纹理创建图像实例
        /// </summary>
        public Image WithTexture(Texture texture)
        {
            var newImage = new Image(texture, Events, ScaleMode, Alpha, TintColor, UVRect, SlicedBorder, UseSlicedMode, Style);
            newImage.Key = Key;
            return newImage;
        }

        /// <summary>
        /// 使用新的缩放模式创建图像实例
        /// </summary>
        public Image WithScaleMode(ScaleMode scaleMode)
        {
            var newImage = new Image(Texture, Events, scaleMode, Alpha, TintColor, UVRect, SlicedBorder, UseSlicedMode, Style);
            newImage.Key = Key;
            return newImage;
        }

        /// <summary>
        /// 使用新的透明度创建图像实例
        /// </summary>
        public Image WithAlpha(float alpha)
        {
            var newImage = new Image(Texture, Events, ScaleMode, alpha, TintColor, UVRect, SlicedBorder, UseSlicedMode, Style);
            newImage.Key = Key;
            return newImage;
        }

        /// <summary>
        /// 使用新的色调创建图像实例
        /// </summary>
        public Image WithTintColor(Color tintColor)
        {
            var newImage = new Image(Texture, Events, ScaleMode, Alpha, tintColor, UVRect, SlicedBorder, UseSlicedMode, Style);
            newImage.Key = Key;
            return newImage;
        }

        /// <summary>
        /// 使用新的UV矩形创建图像实例
        /// </summary>
        public Image WithUVRect(Rect uvRect)
        {
            var newImage = new Image(Texture, Events, ScaleMode, Alpha, TintColor, uvRect, SlicedBorder, UseSlicedMode, Style);
            newImage.Key = Key;
            return newImage;
        }
        
        /// <summary>
        /// 使用新的九宫格边距创建图像实例
        /// </summary>
        public Image WithSlicedBorder(RectOffset slicedBorder)
        {
            var newImage = new Image(Texture, Events, ScaleMode, Alpha, TintColor, UVRect, slicedBorder, UseSlicedMode, Style);
            newImage.Key = Key;
            return newImage;
        }
        
        /// <summary>
        /// 使用新的九宫格模式创建图像实例
        /// </summary>
        public Image WithUseSlicedMode(bool useSlicedMode)
        {
            var newImage = new Image(Texture, Events, ScaleMode, Alpha, TintColor, UVRect, SlicedBorder, useSlicedMode, Style);
            newImage.Key = Key;
            return newImage;
        }
        
        /// <summary>
        /// 设置为原始尺寸
        /// </summary>
        public Image SetNativeSize()
        {
            if (Texture != null)
            {
                var newStyle = new DeclStyle(Texture.width, Texture.height);
                var currentStyle = Style as DeclStyle?;
                if (currentStyle.HasValue)
                {
                    newStyle = currentStyle.Value.SetSize(Texture.width, Texture.height);
                }
                else
                {
                    newStyle = new DeclStyle(Texture.width, Texture.height);
                }
                
                var newImage = new Image(Texture, Events, ScaleMode, Alpha, TintColor, UVRect, SlicedBorder, UseSlicedMode, newStyle);
                newImage.Key = Key;
                return newImage;
            }
            return this;
        }
    }
}