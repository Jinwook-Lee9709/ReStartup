using System;
using UnityEngine;

public class EmployeeFSM : WorkerBase, IInteractor, ITransportable
{
    public enum EnployedState
    {
        Idle,
        ReturnidleArea,
        Working
    }


    [SerializeField] private Transform handPivot;
    private readonly float upgradeWorkSpeedValue = 0.02f;

    private EnployedState currentStatus;

    private float interactionSpeed = 1f;

    public EmployeeTableGetData EmployeeData { get; set; } = new();

    public EnployedState CurrentStatus
    {
        get => currentStatus;
        set
        {
            var prevStatus = currentStatus;
            currentStatus = value;
            if (currentStatus == EnployedState.ReturnidleArea) agent.SetDestination(idleArea.position);
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        EmployeeData.OnUpgradeEvent += () =>
        {
            EmployeeData.MoveSpeed = EmployeeData.MoveSpeed + EmployeeData.upgradeSpeed;
            agent.speed = EmployeeData.MoveSpeed;
            InteractionSpeed = EmployeeData.WorkSpeed - upgradeWorkSpeedValue * EmployeeData.upgradeCount;
            Debug.Log($"{name} : {agent.speed} , {InteractionSpeed}");
        };
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

    public float InteractionSpeed { get; set; }

    public Transform HandPivot => handPivot;

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
        CurrentStatus = EnployedState.Working;
    }

    private void UpdateIdle()
    {
    }

    private void UpdateReturnidleArea()
    {
        var distance = Vector3.Distance(transform.position, idleArea.position);
        if (distance <= agent.stoppingDistance) CurrentStatus = EnployedState.Idle;
    }

    private void UpdateWorking()
    {
        if (currentWork == null)
        {
            CurrentStatus = EnployedState.ReturnidleArea;
            //택스트 지워주기
            return;
        }
      
        currentWork.DoWork();
    }
}