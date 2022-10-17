using System;

namespace AlphaX.FormulaEngine
{
    public class FormulaArgument
    {
        /// <summary>
        /// Gets the position of the argument.
        /// </summary>
        public int Index { get; }
        /// <summary>
        /// Gets the name of the argument.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// Gets if the argument is required.
        /// </summary>
        public bool Required { get; }
        /// <summary>
        /// Gets the description of the argument.
        /// </summary>
        public string Description { get; }

        public FormulaArgument(string name, Type type, bool required, int index, string description)
        {
            Name = name;
            Type = type;
            Required = required;
            Index = index;
            Description = description;
        }
    }
}
