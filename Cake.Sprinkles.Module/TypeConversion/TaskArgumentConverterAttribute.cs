﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.TypeConversion
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TaskArgumentConverterAttribute : Attribute
    {
        public ITaskArgumentTypeConverter? Converter { get; }

        public TaskArgumentConverterAttribute(Type type)
        {
            if (type.IsAssignableTo(typeof(ITaskArgumentTypeConverter)))
            {
                Converter = (ITaskArgumentTypeConverter)type;
            }
        }
    }
}