using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoTestRequiredReference : MonoBehaviour
{
    public GameObject requiredReferenceGameobject;
    public MonoBehaviour requiredReferenceMonobehaviour;
    public ScriptableObject requiredReferenceScriptableObject;

    private void Start()
    {
        Debug.Log(requiredReferenceGameobject);
        Debug.Log(requiredReferenceMonobehaviour);
        Debug.Log(requiredReferenceScriptableObject);
    }
}
