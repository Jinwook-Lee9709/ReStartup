using System;
using UnityEngine;
using UnityEngine.AI;

public class Player : WorkerBase, IInteractor, ITransportable
{
    [SerializeField] private Transform handPivot;
    [SerializeField] private Transform hallIdleArea, kitchenIdleArea;
    [SerializeField] private SPUM_Prefabs model;
    
    private WorkManager workManager;
    private float interactionSpeed = Constants.PLAYER_INTERACTION_SPEED;
    public float InteractionSpeed => interactionSpeed;
    public SPUM_Prefabs Model => model;
    public Transform HandPivot => handPivot;
    private bool isCameraOnHall;

    private void Awake()
    {
        base.Awake();
        var pivots = ServiceLocator.Instance.GetSceneService<GameManager>().ObjectPivotManager.GetIdleArea(WorkType.All);
        hallIdleArea = pivots[0];
        kitchenIdleArea = pivots[1];
    }

    private void Start()
    {
        model.OverrideControllerInit();
    }
    public void SetWorkManager(WorkManager workManager)
    {
        this.workManager = workManager;
    }

    public WorkerState CurrentStatus
    {
        get => currentStatus;
        private set
        {
            currentStatus = value;
            switch (currentStatus)
            {
                case WorkerState.Idle:
                    model.PlayAnimation(PlayerState.IDLE, 0);
                    break;
                case WorkerState.ReturnidleArea:
                    model.PlayAnimation(PlayerState.MOVE, 0);
                    agent.SetDestination(isCameraOnHall ? hallIdleArea.position : kitchenIdleArea.position);
                    break;
                case WorkerState.Working:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private WorkerState currentStatus;

    private void Update()
    {
        var currentVelocity = agent.velocity;
        if(currentVelocity.x < 0)
            model.transform.localScale = new Vector3(1, 1, 1);
        else if(currentVelocity.x > 0)
            model.transform.localScale = new Vector3(-1, 1, 1);
        switch (currentStatus)
        {
            case WorkerState.Idle:
                UpdateIdle();
                break;
            case WorkerState.ReturnidleArea:
                UpdateReturnidleArea();
                break;
            case WorkerState.Working:
                UpdateWorking();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override void AssignWork(WorkBase work)
    {
        base.AssignWork(work);
        CurrentStatus = WorkerState.Working;
    }
    
    public void PlayWorkAnimation()
    {
        Model.PlayAnimation(PlayerState.IDLE, 1);
    }

    public void PlayWalkAnimation()
    {
        model.PlayAnimation(PlayerState.MOVE, 0);
    }

    private void UpdateIdle()
    {
    }

    private void UpdateReturnidleArea()
    {
        Transform targetTransform = isCameraOnHall ? hallIdleArea : kitchenIdleArea;
        if (agent.IsArrive(targetTransform)) CurrentStatus = WorkerState.Idle;
    }

    private void UpdateWorking()
    {
        if (currentWork == null)
        {
            CurrentStatus = WorkerState.ReturnidleArea;
            return;
        }

        currentWork.DoWork();
    }

    public void UpdateIdleArea(bool isCameraOnHall)
    {
        this.isCameraOnHall = isCameraOnHall;
        if (CurrentStatus != WorkerState.Working)
        {
            CurrentStatus = WorkerState.ReturnidleArea;
            agent.SetDestination(isCameraOnHall ? hallIdleArea.position : kitchenIdleArea.position);
        }
    }

    public void OnTouch(Vector2 pos)
    {
        if (currentWork != null)
        {
            return;
        }

        GameObject targetObject = null;

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(pos);
        var hit = Physics2D.RaycastAll(worldPoint, Vector2.zero);
        for (var i = 0; i < hit.Length; i++)
            if (hit[i].collider != null)
            {
                if (hit[i].collider.CompareTag(Strings.InteractableObjectTag))
                {
                    targetObject = hit[i].collider.gameObject;
                    break;
                }
            }
              

        if (targetObject != null)
        {
            var listener = targetObject.GetComponent<IconTouchListener>();
            InteractableObjectTouched(listener.InteractableObject);
        }
    }

    public void InteractableObjectTouched(InteractableObjectBase interactableObject)
    {
        if (interactableObject != null)
        {
            if (interactableObject.CurrentWork != null)
            {
                if (interactableObject.CurrentWork.IsInteruptible)
                    workManager.OnPlayerStartWork(interactableObject.CurrentWork, this);
            }
        }
    }

    public void LiftPackage(GameObject package)
    {
        package.transform.SetParent(handPivot);
        package.transform.localPosition = Vector3.zero;
    }

    public void DropPackage(Transform dropPoint)
    {
        if (handPivot.childCount > 0)
        {
            var package = handPivot.GetChild(0).gameObject;
            package.transform.SetParent(dropPoint);
            package.transform.localPosition = Vector3.zero;
        }
    }
}