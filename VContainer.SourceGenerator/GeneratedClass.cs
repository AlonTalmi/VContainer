namespace VContainer.SourceGenerator
{
    public readonly struct GeneratedClass
    {
        public readonly string Name;
        public readonly string Content;

        public GeneratedClass(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}