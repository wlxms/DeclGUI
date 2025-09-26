using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 主题字典，用于存储主题名称到DeclTheme的映射
    /// </summary>
    [Serializable]
    public class ThemeDictionary : SerializedDictionary<string, DeclTheme>
    {
        // 可以在这里添加特定于主题字典的逻辑
    }
}