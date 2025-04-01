using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ConsumerFSM : MonoBehaviour
{
    public enum ConsumerState
    {
        Waiting,
        BeforeOrder,
        AfterOrder,
        Eatting,
        WaitForPay,
        Paying,
        Exit,

        Disappointed
    }

    public enum Satisfaction
    {
        High,
        Middle,
        Low
    }

    public ConsumerManager consumerManager;

    [SerializeField] private List<float> satisfactionChangeLimit = new()
    {
        15f,
        0f
    };


    [SerializeField] public ConsumerState currentStatus = ConsumerState.Waiting;
    [SerializeField] private Satisfaction currentSatisfaction = Satisfaction.High;


    private NavMeshAgent agent;
    private CashierCounter cashierCounter;
    private Consumer consumer;
    public ConsumerData consumerData = new();
    private bool isOnSeat;

    private bool isPaying;
    private Vector2 targetPivot;

    public Satisfaction CurrentSatisfaction
    {
        get => currentSatisfaction;
        set
        {
            var prevSatisfaction = currentSatisfaction;
            switch (value)
            {
                case Satisfaction.High:
                    break;
                case Satisfaction.Middle:
                    // TODO : Serving Delay Script Play
                    Debug.Log("늦네에.. \n저, 할아버지가 되어버려요?");
                    // UnityEditor.EditorApplication.isPaused = true;
                    break;
                case Satisfaction.Low:
                    break;
            }

            currentSatisfaction = value;
        }
    }

    public ConsumerState CurrentStatus
    {
        get => currentStatus;
        set
        {
            var prevStatus = currentStatus;

            switch (value)
            {
                case ConsumerState.Waiting:
                    break;
                case ConsumerState.BeforeOrder:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.BeforeOrder);
                    consumerManager.OnWaitingLineUpdate(consumer);
                    //if (consumer.pairData?.partner == consumer)
                    //{
                    //    break;
                    //}
                    //if (consumer.pairData?.owner == consumer)
                    //{
                    //    consumer.pairData.partner.FSM.CurrentStatus = ConsumerState.BeforeOrder;
                    //    targetPivot = consumer.pairData.pairTable.InteractablePoints[2].position;
                    //    consumer.pairData.partner.GetComponent<NavMeshAgent>().SetDestination(targetPivot);
                    //}
                    //targetPivot = consumer.pairData.pairTable.InteractablePoints[1].position;
                    var permission = InteractPermission.Consumer;
                    targetPivot = consumer.currentTable.GetInteractablePoints(permission)[0].transform.position;
                    agent.SetDestination(targetPivot);
                    break;
                case ConsumerState.AfterOrder:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.AfterOrder);
                    break;
                case ConsumerState.Eatting:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.Eatting);
                    StartCoroutine(EattingCoroutine());
                    break;
                case ConsumerState.WaitForPay:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.WaitForPay);
                    consumerManager.OnEndMeal(consumer);
                    break;
                case ConsumerState.Exit:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.Exit);
                    switch (currentSatisfaction)
                    {
                        case Satisfaction.High:
                            UserDataManager.Instance.CurrentUserData.PositiveCnt++;
                            break;
                        case Satisfaction.Low:
                            UserDataManager.Instance.CurrentUserData.NegativeCnt++;
                            consumerManager.workFlowController.CancelOrder(consumer);
                            consumerManager.workFlowController.ReturnTable(consumer.currentTable);
                            break;
                    }

                    agent.SetDestination(consumerManager.spawnPoint.position);
                    break;
                case ConsumerState.Disappointed:

                    break;
            }

            if (currentStatus == prevStatus)
                currentStatus = value;
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        consumer = GetComponent<Consumer>();
    }

    private void Update()
    {
        switch (currentStatus)
        {
            case ConsumerState.Waiting:
                UpdateWaiting();
                break;
            case ConsumerState.BeforeOrder:
                UpdateBeforeOrder();
                break;
            case ConsumerState.AfterOrder:
                UpdateAfterOrder();
                break;
            case ConsumerState.Eatting:
                UpdateEatting();
                break;
            case ConsumerState.Paying:
                UpdatePaying();
                break;
            case ConsumerState.Exit:
                UpdateExit();
                break;
        }
    }

    private void OnEnable()
    {
        currentSatisfaction = Satisfaction.High;
        consumerData.Init();
        isPaying = false;
        isOnSeat = false;
    }

    public event Action<Consumer> OnSeatEvent;


    public void OnStartPayment()
    {
        CurrentStatus = ConsumerState.Paying;
    }

    public void OnEndPayment()
    {
        ///TODO : If End Payment
        StartCoroutine(UserDataManager.Instance.OnGoldUp(consumer));
        ///And Play Tip PopUp

        CurrentStatus = ConsumerState.Exit;
    }

    public void SetCashierCounter(CashierCounter cashierCounter)
    {
        this.cashierCounter = cashierCounter;
    }

    private IEnumerator EattingCoroutine()
    {
        var eattingTimer = 0f;
        while (eattingTimer < consumerData.MaxEattingLimit)
        {
            eattingTimer += Time.deltaTime;
            yield return null;
        }

        CurrentStatus = ConsumerState.WaitForPay;
    }

    public void OnOrderComplete()
    {
        CurrentStatus = ConsumerState.AfterOrder;
    }

    public void OnGetFood()
    {
        CurrentStatus = ConsumerState.Eatting;
    }

    private void UpdateWaiting()
    {
        //���ڸ��� ���� ����ϴ� ����.
        //�ջ���� ������� �ջ�� �ڸ��� �̵�.
    }

    private void UpdateBeforeOrder()
    {
        //���ڸ��� ���� ���ڸ��� �̵� �� �ֹ�.
        //������ �ֹ��� �޾ư��� �������� ����.
        //if(consumer.pairData != null)
        //{
        //    if (agent.IsArrive(targetPivot))
        //    {
        //        OnSeatEvent?.Invoke(consumer);
        //    }
        //    return;
        //}
        if (agent.IsArrive(targetPivot) && !isOnSeat)
        {
            isOnSeat = true;
            OnSeatEvent?.Invoke(consumer);
        }
    }

    private void UpdateAfterOrder()
    {
        //�ֹ��� �޾ư� ��, ������ ����������� ����.
        //deltaTime�� �����Ͽ� ������ ���¸� ����.
        consumerData.orderWaitTimer -= Time.deltaTime;
        if (consumerData.GuestType == GuestType.BadGuest)
            consumerData.orderWaitTimer -= Time.deltaTime;

        // Debug.Log(consumerData.orderWaitTimer);
        switch (consumerData.orderWaitTimer)
        {
            case var t when t < satisfactionChangeLimit[0] && t > satisfactionChangeLimit[1]:
                CurrentSatisfaction = Satisfaction.High;
                break;
            case var t when t < satisfactionChangeLimit[1] && t > satisfactionChangeLimit[2] &&
                            CurrentSatisfaction != Satisfaction.Middle:
                CurrentSatisfaction = Satisfaction.Middle;
                break;
            case var t when t < satisfactionChangeLimit[2]:
                CurrentSatisfaction = Satisfaction.Low;
                CurrentStatus = ConsumerState.Exit;
                break;
        }
    }

    private void UpdateEatting()
    {
        //�Ļ����� ����.
    }


    private void UpdateBeforePay()
    {
    }

    private void UpdatePaying()
    {
        var destination = agent.destination;
        if (agent.IsArrive(destination) && !isPaying)
        {
            isPaying = true;
            consumerManager.OnPayStart(consumerData);
        }
    }

    private void UpdateExit()
    {
        //��� �� �����ϴ� ����.
        //���� ������ƮǮ�� ��ȯ��.
        if (agent.IsArrive(consumerManager.spawnPoint)) consumerManager.consumerPool.Release(gameObject);
    }
}