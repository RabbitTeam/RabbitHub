using CommandLine.Text;

namespace Rabbit.Components.Command.Services
{
    internal sealed class ChineseSentenceBuilder : BaseSentenceBuilder
    {
        #region Overrides of BaseSentenceBuilder

        /// <summary>
        /// Gets a string containing word 'option'.
        /// </summary>
        /// <value>
        /// The word 'option'.
        /// </value>
        public override string OptionWord
        {
            get { return "选项"; }
        }

        /// <summary>
        /// Gets a string containing the word 'and'.
        /// </summary>
        /// <value>
        /// The word 'and'.
        /// </value>
        public override string AndWord
        {
            get { return "和"; }
        }

        /// <summary>
        /// Gets a string containing the sentence 'required option missing'.
        /// </summary>
        /// <value>
        /// The sentence 'required option missing'.
        /// </value>
        public override string RequiredOptionMissingText
        {
            get { return "所需选项丢失"; }
        }

        /// <summary>
        /// Gets a string containing the sentence 'violates format'.
        /// </summary>
        /// <value>
        /// The sentence 'violates format'.
        /// </value>
        public override string ViolatesFormatText
        {
            get { return "违反格式"; }
        }

        /// <summary>
        /// Gets a string containing the sentence 'violates mutual exclusiveness'.
        /// </summary>
        /// <value>
        /// The sentence 'violates mutual exclusiveness'.
        /// </value>
        public override string ViolatesMutualExclusivenessText
        {
            get { return "违反互斥"; }
        }

        /// <summary>
        /// Gets a string containing the error heading text.
        /// </summary>
        /// <value>
        /// The error heading text.
        /// </value>
        public override string ErrorsHeadingText
        {
            get { return "和"; }
        }

        #endregion Overrides of BaseSentenceBuilder
    }
}