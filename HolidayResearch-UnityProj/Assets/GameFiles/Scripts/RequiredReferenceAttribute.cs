using System;

[AttributeUsage(AttributeTargets.Field)]
public class RequiredReferenceAttribute : Attribute
{
    public RequiredReferenceAttribute() { }
}
