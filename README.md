

# AlphaX.FormulaEngine

A strong and fast library to parse and evaluate formulas. It also supports custom formulas. This library is built using '[AlphaX.Parserz](https://www.nuget.org/packages/AlphaX.Parserz)' library.

GitHub Repo : https://github.com/kartikdeepsagar/AlphaX.FormulaEngine

Feedback is very much appreciated : https://forms.gle/dfv8E8zpC2qPJS7i7

# Using AlphaXFormulaEngine

For evaluating formulas using AlphaXFormulaEngine, you can simply initialize the engine and start using its Evaluate method:
```c#
AlphaX.FormulaEngine.AlphaXFormulaEngine engine = new AlphaX.FormulaEngine.AlphaXFormulaEngine();
AlphaX.FormulaEngine.IEvaluationResult result = engine.Evaluate("SUM([1,2,12.3,5.9099])");
Console.WriteLine(result.Value); // 21.2099
```

AlphaXFormulaEngine comes with a limited number (not many) of inbuilt formulas i.e. 

#### Arithmetic Formulas
- SUM - Returns sum of provided values. For example: SUM([1,2,4]) // 7
- AVERAGE - Returns average of provided values. For example: AVERAGE([3,2,4]) // 3

#### String Formulas
- LOWER - Returns lower cased string. For example: LOWER("TESTSTRING") // teststring
- UPPER - Returns upper cased string. For example: UPPER("teststring") // TESTSTRING
- TEXTSPLIT - Returns splitted string using a seperator. For example: TEXTSPLIT(".", "John.Doe") // John  Doe
- CONCAT - Joins multiple strings into one string: For example: CONCAT(["Test","String","1"]) // TestString1
- LENGTH - Gets the length of a string. For example: LENGTH("AlphaX")  // 6
- CONTAINS - Checks if a string contains another string. For example: CONTAINS("AlphaX", "pha")  // true
- STARTSWITH - Checks if a string starts with the provided string. Accepts third (optional) parameter as boolean to match case. Default = false. For example: STARTSWITH("AlphaX", "Al", true)  // true
- ENDSWITH - Checks if a string starts with the provided string. Accepts third (optional) parameter as boolean to match case. Default = false. For example: ENDSWITH("AlphaX", "Al")  // false

#### DateTime Formulas
- TODAY - Returns system date. For example: TODAY() // 28-04-2023
- NOW -  Returns system date time // 28-04-2023 10:52:53 PM

#### Logical Formulas
- EQUALS - Checks if two values/expressions are equal. For example: EQUALS(true, 1 > 3)  // false
- GREATERTHAN - Checks if one value/expressions is greater than other. For example: GREATERTHAN(5,2)  // true
- GREATERTHANEQUALS - Checks if one value/expressions is greater than or equal to other. For example: GREATERTHANEQUALS(5,2)  // true
- LESSTHAN - Checks if one value/expressions is less than other. For example: LESSTHAN(5,2)  // false
- LESSTHANEQUALS - Checks if one value/expressions is less than or equal to other. For example: LESSTHANEQUALS(5,2)  // false
- NOT - Inverse a boolean value/expression. For example: NOT(1 == 1)  // false
- AND - Performs AND (&&) operation between 2 boolean values/expressions. For example: AND(true, 1 != 1)  // false
- OR - Performs OR (||) operation between 2 boolean values/expressions. For example: OR(true, 1 != 1)  // true
- IF - Checks whether condition is met. Returns first value if true and return second value if false. For example: IF(UPPER(\"alphax\") = UPPER(\"ALphaX\"), true, false) // true

#### Array Formulas
- ARRAYCONTAINS - Checks if array contains a value. For example: ARRAYCONTAINS([1,2,3], 2)  // true
- ARRAYINCLUDES - Checks if array includes all values. For example: ARRAYINCLUDES([1,2,3,4], [3,4])  // true

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

### Formula format

AlphaXFormulaEngine allows you to customize the formula string format. By, default the formula format is :

FormulaName(argument1, argument2, argument3......)

However, you can customize this as per your needs. For example, you can change it to:

FormulaName[argument1 | argument 2 | argument 3....]

Doing this is a piece of cake using engine settings as follows:
```c#
AlphaX.FormulaEngine.AlphaXFormulaEngine engine = new AlphaX.FormulaEngine.AlphaXFormulaEngine();
// apply settings
_formulaEngine.ApplySettings(new EngineSettings()
{
     OpenBracketSymbol = "[",
     CloseBracketSymbol = "]",
     ArgumentsSeparatorSymbol = "|",
});

engine.AddFormula(new MyFormula());
AlphaX.FormulaEngine.IEvaluationResult result = engine.Evaluate("MyFormula[4|3]");
Console.WriteLine(result.Value); // 64
```
### Operator Mode

AlphaXFormulaEngine allows you to choose different type logical operators. For example, You can use 'eq' instead of '=' with logical expression. For example, IF(1 eq 1, true, false)
```c#
_formulaEngine.ApplySettings(new EngineSettings()
{
     LogicalOperatorMode = LogicalOperatorMode.Query
});
```
Below are the current supported operators in Query mode.
```
'=' as 'eq'
'!=' as 'ne'
'<' as 'lt'
'>' as 'gt'
'<=' as 'le'
'>=' as 'ge'
'&&' as 'and'
'||' as 'or'
```
### Parse Order

AlphaXFormulaEngine allows you to optimize the engine performance based on your use case. You can argument parsing order. For example, if your use case requires formulas with only the number arguments then you can specify the parse order as:
```c#
ParseOrder order = new ParseOrder(ParseType.Number); // first try to parse a number
order.Add(ParseType.String) //then try to parse a string.
order.Add(ParseType.Boolean) // then try to parse a boolean i.e. true/false
EngineSettings settings = new EngineSettings();
settings.EngineParseOrder = order;
_formulaEngine.ApplySettings(settings);
```
The same thing can be done to provide the parse order for the values present inside array argument using EngineSetting's ArrayParseOrder property.

# Custom Name as Arguments
AlphaXFormulaEngine allows you to use custom names as formula arguments which can be resolved using AlphaX.FormulaEngine.IEngineContext.

For example, we can provide support for the 'CustomName1', ' 'CustomName2, 'CustomName3' custom names as follows:
1. Create an engine context using IEngineContext interface:
```c#
public class TestEngineContext : IEngineContext
{
    public object Resolve(string key)
    {
        switch (key)
        {
            case "CustomName1":
                return 2; // return 2 if custom name is 'CustomName1'

            case "CustomName2":
                return 10; // return 10 if custom name is 'CustomName2'

            case "CustomName3":
                return "TestString"; // return 'TestString' if custom name is 'CustomName3'
        }

        throw new Exception("Invalid custom name");
    }
}
```
2. Now pass the context to the engine as follows:
```c#
 AlphaXFormulaEngine formulaEngine = new AlphaXFormulaEngine(new TestEngineContext());
```
Now since we have create our context. we can simply evaluate it as follows:
```
AlphaX.FormulaEngine.IEvaluationResult result = engine.Evaluate("EQUALS($CustomName1, 2)");
Console.WriteLine(result.Value);  // true
```

# Nested Formulas

To make your life easy, we have also added support for nested formulas. So, you can use a formula as a formula argument for another formula as follows:
```c#
AlphaX.FormulaEngine.IEvaluationResult result = engine.Evaluate("MyFormula(4, MyFormula(2,2))");
Console.WriteLine(result.Value); // 256
```

That's all of it :-)