using UnityEngine;

[FieldReferenceInheritor]
public class InheritanceTestChild : InheritanceTestBase
{
    [OpenRefCache]
    public float e;

    [SerializeField, HideInInspector, RequiredReference]
    GameObject fieldChild;
}
