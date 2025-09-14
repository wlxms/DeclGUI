namespace DeclGUI.Core
{
    /// <summary>
    /// DeclGUI 事件类型枚举
    /// 定义所有支持的事件类型
    /// </summary>
    public enum DeclEventType
    {
        Click,       // 点击事件
        PressDown,   // 按下事件
        PressUp,     // 释放事件
        HoverEnter,  // 悬停进入
        HoverExit,   // 悬停退出
        Drag,        // 拖拽事件
        Scroll,      // 滚动事件
        Custom       // 自定义事件
    }
}