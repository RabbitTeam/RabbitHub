using System;

namespace Rabbit.Components.Data
{
    /// <summary>
    /// 表示类型是一个实体。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class EntityAttribute : Attribute
    {
    }
}