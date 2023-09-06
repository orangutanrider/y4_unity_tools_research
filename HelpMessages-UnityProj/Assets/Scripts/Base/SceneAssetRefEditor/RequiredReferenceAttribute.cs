using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field)]
public class RequiredReferenceAttribute : PropertyAttribute
{
    public RequiredReferenceAttribute() { }
}
