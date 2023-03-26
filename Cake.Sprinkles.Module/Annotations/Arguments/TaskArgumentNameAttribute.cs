namespace Cake.Sprinkles.Module.Annotations.Arguments
{
    /// <summary>
    /// Allows you to define a Task Argument by describing the name that a user should include a value for.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentNameAttribute : Attribute
    {
        /// <summary>
        /// The Task Argument Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskArgumentNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The Task Argument Name.</param>
        public TaskArgumentNameAttribute(string name)
        {
            Name = name;
        }
    }
}
