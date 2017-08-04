﻿namespace ModernApplicationFramework.Extended.Input
{
    public abstract class SchemeDefinition
    {
        public string Name { get; }

        protected SchemeDefinition(string name)
        {
            Name = name;
        }
       
        public abstract KeyBindingScheme Load();
    }
}