﻿using System;

namespace AlphaX.FormulaEngine
{
    public abstract class FormulaArgument
    {
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

        public FormulaArgument(string name, Type type, bool required)
        {
            Name = name;
            Type = type;
            Required = required;
        }
    }

    public class DoubleArgument : FormulaArgument
    {
        public DoubleArgument(string name, bool required) : base(name, typeof(double), required)
        {
            
        }
    }

    public class ObjectArgument : FormulaArgument
    {
        public ObjectArgument(string name, bool required) : base(name, typeof(object), required)
        {

        }
    }

    public class StringArgument : FormulaArgument
    {
        public StringArgument(string name, bool required) : base(name, typeof(string), required)
        {

        }
    }

    public class BooleanArgument : FormulaArgument
    {
        public BooleanArgument(string name, bool required) : base(name, typeof(bool), required)
        {

        }
    }

    public class ArrayArgument : FormulaArgument
    {
        public ArrayArgument(string name, bool required) : base(name, typeof(object[]), required)
        {

        }
    }
}
