using UnityEngine;

public class WorkTest : MonoBehaviour
{
    [SerializeField] private WorkManager workManager;
    [SerializeField] private WorkerManager workerManager;
    [SerializeField] private Table interactableObject;
    [SerializeField] [Range(1f, 5f)] private float testDuration = 1f;

    [ContextMenu("Assign Work")]
    public void AssingWork()
    {
        InteractWorkBase cleanWorkBase = new WorkGetOrder(workManager, WorkType.Hall);
        cleanWorkBase.SetInteractable(interactableObject);
        interactableObject.SetWork(cleanWorkBase);
        workManager.AddWork(cleanWorkBase);
    }
}