using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ComponentNullableAttribute))]
public class ComponentNullableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.objectReferenceValue == null)
        {
            PropertyAlertGUI.ManualDrawAlertProperty(position, property, label, PropertyAlertGUI.AlertType.Info);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
