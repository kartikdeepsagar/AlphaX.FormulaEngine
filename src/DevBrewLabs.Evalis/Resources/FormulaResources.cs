using System;
using System.Collections.Generic;
using System.Text;

namespace DevBrewLabs.Evalis.Resources
{
    internal static class FormulaResources
    {
        public const string InvalidArgumentCount =
            "Invalid argument count. Expected min={0}, max={1} arguments.";

        public const string InvalidArrayArgument =
            "Invalid argument at index {0}. Expected an array of values.";

        public const string InvalidBooleanArgument =
            "Invalid argument at index {0}. Expected a boolean value.";

        public const string InvalidDecimalArgument =
            "Invalid argument at index {0}. Expected a number.";

        public const string InvalidObjectArgument =
            "Invalid argument at index {0}. Expected a string/number/boolean value.";

        public const string InvalidStringArgument =
            "Invalid argument at index {0}. Expected a string value.";
    }
}
