using System;
using System.Collections.Generic;
using UnityEditor;

public struct InheritedSerializedProperties 
{
    public InheritedSerializedProperties(Type _type)
    {
        type = _type;
        properties = new List<SerializedProperty>();
    }

    public Type type;
    public List<SerializedProperty> properties;
}
