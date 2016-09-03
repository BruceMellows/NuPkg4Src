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