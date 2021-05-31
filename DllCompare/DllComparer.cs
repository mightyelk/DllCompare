using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace DllCompare
{
    public class DllComparer
    {
        string _f1;
        string _f2;

        public DllComparer(string file1, string file2)
        {
            _f1 = file1;
            _f2 = file2;

        }

        [System.Flags()]
        public enum DifferenceType
        {
            None=0,
            MissingMethods=0x01,
            MissingProperties=0x02,
            MissingTypes=0x04,
            DifferentParameters=0x08,
            IgnoreHideBySig=0x10,
            DifferentAccessLevel=0x20,
            DifferentReturnType=0x40,
        }

        public const DifferenceType DEFAULT_DIFFERENCES = DifferenceType.DifferentAccessLevel | DifferenceType.DifferentParameters | DifferenceType.MissingMethods | DifferenceType.MissingProperties;

        public string GetResult(DifferenceType diffs)
        {
            AssemblyName an1 = AssemblyName.GetAssemblyName(_f1);
            AssemblyName an2 = AssemblyName.GetAssemblyName(_f2);
          
            StringBuilder ret = new StringBuilder();

            Assembly a1 = Assembly.Load(an1);
            Assembly a2 = Assembly.Load(an2);

            if (an1.ToString()!=an2.ToString())
              ret.AppendLine("Assemblies not of same Type,Name or Version.");

            ret.Append(GetDifferences(a1, a2, diffs));
            //ret.Append(GetDifferences(a2, a1, diffs));
         
            return ret.ToString();
        }


        
        private string GetDifferences(Assembly a1, Assembly a2, DifferenceType whichOnes)
        {
            string a1Name = a1.ManifestModule.Name;
            BindingFlags flags = BindingFlags.Instance|  BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            StringBuilder ret = new StringBuilder();
            foreach (Type t1 in a1.GetTypes())
            {
                
                Type t2 = a2.GetTypeByClassName(t1.Name);
                if (t2 != null)
                {
                    foreach( MethodInfo mi1 in t1.GetMethods(flags))
                    {
                        ParameterInfo[] pi1 = mi1.GetParameters();
                        MethodInfo mi2 = t2.GetMethod(mi1.Name, flags, pi1);
                        
                        if (mi2!=null && mi1.ToString()==mi2.ToString())
                        {
                            ParameterInfo[] pi2 = mi2.GetParameters();

                            bool access = mi1.Attributes == mi2.Attributes;
                            if (TestFlag(whichOnes, DifferenceType.IgnoreHideBySig))
                            {
                                access = (mi1.Attributes & ~MethodAttributes.HideBySig) == (mi2.Attributes & ~MethodAttributes.HideBySig);
                            }

                            bool parameters = CompareParamInfo(pi1, pi2);
                            bool returnType = mi1.ReturnType.Equals(mi2.ReturnType);

                            if (!access & TestFlag(whichOnes, DifferenceType.DifferentAccessLevel))
                                ret.AppendFormat("{0}: Class '{1}', Method '{2}' attributes/access modifiers do not match --> '{3}' vs '{4}'", a1Name, t1.Name, mi1.ToString(), mi1.Attributes, mi2.Attributes);

                            if (!parameters & TestFlag (whichOnes, DifferenceType.DifferentParameters))
                                ret.AppendFormat("{0}: Class '{1}', Method '{2}' calling parameters do not match.", a1Name, t1.Name, mi1.ToString());
                            if (!returnType & TestFlag(whichOnes, DifferenceType.DifferentReturnType))
                                ret.AppendFormat("{0}: Class '{1}', Method '{2}' return parameter does not match.", a1Name, t1.Name, mi1.ToString());
                        }
                        else
                            if (TestFlag(whichOnes, DifferenceType.MissingMethods))
                                ret.AppendFormat("{0}: Class '{1}', Method '{2}' does not exist in other DLL.", a1Name, t1.Name, mi1.ToString());
                    }
                    
                }
                else
                {
                    if (TestFlag(whichOnes, DifferenceType.MissingTypes))
                        ret.AppendFormat("{0}: Type '{1}' does not exist in other DLL.", a1Name, t1.Name);

                }

            }

            return ret.ToString();
        }


        private bool TestFlag(DifferenceType d1, DifferenceType d2)
        {
            int a = (int)d1;
            int b = (int)d2;

            return (a & b) > 0;
        }

        public static bool CompareParamInfo(ParameterInfo[] pi1, ParameterInfo[] pi2)
        {
            if (pi1.Length != pi2.Length)
                return false;
            if (pi1.Length == 0 & pi2.Length == 0)
                return true;


            
            for (int i=0; i<pi1.Length; i++)
            {
                //if (pi1[i].Name != pi2[i].Name)
                //    return false;
                if (pi1[i].ParameterType.Name != pi2[i].ParameterType.Name)
                    return false;
            }
            return true;   
        }
    }
}
