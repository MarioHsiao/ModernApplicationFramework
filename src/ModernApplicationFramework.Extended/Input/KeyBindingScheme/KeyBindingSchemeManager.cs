using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Extended.Input.KeyBindingScheme
{
    public abstract class KeyBindingSchemeManager : IKeyBindingSchemeManager
    {      
        protected IKeyGestureService GestureService { get; }

        public ICollection<SchemeDefinition> SchemeDefinitions { get; protected set; }
        
        public SchemeDefinition CurrentScheme { get; protected set; }

        protected KeyBindingSchemeManager()
        {
            SchemeDefinitions = new List<SchemeDefinition>();
            GestureService = IoC.Get<IKeyGestureService>();
        }

        public abstract void LoadSchemeDefinitions();

        public void SetScheme(SchemeDefinition definition)
        {
            if (!SchemeDefinitions.Contains(definition))
                SchemeDefinitions.Add(definition);
            CurrentScheme = definition;
        }
    }

    [Export(typeof(IKeyBindingSchemeManager))]
    public sealed class DefaultKeyBindingSchemeManager : KeyBindingSchemeManager
    {
        public override void LoadSchemeDefinitions()
        {
            CreateDefaultScheme();
        }

        private void CreateDefaultScheme()
        {
            SchemeDefinitions.Add(new DefaultSchemeDefinition());
        }

        private class DefaultSchemeDefinition : SchemeDefinition
        {
            public DefaultSchemeDefinition() : base("Default")
            {
            }

            public override KeyBindingScheme Load()
            {
                var commands = IoC.GetAll<CommandDefinitionBase>().OfType<CommandDefinition>();


                var possibleCommads = commands.Where(x => x.DefaultKeyGestures != null || x.DefaultGestureScope != null);

                var list = GetMappings(possibleCommads);


                //var list = commands.Where(x => x.DefaultKeyGesture != null || x.DefaultGestureScope != null)
                //    .Select(command =>
                //    {
                //        return new CommandGestureScopeMapping(command,
                //            new GestureScopeMapping(command.DefaultGestureScope, command.DefaultKeyGesture));
                //    });
                return new KeyBindingScheme(Name, list);
            }

            private IEnumerable<CommandGestureScopeMapping> GetMappings(IEnumerable<CommandDefinition> commands)
            {
                return from command in commands
                    from gesture in command.DefaultKeyGestures
                    select new CommandGestureScopeMapping(command,
                        new GestureScopeMapping(command.DefaultGestureScope, gesture));
            }
        }
    }
}