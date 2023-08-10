using Amazon.CloudFront;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PITB.CRM_API
{
    public class FunctionInvoker
    {
        private static Dictionary<string, MethodInfo> dictMethodInfo = new Dictionary<string, MethodInfo>();

        public static void RegisterURI(Dictionary<string, string> dictRouteMapping)
        {
            foreach (KeyValuePair<string, string> keyVal in dictRouteMapping)
            {
                dictMethodInfo.Add(keyVal.Key, GetMethodInfo(keyVal.Value));
            }
        }

        public static MethodInfo GetMethodInfo(string funcStr)
        {
            string[] assemblyClasstArr = funcStr.Split(new string[] { "::" }, StringSplitOptions.None);
            string assemblyName = assemblyClasstArr[0];
            string[] classFuncArr = assemblyClasstArr[1].Split(new string[] { "." }, StringSplitOptions.None);
            Assembly asm = Assembly.Load(assemblyName);

            //List<MethodInfo> listMethodInfo = asm.GetType().SelectMany(t => t.GetMethods())
            //    .Where(m => m(typeof(XeeshiConnAtt), false).Length > 0).ToList();
            //return null;

            //List<Type> typeArr = asm.GetTypes().ToList().Where(n => n.FullName.Contains("PITB.CRM_API.Handlers.Business.SchoolEducation.BlSchoolEducation")).ToList();
            string className = assemblyClasstArr[0]+"."+string.Join(".", classFuncArr, 0, classFuncArr.Length - 1);
            Type t = asm.GetType(className);
            //List<MethodInfo> listMethodInfo = t.GetMethods().ToList();
            return t.GetMethod(classFuncArr[classFuncArr.Length-1]);
        }

        //private void RegisterAssemblies(Dictionary<string,string> dictRouteMapping, List<string> listAssemblyNames = null)
        //{
        //    List<Assembly> listAssembly = new List<Assembly>();
        //    List<MethodInfo> listMethodInfo = new List<MethodInfo>();
        //    if (listAssemblyNames == null || listAssemblyNames.Count==0) // if no assembly is registered
        //    {
        //        listAssembly = AppDomain.CurrentDomain.GetAssemblies().ToList();
        //        foreach (Assembly assembly in listAssembly)
        //        {
        //            listMethodInfo.AddRange(assembly.GetTypes()
        //                  .SelectMany(t => t.GetMethods())
        //                  //.Where(m => m.GetCustomAttributes(typeof(XeeshiConnAtt), false).Length > 0)
        //                  .ToList());
        //        }
        //    }
        //    else
        //    {
        //        Assembly asm = null; // declare a variable of type 'assembly'

        //        // 1. Get access to the assembly

        //        foreach (string assemblyName in listAssemblyNames)
        //        {
        //            try
        //            {
        //                // attempt to obtain information about the assembly
        //                asm = Assembly.Load(assemblyName);
        //                listAssembly.Add(asm);
        //                // check if everything is OK
        //                //if (asm == null)
        //                //{
        //                //    status.SetException("Assemble not found");
        //                //    return status;
        //                //}
        //            }
        //            catch (FileNotFoundException ex)
        //            {
        //                throw;
        //                // if the attempt is unsuccessful, an error message is occured.
        //                //status.SetException(ex.Message);
        //                //return status;
        //            }
        //        }

        //        foreach (Assembly assembly in listAssembly)
        //        {
        //            listMethodInfo.AddRange(assembly.GetTypes()
        //                  .SelectMany(t => t.GetMethods())
        //                  //.Where(m => m.GetCustomAttributes(typeof(XeeshiConnAtt), false).Length > 0)
        //                  .ToList());
        //        }
        //    }

        //    foreach (MethodInfo methodInfo in listMethodInfo)
        //    {
        //        //XeeshiConnAtt xeeshiConnectAttr = (XeeshiConnAtt)methodInfo.GetCustomAttributes(typeof(XeeshiConnAtt), false)[0];
        //        //if (string.IsNullOrEmpty(xeeshiConnectAttr.uri))
        //        //{
        //        //    xeeshiConnectAttr.uri = methodInfo.DeclaringType.FullName + "." + methodInfo.Name;
        //        //}
        //        dictMethodInfo.Add(, methodInfo);
        //    }


        //    //listMethodInfo.ToDictionary(n=> n.GetCustomAttributes(typeof(XeeshiConnAtt), false))
        //}


        public static object InvokeFunctionWithDynamicParam(string uri, dynamic d)
        {
            //Status status = new Status();
            // 2. Get the instance of ComplexOperations type from ClassLibrary1.dll asembly
            Type tp;
            Type parameterType;

            MethodInfo mi = dictMethodInfo[uri]; // get the instance of AreaTriangle() method

            // 3.2. Form the method's parameters
            //double a = 5, b = 4, c = 3;
            //object[] parameters = new object[] { a, b, c };

            ParameterInfo[] paramInfoArr = mi.GetParameters();
            object[] paramsObjArr = new object[paramInfoArr.Length];
            paramsObjArr[0] = d;

            // 3.3. Invoke the method
            //double area;
            tp = mi.DeclaringType;
            object obj = Activator.CreateInstance(tp);
            object result = null;
            if (mi.IsStatic)
            {
                result = mi.Invoke(null, paramsObjArr); // area = 6
            }
            else
            {
                result = mi.Invoke(obj, paramsObjArr); // area = 6
            }
            //object result = mi.Invoke(obj, funcParams); // area = 6
            //Console.WriteLine("Area = {0}", result.ToString());
            return result;

        }




        public static object InvokeFunction(string uri/*, string json*/, params object[] funcParams)
        {
            //Status status = new Status();
            // 2. Get the instance of ComplexOperations type from ClassLibrary1.dll asembly
            Type tp;
            Type parameterType;
            try
            {

                MethodInfo mi = dictMethodInfo[uri]; // get the instance of AreaTriangle() method

                // 3.2. Form the method's parameters
                //double a = 5, b = 4, c = 3;
                //object[] parameters = new object[] { a, b, c };

                ParameterInfo[] paramInfoArr = mi.GetParameters();
                object[] paramsObjArr = new object[paramInfoArr.Length];
                for (int i=0; i<paramInfoArr.Length;i++)
                {
                    parameterType = paramInfoArr[i].ParameterType;
                    //if(funcParams.Where(n=>n.GetType()== parameterType).FirstOrDefault()!=null)
                    if(parameterType != funcParams[i].GetType())
                    {
                        //paramsObjArr[i] = JsonConvert.DeserializeObject(json, parameterType);
                        paramsObjArr[i] = JsonConvert.DeserializeObject((string)funcParams[i], parameterType);
                    }
                    else
                    {
                        paramsObjArr[i] = funcParams[i];
                    }
                }

                // 3.3. Invoke the method
                //double area;
                tp = mi.DeclaringType;
                object obj = Activator.CreateInstance(tp);
                object result = null;
                if (mi.IsStatic)
                {
                    result = mi.Invoke(null, paramsObjArr); // area = 6
                }
                else
                {
                    result = mi.Invoke(obj, paramsObjArr); // area = 6
                }
                //object result = mi.Invoke(obj, funcParams); // area = 6
                //Console.WriteLine("Area = {0}", result.ToString());
                return result;
            }
            catch (Exception ex)
            {
                throw;
                //return 1;
            }
            return -1;
        }

        //public static object InvokeFunction(string uri, object[] objArr/*, object[] funcParams*/)
        //{
        //    //Status status = new Status();
        //    // 2. Get the instance of ComplexOperations type from ClassLibrary1.dll asembly
        //    Type tp;
        //    Type parameterType;
        //    try
        //    {

        //        MethodInfo mi = dictMethodInfo[uri]; // get the instance of AreaTriangle() method

        //        // 3.2. Form the method's parameters
        //        //double a = 5, b = 4, c = 3;
        //        //object[] parameters = new object[] { a, b, c };

        //        ParameterInfo[] paramInfoArr = mi.GetParameters();
        //        object[] paramsObjArr = new object[paramInfoArr.Length];
        //        for (int i = 0; i < paramInfoArr.Length; i++)
        //        {
        //            parameterType = paramInfoArr[i].ParameterType;
        //            paramsObjArr[i] = objArr[i];
        //        }

        //        // 3.3. Invoke the method
        //        //double area;
        //        tp = mi.DeclaringType;
        //        object obj = Activator.CreateInstance(tp);
        //        object result = null;
        //        if (mi.IsStatic)
        //        {
        //            result = mi.Invoke(null, paramsObjArr); // area = 6
        //        }
        //        else
        //        {
        //            result = mi.Invoke(obj, paramsObjArr); // area = 6
        //        }
        //        //object result = mi.Invoke(obj, funcParams); // area = 6
        //        //Console.WriteLine("Area = {0}", result.ToString());
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //        //return 1;
        //    }
        //    return -1;
        //}
    }
}