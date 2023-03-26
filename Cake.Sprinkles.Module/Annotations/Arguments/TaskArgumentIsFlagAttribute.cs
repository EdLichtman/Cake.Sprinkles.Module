namespace Cake.Sprinkles.Module.Annotations.Arguments
{
    /// <summary>
    /// Allows you to describe that the Task Argument accepts a flag, and converts it into true or false. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentIsFlagAttribute : Attribute
    {
        /// <summary>
        /// Gets a value indicating whether the Task Argument accepts a flag.
        /// </summary>
        public bool IsFlag => true;
    }
}
