using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RequiredReferenceAttribute))]
public class RequiredReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(property.objectReferenceValue == null)
        {
            PropertyAlertGUI.ManualDrawAlertProperty(position, property, label, PropertyAlertGUI.AlertType.Error);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
