using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

[CreateAssetMenu(fileName = "HelpMessageTypeStorage", menuName = "Misc/HelpMessageTypeStorage")]
public class HelpMessageTypeStorage : ScriptableObject
{
    public List<Assembly> assembliesList = new List<Assembly>();

    public List<MethodInfo> methodList = new List<MethodInfo>();

    public void GetInterfacesButton()
    {
        foreach (Assembly assembly in assembliesList)
        {
            List<MethodInfo> methodInfo = new List<MethodInfo>();
            methodInfo = GetMethods(assembly);

            foreach (MethodInfo method in methodInfo)
            {
                methodList.Add(method);
            }
        }
    }

    public void GetAssemblies()
    {
        assembliesList.Clear();

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            if(CheckAssemblyForMethod(assembly) == true)
            {
                assembliesList.Add(assembly);
            }
        }
    }

    List<MethodInfo> GetMethods(Assembly assembly)
    {
        List<MethodInfo> returnList = new List<MethodInfo>();

        Type[] types = assembly.GetTypes();

        foreach (Type type in types)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod;
            MethodInfo[] methodInfo = type.GetMethods(flags);
            foreach(MethodInfo method in methodInfo)
            {
                if(method.GetCustomAttribute<HelpMessageEditorProviderAttribute>() != null)
                {
                    returnList.Add(method);
                }
            }
        }

        return returnList;
    }

    bool CheckAssemblyForMethod(Assembly assembly)
    {
        Type[] types = assembly.GetTypes();

        foreach (Type type in types)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod;
            MethodInfo[] methodInfo = type.GetMethods(flags);
            foreach (MethodInfo method in methodInfo)
            {
                if (method.GetCustomAttribute<HelpMessageEditorProviderAttribute>() != null)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
