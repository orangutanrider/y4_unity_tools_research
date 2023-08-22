using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class RequiredReferenceAttribute : Attribute
{
    public RequiredReferenceAttribute() { }
}
