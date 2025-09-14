using System;

namespace DeclGUI.Core
{
    /// <summary>
    /// 上下文消费者接口
    /// 用于消费上下文信息
    /// </summary>
    public interface IContextConsumer : IElement
    {
        new Func<IContextReader, IElement> Render { get; }
    }
}