using System;

namespace Rabbit.Components.Data.DataAnnotations
{
    /// <summary>
    /// 不进行映射。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class NotMappedAttribute : Attribute
    {
    }
}