using System;

namespace DeclGUI.Core
{
    /// <summary>
    /// 上下文读取器接口
    /// 用于从上下文栈中读取上下文信息
    /// </summary>
    public interface IContextReader
    {
        /// <summary>
        /// 获取指定类型的上下文值
        /// </summary>
        T Get<T>() where T : struct, IContextProvider;

        /// <summary>
        /// 尝试获取指定类型的上下文值
        /// </summary>
        bool TryGet<T>(out T value) where T : struct, IContextProvider;

        /// <summary>
        /// 检查是否存在指定类型的上下文
        /// </summary>
        bool Has<T>() where T : struct, IContextProvider;
    }
}