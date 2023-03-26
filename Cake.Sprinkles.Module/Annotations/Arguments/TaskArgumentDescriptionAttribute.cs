namespace Cake.Sprinkles.Module.Annotations.Arguments
{
    /// <summary>
    /// Allows you to describe an argument to a user of your console application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Gets the Description of the Task Argument.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskArgumentDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="description">The description of the Task Argument.</param>
        public TaskArgumentDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
