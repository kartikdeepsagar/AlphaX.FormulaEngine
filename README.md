# DevBrewLabs.Evalis

A robust, extensible, and blazing fast engine to parse and evaluate formulas dynamically. 
Built on top of [DevBrewLabs.Parserly](https://www.nuget.org/packages/DevBrewLabs.Parserly), it seamlessly supports both natively provided formulas and your own custom logic.

🔗 [DevBrewLabs.Evalis GitHub Repo](https://github.com/kartikdeepsagar/DevBrewLabs.Evalis)

---

## 🚀 What's New in v1.0.1
- **🚀 ~3x Performance Boost & Engine Optimization:**
  - **⚡ Up to 3x Faster Execution:** Major hot-path evaluation optimizations deliver up to a 3x times reduction in evaluation latency** and significantly lower GC memory pressure across complex expressions.
  - **AST Postfix Caching (`ConditionalWeakTable`):** Zero-allocation caching of transformed postfix ASTs for previously parsed expressions, eliminating repeat tree transformation overhead during repeated evaluations.
  - **Fast-Path Operator Priorities:** Removed dictionary lookups in favor of O(1) `switch` statements for resolving operator priorities.
  - **Pre-Allocated Collection Capacities:** Internal stacks and lists in `InfixToPostfix` are now pre-sized based on expression length to eliminate dynamic array reallocations.
  - **Unboxed Numeric Operations:** Optimized `AsDouble` type resolution for primitive numbers (`int`, `float`, `byte`, etc.) to eliminate unnecessary object boxing during arithmetic and logical comparisons.

---

## ⚡ Performance Benchmarks

> 📊 **Benchmark Environment & Configuration:**  
> - **Runtime:** .NET 10.0 (`DevBrewLabs.Evalis.PlayGround`)  
> - **Iterations:** **500 iterations per test case**   

| Test Case | Category | Total (ms) | Avg (ms/op) | Avg (µs/op) | Throughput |
| :--- | :--- | :---: | :---: | :---: | :---: |
| Large Integer SUM (10,000 args) | Aggregation | 560.2ms | 1.1204ms | 1,120.4µs | 893 ops/s |
| Complex Nested Math & Rounding | Arithmetic | 2.3ms | 0.0046ms | 4.6µs | 219,722 ops/s |
| Trig & Exponential Formulas | Arithmetic | 1.3ms | 0.0025ms | 2.5µs | 398,121 ops/s |
| Nested String Transformations | String | 4.6ms | 0.0092ms | 9.2µs | 108,321 ops/s |
| Multi-Condition IF Branching | Logical | 1.9ms | 0.0038ms | 3.8µs | 262,467 ops/s |
| Multi-Branch IFS Matrix | Logical | 2.0ms | 0.0041ms | 4.1µs | 245,845 ops/s |
| Array Membership Search | Array | 0.6ms | 0.0012ms | 1.2µs | 814,598 ops/s |
| Deep Call Tree (15 Levels) | Recursion | 2.6ms | 0.0051ms | 5.1µs | 194,235 ops/s |
| Real-World Business Rule | Business | 1.9ms | 0.0038ms | 3.8µs | 260,417 ops/s |
| Error Handling & Coalesce | Logical | 8.4ms | 0.0168ms | 16.8µs | 59,645 ops/s |
| **TOTAL / OVERALL METRICS** | **All** | **585.8ms** | **0.1172ms** | **117.2µs** | **8,535 ops/s** |

---

## ⚡ Quick Start

You can initialize the engine and evaluate expressions either synchronously or asynchronously:

```csharp
using DevBrewLabs.Evalis;

FormulaEngine engine = new FormulaEngine();

// Synchronous Evaluation
IEvaluationResult resultSync = engine.Evaluate("SUM(1, 2, 12.3, 5.9)");
Console.WriteLine(resultSync.Value); // 21.2

// Asynchronous Evaluation
IEvaluationResult resultAsync = await engine.EvaluateAsync("SUM(1, 2, 12.3, 5.9)");
Console.WriteLine(resultAsync.Value); // 21.2
```

> **Pro Tip:** Formulas can naturally nest! Feel free to parse deep trees natively: `(SUM(1,2) + AVERAGE(5,10)) * 10`.

---

## 📚 Inbuilt Formulas

DevBrewLabs.Evalis ships with a wide array of powerful formulas right out of the box:

- **🧮 Arithmetic**: `SUM`, `AVERAGE`, `FLOOR`, `ROUND`, `MIN`, `MAX`, `POWER`, `SQRT`
- **🔤 String**: `LOWER`, `UPPER`, `TEXTSPLIT`, `CONCAT`, `LENGTH`, `TRIM`, `SUBSTRING`, `INDEXOF`, `REGEXMATCH`
- **📅 DateTime**: `TODAY`, `NOW`, `YEAR`, `MONTH`, `DAY`, `DATETIME`
- **🧠 Logical**: `EQUALS`, `GREATERTHAN`, `OR`, `AND`, `IF`, `COALESCE`, `ISNUMBER`, `ISSTRING`
- **📦 Array**: `ARRAYCONTAINS`, `ARRAYINCLUDES`, `INDEX`, `JOIN`, `COUNT`

👉 **[Click here to see the full list of inbuilt formulas and examples](https://github.com/kartikdeepsagar/DevBrewLabs.Evalis/blob/master/Formulas.md)**

---

## 🛠 Creating Your Own Formulas

DevBrewLabs.Evalis provides maximum flexibility to write and integrate your own custom logic effortlessly.

### 1. Create a `Formula` Class
Inherit from `DevBrewLabs.Evalis.Formula`. Below is a custom `StartsWith` formula implementation:

```csharp
public class StartsWithFormula : DevBrewLabs.Evalis.Formula
{
    public StartsWithFormula() : base("StartsWith") { }

    public override IEvaluationResult Evaluate(IFormulaContext context)
    {
        // Argument counts are now automatically validated by the engine!
        // Retrieve arguments safely using the context methods:
        string source = context.GetStringArg(0); 
        string value = context.GetStringArg(1);

        // Safely retrieves the 3rd argument, or defaults to false
        context.TryGetArg(2, out bool matchCase); 
        
        bool result = source.StartsWith(value, matchCase ? StringComparison.Ordinal : StringComparison.InvariantCultureIgnoreCase);
        return EvaluationResult.WithValue(result);
    }

    protected override FormulaInfo GetFormulaInfo()
    {
        FormulaInfo info = new FormulaInfo(Name)
        {
            Description = "Checks if the provided string starts with the specified value."
        };

        // Define arguments for function documentation/validation
        info.AddArgument(new StringArgument("source", true) { Description = "The source string." });
        info.AddArgument(new StringArgument("value", true) { Description = "The value to check for." });
        info.AddArgument(new BooleanArgument("matchCase", false) { Description = "Match case while checking." });
        
        return info;
    }
}
```

### 2. Register & Evaluate
Simply add your formula to the `FormulaStore` and it is immediately ready for use!

```csharp
FormulaEngine engine = new FormulaEngine();
engine.FormulaStore.Add(new StartsWithFormula());

var result1 = engine.Evaluate("StartsWith(\"This is test\", \"This\")");
Console.WriteLine(result1.Value); // true

var result2 = engine.Evaluate("StartsWith(\"This is test\", \"hello\")");
Console.WriteLine(result2.Value); // false
```

### 3. Asynchronous Formulas

Need to fetch data from an API or database during evaluation? Simply inherit from `DevBrewLabs.Evalis.AsyncFormula`:

```csharp
public class FetchDataFormula : DevBrewLabs.Evalis.AsyncFormula
{
    public FetchDataFormula() : base("FETCH") { }

    public override async Task<IEvaluationResult> EvaluateAsync(IFormulaContext context)
    {
        string url = context.GetStringArg(0);
        
        // Example: await your async calls natively!
        string result = await MyHttpClient.GetAsync(url); 
        
        return EvaluationResult.WithValue(result);
    }
    
    // ... GetFormulaInfo() omitted for brevity
}
```

> **Note:** To evaluate an AST that contains an async formula, you must execute the engine via `await engine.EvaluateAsync(...)` instead of the synchronous `engine.Evaluate(...)`.

---

## ⚙️ Advanced Configuration

FormulaEngine allows you to configure the engine to fit your exact domain needs.

### 1. Toggle String Quotes
By default, strings are parsed with double quotes (`"text"`). You can toggle this to accept single quotes (`'text'`) by updating the engine settings:
```csharp
engine.ApplySettings(new EngineSettings()
{
     DoubleQuotedStrings = false
});
```

### 2. Logical Operator Modes
You can configure the engine to parse query-like operators (`eq` instead of `=`) via `LogicalOperatorMode`.
```csharp
engine.ApplySettings(new EngineSettings()
{
     LogicalOperatorMode = LogicalOperatorMode.Query
});
```
**Query Mode Operators:**
- `=` → `eq` | `!=` → `ne`
- `<` → `lt` | `>` → `gt`
- `<=` → `le` | `>=` → `ge`
- `&&` → `and` | `||` → `or`

### 3. Parsing Optimization Order
You can manually sequence the type resolution tree (e.g. parse Numbers before Strings) to drastically improve performance if you know your data bounds:
```csharp
ParseOrder order = new ParseOrder(ParseType.Number);
order.Add(ParseType.String);
order.Add(ParseType.Boolean);

engine.ApplySettings(new EngineSettings() { EngineParseOrder = order });
```

---

## 🎯 Variables & Dependency Extraction

DevBrewLabs.Evalis allows you to inject variables directly into expressions by providing a custom `IEngineContext` to resolve their values at runtime.

### Standard Variables
By default, variables are prefixed with `$`:

```csharp
public class TestEngineContext : IEngineContext
{
    public async Task<object> Resolve(string key)
    {
        return key switch
        {
            "UserId" => 1024,
            "Role" => "Admin",
            _ => throw new Exception("Invalid variable name")
        };
    }
}

FormulaEngine engine = new FormulaEngine(new TestEngineContext());
IEvaluationResult result = engine.Evaluate("EQUALS($UserId, 1024)"); // true
```

### Custom Token Parsers
If you are building a complex rule engine, you may want to parse variables without the `$` prefix, for example `[Col Name]` or cell references like `A1:B10`. You can inject `CustomTokenParsers` into the engine settings:

```csharp
using DevBrewLabs.Parserly;

// Create a custom RegexParser
public class MyCustomTokenParser : RegexParser<StringResult>
{
    public MyCustomTokenParser(string pattern) 
        : base(new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.Compiled), true) { }

    protected override StringResult ConvertResult(System.Text.RegularExpressions.Match value) => new StringResult(value.Value);
    protected override IParserError CreateError(int index, string value) => new ParserError(index, "Unexpected custom token");
}

// Apply settings
var settings = new EngineSettings();
settings.CustomTokenParsers = new List<IParser>
{
    new MyCustomTokenParser(@"^\[[A-Za-z0-9_ ]+\]"), // e.g. [Column Name]
    new MyCustomTokenParser(@"^[A-Za-z]+[0-9]+:[A-Za-z]+[0-9]+") // e.g. A1:B10
};
engine.ApplySettings(settings);

// The engine now parses these custom tokens as Variables!
// They will be passed directly into your `IEngineContext.Resolve` method during evaluation!
```

### AST Dependency Extraction
For building dependency graphs (e.g. knowing which fields to recalculate when a variable changes), you can statically extract all parsed variables from a formula *without* evaluating it:

```csharp
var variables = engine.ExtractVariables("SUM([Tax], A2, [Subtotal])");

// variables: ["[Tax]", "A2", "[Subtotal]"]
```

---

## 🔗 Sequenced / Chained Expressions

Evaluating massive, complicated expression walls can be extremely difficult to read or debug (e.g. `SUM(1, 2, AVERAGE(1, 2, SUM(1, 2, 12)))`).

Evalis provides a `SequencedExpressionBuilder` to break these down natively into readable variables:

```csharp
var engine = new FormulaEngine();

var expression = SequencedExpressionBuilder
    .Create("Step1", "SUM(1, 2, 12)")
    .Next("Step2", "AVERAGE(1, 2, $Step1)")
    .Next("Final", "SUM(1, 2, $Step2)");

var result = engine.Evaluate(expression); 
// Final Result evaluates properly by cascading through the sequenced variables!
```

---

*Built by developers, for developers :-)*