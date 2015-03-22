namespace Rabbit.Kernel.Localization
{
    /// <summary>
    /// 一个空的本地化实例。
    /// </summary>
    public static class NullLocalizer
    {
        static NullLocalizer()
        {
            Localizer = (format, args) => new LocalizedString((args == null || args.Length == 0) ? format : string.Format(format, args));
        }

        private static readonly Localizer Localizer;

        /// <summary>
        /// 本地化实例。
        /// </summary>
        public static Localizer Instance { get { return Localizer; } }
    }
}