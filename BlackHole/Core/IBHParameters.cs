﻿
namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHParameters
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        void Add(string? Name, object? Value);

        /// <summary>
        /// 
        /// </summary>
        void Clear();
    }
}
