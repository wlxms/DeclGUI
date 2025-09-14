using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// DeclGUI 主题配置
    /// 使用 ScriptableObject 支持可视化编辑
    /// </summary>
    [CreateAssetMenu(fileName = "NewDeclTheme", menuName = "DeclGUI/Theme")]
    public class DeclTheme : ScriptableObject
    {
        [SerializeField]
        private List<StyleSetEntry> styleSets = new List<StyleSetEntry>();
        
        private Dictionary<string, IDeclStyle> _styleSetCache;
        
        /// <summary>
        /// 获取样式集
        /// </summary>
        public IDeclStyle GetStyleSet(string id)
        {
            if (_styleSetCache == null)
                BuildCache();
            
            return _styleSetCache.TryGetValue(id, out var styleSet) ? styleSet : null;
        }
        
        /// <summary>
        /// 注册样式集
        /// </summary>
        public void RegisterStyleSet(string id, IDeclStyle styleSet)
        {
            if (_styleSetCache == null)
                BuildCache();
            
            _styleSetCache[id] = styleSet;
            
            // 更新序列化数据
            var entry = styleSets.Find(e => e.Id == id);
            if (entry == null)
            {
                entry = new StyleSetEntry { Id = id };
                styleSets.Add(entry);
            }
            
            if (styleSet is DeclStyleSet styleSetImpl)
            {
                entry.StyleSet = styleSetImpl;
            }
        }
        
        private void BuildCache()
        {
            _styleSetCache = new Dictionary<string, IDeclStyle>();
            foreach (var entry in styleSets)
            {
                if (!string.IsNullOrEmpty(entry.Id) && entry.StyleSet != null)
                {
                    _styleSetCache[entry.Id] = entry.StyleSet;
                }
            }
        }
        
        [System.Serializable]
        private class StyleSetEntry
        {
            public string Id;
            public DeclStyleSet StyleSet;
        }
    }
}