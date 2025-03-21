using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkTest : MonoBehaviour
{
    [SerializeField] private WorkManager workManager;
    [SerializeField] WorkerManager workerManager;
    [SerializeField] Table interactableObject;

    [ContextMenu( "Assign Work" )]
    public void AssingWork()
    {
        ProgressiveWork cleanWork = new ProgressiveWork(workManager, WorkType.Clean);
        cleanWork.SetInteractable(interactableObject);
        interactableObject.SetWork(cleanWork);
        workManager.AddWork(cleanWork);
    }
    
}
