using System;
using System.Collections;
using System.Collections.Generic;

namespace DeclGUI.Core
{
    /// <summary>
    /// 上下文消费者
    /// 用于消费上下文信息并渲染结果
    /// </summary>
    public struct ContextConsumer : IContextConsumer
    {
        public Func<IContextReader, IElement> Render { get; }

        public ContextConsumer(Func<IContextReader, IElement> render)
        {
            this.Render = render;
        }

        IElement IElement.Render()
        {
            return null;
        }
    }
}