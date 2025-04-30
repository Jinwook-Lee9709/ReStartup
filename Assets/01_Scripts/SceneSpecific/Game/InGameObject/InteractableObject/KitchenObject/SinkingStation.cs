using System;
using TMPro;
using UnityEngine;

public class SinkingStation : InteractableObjectBase, IInterior
{
    private static readonly string CountFomrat = "{0} / {1}";
    [SerializeField] private int trayCapacity = 5;
    [SerializeField] private TextMeshPro countText;

    [SerializeField] private SpriteRenderer objectRenderer;
    [SerializeField] private IconBubble iconBubble;
    private WorkFlowController controller;
    private bool isWashWorkAssigned;

    private WorkManager workManager;
    public int TrayCapacity => trayCapacity;
    public int CurrentTrayCount { get; private set; }

    public bool IsSinkFull => CurrentTrayCount >= trayCapacity;

    private void Start()
    {
        UpdateCountText();
        ReferencingManagers();
    }


    public void ChangeSpirte(params Sprite[] sprite)
    {
        objectRenderer.sprite = sprite[0];
    }

    public event Action OnSinkVacated;

    public void ChangeCapacity(int capacity)
    {
        trayCapacity = capacity;
        UpdateCountText();
    }

    private void ReferencingManagers()
    {
        workManager = ServiceLocator.Instance.GetSceneService<GameManager>().WorkManager;
        controller = ServiceLocator.Instance.GetSceneService<GameManager>().WorkFlowController;
    }

    public void AddTray(int trayCount)
    {
        CurrentTrayCount += trayCount;
        if (CurrentTrayCount >= trayCapacity) AssignTrayWashWork();

        UpdateCountText();
    }

    public override void OnInteractCompleted()
    {
        base.OnInteractCompleted();
        CurrentTrayCount -= trayCapacity;
        CurrentTrayCount = Mathf.Max(CurrentTrayCount, 0, CurrentTrayCount);
        if (CurrentTrayCount < trayCapacity)
        {
            isWashWorkAssigned = false;
            OnSinkVacated?.Invoke();
        }

        UpdateCountText();
    }

    public override bool ShowIcon(IconPivots pivot, Sprite icon, Sprite background = null, bool flipBackground = false)
    {
        iconBubble.ShowIcon(icon, iconBubble.transform.position, flipBackground);
        return true;
    }

    public override void HideIcon()
    {
        iconBubble.HideIcon();
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
        countText.text = string.Format(CountFomrat, CurrentTrayCount, trayCapacity);
    }
}