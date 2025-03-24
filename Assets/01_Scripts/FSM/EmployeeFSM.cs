using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class EmployeeFSM : WorkerBase
{
    [SerializeField]
    private Transform idleArea;
    public float Speed { get; private set; }
    private float defultSpeed = 1f;
    private int upgradeCount;
    public new string name;
    public EmployeeManager employeeManager;


    public enum EnployedState
    {
        Idle,
        ReturnidleArea,
        Working,
    }

    private EnployedState currentStatus;

    public EnployedState CurrentStatus
    {
        get { return currentStatus; }
        set
        {
            EnployedState prevStatus = currentStatus;
            currentStatus = value;
            switch (currentStatus)
            {
                case EnployedState.Idle:
                    //DataTable필요
                    break;
                case EnployedState.ReturnidleArea:
                    if (currentWork != null)
                        currentStatus = EnployedState.Working;

                    agent.SetDestination(idleArea.position);
                    break;
                case EnployedState.Working:
                    if (currentWork == null)
                        currentStatus = EnployedState.ReturnidleArea;

                    currentWork.DoWork();
                    break;
            }
        }
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        Debug.Log("EmployeeFSM 호출");
        employeeManager.AddEmployee(name, this);
    }
    public void OnUpgrade()
    {
        upgradeCount++;
        Speed = defultSpeed * upgradeCount;
        Debug.Log(Speed);
    }
}
