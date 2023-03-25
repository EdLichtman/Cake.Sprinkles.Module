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

* Attributes:
  * ***TaskArgumentNameAttribute***
    * Allows you to decorate a property with an argument name. Whatever you add here can be passed in with --{argument_name}
  * ***TaskArgumentDescriptionAttribute***
    * Allows you to decorate a property with an argument description. Whatever you add here will be written to the console when the user runs --description with the --target=TaskName
  * ***TaskArgumentExampleValueAttribute***
    * Allows you to decorate a property with examples of how to use the argument. Whatever you add here will be output to the console with "Usage: --{argument_name}={example_value}"
  * ***TaskArgumentIsRequiredAttribute***
    * Allows you to automatically prevent the task from continuing if the task argument is not provided by the user
  * ***TaskArgumentIsFlagAttribute***
    * Allows the user to use input based on the existence of a flag. i.e. --force. If an argument is a flag, you can still override the flag value, i.e. --force=false
  * ***TaskArgumentEnumerationDelimiterAttribute***
    * Allows you to specify a delimiter which will split an argument. If you use a cake.config file, you can only specify an argument once, so if you need multiple values, this attribute will be useful.
  * ***TaskArgumentsAttribute***
    * Allows you to offload task arguments to another object. For example, you can have a "PublishTask", and you can have a property "PublishArguments". By decorating the property with **`[TaskArguments]`**, you're telling Sprinkles to create and hydrate that Property with any Arguments. 
    * TaskArgumentsAttribute existence on one property doesn't prevent you from adding other, custom arguments to your task. This way you can share some arguments and custom declare others.

* `TaskArgumentTypeConverter<TType>`
  * An abstract class to create an implementation of. It allows you to customize your own types. For example, if you wanted to create a custom conversion from a globbed file path to the first globbed file, you could use a TaskArgumentTypeConverter.
  * To use it, you must register it on the CakeHost with the `public static CakeHost RegisterTypeConverter<TType>(this CakeHost host);` method.
  * If you use this, none of the parsing attributes work.
  * An optional override (not required) is the `IEnumerable<List> GetExampleValues();` method. It will automatically populate Usages for any Task Arguments 

* Validation

## Roadmap:
* Need to finish Documenting.
