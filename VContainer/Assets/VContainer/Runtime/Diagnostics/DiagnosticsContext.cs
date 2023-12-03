using System;
using System.Collections.Generic;
using System.Linq;

namespace VContainer.Diagnostics
{
    public static class DiagnositcsContext
    {
        static readonly Dictionary<string, DiagnosticsCollector> collectors
            = new Dictionary<string, DiagnosticsCollector>();

        public static event Action<IObjectResolver> OnContainerBuilt;

        public static DiagnosticsCollector GetCollector(string name)
        {
            lock (collectors)
            {
                if (!collectors.TryGetValue(name, out var collector))
                {
                    collector = new DiagnosticsCollector(name);
                    collectors.Add(name, collector);
                }
                return collector;
            }
        }

        public static ILookup<string, DiagnosticsInfo> GetGroupedDiagnosticsInfos()
        {
            lock (collectors)
            {
                return collectors
                    .SelectMany(x => x.Value.GetDiagnosticsInfos())
                    .Where(x => x.ResolveInfo.MaxDepth <= 1)
                    .ToLookup(x => x.ScopeName);
            }
        }

        public static IEnumerable<DiagnosticsInfo> GetDiagnosticsInfos()
        {
            lock (collectors)
            {
                return collectors.SelectMany(x => x.Value.GetDiagnosticsInfos());
            }
        }

        public static void NotifyContainerBuilt(IObjectResolver container)
        {
            OnContainerBuilt?.Invoke(container);
        }

        internal static DiagnosticsInfo FindByRegistration(Registration registration)
        {
            return GetDiagnosticsInfos().FirstOrDefault(x => x.ResolveInfo.Registration == registration);
        }

#if UNITY_EDITOR        
        public static Dictionary<int, int> injectionCount { get; private set; }
        
        public static void RegisterInjection(object injectedInstance, Type injectedInstanceType, IInjector injector)
        {
            if(injectedInstance is not UnityEngine.Object unityObj)
                return;

            //I'm using instance id to not block the garbage collector from collection, another option is using WeakReference then we can support all types
            var instanceID = unityObj.GetInstanceID();
            injectionCount ??= new Dictionary<int, int>();
            injectionCount[instanceID] = injectionCount.GetValueOrDefault(instanceID) + 1;
            
            if(injectionCount[instanceID] > 1)
                Debug.LogWarning($"Duplicated injection in {injectedInstance} ({injectedInstanceType.Name})");
        }
#endif
    }
}
