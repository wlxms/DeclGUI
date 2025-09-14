using System;
using UnityEngine;

namespace DeclGUI.Core
{
    /// <summary>
    /// 事件分发器
    /// 负责将Unity GUI事件传递给RenderManager进行处理
    /// </summary>
    public class EventDispatcher
    {
        private readonly RenderManager _renderManager;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="renderManager">渲染管理器实例</param>
        public EventDispatcher(RenderManager renderManager)
        {
            _renderManager = renderManager ?? throw new ArgumentNullException(nameof(renderManager));
        }
        
        /// <summary>
        /// 处理GUI事件
        /// </summary>
        /// <param name="guiEvent">Unity GUI事件</param>
        public void ProcessGUIEvent(Event guiEvent)
        {
            if (guiEvent == null)
                return;
                
            // 设置当前事件到渲染管理器，事件处理将在渲染过程中进行
            _renderManager.SetCurrentEvent(guiEvent);
        }
        
        /// <summary>
        /// 清除当前事件
        /// </summary>
        public void ClearCurrentEvent()
        {
            _renderManager.ClearCurrentEvent();
        }
    }
}