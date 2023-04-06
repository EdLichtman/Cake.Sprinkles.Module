# Cake Sprinkles
**Cake (C# Make)** is a build automation system with a C# DSL to do things like compiling code, copy files/folders, running unit tests, compress files and build NuGet packages.

**Cake Frosting** is a .NET host which allows you to write your own build scripts as a console application.

**Cake Sprinkles** is a layer on top of Cake Frosting. It's an add-in Module to Cake to allow for Configurable TaskArguments to be instantiated through decoration.

## What are benefits?
### Cleaner code
A **Cake Frosting** class may look like the following:
```c#
[TaskName("DeployWebApp")]
[TaskDescription("Deploys a Web Application.")]
public sealed class DeployWebAppTask : AsyncFrostingTask<BuildContext>
{
    public override async Task RunAsync(BuildContext context)
    {
      var webApplicationName = context.Argument<string>("web_app");
      var configuration = context.Argument<string>("configuration", "release");
       // do deployment logic here...
    }
}
```

However, **Cake Sprinkles** allows you to add TaskArgument Properties to your class, for your code to be self-documenting.

```c#
[TaskName("DeployWebApp")]
[TaskDescription("Deploys a Web Application.")]
public sealed class DeployWebAppTask : AsyncFrostingTask<BuildContext>
{
    [TaskArgumentName("web_app")]
    [TaskArgumentDescription("The name of the web application to deploy.")]
    [TaskArgumentIsRequired]
    public String WebApplicationName { get; set; }

    [TaskArgumentName("configuration")]
    [TaskArgumentDescription("The build configuration to run.")]
    public String BuildConfiguration { get; set; }

    public override async Task RunAsync(BuildContext context)
    {
       // do deployment logic here...
    }
}
```

### Enhanced documentation
When running your dotnet build tool, you can request a description.

Using the "DeployWebApp" task above...

```
>dotnet build --description
Task                          Description
================================================================================
DeployWebApp                  Deploys a Web Application.
```

You can provide an override to the "ShouldRun" method in a task...
```c#
[TaskName("DeployWebApp")]
[TaskDescription("Deploys a Web Application.")]
public sealed class DeployWebAppTask : AsyncFrostingTask<BuildContext>
{
    public override Task RunAsync(BuildContext context)
    {
      var webApplicationName = context.Argument<string>("web_app");
      var configuration = context.Argument<string>("configuration", "release");
       // do deployment logic here...
       return Task.FromResult(new NotImplementedException());
    }
    
    public override bool ShouldRun(BuildContext context) {
      if (context.Arguments.HasArgument("--description")) {
        // implement custom documentation
        return false;
      }
    }
}
```

But if you don't do that, you must run a task to find out whether or not an argument is required, and if the argument is optional, then any team member using the task must know the inner workings of the task, regardless of them being the author. Otherwise, you need to heavily document it for your team.

Once you add **Cake Sprinkles**, this is what you will see when you request a description.

```
>dotnet build --description
Task                          Description
================================================================================
DeployWebApp                  Deploys a Web Application.

================================================================================
Run this command while specifying target (-t,--target) to describe the allowed arguments.
```

Notice how you can now include --target, to specify the task. This is what happens when you run the description with a target.
```
>dotnet build --description --target=DeployWebApp
Task                          Description
================================================================================
DeployWebApp                  Deploys a Web Application.

The following properties are required:
 * web_app
   * Description: The name of the web application to deploy.
   * Accepts: String

The following properties are optional:
 * configuration
   * Description: The build configuration to run.
   * Accepts: String

```

### Extensible design
You can create your own ITaskArgumentTypeConverter<TType>. A simple example would be, your own globber converter to convert a glob into a DirectoryInfo or FileInfo. 

### It's intuitive
* There are no custom types.
  * Other libraries I've worked with in the past have required the user to understand library types, such as "FileSet". Even Cake Globber has it's own "built-in" type, called "Path", which is used to retrieve globs. 
    * This can be necessary, and helpful, for some things, but strictly speaking as an interface from the console to a build target, I prefer the user to define their own types for conversion. 
  * Therefore, the implementer knows exactly what they are intending to convert a string into, and how.
* Everything starts with TaskArgument, therefore it's easy to discover additional attributes.
* Naming conventions are easy to understand.

## Limitations
* Because of how cake works, you can pass multiple arguments in via command line. However, Cake accepts Environment Variables and cake.config file arguments. You can only have one of each Environment Variable, and if you have duplicate Cake.Config file arguments, only the last is used. Therefore, there is the built-in TaskArgumentEnumerationDelimiterAttribute to allow you to convert a cake.config argument into an enumerable. 
  * Note that the delimiter can be a string of any sort. For example, it can be ";", or it can be "{mysuperspecialdelimiterthatshouldneverbeinastring}"
  * Note that this means you cannot pass in multiples of the same argument if you've decorated an argument with this attribute.
  * i.e. Without Delimiter: --argument1=foo --argument1=bar
  * i.e. With Delimiter: --argument1=foo{delimiter}bar
* I wanted to add TaskCategoryAttribute, so that we could name tasks the same but in different categories, but due to the sheer scope of it, I would be rewriting Cake's graphing engine.
  * Same idea applies to TaskIsPrivateAttribute, or TaskIsReservedAttribute, the idea of having a task that could only be run by the task runner.
* Because of how Cake works, you can only have one TaskSetup. However, I can run an event handler on the BeforeTaskSetup and AfterTaskSetup events. But if I do that, the task continues to run, regardless of Sprinkles Decoration Failures.
  * Therefore, I had the option of "taking over the one IFrostingTaskSetup option" or "letting a task run without any arguments". I chose to take over the one allowed IFrostingTaskSetup option.

## API
The Programming Interface between you, the developer, and Sprinkles, includes the following:

* Enhanced Descriptions:
  * ***TaskArgumentNameAttribute***
    * Decorates a property with an argument name. Describes the name of the argument being passed in via the CLI, the cake.config file, or the environment variable.
  * ***TaskArgumentDescriptionAttribute***
    * Decorates a property with an argument description. Whatever you add here will described when the user runs --description with the --target=TaskName
  * ***TaskArgumentExampleValueAttribute***
    * Decorates a property with examples of how to use the argument. Whatever you add here will be output to the console with "Usage: --{argument_name}={example_value}"

* Enhanced Descriptions - Allowed with TypeConversion
  * ***TaskArgumentIsRequiredAttribute***
    * Automatically prevents the task from continuing if the task argument is not provided by the user, and describes this behavior to the user.
  
* Enhanced Parsing Behavior:
  * If a type can be converted from a string, then it will automatically convert it for you. For example, if you want an Int32 value, and the argument is "1", then it will parse it as 1.
    * Enum values can be parsed as well.
    * If a class has a single constructor, that accepts a string, it will parse it for you. 
      * For instance, a DirectoryInfo, or DateTime could be created from a string.
  * If a type has multiple arguments, (i.e. --argument=foo --argument=bar), you can parse it as a collection. You must use `ImmutableList<TType>` or `ImmutableHashSet<TType>`.
  * If a value is added to the environment variable or cake.config file, you cannot specify multiple arguments. Therefore, you can use...
    * ***TaskArgumentEnumerationDelimiterAttribute***
      * Specifies a delimiter which will split an argument. 
  * If a value is a flag (i.e. --has_argument) and has no value on it, you can mark the argument as a flag.
    * ***TaskArgumentIsFlagAttribute***
      * If an argument is a flag, you can still override the flag value.
        * i.e. --force=false
  * If you want to share common Task Arguments with other tasks, or simplify your Task Class by not having so much annotations on it, you can declare `[TaskArguments]`. Any property on the class described by `[TaskArguments]` will be populated as if it were on the Task.
    * ***TaskArgumentsAttribute***
        * By using this feature, you aren't prevented from adding other custom arguments to your task. You can still define `[TaskArgumentName()]` on one property, and `[TaskArguments]` on another property in the same class.

* Type Conversion
  * By implementing your own version of the `TaskArgumentTypeConverter<TType>`, you can customize your own conversion from a string. 
    * *For example, if you wanted to create a custom conversion from a string to a series of globbed file paths, you could implement this abstract interface.*
  * Once you create it, you must register it on the CakeHost with `host.RegisterTypeConverter<TType>()`.
  * Once used, you cannot use any of the built-in "Enhanced Parsing Behavior" attributes, other than the "Required" attribute. You can, however still use the "Enhanced Descriptions", as well as the "Required" attribute.
  * An optional override (not required) is the `IEnumerable<List> GetExampleValues();` method. It will automatically populate Usages for any Task Arguments.
  * You can implement multiple TypeConversions for the same type. 
    * *As an example: This may be useful if you want to create a glob converter that specifically only allows relative paths within your current directory, and another glob converter that allows you to go anywhere on disc.*
    * If you do this, any time you use this custom converted type, you must specify the TypeConverter
      * **TaskArgumentConverterAttribute**
        * Takes in a type, and allows you to specify your converter. i.e. `[TaskArgumentConverter(typeof(GlobPathInDirectoryConverter))]`

* Validation
  * By implementing your own version of the `[TaskArgumentValidation]` abstract attribute, you can add enhanced validation.
    * For example, you could add your own "Number must be between 0 and 255" validation attribute, which will prevent the task from running if the value coming in is incorrect.
