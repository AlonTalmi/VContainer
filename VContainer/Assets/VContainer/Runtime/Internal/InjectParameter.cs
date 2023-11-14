using System;
using System.Collections.Generic;

namespace VContainer.Internal
{
    sealed class InstanceProviderParameter : IInjectParameter
    {
        public readonly Type InterfaceType;
        public readonly IInjector Injector;
        public readonly IReadOnlyList<IInjectParameter> CustomParameters;

        public InstanceProviderParameter(Type interfaceType, Type implementType, IReadOnlyList<IInjectParameter> customParameters = null)
        {
            InterfaceType = interfaceType;
            CustomParameters = customParameters;
            Injector = InjectorCache.GetOrBuild(implementType);
        }
        
        public bool Match(Type parameterType, string _) => parameterType == InterfaceType;

        public object GetValue(IObjectResolver resolver)
        {
            return Injector.CreateInstance(resolver, CustomParameters);
        }
    }
    
    sealed class TypedParameter : IInjectParameter
    {
        public readonly Type Type;
        public readonly object Value;

        public TypedParameter(Type type, object value)
        {
            Type = type;
            Value = value;
        }

        public bool Match(Type parameterType, string _) => parameterType == Type;
        
        public object GetValue(IObjectResolver _)
        {
            return Value;
        }
    }
    
    sealed class FuncTypedParameter : IInjectParameter
    {
        public readonly Type Type;
        public readonly Func<IObjectResolver, object> Func;

        public FuncTypedParameter(Type type, Func<IObjectResolver, object> func)
        {
            Type = type;
            Func = func;
        }

        public bool Match(Type parameterType, string _) => parameterType == Type;
        
        public object GetValue(IObjectResolver resolver)
        {
            return Func(resolver);
        }
    }

    sealed class NamedParameter : IInjectParameter
    {
        public readonly string Name;
        public readonly object Value;

        public NamedParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public bool Match(Type _, string parameterName) => parameterName == Name;
        
        public object GetValue(IObjectResolver _)
        {
            return Value;
        }
    }
    
    sealed class FuncNamedParameter : IInjectParameter
    {
        public readonly string Name;
        public readonly Func<IObjectResolver, object> Func;

        public FuncNamedParameter(string name, Func<IObjectResolver, object> func)
        {
            Name = name;
            Func = func;
        }

        public bool Match(Type _, string parameterName) => parameterName == Name;
        
        public object GetValue(IObjectResolver resolver)
        {
            return Func(resolver);
        }
    }
}
