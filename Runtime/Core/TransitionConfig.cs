using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 过渡效果配置
    /// </summary>
    public struct TransitionConfig
    {
        public float Duration;          // 过渡持续时间（秒）
        public AnimationCurve EasingCurve; // 缓动曲线
        public string[] Properties;     // 应用过渡的属性列表
        
        public static TransitionConfig Default => new TransitionConfig
        {
            Duration = 0.3f,
            EasingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1),
            Properties = new[] { "color", "width", "height", "background-color", "border-color" }
        };
    }
}