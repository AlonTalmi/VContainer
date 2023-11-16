using System;
using UnityEngine.SceneManagement;

namespace VContainer.Unity
{
    public sealed class FromHierarchyComponentRegistrationBuilder : ComponentRegistrationBuilder
    {
        readonly Scene scene;
        
        internal FromHierarchyComponentRegistrationBuilder(in Scene scene, Type implementationType)
            : base(implementationType, Lifetime.Scoped)
        {
            this.scene = scene;
        }
        
        public override Registration Build()
        {
            var provider = new FindComponentProvider(ImplementationType, Parameters, in scene, in destination);
            
            return new Registration(ImplementationType, Lifetime, InterfaceTypes, provider);
        }
    }
}