[![Build status](https://ci.appveyor.com/api/projects/status/dqhgm5oc0hy7852h/branch/master?svg=true)](https://ci.appveyor.com/project/alvinward/dwaas-param-documentation-analyzer/branch/master)

# Dwaas.Param.Documentation.Analyzer

Uses a dictionary of names and descriptions to verify and correct C# &lt;param> comment documentation tags.

## Installation

The Param Analyzer is available as a [NuGet package](https://www.nuget.org/packages/Dwaas.Param.Documentation.Analyzer/). You can install it using the NuGet Package Console window:

```
PM> Install-Package Dwaas.Param.Documentation.Analyzer
```

Or [download](https://ci.appveyor.com/project/alvinward/dwaas-param-documentation-analyzer/branch/master/artifacts) and install the Visual Studio Extension.

## Configuration

Add a file named *Dwaas.Param.Documentation.Analyzer.json* to the project (if it has not been added automatically) and set it's *BuildAction* to *AdditionalFiles* in the properties window.

The file should contain a list of the parameter or property names and their descriptions, for example:

```
{
  "Example1": "A description for the param 'Example1'.",
  "Example2": "A description for the param 'Example2'."
}
```