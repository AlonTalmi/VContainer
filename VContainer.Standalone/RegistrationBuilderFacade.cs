using System;
using System.Collections.Generic;
using VContainer.Internal;

namespace VContainer
{
    public class RegistrationBuilderFacade : IRegistrationBuilder
    {
        private readonly IRegistrationBuilder _registrationBuilder;

        public RegistrationBuilderFacade(IRegistrationBuilder registrationBuilder)
        {
            _registrationBuilder = registrationBuilder;
        }

        public RegistrationBuilderFacade(Type type, Lifetime lifetime)
            : this(GetBuilder(type, lifetime))
        {
        }

        private static IRegistrationBuilder GetBuilder(Type type, Lifetime lifetime)
        {
            if (type.IsGenericType && type.IsGenericTypeDefinition)
                return new OpenGenericRegistrationBuilder(type, lifetime);
        
            return new RegistrationBuilder(type, lifetime);
        }

        public Type ImplementationType => _registrationBuilder.ImplementationType;

        public IReadOnlyList<Type> InterfaceTypes => _registrationBuilder.InterfaceTypes;

        public Registration Build()
        {
            return _registrationBuilder.Build();
        }

        public IRegistrationBuilder As(Type interfaceType)
        {
            return _registrationBuilder.As(interfaceType);
        }

        public IRegistrationBuilder As(Type interfaceType1, Type interfaceType2)
        {
            return _registrationBuilder.As(interfaceType1, interfaceType2);
        }

        public IRegistrationBuilder As(Type interfaceType1, Type interfaceType2, Type interfaceType3)
        {
            return _registrationBuilder.As(interfaceType1, interfaceType2, interfaceType3);
        }

        public IRegistrationBuilder As(params Type[] interfaceTypes)
        {
            return _registrationBuilder.As(interfaceTypes);
        }

        public IRegistrationBuilder AsSelf()
        {
            return _registrationBuilder.AsSelf();
        }

        public IRegistrationBuilder AsImplementedInterfaces()
        {
            return _registrationBuilder.AsImplementedInterfaces();
        }

        public IRegistrationBuilder WithParameter(string name, object value)
        {
            return _registrationBuilder.WithParameter(name, value);
        }

        public IRegistrationBuilder WithParameter(string name, Func<IObjectResolver, object> value)
        {
            return _registrationBuilder.WithParameter(name, value);
        }

        public IRegistrationBuilder WithParameter(Type type, object value)
        {
            return _registrationBuilder.WithParameter(type, value);
        }

        public IRegistrationBuilder WithParameter(Type type, Func<IObjectResolver, object> value)
        {
            return _registrationBuilder.WithParameter(type, value);
        }
    }
}