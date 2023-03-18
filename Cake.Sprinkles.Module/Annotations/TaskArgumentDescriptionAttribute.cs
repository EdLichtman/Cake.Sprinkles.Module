﻿namespace Cake.Sprinkles.Module.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentDescriptionAttribute : Attribute
    {
        public string Description { get; }
        public TaskArgumentDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
