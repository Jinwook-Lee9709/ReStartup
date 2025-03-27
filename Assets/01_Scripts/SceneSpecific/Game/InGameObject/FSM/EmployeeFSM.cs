using System;
using UnityEngine;

public class EmployeeFSM : WorkerBase, IInteractor, ITransportable
{
    private EmployeeTableGetData employeeData = new();
    public EmployeeTableGetData EmployeeData
    {
        get => employeeData;
        set => employeeData = value;
    }
    private float upgradeWorkSpeedValue = 0.02f;
    public enum EnployedState
    {
        Idle,
        ReturnidleArea,
        Working,
    }

    private float interactionSpeed = 1f;
    public float InteractionSpeed { get; set; }

    private EnployedState currentStatus;

    public EnployedState CurrentStatus
    {
        get { return currentStatus; }
        set
        {
            EnployedState prevStatus = currentStatus;
            currentStatus = value;
            if (currentStatus == EnployedState.ReturnidleArea)
            {
                agent.SetDestination(idleArea.position);
            }
        }
    }

    private void Update()
    {
        switch (currentStatus)
        {
            case EnployedState.Idle:
                UpdateIdle();
                break;
            case EnployedState.ReturnidleArea:
                UpdateReturnidleArea();
                break;
            case EnployedState.Working:
                UpdateWorking();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void AssignWork(WorkBase work)
    {
        base.AssignWork(work);
        CurrentStatus = EnployedState.Working;
    }

    private void UpdateIdle()
    {

    }

    private void UpdateReturnidleArea()
    {
        var distance = Vector3.Distance(transform.position, idleArea.position);
        if (distance <= agent.stoppingDistance)
        {
            CurrentStatus = EnployedState.Idle;
        }
    }

    private void UpdateWorking()
    {
        if (currentWork == null)
        {
            CurrentStatus = EnployedState.ReturnidleArea;
            return;
        }
        currentWork.DoWork();
    }


    [SerializeField] private Transform handPivot;

    public Transform HandPivot { get; }
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
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        employeeData.OnUpgradeEvent += () =>
        {
            employeeData.MoveSpeed = employeeData.MoveSpeed + (employeeData.upgradeSpeed);
            agent.speed = employeeData.MoveSpeed;
            InteractionSpeed = employeeData.WorkSpeed - (upgradeWorkSpeedValue * employeeData.upgradeCount);
            Debug.Log($"{name} : {agent.speed} , {InteractionSpeed}");
        };
    }
}
