using System;
using UnityEngine;

namespace VContainer.Unity
{
    public abstract class ComponentRegistrationBuilder : RegistrationBuilder
    {

        protected ComponentDestination destination;

        protected ComponentRegistrationBuilder(Type implementationType, Lifetime lifetime)
            : base(implementationType, lifetime)
        {
        }

        public ComponentRegistrationBuilder UnderTransform(Transform parent)
        {
            destination.Parent = parent;
            return this;
        }

        public ComponentRegistrationBuilder UnderTransform(Func<Transform> parentFinder)
        {
            destination.ParentFinder = _ => parentFinder();
            return this;
        }

        public ComponentRegistrationBuilder UnderTransform(Func<IObjectResolver, Transform> parentFinder)
        {
            destination.ParentFinder = parentFinder;
            return this;
        }

        public ComponentRegistrationBuilder DontDestroyOnLoad()
        {
            destination.DontDestroyOnLoad = true;
            return this;
        }
   }
}