using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OpenCustomInspectorButtonAttribute))]
public class OpenCustomInspectorButtonDecorator : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        OpenCustomInspectorButtonAttribute openButtonAttribute = (OpenCustomInspectorButtonAttribute)attribute;

        if (GUI.Button(position, "Open " + openButtonAttribute.CustomInspectorType.Name))
        {
            CustomInspector.Open(openButtonAttribute.CustomInspectorType, Selection.activeObject, Selection.activeGameObject);
        }
    }
}
