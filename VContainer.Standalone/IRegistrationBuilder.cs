using System;
using System.Collections.Generic;

namespace VContainer
{
    public interface IRegistrationBuilder
    {
        Type ImplementationType { get; }
        IReadOnlyList<Type> InterfaceTypes { get; }
        
        Registration Build();
        IRegistrationBuilder As(Type interfaceType);
        IRegistrationBuilder As(Type interfaceType1, Type interfaceType2);
        IRegistrationBuilder As(Type interfaceType1, Type interfaceType2, Type interfaceType3);
        IRegistrationBuilder As(params Type[] interfaceTypes);
        IRegistrationBuilder AsSelf();
        IRegistrationBuilder AsImplementedInterfaces();
        IRegistrationBuilder WithParameter(string name, object value);
        IRegistrationBuilder WithParameter(string name, Func<IObjectResolver, object> value);
        IRegistrationBuilder WithParameter(Type type, object value);
        IRegistrationBuilder WithParameter(Type type, Func<IObjectResolver, object> value);
    }
}