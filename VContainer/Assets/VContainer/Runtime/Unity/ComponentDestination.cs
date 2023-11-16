using System;
using UnityEngine;

namespace VContainer.Unity
{
    public struct ComponentDestination
    {
        public Transform Parent;
        public Func<IObjectResolver, Transform> ParentFinder;
        public bool DontDestroyOnLoad;

        public Transform GetParent(IObjectResolver resolver)
        {
            if (Parent != null)
                return Parent;
            if (ParentFinder != null)
                return ParentFinder(resolver);
            return null;
        }

        public void ApplyDontDestroyOnLoadIfNeeded(Component component)
        {
            if (DontDestroyOnLoad)
            {
                UnityEngine.Object.DontDestroyOnLoad(component);
            }
        }
    }
}