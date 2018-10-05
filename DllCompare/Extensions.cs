using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DllCompare
{
    public static class Extensions
    {
        public static Type GetTypeByClassName(this Assembly a, string className)
        {
            foreach(Type t in a.GetTypes())
            {
                if (t.Name == className)
                    return t;
            }
            return null; 
        }

        public static MethodInfo GetMethod(this Type t, string methodName, BindingFlags bindingFlags, ParameterInfo[] parameterInfo)
        {
            MethodInfo[] methods = t.GetMethods(bindingFlags);

            foreach (MethodInfo mi in methods)
            {
                if (mi.Name==methodName && DllComparer.CompareParamInfo(parameterInfo, mi.GetParameters()))
                    return mi;
            }

            return null;

        }
    }
}
