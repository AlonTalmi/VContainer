using System;
using UnityEngine;
using VContainer.Internal;

namespace VContainer.Unity
{
    public sealed class FromPrefabComponentRegistrationBuilder : ComponentRegistrationBuilder
    {
        readonly Func<IObjectResolver, Component> prefabFinder;
        
        internal FromPrefabComponentRegistrationBuilder(
            Func<IObjectResolver, Component> prefabFinder,
            Type implementationType,
            Lifetime lifetime)
            : base(implementationType, lifetime)
        {
            this.prefabFinder = prefabFinder;
        }

        public override Registration Build()
        {
            var injector = InjectorCache.GetOrBuild(ImplementationType);
            
            var provider = new PrefabComponentProvider(prefabFinder, injector, Parameters, in destination);
            
            return new Registration(ImplementationType, Lifetime, InterfaceTypes, provider);
        }
    }
}