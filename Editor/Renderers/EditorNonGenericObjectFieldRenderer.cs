using DeclGUI.Components;
using DeclGUI.Core;
using System;
using UnityEditor;
using UnityEngine;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// 非泛型ObjectField组件的Editor渲染器
    /// 处理ObjectField类型（非泛型版本）
    /// </summary>
    public class EditorNonGenericObjectFieldRenderer : EditorElementRenderer<ObjectField>
    {
        /// <summary>
        /// 渲染非泛型ObjectField组件
        /// </summary>
        /// <param name="mgr">渲染管理器</param>
        /// <param name="element">UI元素</param>
        public override void Render(RenderManager mgr, in ObjectField element, in IDeclStyle styleParam)
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
                // 应用样式
                var editorMgr = mgr as EditorRenderManager;
                if (editorMgr == null)
                    return;

                var currentStyle = styleParam ?? element.Style;
                var guiStyle = editorMgr.ApplyStyle(currentStyle, EditorStyles.label);
                var width = editorMgr.GetStyleWidth(currentStyle);

                // 渲染对象选择器
                UnityEngine.Object newValue = null;
                try
                {
                    if (width > 0)
                    {
                        newValue = EditorGUILayout.ObjectField(
                             string.IsNullOrEmpty(element.Label) ? GUIContent.none : new GUIContent(element.Label),
                             element.Value,
                             element.ObjectType,
                             element.AllowSceneObjects,
                             GUILayout.Width(width)
                         );
                    }
                    else
                    {
                        newValue = EditorGUILayout.ObjectField(
                             string.IsNullOrEmpty(element.Label) ? GUIContent.none : new GUIContent(element.Label),
                             element.Value,
                             element.ObjectType,
                             element.AllowSceneObjects);
                    }

                }
                catch (UnityEngine.ExitGUIException)
                {
                    // 忽略ExitGUIException，这是Unity GUI系统的正常行为
                    return;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"非泛型ObjectField渲染错误: {ex.Message}");
                    return;
                }

                // 检查值是否变化并触发回调
                if (!Equals(newValue, element.Value) && element.OnValueChanged != null)
                {
                    try
                    {
                        element.OnValueChanged.Invoke(newValue);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"非泛型ObjectField回调错误: {ex.Message}");
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
        /// 计算非泛型ObjectField元素的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in ObjectField element, in IDeclStyle style)
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