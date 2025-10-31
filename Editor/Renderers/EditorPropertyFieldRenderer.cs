using DeclGUI.Core;
using DeclGUI.Components;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

namespace DeclGUI.Editor.Renderers
{
    /// <summary>
    /// PropertyField 组件的 Editor 渲染器
    /// </summary>
    public class EditorUnityPropertyFieldRenderer : EditorElementRenderer<UnityPropertyField>
    {
        public override void Render(RenderManager mgr, in UnityPropertyField element, in IDeclStyle styleParam)
        {
            var editorMgr = mgr as EditorRenderManager;
            if (editorMgr == null)
                return;

            if (element.Value == null)
            {
                GUILayout.Label("值为空");
                return;
            }

            try
            {
                EditorGUI.BeginChangeCheck();
                
                // 创建临时 ScriptableObject 来包装值
                var wrapper = ScriptableObject.CreateInstance<ValueWrapper>();
                try
                {
                    // 使用反射设置值
                    var valueField = typeof(ValueWrapper).GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
                    valueField.SetValue(wrapper, element.Value);
                    
                    // 创建 SerializedObject
                    var serializedObject = new SerializedObject(wrapper);
                    var valueProperty = serializedObject.FindProperty("_value");
                    
                    if (valueProperty != null)
                    {
                        // 渲染属性字段
                        var rect = GUILayoutUtility.GetRect(
                            GUILayoutUtility.GetLastRect().width,
                            EditorGUI.GetPropertyHeight(valueProperty, true)
                        );
                        
                        EditorGUI.PropertyField(rect, valueProperty, true);
                        
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            
                            // 获取更新后的值
                            var newValue = valueField.GetValue(wrapper);
                            
                            // 调用值变化回调
                            element.OnValueChanged?.Invoke(newValue);
                        }
                    }
                    else
                    {
                        GUILayout.Label("无法渲染属性");
                    }
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(wrapper);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"渲染属性字段失败: {ex.Message}");
                GUILayout.Label($"渲染失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取 PropertyField 元素的屏幕区域
        /// </summary>
        /// <returns>PropertyField 的屏幕矩形</returns>
        public override Rect GetElementRect()
        {
            return GUILayoutUtility.GetLastRect();
        }

        /// <summary>
        /// 计算 PropertyField 的期望大小
        /// </summary>
        public override Vector2 CalculateSize(RenderManager mgr, in UnityPropertyField element, in IDeclStyle style)
        {
            if (element.Value == null)
            {
                return new Vector2(100, EditorGUIUtility.singleLineHeight);
            }

            try
            {
                // 创建临时包装器来计算高度
                var wrapper = ScriptableObject.CreateInstance<ValueWrapper>();
                try
                {
                    var valueField = typeof(ValueWrapper).GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
                    valueField.SetValue(wrapper, element.Value);
                    
                    var tempSerializedObject = new SerializedObject(wrapper);
                    var valueProperty = tempSerializedObject.FindProperty("_value");
                    
                    if (valueProperty != null)
                    {
                        float height = EditorGUI.GetPropertyHeight(valueProperty, true);
                        return new Vector2(EditorGUIUtility.currentViewWidth - 30, height);
                    }
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(wrapper);
                }
            }
            catch
            {
                // 如果计算失败，使用默认高度
            }

            return new Vector2(EditorGUIUtility.currentViewWidth - 30, EditorGUIUtility.singleLineHeight * 2);
        }

        /// <summary>
        /// 用于包装任意值的 ScriptableObject 包装器
        /// </summary>
        [System.Serializable]
        private class ValueWrapper : ScriptableObject
        {
            [SerializeField]
            private object _value;
        }
    }
}