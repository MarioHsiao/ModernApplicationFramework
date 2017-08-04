namespace ModernApplicationFramework.Extended.Input
{
    public abstract class FileSchemeDefinition : SchemeDefinition
    {
        public string FileLocation { get; }

        protected FileSchemeDefinition(string name, string fileLocation) : base(name)
        {
            FileLocation = fileLocation;
        }
    }
}