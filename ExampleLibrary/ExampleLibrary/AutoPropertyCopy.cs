// NuPkg4Src-Authors: Bruce Mellows
// NuPkg4Src-Description: Provides a mechanism to automatically copy compatible properties from one object to another (using reflected names and types)
// NuPkg4Src-Tags: CSharp Source Auto Property Copy
// NuPkg4Src-Id: ExampleLibrary.AutoPropertyCopy
// NuPkg4Src-InternalSourceDependencies: ExampleLibrary.StaticTuple
// NuPkg4Src-ContentPath: ExampleLibrary
// NuPkg4Src-MakePublic: AutoPropertyCopy
// NuPkg4Src-Hash: SHA512Managed:F49ADA6D8EE2B16CFDDD16FFE824D5CFBB4A5A2EDBF8C65D81258F4A5CEC16C329DCE2D7823E671EAE916015E118606DFD91343B13F72E9875D93363BE8C2CAF
// NuPkg4Src-Version: 1.0.0
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace System
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class AutoPropertyCopy
    {
        private static Dictionary<ExampleLibrary.StaticTuple<Type, Type>, List<SinglePropertyCopy>> cache = new Dictionary<ExampleLibrary.StaticTuple<Type, Type>, List<SinglePropertyCopy>>();

        internal static int CacheCount
        {
            get
            {
                return cache.Count;
            }
        }

        public static void CopyProperties(this object src, object tgt)
        {
            if (src != null && tgt != null)
            {
                GetCachedCopier(src, tgt).CopyProperties(src, tgt);
            }
        }

        private static void CopyProperties(this IEnumerable<SinglePropertyCopy> copier, object src, object tgt)
        {
            if (copier != null)
            {
                foreach (var singleCopy in copier)
                {
                    singleCopy.CopyProperty(src, tgt);
                }
            }
        }

        private static List<SinglePropertyCopy> GetCachedCopier(object src, object tgt)
        {
            var srcType = src.GetType();
            var tgtType = tgt.GetType();

            var cacheKey = ExampleLibrary.StaticTuple.Create(srcType, tgtType);
            List<SinglePropertyCopy> cacheValue;
            if (!cache.TryGetValue(cacheKey, out cacheValue))
            {
                var srcProperties = GetProperties(srcType, x => x.GetGetMethod()).ToArray();
                var tgtProperties = GetProperties(tgtType, x => x.GetSetMethod()).ToArray();
                var copiedProperties = srcProperties.Select(x => x.Name).Intersect(tgtProperties.Select(x => x.Name)).ToArray();

                cacheValue = copiedProperties
                    .Select(x => CreatePropertyCopier(srcProperties, tgtProperties, x))
                    .Where(x => x != null)
                    .ToList();

                if (!cache.ContainsKey(cacheKey))
                {
                    var newCache = cache.ToDictionary(x => x.Key, x => x.Value);
                    newCache[cacheKey] = cacheValue;
                    cache = newCache;
                }
            }

            return cacheValue;
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type type, Func<PropertyInfo, MethodInfo> getTargetMethod)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x =>
            {
                var targetMethod = getTargetMethod(x);
                return targetMethod != null && targetMethod.IsPublic;
            });
        }

        private static Type GetEffectiveType(PropertyInfo propertyInfo)
        {
            var effectiveType = propertyInfo.PropertyType;

            return (effectiveType.IsGenericType && effectiveType.GetGenericTypeDefinition() == typeof(Nullable<>))
                ? effectiveType.GetGenericArguments().Single()
                : effectiveType;
        }

        private static SinglePropertyCopy CreatePropertyCopier(IEnumerable<PropertyInfo> srcProperties, IEnumerable<PropertyInfo> tgtProperties, string copiedProperty)
        {
            var getProperty = srcProperties.Single(x => copiedProperty == x.Name);
            var getMethod = getProperty.GetGetMethod();
            var effectiveGetType = GetEffectiveType(getProperty);

            var setProperty = tgtProperties.Single(x => copiedProperty == x.Name);
            var setMethod = setProperty.GetSetMethod();
            var effectiveSetType = GetEffectiveType(setProperty);

            return effectiveGetType == effectiveSetType
                ? new SinglePropertyCopy(new Action<object, object>((s, t) => setMethod.Invoke(t, new object[] { getMethod.Invoke(s, null) })))
                : null;
        }

        private sealed class SinglePropertyCopy
        {
            public SinglePropertyCopy(Action<object, object> copyProperty)
            {
                this.CopyProperty = copyProperty;
            }

            public Action<object, object> CopyProperty { get; private set; }
        }
    }
}
