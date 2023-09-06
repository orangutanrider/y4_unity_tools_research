using UnityEditor;
using UnityEngine;
using System;

[CustomPropertyDrawer(typeof(OpenCustomInspectorButtonAttribute))]
public class OpenCustomInspectorButtonDecorator : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        OpenCustomInspectorButtonAttribute openButtonAttribute = (OpenCustomInspectorButtonAttribute)attribute;

        // I made it do this to avoid the weird situations that can arise when the button gets attached to a miscellaneous field
        if (property.propertyType != SerializedPropertyType.Boolean)
        {
            Debug.LogError
                (
                "Error with custom inspector property drawn button, inside class '" + property.serializedObject.targetObject.GetType().Name + "'" + Environment.NewLine +
                "(Please declare the button on a dedicated boolean field.)" + Environment.NewLine 
                );
            return;
        }

        property.boolValue = GUI.Button(position, "Open " + openButtonAttribute.CustomInspectorType.Name);

        if (property.boolValue == true)
        {
            CustomInspector.Open(openButtonAttribute.CustomInspectorType, Selection.activeObject, Selection.activeGameObject);
        }
    }
}
