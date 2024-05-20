using System;

namespace AlphaX.FormulaEngine
{
    public abstract class Formula
    {
        /// <summary>
        /// Gets the unique formula name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the formula information.
        /// </summary>
        public FormulaInfo Info { get; }

        public Formula(string name)
        {
            Name = name;
            Info = GetFormulaInfo();

            if(Info == null)
                throw new ArgumentNullException(nameof(Info));
        }

        /// <summary>
        /// Gets the evaluated result.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public abstract object Evaluate(params object[] args);

        protected abstract FormulaInfo GetFormulaInfo();

        public override string ToString()
        {
            return Name;
        }

        #region Argument Validation
        internal void ValidateArguments(object[] arguments)
        {
            ValidateArgumentCount(Info.MinArgsCount, Info.MaxArgsCount, Name, arguments);

            for (int index = 0; index < Info.MaxArgsCount; index++)
            {
                var argument = Info.Arguments[index];

                if (index > arguments.Length - 1)
                    break;

                var argumentValue = arguments[index];

                if (argumentValue == null)
                {
                    if (argument.AllowNull)
                    {
                        continue;
                    }
                    else
                    {
                        throw new EvaluationException($"Null is not allowed as argument ({argument.Name}) value.");
                    }
                }

                if (argument.Type.IsArray)
                {
                    ValidateArrayArgument(Name, argument, argumentValue);
                }
                else
                {
                    ValidateNonArrayArgument(Name, argument, argumentValue);
                }
            }
        }

        private void ValidateArgumentCount(int minArgs, int maxArgs, string formulaName, object[] arguments)
        {
            if (arguments.Length > maxArgs || arguments.Length < minArgs)
            {
                throw new EvaluationException($"Invalid number of arguments for '{formulaName}' formula. Expected Min = {minArgs}, Max = {maxArgs} arguments.");
            }
        }

        private void ValidateArrayArgument(string formulaName, FormulaArgument argument, object argumentValue)
        {
            if (argumentValue.GetType() != argument.Type)
            {
                throw new EvaluationException($"{GetTypeMismatchError(formulaName, argument)}. Expected array type.");
            }
        }

        private void ValidateNonArrayArgument(string formulaName, FormulaArgument argument, object argumentValue)
        {
            if (argument is ObjectArgument)
            {
                if (!MatchesSupportedTypes(argumentValue.GetType()))
                {
                    throw new EvaluationException($"{GetTypeMismatchError(formulaName, argument)}. Expected String/Double/Int32/Boolean type.");
                }
            }
            else if (argument.Type != argumentValue.GetType())
            {
                throw new EvaluationException($"{GetTypeMismatchError(formulaName, argument)}. Expected {argument.Type.Name} type.");
            }
        }

        private bool MatchesSupportedTypes(Type type)
        {
            return type == typeof(string) || type == typeof(double) || type == typeof(int) || type == typeof(bool);
        }

        private string GetTypeMismatchError(string formulaName, FormulaArgument argument)
        {
            return $"Formula : '{formulaName}' - Argument ({argument.Name}) type does't match";
        }
        #endregion
    }
}
