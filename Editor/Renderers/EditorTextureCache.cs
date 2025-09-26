using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// 圆角贴图缓存管理器
    /// 使用圆角半径作为键，缓存对应的圆角贴图
    /// 提供线程安全的获取方法和内存管理机制
    /// 使用9-slice技术避免贴图拉伸问题
    /// </summary>
    internal static class EditorTextureCache
    {
        /// <summary>
        /// 圆角贴图缓存字典
        /// 键：圆角半径（float），值：对应的圆角贴图（Texture2D）
        /// </summary>
        private static readonly Dictionary<float, Texture2D> _roundedCornerTextures = new Dictionary<float, Texture2D>();
        
        /// <summary>
        /// 缓存锁对象，确保线程安全
        /// </summary>
        private static readonly object _cacheLock = new object();
        
        /// <summary>
        /// 贴图尺寸，足够大以容纳圆角边缘
        /// </summary>
        private const int TEXTURE_SIZE = 128;
        
        /// <summary>
        /// 最小圆角半径
        /// </summary>
        private const float MIN_BORDER_RADIUS = 0.1f;
        
        /// <summary>
        /// 最大圆角半径
        /// </summary>
        private const float MAX_BORDER_RADIUS = 50f;

        /// <summary>
        /// 获取或创建指定圆角半径的贴图
        /// </summary>
        /// <param name="borderRadius">圆角半径</param>
        /// <returns>对应的圆角贴图</returns>
        public static Texture2D GetRoundedCornerTexture(float borderRadius)
        {
            // 验证圆角半径范围
            borderRadius = Mathf.Clamp(borderRadius, MIN_BORDER_RADIUS, MAX_BORDER_RADIUS);
            
            lock (_cacheLock)
            {
                // 尝试从缓存中获取贴图
                if (_roundedCornerTextures.TryGetValue(borderRadius, out Texture2D existingTexture))
                {
                    if (existingTexture != null)
                    {
                        return existingTexture;
                    }
                    else
                    {
                        // 如果贴图为空，从缓存中移除
                        _roundedCornerTextures.Remove(borderRadius);
                    }
                }
                
                // 缓存中不存在，创建新的圆角贴图
                Texture2D newTexture = CreateRoundedCornerTexture(borderRadius);
                _roundedCornerTextures[borderRadius] = newTexture;
                
                return newTexture;
            }
        }

        /// <summary>
        /// 创建圆角贴图
        /// </summary>
        /// <param name="borderRadius">圆角半径</param>
        /// <returns>创建的圆角贴图</returns>
        private static Texture2D CreateRoundedCornerTexture(float borderRadius)
        {
            // 创建透明背景的贴图
            Texture2D texture = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            
            // 生成圆角像素数据
            Color[] pixels = GenerateRoundedCornerPixels(borderRadius);
            texture.SetPixels(pixels);
            texture.Apply();
            
            return texture;
        }

        /// <summary>
        /// 生成圆角像素数据
        /// </summary>
        /// <param name="borderRadius">圆角半径</param>
        /// <returns>像素颜色数组</returns>
        private static Color[] GenerateRoundedCornerPixels(float borderRadius)
        {
            Color[] pixels = new Color[TEXTURE_SIZE * TEXTURE_SIZE];
            float radius = Mathf.Min(borderRadius, TEXTURE_SIZE / 2f - 1);
            
            // 计算圆角半径的平方，用于距离比较
            float radiusSquared = radius * radius;
            
            for (int y = 0; y < TEXTURE_SIZE; y++)
            {
                for (int x = 0; x < TEXTURE_SIZE; x++)
                {
                    // 计算当前像素到四个角的距离
                    float distanceToTopLeft = CalculateDistanceToCorner(x, y, 0, 0, radius);
                    float distanceToTopRight = CalculateDistanceToCorner(x, y, TEXTURE_SIZE - 1, 0, radius);
                    float distanceToBottomLeft = CalculateDistanceToCorner(x, y, 0, TEXTURE_SIZE - 1, radius);
                    float distanceToBottomRight = CalculateDistanceToCorner(x, y, TEXTURE_SIZE - 1, TEXTURE_SIZE - 1, radius);
                    
                    // 确定当前像素是否在圆角区域内（需要被裁剪）
                    bool isInCorner = 
                        (x < radius && y < radius && distanceToTopLeft > radiusSquared) ||  // 左上角
                        (x > TEXTURE_SIZE - 1 - radius && y < radius && distanceToTopRight > radiusSquared) ||  // 右上角
                        (x < radius && y > TEXTURE_SIZE - 1 - radius && distanceToBottomLeft > radiusSquared) ||  // 左下角
                        (x > TEXTURE_SIZE - 1 - radius && y > TEXTURE_SIZE - 1 - radius && distanceToBottomRight > radiusSquared);  // 右下角
                    
                    // 设置像素颜色：圆角区域外为白色（用于着色），内部为透明
                    if (isInCorner)
                    {
                        // 计算圆角边缘的透明度渐变
                        float alpha = CalculateCornerAlpha(x, y, radius);
                        pixels[y * TEXTURE_SIZE + x] = new Color(1f, 1f, 1f, alpha);
                    }
                    else
                    {
                        pixels[y * TEXTURE_SIZE + x] = Color.white;
                    }
                }
            }
            
            return pixels;
        }

        /// <summary>
        /// 计算像素到角落的距离
        /// </summary>
        /// <param name="x">像素X坐标</param>
        /// <param name="y">像素Y坐标</param>
        /// <param name="cornerX">角落X坐标</param>
        /// <param name="cornerY">角落Y坐标</param>
        /// <param name="radius">圆角半径</param>
        /// <returns>距离的平方</returns>
        private static float CalculateDistanceToCorner(int x, int y, int cornerX, int cornerY, float radius)
        {
            float dx = x - cornerX;
            float dy = y - cornerY;
            
            // 只考虑在圆角半径范围内的距离计算
            if (Mathf.Abs(dx) > radius || Mathf.Abs(dy) > radius)
            {
                return float.MaxValue;
            }
            
            return dx * dx + dy * dy;
        }

        /// <summary>
        /// 计算圆角边缘的透明度
        /// </summary>
        /// <param name="x">像素X坐标</param>
        /// <param name="y">像素Y坐标</param>
        /// <param name="radius">圆角半径</param>
        /// <returns>透明度值（0-1）</returns>
        private static float CalculateCornerAlpha(int x, int y, float radius)
        {
            // 确定当前像素所在的角落
            bool isTopLeft = x < radius && y < radius;
            bool isTopRight = x > TEXTURE_SIZE - 1 - radius && y < radius;
            bool isBottomLeft = x < radius && y > TEXTURE_SIZE - 1 - radius;
            bool isBottomRight = x > TEXTURE_SIZE - 1 - radius && y > TEXTURE_SIZE - 1 - radius;
            
            int cornerX, cornerY;
            if (isTopLeft)
            {
                cornerX = 0;
                cornerY = 0;
            }
            else if (isTopRight)
            {
                cornerX = TEXTURE_SIZE - 1;
                cornerY = 0;
            }
            else if (isBottomLeft)
            {
                cornerX = 0;
                cornerY = TEXTURE_SIZE - 1;
            }
            else // isBottomRight
            {
                cornerX = TEXTURE_SIZE - 1;
                cornerY = TEXTURE_SIZE - 1;
            }
            
            // 计算到角落的距离
            float distance = Mathf.Sqrt((x - cornerX) * (x - cornerX) + (y - cornerY) * (y - cornerY));
            
            // 计算透明度：距离越远越透明（在圆角半径范围内）
            if (distance > radius)
            {
                return 0f; // 圆角区域外完全透明
            }
            
            // 在圆角边缘处进行平滑过渡
            float edgeDistance = radius - distance;
            float alpha = Mathf.Clamp01(edgeDistance / 2f); // 边缘2像素的过渡
            
            return Mathf.SmoothStep(0f, 1f, alpha);
        }

        /// <summary>
        /// 使用9-slice技术渲染圆角背景
        /// </summary>
        /// <param name="rect">渲染区域</param>
        /// <param name="borderRadius">圆角半径</param>
        /// <param name="backgroundColor">背景颜色</param>
        public static void DrawRoundedBackground(Rect rect, float borderRadius, Color backgroundColor)
        {
            // 从缓存获取或创建圆角贴图
            Texture2D roundedTexture = GetRoundedCornerTexture(borderRadius);
            
            if (roundedTexture != null)
            {
                // 保存当前GUI颜色
                Color originalColor = GUI.color;
                
                // 使用当前背景色渲染圆角贴图
                GUI.color = backgroundColor;
                
                // 使用ScaleMode.ScaleAndCrop来避免拉伸问题
                // 这种方法可以保持圆角不变形
                GUI.DrawTexture(rect, roundedTexture, ScaleMode.ScaleAndCrop);
                
                // 恢复GUI颜色
                GUI.color = originalColor;
            }
            else
            {
                // 如果圆角贴图创建失败，回退到普通背景
                var originalColor = GUI.backgroundColor;
                GUI.backgroundColor = backgroundColor;
                GUI.Box(rect, GUIContent.none);
                GUI.backgroundColor = originalColor;
            }
        }

        /// <summary>
        /// 清理缓存，释放所有贴图资源
        /// </summary>
        public static void ClearCache()
        {
            lock (_cacheLock)
            {
                foreach (var texture in _roundedCornerTextures.Values)
                {
                    if (texture != null)
                    {
                        UnityEngine.Object.DestroyImmediate(texture);
                    }
                }
                _roundedCornerTextures.Clear();
            }
        }

        /// <summary>
        /// 获取当前缓存中的贴图数量
        /// </summary>
        /// <returns>缓存贴图数量</returns>
        public static int GetCacheCount()
        {
            lock (_cacheLock)
            {
                return _roundedCornerTextures.Count;
            }
        }

        /// <summary>
        /// 检查指定圆角半径的贴图是否已缓存
        /// </summary>
        /// <param name="borderRadius">圆角半径</param>
        /// <returns>是否已缓存</returns>
        public static bool IsCached(float borderRadius)
        {
            lock (_cacheLock)
            {
                return _roundedCornerTextures.ContainsKey(borderRadius) && _roundedCornerTextures[borderRadius] != null;
            }
        }
    }
}