using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VContainer.SourceGenerator.CodeBuilder
{
    public class ObjectDefinition
    {
        public static ObjectDefinition ClassDefault => new ObjectDefinition();
        public static ObjectDefinition StructDefault => new ObjectDefinition();

        public AccessibilityLevel AccessibilityLevel { get; set; } = AccessibilityLevel.Public;
        public bool IsStatic { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsPartial { get; set; }
        public List<string> Interfaces { get; } = new List<string>();
        public List<string> GenericTypes { get; } = new List<string>();

        public ObjectDefinition WithInterface(string interfaceName)
        {
            Interfaces.Add(interfaceName);
            return this;
        }
            
        public ObjectDefinition WithGenericType(string genericTypeName)
        {
            GenericTypes.Add(genericTypeName);
            return this;
        }

        public ObjectDefinition AndPartial()
        {
            IsPartial = true;
            return this;
        }

        public string CreateStructStartLine(string name)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(string.Join(" ", StartLineParts().Where(s => !string.IsNullOrWhiteSpace(s))));

            AppendGenerics(stringBuilder);
            AppendInterfaces(stringBuilder);
                
            return stringBuilder.ToString();

            IEnumerable<string?> StartLineParts()
            {
                yield return AccessibilityLevel.GetAccessibilityLevelName();
                yield return IsReadOnly ? "readonly" : null;
                yield return IsPartial ? "partial" : null;
                yield return "struct";
                yield return name;
            }
        }
            
        public string CreateClassStartLine(string name)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(string.Join(" ", StartLineParts().Where(s => !string.IsNullOrWhiteSpace(s))));

            AppendGenerics(stringBuilder);
            AppendInterfaces(stringBuilder);
                
            return stringBuilder.ToString();
            
            IEnumerable<string?> StartLineParts()
            {
                yield return AccessibilityLevel.GetAccessibilityLevelName();
                yield return IsStatic ? "static" : null;
                yield return IsPartial ? "partial" : null;
                yield return "class";
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

        void AppendInterfaces(StringBuilder stringBuilder)
        {
            if (Interfaces.Count > 0)
            {
                stringBuilder.Append($" : {string.Join(",", Interfaces)}");
            }
        }
    }
}