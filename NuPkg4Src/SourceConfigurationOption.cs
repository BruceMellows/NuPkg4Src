// NO LICENSE
// ==========
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace NuPkg4Src
{
    using System;

    internal sealed class SourceConfigurationOption
    {
        public SourceConfigurationOption(string optionTypeText, string value)
            : this(ParseOption(optionTypeText), value)
        {
        }

        public SourceConfigurationOption(SourceConfigurationOptionType optionType, string value)
        {
            this.OptionType = optionType;
            this.Value = value;
        }

        public SourceConfigurationOptionType OptionType { get; private set; }

        public string Value { get; private set; }

        public override string ToString()
        {
            return this.OptionType.ToString() + '=' + this.Value;
        }

        private static SourceConfigurationOptionType ParseOption(string optionTypeText)
        {
            SourceConfigurationOptionType optionType;
            return Enum.TryParse(optionTypeText, true, out optionType) ? optionType : SourceConfigurationOptionType.Error;
        }
    }
}