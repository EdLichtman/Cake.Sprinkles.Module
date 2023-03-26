namespace Cake.Sprinkles.Module.Annotations.Arguments
{
    /// <summary>
    /// Allows you to describe that a Task Argument is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentIsRequiredAttribute : Attribute
    {
        /// <summary>
        /// Gets a value indicating whether the Task Argument is required.
        /// </summary>
        public bool IsRequired => true;
    }
}
