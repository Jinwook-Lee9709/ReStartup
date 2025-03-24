using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkTest : MonoBehaviour
{
    [SerializeField] private WorkManager workManager;
    [SerializeField] WorkerManager workerManager;
    [SerializeField] Table interactableObject;

    [ContextMenu("Assign Work")]
    public void AssingWork()
    {
        InteractWorkBase cleanWorkBase = new WorkGetOrder(workManager, WorkType.Hall, 1f);
        cleanWorkBase.SetInteractable(interactableObject);
        interactableObject.SetWork(cleanWorkBase);
        workManager.AddWork(cleanWorkBase);
    }

}
