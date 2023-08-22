using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InheritanceTestBase : MonoBehaviour
{
    [OpenRefCache]
    [SerializeField] float z;

    [SerializeField, HideInInspector, RequiredReference]
    GameObject fieldBase;
}
