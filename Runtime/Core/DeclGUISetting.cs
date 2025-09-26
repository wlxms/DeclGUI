using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// DeclGUI 设置管理器
    /// 单例模式，用于管理所有主题和样式集
    /// </summary>
    public class DeclGUISetting : ScriptableObject
    {
        private static DeclGUISetting _instance;

        [SerializeField]
        private DeclTheme _defaultTheme;

        [SerializeField]
        private ThemeDictionary themes = new ThemeDictionary();

        // 反向映射：DeclStyleSet -> DeclTheme
        private Dictionary<DeclStyleSet, DeclTheme> _styleSetToThemeMap;

        // 缓存所有主题中的样式集
        private Dictionary<string, DeclStyleSet> _allStyleSets;

        // 自动刷新相关字段
        private static bool _isInitialized = false;
        private static HashSet<string> _lastKnownThemeGuids = new HashSet<string>();

        /// <summary>
        /// 单例实例
        /// </summary>
        public static DeclGUISetting Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 尝试从Resources加载
                    _instance = Resources.Load<DeclGUISetting>("DeclGUI/DeclGUISetting");

                    // 如果Resources中没有，创建一个新的实例
                    if (_instance == null)
                    {
                        _instance = CreateInstance<DeclGUISetting>();
                    }
                    _instance.CollectAllThemes();
                    _instance.BuildStyleSetToThemeMap();
                    if (_instance.DefaultTheme)
                    {
                        if (_instance.themes.Count > 0)
                            _instance.DefaultTheme = _instance.themes.values.First();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 默认主题
        /// </summary>
        public DeclTheme DefaultTheme
        {
            get => _defaultTheme;
            set => _defaultTheme = value;
        }

        /// <summary>
        /// 所有主题的字典
        /// </summary>
        public ThemeDictionary Themes => themes;

        private void OnEnable()
        {
            BuildStyleSetToThemeMap();
            InitializeAutoRefresh();
        }

        /// <summary>
        /// 在Awake时收集所有主题
        /// </summary>
        private void Awake()
        {
            Refresh();
            InitializeAutoRefresh();
        }

        /// <summary>
        /// 在OnDisable时清理事件监听
        /// </summary>
        private void OnDisable()
        {
            CleanupAutoRefresh();
        }

        /// <summary>
        /// 在OnDestroy时清理事件监听
        /// </summary>
        private void OnDestroy()
        {
            CleanupAutoRefresh();
        }

        public void Refresh()
        {
            CollectAllThemes();
            BuildStyleSetToThemeMap();
        }



        /// <summary>
        /// 构建样式集到主题的反向映射
        /// </summary>
        private void BuildStyleSetToThemeMap()
        {
            _styleSetToThemeMap = new Dictionary<DeclStyleSet, DeclTheme>();
            _allStyleSets = new Dictionary<string, DeclStyleSet>();

            foreach (var themePair in themes)
            {
                var theme = themePair.Value;
                if (theme == null) continue;

                // 遍历主题中的所有样式集
                foreach (var styleSetEntry in theme.StyleSets)
                {
                    if (!string.IsNullOrEmpty(styleSetEntry.Id) && styleSetEntry.StyleSet != null)
                    {
                        // 建立反向映射
                        _styleSetToThemeMap[styleSetEntry.StyleSet] = theme;

                        // 添加到全局样式集缓存
                        if (!_allStyleSets.ContainsKey(styleSetEntry.Id))
                        {
                            _allStyleSets[styleSetEntry.Id] = styleSetEntry.StyleSet;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 注册主题
        /// </summary>
        public void RegisterTheme(DeclTheme theme)
        {
            if (theme == null)
            {
                Debug.LogWarning("无法注册主题：主题为空");
                return;
            }

            string themeKey = GetThemeKey(theme);
            themes[themeKey] = theme;
            RebuildMaps();
        }

        /// <summary>
        /// 注销主题
        /// </summary>
        public void UnregisterTheme(string themeKey)
        {
            if (string.IsNullOrEmpty(themeKey))
            {
                Debug.LogWarning("无法注销主题：主题键为空");
                return;
            }

            if (themes.ContainsKey(themeKey))
            {
                themes.Remove(themeKey);
                RebuildMaps();
            }
        }

        /// <summary>
        /// 获取主题的唯一键名（使用GUID确保唯一性）
        /// </summary>
        private string GetThemeKey(DeclTheme theme)
        {
            if (theme == null) return string.Empty;

            // 在编辑器模式下使用Asset GUID作为唯一标识
#if UNITY_EDITOR
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(theme);
            if (!string.IsNullOrEmpty(assetPath))
            {
                string guid = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
                if (!string.IsNullOrEmpty(guid))
                {
                    return guid;
                }
            }
#endif

            // 运行时使用实例ID作为后备
            return theme.GetInstanceID().ToString();
        }

        /// <summary>
        /// 获取主题
        /// </summary>
        public DeclTheme GetTheme(string themeKey)
        {
            if (string.IsNullOrEmpty(themeKey))
                return null;

            return themes.TryGetValue(themeKey, out var theme) ? theme : null;
        }

        /// <summary>
        /// 通过样式集查找对应的主题
        /// </summary>
        public DeclTheme GetThemeForStyleSet(DeclStyleSet styleSet)
        {
            if (styleSet == null)
                return null;

            return _styleSetToThemeMap.TryGetValue(styleSet, out var theme) ? theme : null;
        }

        /// <summary>
        /// 获取所有样式集
        /// </summary>
        public Dictionary<string, DeclStyleSet> GetAllStyleSets()
        {
            return new Dictionary<string, DeclStyleSet>(_allStyleSets);
        }

        /// <summary>
        /// 获取指定ID的样式集
        /// </summary>
        public DeclStyleSet GetStyleSet(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return _allStyleSets.TryGetValue(id, out var styleSet) ? styleSet : null;
        }

        /// <summary>
        /// 重新构建映射关系
        /// </summary>
        private void RebuildMaps()
        {
            BuildStyleSetToThemeMap();
        }

        /// <summary>
        /// 收集项目中的所有主题资源
        /// </summary>
        public void CollectAllThemes()
        {
#if UNITY_EDITOR
            // 在编辑器模式下收集所有DeclTheme资源
            var guids = UnityEditor.AssetDatabase.FindAssets("t:DeclTheme");

            // 先清除现有主题中已经不存在的资源
            var keysToRemove = new List<string>();
            foreach (var themePair in themes)
            {
                if (themePair.Value == null)
                {
                    keysToRemove.Add(themePair.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                themes.Remove(key);
            }

            // 添加或更新所有找到的主题
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var theme = UnityEditor.AssetDatabase.LoadAssetAtPath<DeclTheme>(path);
                if (theme != null)
                {
                    string themeKey = GetThemeKey(theme);
                    themes[themeKey] = theme;
                }
            }
            RebuildMaps();
#else
            // 运行时模式：从Resources文件夹加载主题
            var loadedThemes = Resources.LoadAll<DeclTheme>("");
            foreach (var theme in loadedThemes)
            {
                if (theme != null)
                {
                    string themeKey = GetThemeKey(theme);
                    themes[themeKey] = theme;
                }
            }
            RebuildMaps();
#endif
        }

        /// <summary>
        /// 初始化自动刷新机制
        /// </summary>
        private void InitializeAutoRefresh()
        {
#if UNITY_EDITOR
            if (!_isInitialized)
            {
                // 监听AssetDatabase事件
                UnityEditor.EditorApplication.projectChanged += OnProjectChanged;
                UnityEditor.AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
                UnityEditor.AssetDatabase.importPackageCancelled += OnImportPackageCancelled;
                UnityEditor.AssetDatabase.importPackageFailed += OnImportPackageFailed;
                
                _isInitialized = true;
                Debug.Log("DeclGUISetting: 自动刷新机制已初始化");
            }

            // 记录当前已知的主题GUID
            UpdateLastKnownThemeGuids();
#endif
        }

        /// <summary>
        /// 清理自动刷新机制
        /// </summary>
        private void CleanupAutoRefresh()
        {
#if UNITY_EDITOR
            if (_isInitialized)
            {
                UnityEditor.EditorApplication.projectChanged -= OnProjectChanged;
                UnityEditor.AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
                UnityEditor.AssetDatabase.importPackageCancelled -= OnImportPackageCancelled;
                UnityEditor.AssetDatabase.importPackageFailed -= OnImportPackageFailed;
                
                _isInitialized = false;
                Debug.Log("DeclGUISetting: 自动刷新机制已清理");
            }
#endif
        }

        /// <summary>
        /// 项目发生变化时的回调
        /// </summary>
        private void OnProjectChanged()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) return;

            // 检查主题资源是否发生变化
            if (HasThemeAssetsChanged())
            {
                Debug.Log("DeclGUISetting: 检测到主题资源变化，自动刷新...");
                Refresh();
            }
#endif
        }

        /// <summary>
        /// 包导入完成时的回调
        /// </summary>
        private void OnImportPackageCompleted(string packageName)
        {
            Refresh();
        }

        /// <summary>
        /// 包导入取消时的回调
        /// </summary>
        private void OnImportPackageCancelled(string packageName)
        {
            Refresh();
        }

        /// <summary>
        /// 包导入失败时的回调
        /// </summary>
        private void OnImportPackageFailed(string packageName, string errorMessage)
        {
            Refresh();
        }

        /// <summary>
        /// 检查主题资源是否发生变化
        /// </summary>
        private bool HasThemeAssetsChanged()
        {
#if UNITY_EDITOR
            var currentGuids = UnityEditor.AssetDatabase.FindAssets("t:DeclTheme");
            var currentGuidSet = new HashSet<string>(currentGuids);

            // 检查是否有新增或删除的主题
            bool hasChanges = !_lastKnownThemeGuids.SetEquals(currentGuidSet);

            if (hasChanges)
            {
                Debug.Log($"DeclGUISetting: 主题资源发生变化 - 之前: {_lastKnownThemeGuids.Count}, 现在: {currentGuidSet.Count}");
            }

            return hasChanges;
#else
            return false;
#endif
        }

        /// <summary>
        /// 更新最后已知的主题GUID
        /// </summary>
        private void UpdateLastKnownThemeGuids()
        {
#if UNITY_EDITOR
            var currentGuids = UnityEditor.AssetDatabase.FindAssets("t:DeclTheme");
            _lastKnownThemeGuids = new HashSet<string>(currentGuids);
#endif
        }

        /// <summary>
        /// 强制刷新并更新缓存
        /// </summary>
        public void ForceRefresh()
        {
            Refresh();
            UpdateLastKnownThemeGuids();
        }
    }
}