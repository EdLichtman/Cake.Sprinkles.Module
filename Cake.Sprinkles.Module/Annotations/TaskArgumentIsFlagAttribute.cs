namespace Cake.Sprinkles.Module.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentIsFlagAttribute : Attribute
    {
        public bool IsFlag => true;
    }
}
