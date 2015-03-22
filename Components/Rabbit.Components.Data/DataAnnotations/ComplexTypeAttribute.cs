using System;

namespace Rabbit.Components.Data.DataAnnotations
{
    /// <summary>
    /// 复杂类型标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ComplexTypeAttribute : Attribute
    {
    }
}