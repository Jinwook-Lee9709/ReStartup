using UnityEngine;

public class ObjectData : MonoBehaviour
{
    private GameObject objectReference;

    public bool IsInteractable
    {
        get
        {
            var interactable = objectReference.GetComponent<IInteractable>();
            return interactable is not null;
        }
    }
}