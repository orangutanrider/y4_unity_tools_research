using System;

[AttributeUsage(AttributeTargets.Field)]
public class TestRequiredReferenceAttribute : Attribute
{
    public TestRequiredReferenceAttribute() { }
}
