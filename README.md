# Dwaas.Param.Documentation.Analyzer

Uses a dictionary of names and descriptions to verify and correct C# &lt;param> comment documentation tags.

## Configuration

Add a file named *Dwaas.Param.Documentation.Analyzer.json* to the project and set it's *BuildAction* to *AdditionalFiles* in the properties window.

The file should contain a list of the parameter or property names and their descriptions, for example:

```
{
  "Example1": "A description for the param 'Example1'.",
  "Example2": "A description for the param 'Example2'."
}
```