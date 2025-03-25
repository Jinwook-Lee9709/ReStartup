using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class EmployeeFSM : WorkerBase, ITransformable
{
    [SerializeField]
    private Transform idleArea;
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
                    //DataTable�ʿ�
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

    private void Update()
    {
        
    }

    private void UpdateIdle()
    {
        
    }

    private void UpdateReturnidleArea()
    {
        
    }

    private void UpdateWorking()
    {
        if (currentWork == null)
        {
            currentStatus = EnployedState.ReturnidleArea;
        }
        
    }
    
    
    public Transform handPivot { get; set; }
    public void LiftPackage(Sprite packageSprite)
    {
        throw new System.NotImplementedException();
    }

    public void DropPackage()
    {
        throw new System.NotImplementedException();
    }
}
