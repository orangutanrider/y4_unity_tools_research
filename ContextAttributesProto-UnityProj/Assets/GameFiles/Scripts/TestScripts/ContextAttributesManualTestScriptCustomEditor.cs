using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(ContextAttributesManualTestScript)), CanEditMultipleObjects]
public class ContextAttributesManualTestScriptCustomEditor : Editor
{
    List<SerializedProperty> properties = new List<SerializedProperty>();
    bool foldoutRequiredReferences = false;

    private void OnEnable()
    {
        #region Soloution1 - Too Inefficient
        //https://www.youtube.com/watch?v=vLKeqS1PeTU&t=290s
        /*
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach(Assembly assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();

            foreach(Type type in types)
            {
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                MemberInfo[] members = type.GetMembers(flags);

                foreach (MemberInfo member in members)
                {
                    if (member.CustomAttributes.ToArray().Length > 0)
                    {
                        RequiredReferenceAttribute attribute = member.GetCustomAttribute<RequiredReferenceAttribute>();
                        if (attribute != null)
                        {
                            Debug.Log(member.Name);
                        }
                    }
                }
            }
        }
        */
        // This worked but it causes a processing loading buffer thingy whenever it happens
        // So obviously, there needs to be something better than this for it to be for real mode
        #endregion

        #region Soloution2 - Wrong Attributes
        /*
        Attribute[] attributes =  RequiredReferenceAttribute.GetCustomAttributes(typeof(ContextAttributesManualTestScript), typeof(RequiredReferenceAttribute));

        foreach (Attribute attribute in attributes)
        {
            Debug.Log(attribute.ToString());
        }
        Debug.Log("========");
        foreach (Attribute attribute in attributes)
        {
            Debug.Log(attribute.GetType().Name);
        }
        */
        // didn't work
        // returns a few miscellaneous attributes
        // attributes that i didn't add, they are inherit or something
        // invisible in the file
        #endregion

        #region Soloution3 - Null
        /*
        RequiredReferenceAttribute requiredReference = (RequiredReferenceAttribute)Attribute.GetCustomAttribute(typeof(ContextAttributesManualTestScript), typeof(RequiredReferenceAttribute));
        Debug.Log(requiredReference);
        */
        // returns null
        #endregion

        #region Soloution4 - Error
        /*
        RequiredReferenceAttribute requiredReference = (RequiredReferenceAttribute)Attribute.GetCustomAttribute(typeof(RequiredReferenceAttribute), typeof(ContextAttributesManualTestScript));
        Debug.Log(requiredReference);
        */
        // returns an error
        // ArgumentException: Type passed in must be derived from System.Attribute or System.Attribute itself.
        #endregion

        // okay, one of the things i'm thinking right now is that the problem is something to do with it not looking inside the class, it's just checking to see if the class itself has attributes

        #region Soloution5 - lowercase null
        /*
        Debug.Log(typeof(ContextAttributesManualTestScript).CustomAttributes);
        Debug.Log("");
        CustomAttributeData[] customAttributes = typeof(ContextAttributesManualTestScript).CustomAttributes.ToArray();
        foreach(CustomAttributeData attribute in customAttributes)
        {
            Debug.Log(attribute.AttributeType);
        }
        // Returns 3 different null values
        // one of the nulls returned is printed in lower case, while the others have their first letter capatalised
        */
        #endregion

        #region Soloution6 - EmptyList
        /*
        RequiredReferenceAttribute[] attributes = (RequiredReferenceAttribute[])Attribute.GetCustomAttributes(typeof(ContextAttributesManualTestScript), typeof(RequiredReferenceAttribute));
        Debug.Log(attributes + " " + attributes.Length);
        foreach(RequiredReferenceAttribute attribute in attributes)
        {
            Debug.Log(attribute);
        }
        */
        // returns an empty list
        #endregion

        #region Soloution7 - Microsoft All Scopes
        //https://learn.microsoft.com/en-us/dotnet/standard/attributes/retrieving-information-stored-in-attributes

        /*
        Type t = typeof(ContextAttributesManualTestScript);

        RequiredReferenceAttribute att;

        // Get the class-level attributes.
        // Put the instance of the attribute on the class level in the att object.
        att = (RequiredReferenceAttribute)Attribute.GetCustomAttribute(t, typeof(RequiredReferenceAttribute));

        Debug.Log("att");
        Debug.Log(att);

        // Get the method-level attributes.
        // Get all methods in this class, and put them
        // in an array of System.Reflection.MemberInfo objects.
        MemberInfo[] MyMemberInfo = t.GetMethods();

        Debug.Log("memberInfo");
        Debug.Log(MyMemberInfo);
        foreach(MemberInfo memberInfo in MyMemberInfo)
        {
            Debug.Log(memberInfo);
        }

        Debug.Log("methods");

        // Loop through all methods in this class that are in the
        // MyMemberInfo array.
        for (int i = 0; i < MyMemberInfo.Length; i++)
        {
            att = (RequiredReferenceAttribute)Attribute.GetCustomAttribute(MyMemberInfo[i], typeof(RequiredReferenceAttribute));
            Debug.Log(att);
        }
        */
        // returns 95 logs
        // none of which contained required reference as a string or anything like it
        // unclear if it wasn't detected at all though
        #endregion

        // hmm, dunno what to think of all this, i think i should go back to soloution 1 and adapt it to be more effecient in some way

        #region Soloution8 - Functional
        /*
        Type type1 = typeof(ContextAttributesManualTestScript);
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        MemberInfo[] members = type1.GetMembers(flags);
        foreach(MemberInfo member in members)
        {
            if (member.CustomAttributes.ToArray().Length > 0)
            {
                RequiredReferenceAttribute attribute = member.GetCustomAttribute<RequiredReferenceAttribute>();
                if (attribute != null)
                {
                    Debug.Log(member.Name);
                    Debug.Log(member.MemberType);
                    Debug.Log(member.DeclaringType);
                    Debug.Log(member.ReflectedType);
                    Debug.Log(" ");
                    Debug.Log(attribute);
                }
            }
        }
        */
        // This works
        #endregion

        // Applied learnings, the only stuff in this method not commented out, here
        #region Applied learnings (Soloution8)

        List<MemberInfo> attributedMemberData = new List<MemberInfo>();

        // Get Attributed Fields
        Type type1 = typeof(ContextAttributesManualTestScript);
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        MemberInfo[] members = type1.GetMembers(flags);
        foreach (MemberInfo member in members)
        {
            if (member.CustomAttributes.ToArray().Length > 0)
            {
                TestRequiredReferenceAttribute attribute = member.GetCustomAttribute<TestRequiredReferenceAttribute>();
                if (attribute != null)
                {
                    attributedMemberData.Add(member);
                }
            }
        }

        // Fill Serialized Property List
        properties.Clear();
        foreach (MemberInfo member in attributedMemberData)
        {
            properties.Add(serializedObject.FindProperty(member.Name));
        }
        #endregion
    }

    public override void OnInspectorGUI()
    {
        ContextAttributesManualTestScript testScript = (ContextAttributesManualTestScript)target;

        foldoutRequiredReferences = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutRequiredReferences, "Required References");
        if (foldoutRequiredReferences)
        {
            foreach(SerializedProperty property in properties)
            {
                EditorGUILayout.PropertyField(property);
            }
            EditorGUILayout.Separator();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        #region Display Error 
        bool isThereNullReference = false;
        foreach (SerializedProperty property in properties)
        {
            if (property.objectReferenceValue == null)
            {
                isThereNullReference = true;
            }
        }

        if (isThereNullReference == true)
        {
            EditorGUILayout.HelpBox("There are NULL RequiredReferences", MessageType.Error);
        }
        #endregion

        serializedObject.ApplyModifiedProperties();
    }
}
