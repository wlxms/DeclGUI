using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DeclGUI.Core;

namespace DeclGUI.Editor
{
    /// <summary>
    /// DeclPropertyTemplate 自定义编辑器
    /// 采用与DeclThemeEditor相同的样式和布局
    /// </summary>
    [CustomEditor(typeof(DeclPropertyTemplate))]
    public class DeclPropertyTemplateEditor : UnityEditor.Editor
    {
        private SerializedProperty _templateProperties;
        private Dictionary<int, bool> _propertyFoldouts = new Dictionary<int, bool>();

        private void OnEnable()
        {
            _templateProperties = serializedObject.FindProperty("templateProperties");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("属性模板配置", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawTemplateProperties();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTemplateProperties()
        {
            EditorGUILayout.BeginVertical(GetImprovedBackgroundStyle());

            // 标题和添加按钮
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"模板属性({_templateProperties.arraySize})", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                AddNewTemplateProperty();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // 渲染每个模板属性，采用与DeclThemeEditor相同的Inline布局
            for (int i = 0; i < _templateProperties.arraySize; i++)
            {
                var property = _templateProperties.GetArrayElementAtIndex(i);
                if (property == null) continue;

                var nameProperty = property.FindPropertyRelative("Name");
                var typeProperty = property.FindPropertyRelative("Type");

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();

                // 显示类型下拉框
                if (typeProperty != null)
                {
                    typeProperty.enumValueIndex = (int)(PropertyType)EditorGUILayout.EnumPopup((PropertyType)typeProperty.enumValueIndex, GUILayout.Width(100));
                }

                // 显示名称
                if (nameProperty != null)
                {
                    nameProperty.stringValue = EditorGUILayout.TextField(nameProperty.stringValue, GUILayout.Width(120));
                }

                // 显示值 - 根据类型显示对应的默认值字段
                if (typeProperty != null)
                {
                    switch ((PropertyType)typeProperty.enumValueIndex)
                    {
                        case PropertyType.Float:
                            var floatValue = property.FindPropertyRelative("FloatValue");
                            if (floatValue != null)
                                floatValue.floatValue = EditorGUILayout.FloatField(floatValue.floatValue);
                            break;
                        case PropertyType.Int:
                            var intValue = property.FindPropertyRelative("IntValue");
                            if (intValue != null)
                                intValue.intValue = EditorGUILayout.IntField(intValue.intValue);
                            break;
                        case PropertyType.Color:
                            var colorValue = property.FindPropertyRelative("ColorValue");
                            if (colorValue != null)
                                colorValue.colorValue = EditorGUILayout.ColorField(colorValue.colorValue);
                            break;
                        case PropertyType.String:
                            var stringValue = property.FindPropertyRelative("StringValue");
                            if (stringValue != null)
                                stringValue.stringValue = EditorGUILayout.TextField(stringValue.stringValue);
                            break;
                        case PropertyType.Boolean:
                            var boolValue = property.FindPropertyRelative("BoolValue");
                            if (boolValue != null)
                            {
                                boolValue.boolValue = EditorGUILayout.Toggle(boolValue.boolValue, GUILayout.Width(40));
                                GUILayout.FlexibleSpace();
                            }
                            break;
                        case PropertyType.Vector2:
                            var vector2Value = property.FindPropertyRelative("Vector2Value");
                            if (vector2Value != null)
                                vector2Value.vector2Value = EditorGUILayout.Vector2Field("", vector2Value.vector2Value, GUILayout.MinWidth(150));
                            break;
                        case PropertyType.Vector3:
                            var vector3Value = property.FindPropertyRelative("Vector3Value");
                            if (vector3Value != null)
                                vector3Value.vector3Value = EditorGUILayout.Vector3Field("", vector3Value.vector3Value, GUILayout.MinWidth(200));
                            break;
                        case PropertyType.RectOffset:
                            GUILayout.Space(18);
                            var rectOffsetValue = property.FindPropertyRelative("RectOffsetValue");
                            if (rectOffsetValue != null)
                            {
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Label("L", GUILayout.Width(15));
                                var leftValue = rectOffsetValue.FindPropertyRelative("m_Left");
                                if (leftValue != null)
                                    leftValue.intValue = EditorGUILayout.IntField(leftValue.intValue, GUILayout.Width(40));

                                GUILayout.Label("R", GUILayout.Width(15));
                                var rightValue = rectOffsetValue.FindPropertyRelative("m_Right");
                                if (rightValue != null)
                                    rightValue.intValue = EditorGUILayout.IntField(rightValue.intValue, GUILayout.Width(40));

                                GUILayout.Label("T", GUILayout.Width(15));
                                var topValue = rectOffsetValue.FindPropertyRelative("m_Top");
                                if (topValue != null)
                                    topValue.intValue = EditorGUILayout.IntField(topValue.intValue, GUILayout.Width(40));

                                GUILayout.Label("B", GUILayout.Width(15));
                                var bottomValue = rectOffsetValue.FindPropertyRelative("m_Bottom");
                                if (bottomValue != null)
                                    bottomValue.intValue = EditorGUILayout.IntField(bottomValue.intValue, GUILayout.Width(40));
                                EditorGUILayout.EndHorizontal();
                            }
                            break;
                    }
                }

                // 显示描述（inline布局）
                var descriptionProperty = property.FindPropertyRelative("Description");
                if (descriptionProperty != null)
                {
                    descriptionProperty.stringValue = EditorGUILayout.TextField(descriptionProperty.stringValue, GUILayout.Width(150));
                }


                // 删除按钮
                var buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = 10;
                buttonStyle.padding = new RectOffset(2, 2, 2, 2);
                if (GUILayout.Button("×", buttonStyle, GUILayout.Width(20)))
                {
                    _templateProperties.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    continue;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            EditorGUILayout.EndVertical();
        }

        private void AddNewTemplateProperty()
        {
            _templateProperties.arraySize++;

            var newProperty = _templateProperties.GetArrayElementAtIndex(_templateProperties.arraySize - 1);
            if (newProperty != null)
            {
                var nameProperty = newProperty.FindPropertyRelative("Name");
                var typeProperty = newProperty.FindPropertyRelative("Type");

                if (nameProperty != null)
                    nameProperty.stringValue = $"property{_templateProperties.arraySize}";

                if (typeProperty != null)
                    typeProperty.enumValueIndex = (int)PropertyType.String;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private GUIStyle GetImprovedBackgroundStyle()
        {
            var style = new GUIStyle();
            style.normal.background = MakeTex(2, 2, EditorGUIUtility.isProSkin ?
                new Color(0.25f, 0.25f, 0.25f, 0.8f) :
                new Color(0.9f, 0.9f, 0.9f, 0.8f));
            style.border = new RectOffset(1, 1, 1, 1);
            style.margin = new RectOffset(0, 0, 2, 2);
            style.padding = new RectOffset(10, 10, 10, 10);
            style.border = new RectOffset(1, 1, 1, 1);
            return style;
        }
    }
}