using System;
using UnityEngine;
using UnityEditor;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// DeclGUI Editor GUI 工具类
    /// 提供常用的编辑器GUI渲染方法封装
    /// </summary>
    public static class DeclEditorGUI
    {
        #region 分割线渲染方法

        /// <summary>
        /// 渲染分割线
        /// </summary>
        /// <param name="height">分割线高度</param>
        /// <param name="startOffset">起始偏移量</param>
        /// <param name="topSpacing">上间距</param>
        /// <param name="bottomSpacing">下间距</param>
        /// <param name="color">分割线颜色</param>
        public static void DrawSplitLine(float height = 1f, float startOffset = 0f, float topSpacing = 0f, float bottomSpacing = 0f, Color? color = null)
        {
            // 添加上间距
            if (topSpacing > 0)
            {
                GUILayout.Space(topSpacing);
            }

            // 获取分割线矩形
            var splitRect = GUILayoutUtility.GetRect(0, height, GUILayout.ExpandWidth(true));

            // 应用起始偏移量
            splitRect.x += startOffset;
            splitRect.width -= startOffset;

            // 保存原始颜色
            var originalColor = GUI.color;

            try
            {
                // 应用指定颜色
                if (color.HasValue)
                {
                    GUI.color = color.Value;
                }

                // 渲染分割线
                GUI.Box(splitRect, GUIContent.none);
            }
            finally
            {
                // 恢复原始颜色
                GUI.color = originalColor;
            }

            // 添加下间距
            if (bottomSpacing > 0)
            {
                GUILayout.Space(bottomSpacing);
            }
        }

        /// <summary>
        /// 渲染分割线（带默认样式）
        /// </summary>
        /// <param name="startOffset">起始偏移量</param>
        /// <param name="topSpacing">上间距</param>
        /// <param name="bottomSpacing">下间距</param>
        public static void DrawSplitLineWithDefaultStyle(float startOffset = 10f, float topSpacing = 2f, float bottomSpacing = 2f)
        {
            DrawSplitLine(1f, startOffset, topSpacing, bottomSpacing, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        }

        #endregion

        #region 颜色渲染方法

        /// <summary>
        /// 使用指定背景颜色渲染内容
        /// </summary>
        /// <param name="backgroundColor">背景颜色</param>
        /// <param name="content">要渲染的内容</param>
        public static void RenderWithBackgroundColor(Color backgroundColor, Action content)
        {
            var originalBackgroundColor = GUI.backgroundColor;

            try
            {
                GUI.backgroundColor = backgroundColor;
                content?.Invoke();
            }
            finally
            {
                GUI.backgroundColor = originalBackgroundColor;
            }
        }

        /// <summary>
        /// 使用指定颜色渲染内容
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="content">要渲染的内容</param>
        public static void RenderWithColor(Color color, Action content)
        {
            var originalColor = GUI.color;

            try
            {
                GUI.color = color;
                content?.Invoke();
            }
            finally
            {
                GUI.color = originalColor;
            }
        }

        /// <summary>
        /// 使用指定背景颜色渲染内容（返回IDisposable）
        /// </summary>
        /// <param name="backgroundColor">背景颜色</param>
        /// <returns>可释放的颜色渲染上下文</returns>
        public static IDisposable BeginBackgroundColor(Color? backgroundColor)
        {
            return new BackgroundColorScope(backgroundColor);
        }

        /// <summary>
        /// 使用指定颜色渲染内容（返回IDisposable）
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns>可释放的颜色渲染上下文</returns>
        public static IDisposable BeginColor(Color color)
        {
            return new ColorScope(color);
        }

        #endregion

        #region 其他有用的渲染工具方法

        /// <summary>
        /// 渲染带边框的盒子
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="borderColor">边框颜色</param>
        /// <param name="backgroundColor">背景颜色</param>
        public static void DrawBorderedBox(Action content, Color? borderColor = null, Color? backgroundColor = null)
        {
            using (BeginBorderedBox(borderColor, backgroundColor))
            {
                content?.Invoke();
            }
        }

        /// <summary>
        /// 开始带边框的盒子区域（返回IDisposable）
        /// </summary>
        /// <param name="borderColor">边框颜色</param>
        /// <param name="backgroundColor">背景颜色</param>
        /// <returns>可释放的盒子渲染上下文</returns>
        public static IDisposable BeginBorderedBox(Color? borderColor = null, Color? backgroundColor = null)
        {
            return new BorderedBoxScope(borderColor, backgroundColor);
        }

        /// <summary>
        /// 渲染带标题的区域
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="isExpanded">是否展开</param>
        /// <returns>新的展开状态</returns>
        public static bool DrawFoldoutArea(string title, Action content, bool isExpanded = false)
        {
            var newExpandedState = EditorGUILayout.Foldout(isExpanded, title, true);

            if (newExpandedState)
            {
                EditorGUI.indentLevel++;
                content?.Invoke();
                EditorGUI.indentLevel--;
            }

            return newExpandedState;
        }

        /// <summary>
        /// 开始折叠区域（返回IDisposable）
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="isExpanded">是否展开</param>
        /// <returns>折叠区域上下文</returns>
        public static FoldoutAreaScope BeginFoldoutArea(string title, bool isExpanded = false)
        {
            return new FoldoutAreaScope(title, isExpanded);
        }

        /// <summary>
        /// 渲染水平分隔区域
        /// </summary>
        /// <param name="content">内容</param>
        public static void HorizontalGroup(Action content)
        {
            using (BeginHorizontal())
            {
                content?.Invoke();
            }
        }

        /// <summary>
        /// 开始水平分隔区域（返回IDisposable）
        /// </summary>
        /// <returns>可释放的水平布局上下文</returns>
        public static IDisposable BeginHorizontal(params GUILayoutOption[] options)
        {
            return new HorizontalScope(options);
        }

        /// <summary>
        /// 渲染垂直分隔区域
        /// </summary>
        /// <param name="content">内容</param>
        public static void VerticalGroup(Action content)
        {
            using (BeginVertical())
            {
                content?.Invoke();
            }
        }

        /// <summary>
        /// 开始垂直分隔区域（返回IDisposable）
        /// </summary>
        /// <returns>可释放的垂直布局上下文</returns>
        public static IDisposable BeginVertical(params GUILayoutOption[] options)
        {
            return new VerticalScope(options);
        }

        /// <summary>
        /// 渲染禁用区域
        /// </summary>
        /// <param name="isDisabled">是否禁用</param>
        /// <param name="content">内容</param>
        public static void DisabledGroup(bool isDisabled, Action content)
        {
            using (BeginDisabledGroup(isDisabled))
            {
                content?.Invoke();
            }
        }

        /// <summary>
        /// 开始禁用区域（返回IDisposable）
        /// </summary>
        /// <param name="isDisabled">是否禁用</param>
        /// <returns>可释放的禁用上下文</returns>
        public static IDisposable BeginDisabledGroup(bool isDisabled)
        {
            return new DisabledScope(isDisabled);
        }

        /// <summary>
        /// 渲染带标签的字段
        /// </summary>
        /// <param name="label">标签</param>
        /// <param name="content">字段内容</param>
        public static void LabeledField(string label, Action content)
        {
            using (BeginHorizontal())
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth));
                content?.Invoke();
            }
        }

        #endregion

        #region 内部辅助类

        /// <summary>
        /// 背景颜色作用域
        /// </summary>
        private struct BackgroundColorScope : IDisposable
        {
            private readonly Color _originalBackgroundColor;

            public BackgroundColorScope(Color? backgroundColor)
            {
                _originalBackgroundColor = GUI.backgroundColor;
                if (backgroundColor.HasValue)
                    GUI.backgroundColor = backgroundColor.Value;
            }

            public void Dispose()
            {
                GUI.backgroundColor = _originalBackgroundColor;
            }
        }

        /// <summary>
        /// 颜色作用域
        /// </summary>
        private struct ColorScope : IDisposable
        {
            private readonly Color _originalColor;

            public ColorScope(Color? color)
            {
                _originalColor = GUI.color;
                if (color.HasValue)
                    GUI.color = color.Value;
            }

            public void Dispose()
            {
                GUI.color = _originalColor;
            }
        }

        /// <summary>
        /// 带边框的盒子作用域
        /// </summary>
        private struct BorderedBoxScope : IDisposable
        {
            private readonly Color _originalBackgroundColor;
            private readonly Color _originalColor;

            public BorderedBoxScope(Color? borderColor, Color? backgroundColor)
            {
                _originalBackgroundColor = GUI.backgroundColor;
                _originalColor = GUI.color;

                // 应用背景颜色
                if (backgroundColor.HasValue)
                {
                    GUI.backgroundColor = backgroundColor.Value;
                }

                // 应用边框颜色
                if (borderColor.HasValue)
                {
                    GUI.color = borderColor.Value;
                }

                GUILayout.BeginVertical(GUI.skin.box);
            }

            public void Dispose()
            {
                GUILayout.EndVertical();
                GUI.backgroundColor = _originalBackgroundColor;
                GUI.color = _originalColor;
            }
        }

        /// <summary>
        /// 折叠区域作用域
        /// </summary>
        public struct FoldoutAreaScope : IDisposable
        {
            private readonly bool _isExpanded;

            public FoldoutAreaScope(string title, bool isExpanded)
            {
                _isExpanded = EditorGUILayout.Foldout(isExpanded, title, true);

                if (_isExpanded)
                {
                    EditorGUI.indentLevel++;
                }
            }

            /// <summary>
            /// 获取是否展开
            /// </summary>
            public bool IsExpanded => _isExpanded;

            public void Dispose()
            {
                if (_isExpanded)
                {
                    EditorGUI.indentLevel--;
                }
            }
        }

        /// <summary>
        /// 水平布局作用域
        /// </summary>
        private class HorizontalScope : IDisposable
        {
            public HorizontalScope(params GUILayoutOption[] options)
            {
                GUILayout.BeginHorizontal(options);
            }

            public void Dispose()
            {
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 垂直布局作用域
        /// </summary>
        private class VerticalScope : IDisposable
        {
            public VerticalScope(params GUILayoutOption[] options)
            {
                GUILayout.BeginVertical(options);
            }

            public void Dispose()
            {
                GUILayout.EndVertical();
            }
        }

        /// <summary>
        /// 禁用作用域
        /// </summary>
        private struct DisabledScope : IDisposable
        {
            public DisabledScope(bool isDisabled)
            {
                EditorGUI.BeginDisabledGroup(isDisabled);
            }

            public void Dispose()
            {
                EditorGUI.EndDisabledGroup();
            }
        }

        #endregion
    }
}