using DevBrewLabs.Parserly;
using System;

namespace DevBrewLabs.Evalis
{
    internal static class EvalisUtil
    {
        public static double? AsDouble(object val)
        {
            if (val is double d) return d;
            if (val is int i) return (double)i;
            if (val is byte b) return (double)b;
            if (val is float f) return (double)f;
            if (val is long l) return (double)l;
            if (val is short s) return (double)s;
            if (val is uint ui) return (double)ui;
            return null;
        }
        public static bool? Compare(object left, string @operator, object right, LogicalOperator supportedOperators)
        {
            try
            {
                if (@operator == supportedOperators.EqualsTo)
                    return Equals(left, right);

                if (@operator == supportedOperators.NotEquals)
                    return !Equals(left, right);

                if (@operator == supportedOperators.AND)
                {
                    return (bool)left && (bool)right;
                }

                if (@operator == supportedOperators.OR)
                {
                    return (bool)left || (bool)right;
                }

                if (@operator == supportedOperators.LessThan)
                {
                    double? num1 = AsDouble(left);
                    double? num2 = AsDouble(right);
                    if (num1.HasValue && num2.HasValue)
                    {
                        return num1.Value < num2.Value;
                    }
                    else if (left is DateTime date1 && right is DateTime date2)
                    {
                        return date1 < date2;
                    }
                }

                if (@operator == supportedOperators.LessThanEqualsTo)
                {
                    double? num1 = AsDouble(left);
                    double? num2 = AsDouble(right);
                    if (num1.HasValue && num2.HasValue)
                    {
                        return num1.Value <= num2.Value;
                    }
                    else if (left is DateTime date1 && right is DateTime date2)
                    {
                        return date1 <= date2;
                    }
                }

                if (@operator == supportedOperators.GreaterThan)
                {
                    double? num1 = AsDouble(left);
                    double? num2 = AsDouble(right);
                    if (num1.HasValue && num2.HasValue)
                    {
                        return num1.Value > num2.Value;
                    }
                    else if (left is DateTime date1 && right is DateTime date2)
                    {
                        return date1 > date2;
                    }
                }

                if (@operator == supportedOperators.GreaterThanEqualsTo)
                {
                    double? num1 = AsDouble(left);
                    double? num2 = AsDouble(right);
                    if (num1.HasValue && num2.HasValue)
                    {
                        return num1.Value >= num2.Value;
                    }
                    else if (left is DateTime date1 && right is DateTime date2)
                    {
                        return date1 >= date2;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static ArrayResult Normalize(this ArrayResult result)
        {
            if (result != null && result.Value.Length == 1 && result.Value[0] is ArrayResult res)
            {
                return res;
            }

            return result;
        }
    }
}