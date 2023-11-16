using System;
using VContainer.Internal;

namespace VContainer.Unity
{
    public sealed class FromNewGameObjectComponentRegistrationBuilder : ComponentRegistrationBuilder
    {
        readonly string gameObjectName;
        
        internal FromNewGameObjectComponentRegistrationBuilder(
            string gameObjectName,
            Type implementationType,
            Lifetime lifetime)
            : base(implementationType, lifetime)
        {
            this.gameObjectName = gameObjectName;
        }
        
        public override Registration Build()
        {
            var injector = InjectorCache.GetOrBuild(ImplementationType);

            var provider = new NewGameObjectProvider(ImplementationType, injector, Parameters, in destination, gameObjectName);

            return new Registration(ImplementationType, Lifetime, InterfaceTypes, provider);
        }
    }
}