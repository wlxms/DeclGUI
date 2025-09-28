using UnityEngine;
using UnityEditor;
using DeclGUI.Core;
using DeclGUI.Components;
using System;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// Image组件的Editor渲染器
    /// 支持多种贴图格式的渲染，包括纹理映射、缩放适配、透明度处理等核心渲染功能
    /// </summary>
    public class EditorImageRenderer : EditorElementRenderer<Image>
    {
        public override void Render(RenderManager mgr, in Image element, in IDeclStyle styleParam)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            var currentStyle = styleParam ?? element.Style;
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);
            
            // 保存原始颜色
            var originalBackgroundColor = GUI.backgroundColor;
            var originalColor = GUI.color;
            var originalContentColor = GUI.contentColor;

            // 应用样式颜色
            if (currentStyle?.BackgroundColor != null)
            {
                GUI.backgroundColor = currentStyle.BackgroundColor.Value;
            }
            
            // 应用透明度和色调
            var finalColor = currentStyle?.Color ?? element.TintColor;
            finalColor.a *= element.Alpha; // 应用透明度
            GUI.color = finalColor;
            GUI.contentColor = finalColor;

            try
            {
                // 获取纹理
                var texture = element.Texture;
                if (texture != null)
                {
                    // 确定绘制矩形
                    Rect drawRect;
                    if (width > 0 && height > 0)
                    {
                        drawRect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                    }
                    else
                    {
                        // 如果没有指定尺寸，使用可用空间
                        drawRect = GUILayoutUtility.GetRect(100, 100, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    }

                    // 使用适当的缩放模式绘制纹理
                    if (element.UseSlicedMode && element.SlicedBorder != null)
                    {
                        // 使用九宫格切片模式绘制
                        DrawSlicedTexture(drawRect, texture, element.SlicedBorder, element.UVRect);
                    }
                    else
                    {
                        // 根据指定的缩放模式绘制 - 使用缓存的GUI.color
                        switch (element.ScaleMode)
                        {
                            case ScaleMode.StretchToFill:
                                GUI.DrawTexture(drawRect, texture, ScaleMode.StretchToFill);
                                break;
                            case ScaleMode.ScaleAndCrop:
                                GUI.DrawTexture(drawRect, texture, ScaleMode.ScaleAndCrop);
                                break;
                            case ScaleMode.ScaleToFit:
                            default:
                                GUI.DrawTexture(drawRect, texture, ScaleMode.ScaleToFit);
                                break;
                        }
                    }
                }
                else
                {
                    // 如果没有纹理，绘制一个占位矩形
                    Rect placeholderRect;
                    if (width > 0 && height > 0)
                    {
                        placeholderRect = GUILayoutUtility.GetRect(width, height);
                    }
                    else
                    {
                        placeholderRect = GUILayoutUtility.GetRect(10, 100);
                    }
                    
                    // 绘制占位矩形
                    EditorGUI.DrawRect(placeholderRect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
                    
                    // 绘制占位文本
                    var labelStyle = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        normal = { textColor = Color.white }
                    };
                    GUI.Label(placeholderRect, "No Texture", labelStyle);
                }
            }
            finally
            {
                // 恢复原始颜色
                GUI.backgroundColor = originalBackgroundColor;
                GUI.color = originalColor;
                GUI.contentColor = originalContentColor;
            }
        }

        /// <summary>
        /// 绘制九宫格切片纹理
        /// </summary>
        /// <param name="rect">绘制区域</param>
        /// <param name="texture">纹理</param>
        /// <param name="border">九宫格边距</param>
        /// <param name="uvRect">UV坐标</param>
        private void DrawSlicedTexture(Rect rect, Texture texture, RectOffset border, Rect uvRect = default)
        {
            if (texture == null || border == null)
                return;

            // 使用默认UV矩形，如果未提供
            if (uvRect == default)
            {
                uvRect = new Rect(0,0, 1, 1);
            }

            // 预计算所有需要的值，避免重复计算
            var texWidth = texture.width;
            var texHeight = texture.height;
            
            // 计算UV坐标
            var uvLeft = uvRect.x + (float)border.left / texWidth * uvRect.width;
            var uvRight = uvRect.x + uvRect.width - (float)border.right / texWidth * uvRect.width;
            var uvTop = uvRect.y + (float)border.top / texHeight * uvRect.height;
            var uvBottom = uvRect.y + uvRect.height - (float)border.bottom / texHeight * uvRect.height;
            
            // 计算目标坐标
            var dstLeft = rect.x;
            var dstTop = rect.y;
            var dstRight = rect.x + rect.width;
            var dstBottom = rect.y + rect.height;
            
            // 计算边框尺寸
            var dstLeftBorder = border.left;
            var dstTopBorder = border.top;
            var dstRightBorder = border.right;
            var dstBottomBorder = border.bottom;
            
            // 计算中心区域尺寸
            var dstCenterWidth = Mathf.Max(0, rect.width - dstLeftBorder - dstRightBorder);
            var dstCenterHeight = Mathf.Max(0, rect.height - dstTopBorder - dstBottomBorder);

            // 绘制九个区域，只绘制非零尺寸的区域
            // 左上角
            if (dstLeftBorder > 0 && dstTopBorder > 0)
            {
                GUI.DrawTextureWithTexCoords(
                    new Rect(dstLeft, dstTop, dstLeftBorder, dstTopBorder),
                    texture,
                    new Rect(uvRect.x, uvRect.y, uvLeft - uvRect.x, uvTop - uvRect.y));
            }

            // 顶部边缘
            if (dstTopBorder > 0 && dstCenterWidth > 0)
            {
                GUI.DrawTextureWithTexCoords(
                    new Rect(dstLeft + dstLeftBorder, dstTop, dstCenterWidth, dstTopBorder),
                    texture,
                    new Rect(uvLeft, uvRect.y, uvRight - uvLeft, uvTop - uvRect.y));
            }

            // 右上角
            if (dstRightBorder > 0 && dstTopBorder > 0)
            {
                GUI.DrawTextureWithTexCoords(
                    new Rect(dstRight - dstRightBorder, dstTop, dstRightBorder, dstTopBorder),
                    texture,
                    new Rect(uvRight, uvRect.y, uvRect.x + uvRect.width - uvRight, uvTop - uvRect.y));
            }

            // 左侧边缘
            if (dstLeftBorder > 0 && dstCenterHeight > 0)
            {
                GUI.DrawTextureWithTexCoords(
                    new Rect(dstLeft, dstTop + dstTopBorder, dstLeftBorder, dstCenterHeight),
                    texture,
                    new Rect(uvRect.x, uvTop, uvLeft - uvRect.x, uvBottom - uvTop));
            }

            // 中心
            if (dstCenterWidth > 0 && dstCenterHeight > 0)
            {
                GUI.DrawTextureWithTexCoords(
                    new Rect(dstLeft + dstLeftBorder, dstTop + dstTopBorder, dstCenterWidth, dstCenterHeight),
                    texture,
                    new Rect(uvLeft, uvTop, uvRight - uvLeft, uvBottom - uvTop));
            }

            // 右侧边缘
            if (dstRightBorder > 0 && dstCenterHeight > 0)
            {
                GUI.DrawTextureWithTexCoords(
                    new Rect(dstRight - dstRightBorder, dstTop + dstTopBorder, dstRightBorder, dstCenterHeight),
                    texture,
                    new Rect(uvRight, uvTop, uvRect.x + uvRect.width - uvRight, uvBottom - uvTop));
            }

            // 左下角
            if (dstLeftBorder > 0 && dstBottomBorder > 0)
            {
                GUI.DrawTextureWithTexCoords(
                    new Rect(dstLeft, dstBottom - dstBottomBorder, dstLeftBorder, dstBottomBorder),
                    texture,
                    new Rect(uvRect.x, uvBottom, uvLeft - uvRect.x, uvRect.y + uvRect.height - uvBottom));
            }

            // 底部边缘
            if (dstBottomBorder > 0 && dstCenterWidth > 0)
            {
                GUI.DrawTextureWithTexCoords(
                    new Rect(dstLeft + dstLeftBorder, dstBottom - dstBottomBorder, dstCenterWidth, dstBottomBorder),
                    texture,
                    new Rect(uvLeft, uvBottom, uvRight - uvLeft, uvRect.y + uvRect.height - uvBottom));
            }

            // 右下角
            if (dstRightBorder > 0 && dstBottomBorder > 0)
            {
                GUI.DrawTextureWithTexCoords(
                    new Rect(dstRight - dstRightBorder, dstBottom - dstBottomBorder, dstRightBorder, dstBottomBorder),
                    texture,
                    new Rect(uvRight, uvBottom, uvRect.x + uvRect.width - uvRight, uvRect.y + uvRect.height - uvBottom));
            }
        }

        /// <summary>
        /// 获取Image元素的屏幕区域
        /// </summary>
        /// <returns>Image的屏幕矩形</returns>
        public override Rect GetElementRect()
        {
            // 在Editor环境下，可以使用GUILayoutUtility.GetLastRect()获取最后渲染的矩形
            return GUILayoutUtility.GetLastRect();
        }

        /// <summary>
        /// 计算Image的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in Image element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            var currentStyle = style ?? element.Style;
            var width = editorMgr.GetStyleWidth(currentStyle);
            var height = editorMgr.GetStyleHeight(currentStyle);

            // 如果设置了固定尺寸，直接返回
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            // 如果纹理存在，基于纹理尺寸和缩放模式计算大小
            if (element.Texture != null)
            {
                var texWidth = element.Texture.width;
                var texHeight = element.Texture.height;

                // 检查是否应用了SetNativeSize - 这时样式中应该包含明确的尺寸
                if (currentStyle?.Width.HasValue == true && currentStyle?.Height.HasValue == true)
                {
                    return new Vector2(currentStyle.Width.Value, currentStyle.Height.Value);
                }
                
                // 如果只设置了宽度或高度，按比例计算另一个维度
                if (width > 0)
                {
                    var aspect = (float)texHeight / texWidth;
                    return new Vector2(width, width * aspect);
                }
                else if (height > 0)
                {
                    var aspect = (float)texWidth / texHeight;
                    return new Vector2(height * aspect, height);
                }
                else
                {
                    // 没有指定尺寸，返回默认大小而不是纹理原始尺寸
                    return new Vector2(100, 100);
                }
            }

            // 没有纹理，返回默认大小
            return new Vector2(100, 100);
        }
    }
}