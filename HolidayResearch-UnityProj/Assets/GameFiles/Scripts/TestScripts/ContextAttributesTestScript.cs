using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextAttributesTestScript : MonoBehaviour
{
    [OpenRefCache]
    public float a;

    public float b;

    [SerializeField, HideInInspector, RequiredReference]
    GameObject field1;

    [SerializeField, HideInInspector, RequiredReference]
    GameObject field2;

    [SerializeField, HideInInspector, RequiredReference]
    GameObject field3;
}
