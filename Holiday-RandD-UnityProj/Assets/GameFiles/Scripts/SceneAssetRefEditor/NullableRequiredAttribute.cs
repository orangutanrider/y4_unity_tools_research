using System;

[AttributeUsage(AttributeTargets.Field)]
public class NullableRequiredAttribute : Attribute
{
    public NullableRequiredAttribute() { }
}
