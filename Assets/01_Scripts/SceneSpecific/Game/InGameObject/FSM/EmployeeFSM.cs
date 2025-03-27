using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class EmployeeFSM : WorkerBase, IInteractor, ITransportable
{
    [SerializeField]
    private Transform idleArea;
    private EmployeeTableGetData employeeData = new();
    public EmployeeTableGetData EmployeeData
    {
        get => employeeData;
        set => employeeData = value;
    }
    public EmployeeManager employeeManager;
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


    public Transform handPivot { get; set; }

    public Transform HandPivot => throw new NotImplementedException();

    public void LiftPackage(Sprite packageSprite)
    {
        throw new System.NotImplementedException();
    }

    public void DropPackage()
    {
        throw new System.NotImplementedException();
    }
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        //employeeManager.AddEmployee(this);
        employeeData.OnUpgradeEvent += () =>
        {
            employeeData.MoveSpeed = employeeData.MoveSpeed + (employeeData.upgradeSpeed);
            agent.speed = employeeData.MoveSpeed;
            InteractionSpeed = employeeData.WorkSpeed - (upgradeWorkSpeedValue * employeeData.upgradeCount);
            Debug.Log($"{name} : {agent.speed} , {InteractionSpeed}");
        };
    }
    public void LiftPackage(GameObject package)
    {
        throw new NotImplementedException();
    }

    public void DropPackage(Transform dropPoint)
    {
        throw new NotImplementedException();
    }
}
