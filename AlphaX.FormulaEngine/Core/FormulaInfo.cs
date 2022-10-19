using System.Linq;
using System.Text;

namespace AlphaX.FormulaEngine
{
    public class FormulaInfo
    {
        private string _formattedValue;

        /// <summary>
        /// Gets the formula description.
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Gets the formula arguments.
        /// </summary>
        public FormulaArgument[] Arguments { get; }
        public int MinArgsCount { get; }
        public int MaxArgsCount { get; }

        public FormulaInfo(string description, params FormulaArgument[] arguments)
        {
            Description = description;
            Arguments = arguments;
            MinArgsCount = Arguments.Count(x => x.Required);
            MaxArgsCount = Arguments.Length;
        }

        public FormulaInfo(string description, int minArgs, int maxArgs, params FormulaArgument[] arguments)
        {
            Description = description;
            Arguments = arguments;
            MinArgsCount = minArgs;
            MaxArgsCount = maxArgs;
        }

        public override string ToString()
        {
            if(_formattedValue == null)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("Description : ");
                stringBuilder.Append(Description);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();

                if (Arguments != null && Arguments.Length > 0)
                {
                    stringBuilder.AppendLine("Arguments");
                    stringBuilder.AppendLine();

                    foreach (var argument in Arguments)
                    {
                        var usage = argument.Required ? "[Required]" : "[Optional]";
                        stringBuilder.AppendLine(argument.Index + 1 + ") " + argument.Name + " : " + argument.Description + " " + usage);
                    }
                }

                _formattedValue = stringBuilder.ToString();
            }

            return _formattedValue;
        }
    }
}
