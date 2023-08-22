using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OpenRefCacheAttribute))]
public class ButtonOpenRefCacheEditor : ButtonPropertyDrawer
{
    protected override string ButtonName
    {
        get
        {
            return "Open Ref Cache Editor";
        }
    }

    protected override void ButtonExecutionContainer(SerializedProperty property, GUIContent label)
    {
        RefCacheCustomInspector.Open(property.serializedObject.targetObject);
    }
}
