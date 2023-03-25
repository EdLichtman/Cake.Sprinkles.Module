namespace Cake.Sprinkles.Module.Annotations.Arguments
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentIsRequiredAttribute : Attribute
    {
        public bool IsRequired => true;
    }
}
