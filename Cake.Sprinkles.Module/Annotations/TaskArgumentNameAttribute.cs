﻿namespace Cake.Sprinkles.Module.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskArgumentNameAttribute : Attribute
    {
        public string Name { get; }
        public TaskArgumentNameAttribute(string name)
        {
            Name = name;
        }
    }
}
