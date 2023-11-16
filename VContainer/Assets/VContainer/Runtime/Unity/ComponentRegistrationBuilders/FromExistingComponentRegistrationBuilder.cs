using VContainer.Internal;

namespace VContainer.Unity
{
    public sealed class FromExistingComponentRegistrationBuilder : ComponentRegistrationBuilder
    {
        readonly object instance;
        
        internal FromExistingComponentRegistrationBuilder(object instance)
            : base(instance.GetType(), Lifetime.Singleton)
        {
            this.instance = instance;
        }

        public override Registration Build()
        {
            var injector = InjectorCache.GetOrBuild(ImplementationType);
            
            var provider = new ExistingComponentProvider(instance, injector, Parameters, destination.DontDestroyOnLoad);
            
            return new Registration(ImplementationType, Lifetime, InterfaceTypes, provider);
        }
    }
}