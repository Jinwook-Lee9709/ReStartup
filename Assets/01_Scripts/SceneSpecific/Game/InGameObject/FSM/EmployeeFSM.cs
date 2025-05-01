using System;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class EmployeeFSM : WorkerBase, IInteractor, ITransportable
{
    [SerializeField] private Transform handPivot;
    private readonly float upgradeWorkSpeedValue = 0.02f;

    private WorkerState currentStatus;

    private float interactionSpeed = 1f;

    private readonly float healthDecreaseTimer = 60f;

    private float currentTimer = 0f;
    public EmployeeTableGetData EmployeeData { get; set; } = new();
    public float InteractionSpeed => interactionSpeed;
    public Transform HandPivot => handPivot;
    public int CurrentLevel => UserDataManager.Instance.CurrentUserData.EmployeeSaveData[EmployeeData.StaffID].level;
    public float CalculatedMoveSpeed => EmployeeData.MoveSpeed + EmployeeData.upgradeMoveSpeed * (CurrentLevel - 1);
    public float CalculatedWorkSpeed => EmployeeData.WorkSpeed - upgradeWorkSpeedValue * CurrentLevel;
    public UiManager uiManager;
    
    private SPUM_Prefabs model;
    public SPUM_Prefabs Model => model;

    public float prevXPos;

    private float defaultMoveSpeed;
    private float defaultWorkSpeed;

    public WorkerState CurrentStatus
    {
        get => currentStatus;
        set
        {
            currentStatus = value;
            switch (currentStatus)
            {
                case WorkerState.Idle:
                    model.PlayAnimation(PlayerState.IDLE, 0);
                    break;
                case WorkerState.ReturnidleArea:
                    model.PlayAnimation(PlayerState.MOVE, 0);
                    agent.SetDestination(idleArea.position);
                    break;
                case WorkerState.Working:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private void Start()
    {
        OnUpgrade();
        
        model = GetComponentInChildren<SPUM_Prefabs>();
        model.OverrideControllerInit();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UiManager>();
        uiManager.EmployeeHpUIItemSet(this);
    }

    public void OnUpgrade()
    {
        defaultMoveSpeed = CalculatedMoveSpeed;
        defaultWorkSpeed = CalculatedWorkSpeed;
        agent.speed = defaultMoveSpeed;
        interactionSpeed = defaultWorkSpeed;
    }

    private void Update()
    {
        if(prevXPos > transform.position.x)
            model.transform.localScale = new Vector3(1, 1, 1);
        else
            model.transform.localScale = new Vector3(-1, 1, 1);
        prevXPos = transform.position.x;
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

        float prev = currentTimer;
        currentTimer += Time.deltaTime;
        int currentInterval = (int)(currentTimer / Constants.BUFF_SAVE_INTERVAL);
        int previousInterval = (int)(prev / Constants.BUFF_SAVE_INTERVAL);
    
        if (currentInterval != previousInterval)
        {
            var saveData = UserDataManager.Instance.CurrentUserData.EmployeeSaveData[EmployeeData.StaffID];
            saveData.remainHp = EmployeeData.currentHealth;
            saveData.remainHpDecreaseTime = currentTimer;
            
            EmployeeSaveDataDAC.UpdateEmployeeData(saveData).Forget();
        }
        
        if (healthDecreaseTimer < currentTimer)
        {
            currentTimer = 0;
            if (EmployeeData.currentHealth == 0) 
                return;
            DecreaseHp(Constants.HEALTH_DECREASE_AMOUNT_ONTIMEFINISHED);
            uiManager.EmployeeHpSet(this);
        }
    }

    public void AdjustTimer(float timer)
    {
        currentTimer = timer;
    }
    
    public void PlayWorkAnimation()
    {
        if(model!= null)
            Model.PlayAnimation(PlayerState.IDLE, 1);
    }

    public void PlayWalkAnimation()
    {
        if(model!= null)
            model.PlayAnimation(PlayerState.MOVE, 0);
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
        CurrentStatus = WorkerState.Working;
    }

    public void IncreaseHp(int amount)
    {
        EmployeeData.currentHealth += amount;
        EmployeeData.currentHealth = Mathf.Clamp(EmployeeData.currentHealth, 0, EmployeeData.Health);
        UserDataManager.Instance.CurrentUserData.EmployeeSaveData[EmployeeData.StaffID].remainHp = EmployeeData.currentHealth;
        UserDataManager.Instance.CurrentUserData.EmployeeSaveData[EmployeeData.StaffID].remainHpDecreaseTime = currentTimer;
        
        EmployeeSaveDataDAC.UpdateEmployeeData(UserDataManager.Instance.CurrentUserData.EmployeeSaveData[EmployeeData.StaffID]).Forget();
        
        if (IsExhausted)
        {
            IsExhausted = false;
            workerManager.ReturnRecoveredWorker(this);
        }
    }

    public void DecreaseHp(int amount)
    {
        EmployeeData.currentHealth -= amount;
        EmployeeData.currentHealth = Mathf.Clamp(EmployeeData.currentHealth, 0, EmployeeData.Health);
        if (EmployeeData.currentHealth == 0)
        {
            OnWorkerExhausted();
        }
    }

    private void UpdateIdle()
    {
    }

    private void UpdateReturnidleArea()
    {
        if (agent.IsArrive(idleArea)) 
            CurrentStatus = WorkerState.Idle;
    }

    private void UpdateWorking()
    {
        if (currentWork == null)
        {
            CurrentStatus = WorkerState.ReturnidleArea;
            //택스트 지워주기
            return;
        }

        currentWork.DoWork();
    }

    public void ApplyMoveSpeedBuff(float multiply)
    {
        agent.speed = defaultMoveSpeed * multiply;
    }

    public void ApplyWorkSpeedBuff(float multiply)
    {
        interactionSpeed = defaultWorkSpeed * multiply;
    }

    public void RemoveMoveSpeedBuff()
    {
        agent.speed = defaultMoveSpeed;
    }
    
    public void RemoveWorkSpeedBuff()
    {
        interactionSpeed = defaultWorkSpeed;
    }

    public override int GetHealth()
    {
        return EmployeeData.currentHealth;
    }

    public override float GetInteractSpeed()
    {
        return interactionSpeed;
    }
    
}