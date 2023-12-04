using System;
using System.Collections.Generic;

namespace VContainer.SourceGenerator.CodeBuilder
{
    public class CodeBuilder : IScope
    {
        class Scope : IScope
        {
            public string StartLine { get; }
            public IScope ParentScope { get; }
            readonly List<Action<CodeWriter>> codeOrder = new List<Action<CodeWriter>>();

            public Scope(IScope parentScope, string startLine)
            {
                StartLine = startLine;
                ParentScope = parentScope;
            }
            public IScope AddLineToScopeStart(string line)
            {
                codeOrder.Insert(0, writer => writer.AppendLine(line));
                return this;
            }
            public IScope AddLine(string line)
            {
                codeOrder.Add(writer => writer.AppendLine(line));
                return this;
            }
            public IScope StartScope(string startLine)
            {
                var scope = new Scope(this, startLine);
                codeOrder.Add(writer => WriteScope(scope, writer));
                return scope;
            }

            static void WriteScope(IScope scope, CodeWriter writer)
            {
                using (writer.BeginBlockScope(scope.StartLine))
                {
                    scope.Build(writer);
                }
            }
            public IScope Build(CodeWriter codeWriter)
            {
                foreach (var action in codeOrder)
                {
                    action?.Invoke(codeWriter);
                }

                return this;
            }
            public IScope Break()
            {
                return ParentScope;
            }

            public void Clear()
            {
                codeOrder.Clear();
            } 
        }

        Scope root = new Scope(null!, string.Empty);
        
        public string StartLine => root.StartLine;
        public IScope ParentScope => root.ParentScope;

        public IScope AddLineToScopeStart(string line)
        {
            root.AddLineToScopeStart(line);
            return this;
        }

        public IScope AddLine(string line)
        {
            root.AddLine(line);
            return this;
        }

        public IScope StartScope(string startLine)
        {
            return root.StartScope(startLine);
        }

        public IScope Build(CodeWriter codeWriter)
        {
            root.Build(codeWriter);
            return this;
        }

        public CodeBuilder AddUsing(string nameSpace)
        {
            AddLineToScopeStart($"using {nameSpace};");
            return this;
        }

        IScope IScope.Break()
        {
            return root.Break();
        }

        public void Clear()
        {
            root.Clear();
            root = new Scope(null!, string.Empty);
        }
    }
}