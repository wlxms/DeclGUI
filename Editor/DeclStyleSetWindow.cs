using UnityEngine;
using UnityEditor;
using DeclGUI.Core;
using System.Collections.Generic;

namespace DeclGUI.Editor
{
    /// <summary>
    /// DeclStyleSet 可视化编辑器窗口
    /// </summary>
    public class DeclStyleSetWindow : EditorWindow
    {
        private DeclStyleSet _styleSet;
        private SerializedObject _serializedObject;
        private Vector2 _scrollPosition;
        private DeclStyleSetRenderer _renderer;

        [MenuItem("Tools/DeclGUI/样式集编辑器")]
        public static void ShowWindow()
        {
            var window = GetWindow<DeclStyleSetWindow>("样式集编辑器");
            window.minSize = new Vector2(600, 500);
        }
        
        private void OnEnable()
        {
            // 监听Undo/Redo事件
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }
        
        private void OnDisable()
        {
            // 移除Undo/Redo事件监听
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }
        
        private void OnUndoRedoPerformed()
        {
            // 当Undo/Redo操作发生时，更新SerializedObject并重新绘制界面
            Debug.Log("DeclStyleSetWindow: OnUndoRedoPerformed called");
            if (_serializedObject != null)
            {
                _serializedObject.Update();
            }
            Repaint();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            // 选择样式集
            DrawStyleSetSelector();

            if (_styleSet == null)
            {
                EditorGUILayout.HelpBox("请选择一个 DeclStyleSet 资产", MessageType.Info);
                return;
            }

            // 更新serializedObject以支持Undo/Redo
            _serializedObject.Update();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // 使用 DeclStyleSetRenderer 渲染样式集
            if (_renderer != null)
            {
                _renderer.Render();
            }

            EditorGUILayout.EndScrollView();
            
            // 应用修改并处理Undo/Redo
            _serializedObject.ApplyModifiedProperties();
        }

        private void DrawStyleSetSelector()
        {
            EditorGUILayout.BeginHorizontal();

            var newStyleSet = EditorGUILayout.ObjectField("样式集", _styleSet, typeof(DeclStyleSet), false) as DeclStyleSet;

            if (newStyleSet != _styleSet)
            {
                _styleSet = newStyleSet;
                if (_styleSet != null)
                {
                    _serializedObject = new SerializedObject(_styleSet);
                    _renderer = new DeclStyleSetRenderer(_styleSet, SaveChanges, Repaint);
                }
                else
                {
                    _serializedObject = null;
                    _renderer = null;
                }
            }

            if (GUILayout.Button("新建", GUILayout.Width(60)))
            {
                DeclStyleSetCreator.CreateDeclStyleSet();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void SaveChanges()
        {
            // 保存序列化对象的更改
            if (_serializedObject != null)
            {
                _serializedObject.ApplyModifiedProperties();
            }

            // 显示保存成功消息
            EditorUtility.DisplayDialog("保存成功", "样式集已成功保存", "确定");
        }
    }
}
