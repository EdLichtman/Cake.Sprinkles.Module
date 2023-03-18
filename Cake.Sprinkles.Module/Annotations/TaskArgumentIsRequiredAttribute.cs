namespace Cake.Sprinkles.Module.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentIsRequiredAttribute : Attribute
    {
        public bool IsRequired => true;
    }
}
