using System;
using System.Collections.Generic;
using VContainer.Internal;

namespace VContainer
{
    public class RegistrationBuilder : IRegistrationBuilder
    {
        public Type ImplementationType { get; protected internal set; }
        
        protected internal readonly Lifetime Lifetime;

        protected internal List<Type> InterfaceTypes;
        IReadOnlyList<Type> IRegistrationBuilder.InterfaceTypes => InterfaceTypes;
        
        protected internal List<IInjectParameter> Parameters;

        public RegistrationBuilder(Type implementationType, Lifetime lifetime)
        {
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }


        public virtual Registration Build()
        {
            var injector = InjectorCache.GetOrBuild(ImplementationType);
            var spawner = new InstanceProvider(injector, Parameters);
            return new Registration(
                ImplementationType,
                Lifetime,
                InterfaceTypes,
                spawner);
        }

        public IRegistrationBuilder AsSelf()
        {
            AddInterfaceType(ImplementationType);
            return this;
        }

        public virtual IRegistrationBuilder AsImplementedInterfaces()
        {
            InterfaceTypes = InterfaceTypes ?? new List<Type>();
            InterfaceTypes.AddRange(ImplementationType.GetInterfaces());
            return this;
        }

        public IRegistrationBuilder As(Type interfaceType)
        {
            AddInterfaceType(interfaceType);
            return this;
        }

        public IRegistrationBuilder As(Type interfaceType1, Type interfaceType2)
        {
            AddInterfaceType(interfaceType1);
            AddInterfaceType(interfaceType2);
            return this;
        }

        public IRegistrationBuilder As(Type interfaceType1, Type interfaceType2, Type interfaceType3)
        {
            AddInterfaceType(interfaceType1);
            AddInterfaceType(interfaceType2);
            AddInterfaceType(interfaceType3);
            return this;
        }

        public IRegistrationBuilder As(params Type[] interfaceTypes)
        {
            foreach (var interfaceType in interfaceTypes)
            {
                AddInterfaceType(interfaceType);
            }
            return this;
        }

        public IRegistrationBuilder WithParameter(string name, object value)
        {
            Parameters = Parameters ?? new List<IInjectParameter>();
            Parameters.Add(new NamedParameter(name, value));
            return this;
        }
        
        public IRegistrationBuilder WithParameter(string name, Func<IObjectResolver, object> value)
        {
            Parameters = Parameters ?? new List<IInjectParameter>();
            Parameters.Add(new FuncNamedParameter(name, value));
            return this;
        }

        public IRegistrationBuilder WithParameter(Type type, object value)
        {
            Parameters = Parameters ?? new List<IInjectParameter>();
            Parameters.Add(new TypedParameter(type, value));
            return this;
        }
        
        public IRegistrationBuilder WithParameter(Type type, Func<IObjectResolver, object> value)
        {
            Parameters = Parameters ?? new List<IInjectParameter>();
            Parameters.Add(new FuncTypedParameter(type, value));
            return this;
        }

        protected virtual void AddInterfaceType(Type interfaceType)
        {
            if (!interfaceType.IsAssignableFrom(ImplementationType))
            {
                throw new VContainerException(interfaceType, $"{ImplementationType} is not assignable from {interfaceType}");
            }
            InterfaceTypes = InterfaceTypes ?? new List<Type>();
            if (!InterfaceTypes.Contains(interfaceType))
                InterfaceTypes.Add(interfaceType);
        }
   }
}
