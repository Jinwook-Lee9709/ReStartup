using System;
using UnityEngine;

public class EmployeeFSM : WorkerBase, IInteractor, ITransportable
{
    public enum EmployeeState
    {
        Idle,
        ReturnidleArea,
        Working
    }


    [SerializeField] private Transform handPivot;
    private readonly float upgradeWorkSpeedValue = 0.02f;

    private EmployeeState currentStatus;

    private float interactionSpeed = 1f;

    public EmployeeTableGetData EmployeeData { get; set; } = new();
    public float InteractionSpeed => interactionSpeed;
    public Transform HandPivot => handPivot;


    public EmployeeState CurrentStatus
    {
        get => currentStatus;
        set
        {
            var prevStatus = currentStatus;
            currentStatus = value;
            if (currentStatus == EmployeeState.ReturnidleArea) agent.SetDestination(idleArea.position);
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        EmployeeData.MoveSpeed = EmployeeData.MoveSpeed + EmployeeData.upgradeSpeed;
        agent.speed = EmployeeData.MoveSpeed;
        interactionSpeed = EmployeeData.WorkSpeed - upgradeWorkSpeedValue * EmployeeData.upgradeCount;

        EmployeeData.OnUpgradeEvent += () =>
        {
            EmployeeData.MoveSpeed = EmployeeData.MoveSpeed + EmployeeData.upgradeSpeed;
            agent.speed = EmployeeData.MoveSpeed;
            interactionSpeed = EmployeeData.WorkSpeed - upgradeWorkSpeedValue * EmployeeData.upgradeCount;
            Debug.Log($"{name} : {agent.speed} , {InteractionSpeed}");
        };
    }

    private void Update()
    {
        switch (currentStatus)
        {
            case EmployeeState.Idle:
                UpdateIdle();
                break;
            case EmployeeState.ReturnidleArea:
                UpdateReturnidleArea();
                break;
            case EmployeeState.Working:

                UpdateWorking();
                break;
            default:
                throw new ArgumentOutOfRangeException();
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

    public override void AssignWork(WorkBase work)
    {
        base.AssignWork(work);
        //택스트 넣어주기
        CurrentStatus = EmployeeState.Working;
    }

    private void UpdateIdle()
    {
    }

    private void UpdateReturnidleArea()
    {
        var distance = Vector3.Distance(transform.position, idleArea.position);
        if (distance <= agent.stoppingDistance) CurrentStatus = EmployeeState.Idle;
    }

    private void UpdateWorking()
    {
        if (currentWork == null)
        {
            CurrentStatus = EmployeeState.ReturnidleArea;
            //택스트 지워주기
            return;
        }

        currentWork.DoWork();
    }
}