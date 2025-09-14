using System;
using System.Buffers;

namespace DeclGUI.Core
{
    /// <summary>
    /// 数组池辅助工具类
    /// </summary>
    public static class ArrayPoolHelper
    {
        /// <summary>
        /// 确保数组有足够的容量
        /// </summary>
        public static void EnsureCapacity<T>(ref T[] array, ref int capacity, int count, int requiredCapacity)
        {
            if (requiredCapacity <= capacity) return;

            int newCapacity = GetNextCapacity(requiredCapacity);
            var newArray = ArrayPool<T>.Shared.Rent(newCapacity);
            
            if (count > 0)
            {
                Array.Copy(array, newArray, count);
            }

            if (capacity > 0)
            {
                ArrayPool<T>.Shared.Return(array, true);
            }

            array = newArray;
            capacity = newCapacity;
        }

        /// <summary>
        /// 获取下一个容量大小（指数增长）
        /// </summary>
        public static int GetNextCapacity(int requiredCapacity)
        {
            if (requiredCapacity <= 0) return 0;

            int capacity = 1;
            while (capacity < requiredCapacity)
            {
                capacity *= 2;
            }
            return capacity;
        }

        /// <summary>
        /// 释放数组资源
        /// </summary>
        public static void Dispose<T>(ref T[] array, ref int capacity, ref int count)
        {
            if (capacity > 0)
            {
                ArrayPool<T>.Shared.Return(array, true);
                array = null;
                capacity = 0;
                count = 0;
            }
        }

        /// <summary>
        /// 从源数组初始化
        /// </summary>
        public static void InitializeFromArray<T>(ref T[] array, ref int capacity, ref int count, T[] sourceArray)
        {
            if (sourceArray == null || sourceArray.Length == 0) return;

            capacity = GetNextCapacity(sourceArray.Length);
            array = ArrayPool<T>.Shared.Rent(capacity);
            Array.Copy(sourceArray, array, sourceArray.Length);
            count = sourceArray.Length;
        }

        /// <summary>
        /// 从枚举集合初始化
        /// </summary>
        public static void InitializeFromEnumerable<T>(ref T[] array, ref int capacity, ref int count, System.Collections.Generic.IEnumerable<T> source)
        {
            if (source == null) return;

            var sourceArray = System.Linq.Enumerable.ToArray(source);
            if (sourceArray.Length == 0) return;

            capacity = GetNextCapacity(sourceArray.Length);
            array = ArrayPool<T>.Shared.Rent(capacity);
            Array.Copy(sourceArray, array, sourceArray.Length);
            count = sourceArray.Length;
        }
    }
}