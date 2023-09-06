using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field)]
public class NullableRequiredAttribute : PropertyAttribute
{
    public NullableRequiredAttribute() { }
}
