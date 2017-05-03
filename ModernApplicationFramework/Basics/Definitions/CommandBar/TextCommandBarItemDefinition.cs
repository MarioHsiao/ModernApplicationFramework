using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public class TextCommandBarItemDefinition : IHasTextProperty
    {
        private string _text;

        public TextCommandBarItemDefinition(string text)
        {
            _text = text;
        }

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}