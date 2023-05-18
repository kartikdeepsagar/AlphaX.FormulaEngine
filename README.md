
# AlphaX.FormulaEngine

A strong and fast library to parse and evaluate formulas. It also supports custom formulas. This library is built using '[AlphaX.Parserz](https://www.nuget.org/packages/AlphaX.Parserz)' library.

Feedback is very much appreciated : https://forms.gle/dfv8E8zpC2qPJS7i7

# Using AlphaXFormulaEngine

For evaluating formulas using AlphaXFormulaEngine, you can simply initialize the engine and start using its Evaluate method:
```c#
AlphaX.FormulaEngine.AlphaXFormulaEngine engine = new AlphaX.FormulaEngine.AlphaXFormulaEngine();
AlphaX.FormulaEngine.IEvaluationResult result = engine.Evaluate("SUM([1,2,12.3,5.9099])");
Console.WriteLine(result.Value); // 21.2099
```

AlphaXFormulaEngine comes with a limited number (not many) of inbuilt formulas i.e. 

- SUM - Returns sum of provided values. For example: SUM([1,2,4]) // 7
- AVERAGE - Returns average of provided values. For example: AVERAGE([3,2,4]) // 3
- LOWER - Returns lower cased string. For example: LOWER("TESTSTRING") // teststring
- UPPER - Returns upper cased string. For example: UPPER("teststring") // TESTSTRING
- TEXTSPLIT - Returns splitted string using a seperator. For example: TEXTSPLIT(".", "John.Doe") // John  Doe
- TODAY - Returns system date. For example: TODAY() // 28-04-2023
- CONCAT - Joins multiple strings into one string: For example: CONCAT("Test","String","1") // TestString1
- NOW -  Returns system date time // 28-04-2023 10:52:53 PM

> **Note** : More formulas will be added in future updates.

# Creating a Custom Formula

This is one of the best feature of AlphaXFormulaEngine. It provides you enough flexibility to write your own formula and easily integrate it with the engine.

1. Create a new MyFormula class which inherits from AlphaX.FormulaEngine.**Formula** class
```c#
public class MyFormula : AlphaX.FormulaEngine.Formula
{
        public MyFormula() : base("MyFormula")
        {
        }

        public override object Evaluate(params object[] args)
        {
            throw new NotImplementedException();
        }

        protected override FormulaInfo GetFormulaInfo()
        {
            throw new NotImplementedException();
        }
}
```
In the above code, the base() call accepts the name of the formula to be used in formula string.

2. Let's just say our formula will return a number raised to a power. For example. 2^2 = 4. So, we'll start by writing the code in the above evaluate method as follows:
```c#
public override object Evaluate(params object[] args)
{
            double result = 0;
            if (args.Length != 2)
            {
                double number = (double)args[0];
                double power = (double)args[1];
                result = Math.Pow(number, power);
            }
            return result;
}
```
3. We also need to provide some additional metadata for our formula using the GetFormulaInfo method as follows:
```c#
protected override FormulaInfo GetFormulaInfo()
{
    var info = new FormulaInfo();
    info.AddArgument(new DoubleArgument("number", true));
    info.AddArgument(new DoubleArgument("number", true));
    return info;
}
```
The above code defines that our formula:

- Will have min/max 2 arguments.
- First argument is a number of type double, It is required and will be present at 0 index in formula arguments.
- Second argument is a number of type double, It is required and will be present at 1 index in formula arguments.

4. Now our formula is ready and the only thing left is to integrate it with the engine by using AlphaXFormulaEngine's **AddFormula** method as follows:
```c#
AlphaX.FormulaEngine.AlphaXFormulaEngine engine = new AlphaX.FormulaEngine.AlphaXFormulaEngine();
engine.FormulaStore.Add(new MyFormula());
AlphaX.FormulaEngine.IEvaluationResult result = engine.Evaluate("MyFormula(4,3)");
Console.WriteLine(result.Value); // 64
```
# Customizing Engine Settings

AlphaXFormulaEngine allows you to customize the formula string format. By, default the formula format is :

FormulaName(argument1, argument2, argument3......)

However, you can customize this as per your needs. For example, you can change it to:

FormulaName[argument1 | argument 2 | argument 3....]

Doing this is a piece of cake using engine settings as follows:
```c#
AlphaX.FormulaEngine.AlphaXFormulaEngine engine = new AlphaX.FormulaEngine.AlphaXFormulaEngine();
engine.Settings.ArgumentsSeparatorSymbol = "|";
engine.Settings.OpenBracketSymbol = "[";
engine.Settings.CloseBracketSymbol = "]";
engine.Settings.Save();

engine.AddFormula(new MyFormula());
AlphaX.FormulaEngine.IEvaluationResult result = engine.Evaluate("MyFormula[4|3]");
Console.WriteLine(result.Value); // 64
```
# Nested Formulas

To make your life easy, we have also added support for nested formulas. So, you can use a formula as a formula argument for another formula as follows:
```c#
AlphaX.FormulaEngine.IEvaluationResult result = engine.Evaluate("MyFormula(4, MyFormula(2,2))");
Console.WriteLine(result.Value); // 256
```

That's all of it :-)