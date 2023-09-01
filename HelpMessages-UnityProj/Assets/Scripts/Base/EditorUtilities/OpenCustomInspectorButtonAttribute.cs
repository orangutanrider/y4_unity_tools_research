using System;
using UnityEngine;

public class OpenCustomInspectorButtonAttribute : PropertyAttribute
{
    public OpenCustomInspectorButtonAttribute(Type customInspectorType)
    {
        CustomInspectorType = customInspectorType;
    }

    public Type CustomInspectorType { get; private set; } = null;
}
