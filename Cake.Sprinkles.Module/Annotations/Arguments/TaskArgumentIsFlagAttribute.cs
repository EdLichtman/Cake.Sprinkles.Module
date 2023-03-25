namespace Cake.Sprinkles.Module.Annotations.Arguments
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentIsFlagAttribute : Attribute
    {
        public bool IsFlag => true;
    }
}
