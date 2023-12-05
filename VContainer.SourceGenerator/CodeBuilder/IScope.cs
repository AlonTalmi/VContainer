namespace VContainer.SourceGenerator.CodeBuilder
{
    public interface IScope
    {
        string StartLine { get; }
        IScope ParentScope { get; }
            
        IScope AddLineToScopeStart(string line);
        IScope AddLine(string line);
        IScope StartScope(string startLine);
        void WriteTo(CodeWriter codeWriter);
        IScope Break();
    }
}