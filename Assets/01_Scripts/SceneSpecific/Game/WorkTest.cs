using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkTest : MonoBehaviour
{
    [SerializeField] private WorkManager workManager;
    [SerializeField] WorkerManager workerManager;
    [SerializeField] Table interactableObject;
    [SerializeField] [Range(1f,5f)] float testDuration = 1f;

    [ContextMenu("Assign Work")]
    public void AssingWork()
    {
        InteractWorkBase cleanWorkBase = new WorkGetOrder(workManager, WorkType.Hall, testDuration);
        cleanWorkBase.SetInteractable(interactableObject);
        interactableObject.SetWork(cleanWorkBase);
        workManager.AddWork(cleanWorkBase);
    }

}
