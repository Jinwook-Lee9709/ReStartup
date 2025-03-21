using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour
{
    private GameObject objectReference;
    public bool IsInteractable
    {
        get
        {
            IInteractable interactable = objectReference.GetComponent<IInteractable>();
            return interactable is not null;
        }
    }

}
