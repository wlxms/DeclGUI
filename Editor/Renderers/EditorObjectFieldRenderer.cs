using DeclGUI.Components;
using DeclGUI.Core;
using System;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// ObjectField组件的Editor渲染器基类
    /// 处理所有ObjectField<T>类型
    /// </summary>
    public class EditorObjectFieldRenderer : EditorElementRenderer
    {
        /// <summary>
        /// 渲染ObjectField组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr, in IElement element, in IDeclStyle styleParam)
        {
            // 检查ReadOnly上下文
            bool isReadOnly = false;
            if (mgr.ContextStack.TryGet<DisableContext>(out var readOnlyContext))
            {
                isReadOnly = readOnlyContext.Value;
            }

            // 保存当前GUI enabled状态
            bool originalGUIEnabled = GUI.enabled;

            // 在只读状态下禁用GUI
            GUI.enabled = !isReadOnly;

            try
            {
                // 使用接口来处理ObjectField，避免反射
                if (element is not IObjectField objectField)
                {
                    Debug.LogError($"EditorObjectFieldRenderer只能渲染IObjectField组件，但收到: {element.GetType().Name}");
                    return;
                }

                // 通过接口获取属性值
                var currentValue = objectField.Value;
                var objectType = objectField.ObjectType;
                bool allowSceneObjects = objectField.AllowSceneObjects;
                var style = objectField.Style;
                string label = objectField.Label;

                // 应用样式
                var editorMgr = mgr as EditorRenderManager;
                if (editorMgr == null)
                    return;

                var currentStyle = styleParam ?? style;
                var guiStyle = editorMgr.ApplyStyle(currentStyle, EditorStyles.label);
                var width = editorMgr.GetStyleWidth(currentStyle);

                // 渲染对象选择器
                UnityEngine.Object newValue = null;
                try
                {
                    if (width > 0)
                    {
                        newValue = EditorGUILayout.ObjectField(
                                                string.IsNullOrEmpty(label) ? GUIContent.none : new GUIContent(label),
                                                currentValue as UnityEngine.Object,
                                                objectType,
                                                allowSceneObjects,
                                                GUILayout.Width(width)
                                            );
                    }
                    else
                    {
                        newValue = EditorGUILayout.ObjectField(
                                                string.IsNullOrEmpty(label) ? GUIContent.none : new GUIContent(label),
                                                currentValue as UnityEngine.Object,
                                                objectType,
                                                allowSceneObjects
                                            );
                    }

                }
                catch (UnityEngine.ExitGUIException)
                {
                    // 忽略ExitGUIException，这是Unity GUI系统的正常行为
                    return;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"ObjectField渲染错误: {ex.Message}");
                    return;
                }

                // 检查值是否变化并触发回调
                if (!Equals(newValue, currentValue))
                {
                    try
                    {
                        // 使用接口方法通知值变化
                        objectField.NotifyChanged(newValue);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"ObjectField回调错误: {ex.Message}");
                    }
                }
            }
            finally
            {
                // 恢复原始GUI enabled状态
                GUI.enabled = originalGUIEnabled;
            }
        }

        /// <summary>
        /// 计算ObjectField元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in IElement element, in IDeclStyle style)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return Vector2.zero;

            // 获取样式宽度
            var width = editorMgr.GetStyleWidth(style);
            var height = editorMgr.GetStyleHeight(style);

            // 如果设置了固定尺寸，使用固定尺寸
            if (width > 0 && height > 0)
            {
                return new Vector2(width, height);
            }

            // 对于ObjectField，使用默认的200像素宽度和标准高度
            return new Vector2(width > 0 ? width : 200, EditorGUIUtility.singleLineHeight);
        }
    }
}