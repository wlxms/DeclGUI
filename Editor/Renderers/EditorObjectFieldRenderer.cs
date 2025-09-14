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
            if (mgr.ContextStack.TryGet<ReadOnly>(out var readOnlyContext))
            {
                isReadOnly = readOnlyContext.Value;
            }

            // 保存当前GUI enabled状态
            bool originalGUIEnabled = GUI.enabled;
            
            // 在只读状态下禁用GUI
            GUI.enabled = !isReadOnly;

            try
            {
                // 使用反射来处理泛型ObjectField
                var elementType = element.GetType();
                if (!elementType.IsGenericType || elementType.GetGenericTypeDefinition() != typeof(ObjectField<>))
                {
                    Debug.LogError($"EditorObjectFieldRenderer只能渲染ObjectField<>组件，但收到: {elementType.Name}");
                    return;
                }

                // 获取泛型参数类型
                var objectType = elementType.GetGenericArguments()[0];
                
                // 使用反射获取字段值
                var valueProperty = elementType.GetProperty("Value");
                var onValueChangedProperty = elementType.GetProperty("OnValueChanged");
                var allowSceneObjectsProperty = elementType.GetProperty("AllowSceneObjects");
                var styleProperty = elementType.GetProperty("Style");

                if (valueProperty == null || onValueChangedProperty == null ||
                    allowSceneObjectsProperty == null || styleProperty == null)
                {
                    Debug.LogError("无法访问ObjectField的属性");
                    return;
                }

                object currentValue = valueProperty.GetValue(element);
                var onValueChangedDelegate = onValueChangedProperty.GetValue(element);
                bool allowSceneObjects = (bool)allowSceneObjectsProperty.GetValue(element);
                DeclStyle style = (DeclStyle)styleProperty.GetValue(element);

                // 应用样式
                var editorMgr = mgr as EditorRenderManager;
                if (editorMgr == null)
                    return;
                    
                var currentStyle = styleParam ?? style;
                var guiStyle = editorMgr.ApplyStyle(currentStyle, EditorStyles.label);
                var width = editorMgr.GetStyleWidth(currentStyle);
                
                // 渲染对象选择器
                var newValue = EditorGUILayout.ObjectField(
                    GUIContent.none,
                    currentValue as UnityEngine.Object,
                    objectType,
                    allowSceneObjects,
                    GUILayout.Width(width > 0 ? width : 200)
                );
                
                // 检查值是否变化并触发回调
                if (!Equals(newValue, currentValue) && onValueChangedDelegate != null)
                {
                    // 使用反射调用回调
                    var method = onValueChangedDelegate.GetType().GetMethod("Invoke");
                    method?.Invoke(onValueChangedDelegate, new[] { newValue });
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