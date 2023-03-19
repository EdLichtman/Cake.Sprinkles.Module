# This is the Cake Sprinkles Module
## What is it?

## How is it used?

## What are benefits?
### Extensible
* You can create your own ITaskArgumentTypeConverter<TType>. A simple example would be, your own globber converter to convert a glob into a DirectoryInfo or FileInfo. 
   * Note: This doesn't ship with one because I built this library to be Understandable... See "No custom types".
### Understandable
* No custom types. Other libraries I've worked with in the past have required the user to understand built-in class types, such as "FileSet". Even Cake Globber has it's own "built-in" type, called "Path", which is used to retrieve globs. This library was built to be 100% extensible for custom types (see extensible). That way, the implementer knows exactly what they are intending to convert a string into, and how.
* Task Argument Naming conventions is easy to understand
* Instead of using multiple constructor arguments, TaskArgumentAttributes require typing out the required Properties
### Discoverable
* Everything starts with TaskArgument
* Well-Documented
### Limitations
* Because of how cake works, you can pass multiple arguments in via command line. However, Cake accepts Environment Variables and cake.config file arguments. You can only have one of each Environment Variable, and if you have duplicate Cake.Config file arguments, only the last is used. Therefore, there is the built-in TaskArgumentEnumerationDelimiterAttribute to allow you to convert a cake.config argument into an enumerable. 
  * Note that the delimiter can be a string of any sort. For example, it can be ";", or it can be "{mysuperspecialdelimiterthatshouldneverbeinastring}"
  * Note that this means you cannot pass in multiples of the same argument if you've decorated an argument with this attribute.
  * i.e. Without Delimiter: --argument1=foo --argument1=bar
  * i.e. With Delimiter: --argument1=foo{delimiter}bar

## Roadmap:
* Need to finish Documenting.
* Need to complete all Not Implemented tests
* Need to add more tests, for Int64s, Singles, and Doubles. Am accepting suggestions for other data types to test.
* Need to consider whether I want one TaskArgumentAttribute that has Properties instead of a separate Attribute for each Property
* TaskArgumentsAttribute is still WIP. It should be as easy as checking first to see if the Property is a TaskArgumentsAttribute, and THEN running Sprinkles.Decorate on that type, but it will also need to be heavily tested.
* Need to add in the ITaskArgumentTypeConverter<TType> interface. It will be easy enough to add to the Sprinkles.Parse method, but needs to be heavily tested.