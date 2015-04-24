using System;

namespace Rabbit.Components.Security.Web
{
    /// <summary>
    /// 适用于控制器或动作，将阻止任何行动被过滤的 网站前端访问 许可。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AlwaysAccessibleAttribute : Attribute
    {
    }
}