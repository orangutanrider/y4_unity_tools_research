using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace EditorUtilities.AttributedSerializedExtensions
{

    // trivia test1
    /// <summary>
    /// trivia test2
    /// </summary>
    public class Test
    {

    }

    /**
     * E

     Multiline test 1
     e 
     e
     e
    
     **/

    /*
     * e
     * 
     Multiline test2

    ee
    w
     */

    /**
    <summary>

    Multiline test 5
    El Gringo

    ee
    ee
    ee
    ee

    </summary>
    **/
    public static class AttributedSerializedExtensions
    {
        // trivia test3
        /// trivia test4

        /* Multiline test 3
         * 
         * E
         
        E
        E E  */

        /** Multiline test 4
         * 
         * 
         * E
         **/

        #region GetInheritedPropertiesAttributedWithType()
        /// <summary>
        /// Will recursively look through classes and fill a list with objects of type InheritedSerializedProperties which each contain a list of SerializedProperties. 
        /// It will only look into base classes when the inheritor's class is attributed with [SerializedPropertyInheritor]
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public static List<InheritedSerializedProperties> GetInheritedPropertiesAttributedWithType(this Type attributeType, SerializedObject serializedObject)
        {
            List<InheritedSerializedProperties> returnList = new List<InheritedSerializedProperties>();

            Type currentType = serializedObject.targetObject.GetType();

            if (currentType.GetCustomAttribute<SerializedPropertyInheritorAttribute>() == null) { return returnList; }

            bool inheritedPropertiesClassFlag = true;
            while (inheritedPropertiesClassFlag)
            {
                currentType = currentType.BaseType;

                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                MemberInfo[] memberInfo = currentType.GetMembers(flags);

                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == false)
                {
                    if (currentType.GetCustomAttribute<SerializedPropertyInheritorAttribute>() == null)
                    {
                        inheritedPropertiesClassFlag = false;
                    }
                    continue;
                }

                InheritedSerializedProperties loopProperties = new InheritedSerializedProperties(currentType);
                loopProperties.properties = GetPropertiesAttributedWithType(attributeType, serializedObject, memberInfo);
                returnList.Add(loopProperties);

                if (currentType.GetCustomAttribute<SerializedPropertyInheritorAttribute>() == null)
                {
                    inheritedPropertiesClassFlag = false;
                }
            }

            return returnList;
        }

        /// <summary>
        /// Will recursively look through classes and fill a list with objects of type InheritedSerializedProperties which each contain a list of SerializedProperties. 
        /// It will only look into base classes when the inheritor's class is attributed with [SerializedPropertyInheritor]
        /// Binding Flags are used in each member search.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="serializedObject"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static List<InheritedSerializedProperties> GetInheritedPropertiesAttributedWithType(this Type attributeType, SerializedObject serializedObject, BindingFlags flags)
        {
            List<InheritedSerializedProperties> returnList = new List<InheritedSerializedProperties>();

            Type currentType = serializedObject.targetObject.GetType();

            if (currentType.GetCustomAttribute<SerializedPropertyInheritorAttribute>() == null) { return returnList; }

            bool inheritedPropertiesClassFlag = true;
            while (inheritedPropertiesClassFlag)
            {
                currentType = currentType.BaseType;

                MemberInfo[] memberInfo = currentType.GetMembers(flags);

                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == false)
                {
                    if (currentType.GetCustomAttribute<SerializedPropertyInheritorAttribute>() == null)
                    {
                        inheritedPropertiesClassFlag = false;
                    }
                    continue;
                }

                InheritedSerializedProperties loopProperties = new InheritedSerializedProperties(currentType);
                loopProperties.properties = GetPropertiesAttributedWithType(attributeType, serializedObject, memberInfo);
                returnList.Add(loopProperties);

                if (currentType.GetCustomAttribute<SerializedPropertyInheritorAttribute>() == null)
                {
                    inheritedPropertiesClassFlag = false;
                }
            }

            return returnList;
        }
        #endregion

        #region GetPropertiesAttributedWithType()
        /// <summary>
        /// Will return propeties attributed with attributes of the type.
        /// Binding flags are used in the member search.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="serializedObject"></param>
        /// <param name="serializedObjectType"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static List<SerializedProperty> GetPropertiesAttributedWithType(this Type attributeType, SerializedObject serializedObject, Type serializedObjectType, BindingFlags flags)
        {
            List<SerializedProperty> returnList = new List<SerializedProperty>();

            MemberInfo[] memberInfo = serializedObjectType.GetMembers(flags);

            foreach (MemberInfo member in memberInfo)
            {
                if (member.CustomAttributes.ToArray().Length == 0) { continue; }

                Attribute attribute = member.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    returnList.Add(serializedObject.FindProperty(member.Name));
                }
            }

            return returnList;
        }

        /// <summary>
        /// Will return propeties attributed with attributes of the type.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="serializedObject"></param>
        /// <param name="serializedObjectType"></param>
        /// <returns></returns>
        public static List<SerializedProperty> GetPropertiesAttributedWithType(this Type attributeType, SerializedObject serializedObject, Type serializedObjectType)
        {
            List<SerializedProperty> returnList = new List<SerializedProperty>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MemberInfo[] memberInfo = serializedObjectType.GetMembers(flags);

            foreach (MemberInfo member in memberInfo)
            {
                if (member.CustomAttributes.ToArray().Length == 0) { continue; }

                Attribute attribute = member.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    returnList.Add(serializedObject.FindProperty(member.Name));
                }
            }

            return returnList;
        }

        /// <summary>
        /// Will return propeties attributed with attributes of the type.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="serializedObject"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static List<SerializedProperty> GetPropertiesAttributedWithType(this Type attributeType, SerializedObject serializedObject, MemberInfo[] memberInfo)
        {
            List<SerializedProperty> returnList = new List<SerializedProperty>();

            foreach (MemberInfo member in memberInfo)
            {
                if (member.CustomAttributes.ToArray().Length == 0) { continue; }

                Attribute attribute = member.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    returnList.Add(serializedObject.FindProperty(member.Name));
                }
            }

            return returnList;
        }

        /// <summary>
        /// Will return propeties attributed with attributes of the type.
        /// Binding flags are used in the member search.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="serializedObject"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static List<SerializedProperty> GetPropertiesAttributedWithType(this Type attributeType, SerializedObject serializedObject, BindingFlags flags)
        {
            List<SerializedProperty> returnList = new List<SerializedProperty>();

            Type serializedObjectType = serializedObject.targetObject.GetType();
            MemberInfo[] memberInfo = serializedObjectType.GetMembers(flags);

            foreach (MemberInfo member in memberInfo)
            {
                if (member.CustomAttributes.ToArray().Length == 0) { continue; }

                Attribute attribute = member.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    returnList.Add(serializedObject.FindProperty(member.Name));
                }
            }

            return returnList;
        }

        /// <summary>
        /// Will return propeties attributed with attributes of the type.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public static List<SerializedProperty> GetPropertiesAttributedWithType(this Type attributeType, SerializedObject serializedObject)
        {
            List<SerializedProperty> returnList = new List<SerializedProperty>();

            Type serializedObjectType = serializedObject.targetObject.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MemberInfo[] memberInfo = serializedObjectType.GetMembers(flags);

            foreach (MemberInfo member in memberInfo)
            {
                if (member.CustomAttributes.ToArray().Length == 0) { continue; }

                Attribute attribute = member.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    returnList.Add(serializedObject.FindProperty(member.Name));
                }
            }

            return returnList;
        }
        #endregion

        #region GetSerializedObjectsAttributedWithType()
        /// <summary>
        /// Returns any object if any of its members are attributed with the type.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public static List<SerializedObject> GetSerializedObjectsAttributedWithType(this Type attributeType, List<MonoBehaviour> components)
        {
            List<SerializedObject> returnList = new List<SerializedObject>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    returnList.Add(newSerializedObject);
                }
            }

            return returnList;
        }

        /// <summary>
        /// Returns any object if any of its members are attributed with the type.
        /// Binding Flags are used in each member search.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="components"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static List<SerializedObject> GetSerializedObjectsAttributedWithType(this Type attributeType, List<MonoBehaviour> components, BindingFlags flags)
        {
            List<SerializedObject> returnList = new List<SerializedObject>();

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    returnList.Add(newSerializedObject);
                }
            }

            return returnList;
        }
        #endregion

        #region TryGetSerializedObjectsAttributedWithType()
        /// <summary>
        /// Outputs any serialized object that contains members attributed with attributeType.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="components"></param>
        /// <param name="outputList"></param>
        /// <param name="clearOutputFirst"></param>
        /// <returns></returns>
        public static bool TryGetSerializedObjectsAttributedWithType(this Type attributeType, List<MonoBehaviour> components, ref List<SerializedObject> outputList, bool clearOutputFirst = true)
        {
            if (clearOutputFirst == true)
            {
                outputList.Clear();
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    outputList.Add(newSerializedObject);
                }
            }

            if (outputList.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Outputs any serialized object that contains members attributed with attributeType.
        /// Binding Flags are used in each member search.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="components"></param>
        /// <param name="outputList"></param>
        /// <param name="flags"></param>
        /// <param name="clearOutputFirst"></param>
        /// <returns></returns>
        public static bool TryGetSerializedObjectsAttributedWithType(this Type attributeType, List<MonoBehaviour> components, ref List<SerializedObject> outputList, BindingFlags flags, bool clearOutputFirst = true)
        {
            if (clearOutputFirst == true)
            {
                outputList.Clear();
            }

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    outputList.Add(newSerializedObject);
                }
            }

            if (outputList.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Outputs any serialized object that contains members attributed with attributeType.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="components"></param>
        /// <param name="outputList"></param>
        /// <returns></returns>
        public static bool TryGetSerializedObjectsAttributedWithType(this Type attributeType, List<MonoBehaviour> components, out List<SerializedObject> outputList)
        {
            outputList = new List<SerializedObject>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    outputList.Add(newSerializedObject);
                }
            }

            if (outputList.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Outputs any serialized object that contains members attributed with attributeType.
        /// Binding Flags are used in each member search.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="components"></param>
        /// <param name="outputList"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool TryGetSerializedObjectsAttributedWithType(this Type attributeType, List<MonoBehaviour> components, out List<SerializedObject> outputList, BindingFlags flags)
        {
            outputList = new List<SerializedObject>();

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    outputList.Add(newSerializedObject);
                }
            }

            if (outputList.Count == 0)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region GetSerializedObjectsContainingAttributeTypes()
        /// <summary>
        /// Returns any serialized object that contains members attributed with any of the attributeTypes.
        /// </summary>
        /// <param name="attributeTypes"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public static List<SerializedObject> GetSerializedObjectsContainingAttributeTypes(this Type[] attributeTypes, List<MonoBehaviour> components)
        {
            List<SerializedObject> returnList = new List<SerializedObject>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfTypes(memberInfo, attributeTypes) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    returnList.Add(newSerializedObject);
                }
            }

            return returnList;
        }

        /// <summary>
        /// Returns any serialized object that contains members attributed with any of the attributeTypes.
        /// Binding Flags are applied to each member search.
        /// </summary>
        /// <param name="attributeTypes"></param>
        /// <param name="components"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static List<SerializedObject> GetSerializedObjectsContainingAttributeTypes(this Type[] attributeTypes, List<MonoBehaviour> components, BindingFlags flags)
        {
            List<SerializedObject> returnList = new List<SerializedObject>();

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfTypes(memberInfo, attributeTypes) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    returnList.Add(newSerializedObject);
                }
            }

            return returnList;
        }
        #endregion

        #region TryGetSerializedObjectsContainingAttributeTypes()
        /// <summary>
        /// Will return any object if one or more attributes of the attributeTypes are found on it.
        /// </summary>
        /// <param name="attributeTypes"></param>
        /// <param name="components"></param>
        /// <param name="outputList"></param>
        /// <param name="clearOutputFirst"></param>
        /// <returns></returns>
        public static bool TryGetSerializedObjectsContainingAttributeTypes(this Type[] attributeTypes, List<MonoBehaviour> components, ref List<SerializedObject> outputList, bool clearOutputFirst = true)
        {
            if (clearOutputFirst == true)
            {
                outputList.Clear();
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfTypes(memberInfo, attributeTypes) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    outputList.Add(newSerializedObject);
                }
            }

            if (outputList.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Will return any object if one or more attributes of the attributeTypes are found on it.
        /// Binding Flags are applied to each member search.
        /// </summary>
        /// <param name="attributeTypes"></param>
        /// <param name="components"></param>
        /// <param name="outputList"></param>
        /// <param name="flags"></param>
        /// <param name="clearOutputFirst"></param>
        /// <returns></returns>
        public static bool TryGetSerializedObjectsContainingAttributeTypes(this Type[] attributeTypes, List<MonoBehaviour> components, ref List<SerializedObject> outputList, BindingFlags flags, bool clearOutputFirst = true)
        {
            if (clearOutputFirst == true)
            {
                outputList.Clear();
            }

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfTypes(memberInfo, attributeTypes) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    outputList.Add(newSerializedObject);
                }
            }

            if (outputList.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Will return any object if one or more attributes of the attributeTypes are found on it.
        /// </summary>
        /// <param name="attributeTypes"></param>
        /// <param name="components"></param>
        /// <param name="outputList"></param>
        /// <returns></returns>
        public static bool TryGetSerializedObjectsContainingAttributeTypes(this Type[] attributeTypes, List<MonoBehaviour> components, out List<SerializedObject> outputList)
        {
            outputList = new List<SerializedObject>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfTypes(memberInfo, attributeTypes) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    outputList.Add(newSerializedObject);
                }
            }

            if (outputList.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Will return any object if one or more attributes of the attributeTypes are found on it.
        /// Binding Flags are applied to each member search.
        /// </summary>
        /// <param name="attributeTypes"></param>
        /// <param name="components"></param>
        /// <param name="outputList"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool TryGetSerializedObjectsContainingAttributeTypes(this Type[] attributeTypes, List<MonoBehaviour> components, out List<SerializedObject> outputList, BindingFlags flags)
        {
            outputList = new List<SerializedObject>();

            foreach (MonoBehaviour component in components)
            {
                MemberInfo[] memberInfo = component.GetType().GetMembers(flags);

                if (DoAnyMembersContainAttributesOfTypes(memberInfo, attributeTypes) == true)
                {
                    SerializedObject newSerializedObject = new SerializedObject(component);
                    outputList.Add(newSerializedObject);
                }
            }

            if (outputList.Count == 0)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region DoesObjectContainAttributesOfType()
        /// <summary>
        /// Will return true if any member in the Object is attributed with any attribute matching any of the attributeTypes.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeTypes"></param>
        /// <returns></returns>
        public static bool DoesObjectContainAttributesOfType(this UnityEngine.Object obj, Type[] attributeTypes)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MemberInfo[] memberInfo = obj.GetType().GetMembers(flags);

            foreach (Type attributeType in attributeTypes)
            {
                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Will return true if any member in the Object is attributed with any attribute matching any of the attributeTypes.
        /// Binding Flags are applied to the member search.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeTypes"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool DoesObjectContainAttributesOfType(this UnityEngine.Object obj, Type[] attributeTypes, BindingFlags flags)
        {
            MemberInfo[] memberInfo = obj.GetType().GetMembers(flags);

            foreach (Type attributeType in attributeTypes)
            {
                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == true)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region DoesObjectContainAttributeOfType()
        /// <summary>
        /// Will return true if any member in the Object is attributed with an attribute of attributeType.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static bool DoesObjectContainAttributeOfType(this UnityEngine.Object obj, Type attributeType)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MemberInfo[] memberInfo = obj.GetType().GetMembers(flags);

            foreach (MemberInfo member in memberInfo)
            {
                if (member.CustomAttributes.ToArray().Length == 0) { continue; }

                Attribute attribute = member.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Will return true if any member in the Object is attributed with an attribute of attributeType.
        /// Binding Flags are applied to the member search.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="flags"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static bool DoesObjectContainAttributeOfType(this UnityEngine.Object obj, Type attributeType, BindingFlags flags)
        {
            MemberInfo[] memberInfo = obj.GetType().GetMembers(flags);

            foreach (MemberInfo member in memberInfo)
            {
                if (member.CustomAttributes.ToArray().Length == 0) { continue; }

                Attribute attribute = member.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region DoesTypeContainAttributesOfType()
        /// <summary>
        /// Will return true if any member in the type is attributed with an attribute of any of the attributeTypes.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeTypes"></param>
        /// <returns></returns>
        public static bool DoesTypeContainAttributesOfType(this Type type, Type[] attributeTypes)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MemberInfo[] memberInfo = type.GetMembers(flags);

            foreach (Type attributeType in attributeTypes)
            {
                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Will return true if any member in the type is attributed with an attribute of any of the attributeTypes.
        /// Binding Flags are applied to the member search.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <param name="attributeTypes"></param>
        /// <returns></returns>
        public static bool DoesTypeContainAttributesOfType(this Type type, Type[] attributeTypes, BindingFlags flags)
        {
            MemberInfo[] memberInfo = type.GetMembers(flags);

            foreach (Type attributeType in attributeTypes)
            {
                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == true)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region DoesTypeContainAttributeOfType()
        /// <summary>
        /// Will return true if any member in the type is attributed with an attribute of attributeType.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static bool DoesTypeContainAttributeOfType(this Type type, Type attributeType)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MemberInfo[] memberInfo = type.GetMembers(flags);

            foreach (MemberInfo member in memberInfo)
            {
                if (member.CustomAttributes.ToArray().Length == 0) { continue; }

                Attribute attribute = member.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Will return true if any member in the type is attributed with an attribute of attributeType.
        /// Binding Flags are applied to the member search.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static bool DoesTypeContainAttributeOfType(this Type type, Type attributeType, BindingFlags flags)
        {
            MemberInfo[] memberInfo = type.GetMembers(flags);

            foreach (MemberInfo member in memberInfo)
            {
                if (member.CustomAttributes.ToArray().Length == 0) { continue; }

                Attribute attribute = member.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region GetMembers()
        // Note, these aren't used in any of the functions inside this script, it is done manually in all of them.
        // There is no particular reason for this, it'd just be a pain to go in and change them all to use this instead.

        public static MemberInfo[] GetMembers(this SerializedObject serializedObject)
        {
            return serializedObject.targetObject.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static MemberInfo[] GetMembers(this SerializedObject serializedObject, BindingFlags flags)
        {
            return serializedObject.targetObject.GetType().GetMembers(flags);
        }
        #endregion

        /// <summary>
        /// Checks if any of the members are attributed with any of the attributeTypes, if one of them is found on any member it returns true, otherwise it returns false.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="attributeTypes"></param>
        /// <returns></returns>
        public static bool DoAnyMembersContainAttributesOfTypes(this MemberInfo[] memberInfo, Type[] attributeTypes)
        {
            foreach (Type attributeType in attributeTypes)
            {
                if (DoAnyMembersContainAttributesOfType(memberInfo, attributeType) == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Will return true if any member has a attribute of the attributeType.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static bool DoAnyMembersContainAttributesOfType(this MemberInfo[] memberInfo, Type attributeType)
        {
            foreach (MemberInfo member in memberInfo)
            {
                if (member.CustomAttributes.ToArray().Length == 0) { continue; }

                Attribute attribute = member.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
