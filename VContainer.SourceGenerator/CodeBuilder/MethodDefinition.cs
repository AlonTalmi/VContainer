using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VContainer.SourceGenerator.CodeBuilder
{
    public class MethodDefinition
    {
        public static MethodDefinition Default => new MethodDefinition();
        
        public AccessibilityLevel AccessibilityLevel { get; set; } = AccessibilityLevel.Public;
        public bool IsStatic { get; set; }
        public bool IsPartial { get; set; }
        public List<string> GenericTypes { get; } = new List<string>();
        
        public MethodDefinition WithGenericType(string genericTypeName)
        {
            GenericTypes.Add(genericTypeName);
            return this;
        }
        
        public MethodDefinition AndPartial()
        {
            IsPartial = true;
            return this;
        }
        
        public MethodDefinition AndStatic()
        {
            IsStatic = true;
            return this;
        }
        
        public string CreateStartLine(string returnType, string name, IEnumerable<(string paramType, string paramName)> parameters)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(string.Join(" ", StartLineParts().Where(s => !string.IsNullOrWhiteSpace(s))));
            var paramsStr = $"({string.Join(", ", parameters.Select(type => $"{type.paramType} {type.paramName}"))})";
            stringBuilder.Append(paramsStr);

            AppendGenerics(stringBuilder);
            
            return stringBuilder.ToString();
            
            IEnumerable<string?> StartLineParts()
            {
                yield return AccessibilityLevel.GetAccessibilityLevelName();
                yield return IsStatic ? "static" : null;
                yield return IsPartial ? "partial" : null;
                yield return returnType;
                yield return name;
            }
        }

        void AppendGenerics(StringBuilder stringBuilder)
        {
            if (GenericTypes.Count > 0)
            {
                stringBuilder.Append($"<{string.Join(",", GenericTypes)}>");
            }
        }
    }
}