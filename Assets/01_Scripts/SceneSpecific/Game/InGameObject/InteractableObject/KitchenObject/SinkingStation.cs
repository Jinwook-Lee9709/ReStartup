
using System;
using TMPro;
using UnityEngine;

public class SinkingStation : InteractableObjectBase
{
    private static readonly string CountFomrat = "{0} / {1}";
    [SerializeField] private int trayCapacity = 5;
    [SerializeField] TextMeshPro countText;
    private int currentTrayCount = 0;
    private bool isWashWorkAssigned = false;
    
    WorkManager workManager;
    WorkFlowController controller;
    
    public event Action OnSinkVacated;
    public int TrayCapacity => trayCapacity;
    public int CurrentTrayCount => currentTrayCount;
    public bool IsSinkFull => currentTrayCount >= trayCapacity;

    private void Start()
    {
        UpdateCountText();
        ReferencingManagers();
    }

    private void ReferencingManagers()
    {
        workManager = ServiceLocator.Instance.GetSceneService<GameManager>().WorkManager;
        controller = ServiceLocator.Instance.GetSceneService<GameManager>().WorkFlowController;
    }

    public void AddTray(int trayCount)
    {
        currentTrayCount += trayCount;
        if (currentTrayCount >= trayCapacity)
        {
            AssignTrayWashWork();
        }

        UpdateCountText();
    }

    public override void OnInteractCompleted()
    {
        base.OnInteractCompleted();
        currentTrayCount -= trayCapacity;
        currentTrayCount = Mathf.Max(currentTrayCount, 0, currentTrayCount);
        if (currentTrayCount < trayCapacity)
        {
            isWashWorkAssigned = false;
            OnSinkVacated?.Invoke();
        }

        UpdateCountText();
    }

    private void AssignTrayWashWork()
    {
        if (isWashWorkAssigned)
            return;
        isWashWorkAssigned = true;
       
        var work = new WorkWashTray(workManager, WorkType.Kitchen);
        work.SetContext(controller);
        work.SetInteractable(this);
        work.ModifyPriority(0);
        SetWork(work);
        workManager.AddWork(work);
    }

    private void UpdateCountText()
    {
        countText.text = string.Format(CountFomrat, currentTrayCount, trayCapacity);
    }
    
    
    
}
