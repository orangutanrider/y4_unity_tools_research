using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field)]
public class ComponentNullableAttribute : PropertyAttribute
{
    public ComponentNullableAttribute() { }
}
