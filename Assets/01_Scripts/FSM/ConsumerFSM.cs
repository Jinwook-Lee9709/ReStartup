using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ConsumerFSM : MonoBehaviour
{
    public ConsumerData consumerData = new();
    public ConsumerManager consumerManager;
    private Consumer consumer;
    private CashierCounter cashierCounter;
    private Vector2 targetPivot;
    public enum ConsumerState
    {
        Waiting,
        BeforeOrder,
        AfterOrder,
        Eatting,
        BeforePay,
        AfterPay,
    }


    private NavMeshAgent agent;
    [SerializeField]
    private List<float> satisfactionChangeLimit = new List<float>
    {
        15f,
        0f
    };
    public enum Satisfaction
    {
        High,
        Middle,
        Low,
    }

    public event Action<Consumer> OnSeatEvent;


    [SerializeField] public ConsumerState currentStatus = ConsumerState.Waiting;
    [SerializeField] private Satisfaction currentSatisfaction = Satisfaction.High;

    public ConsumerState CurrentStatus
    {
        get { return currentStatus; }
        set
        {
            ConsumerState prevStatus = currentStatus;

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
                    targetPivot = consumer.currentTable.InteractablePoints[1].position;
                    agent.SetDestination(targetPivot);
                    break;
                case ConsumerState.AfterOrder:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.AfterOrder);
                    break;
                case ConsumerState.Eatting:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.Eatting);
                    StartCoroutine(EattingCoroutine());
                    break;
                case ConsumerState.BeforePay:

                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.BeforePay);
                    consumerManager.OnEndMeal(consumer);
                    break;
                case ConsumerState.AfterPay:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.AfterPay);
                    switch (currentSatisfaction)
                    {
                        case Satisfaction.High:
                            UserDataManager.Instance.CurrentUserData.PositiveCnt++;
                            break;
                        case Satisfaction.Low:
                            UserDataManager.Instance.CurrentUserData.NegativeCnt++;
                            break;
                    }
                    agent.SetDestination(consumerManager.spawnPoint.position);
                    break;
            }
            currentStatus = value;
        }
    }



    public void OnEndPayment()
    {
        ///TODO : If End Payment
        ///if(IsTip)
        ///{
        ///     StartCoroutiner(UserDataManager.Instance.GoldUp(this);
        ///}
        ///And Play Tip PopUp

        CurrentStatus = ConsumerState.AfterPay;
    }

    public void SetCashierCounter(CashierCounter cashierCounter)
    {
        this.cashierCounter = cashierCounter;
    }

    private IEnumerator EattingCoroutine()
    {
        float eattingTimer = 0f;
        while (eattingTimer < consumerData.MaxEattingLimit)
        {
            eattingTimer += Time.deltaTime;
            yield return null;
        }
        CurrentStatus = ConsumerState.BeforePay;
    }

    public void OnOrderComplete()
    {
        CurrentStatus = ConsumerState.AfterOrder;
    }

    public void OnGetFood()
    {
        CurrentStatus = ConsumerState.Eatting;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        consumer = GetComponent<Consumer>();
    }

    private void OnEnable()
    {
        currentSatisfaction = Satisfaction.High;
        consumerData.OrderWaitTimer = consumerData.MaxOrderWaitLimit;
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
            case ConsumerState.BeforePay:
                UpdateBeforePay();
                break;
            case ConsumerState.AfterPay:
                UpdateAfterPay();
                break;
        }
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
        if (agent.IsArrive(targetPivot))
        {
            OnSeatEvent?.Invoke(consumer);
        }
    }
    private void UpdateAfterOrder()
    {
        //�ֹ��� �޾ư� ��, ������ ����������� ����.
        //deltaTime�� �����Ͽ� ������ ���¸� ����.
        consumerData.OrderWaitTimer -= Time.deltaTime;
        if (consumerData.Type == ConsumerData.ConsumerType.Obnoxious)
            consumerData.OrderWaitTimer -= Time.deltaTime;

        // Debug.Log(orderWaitTimer);
        switch (consumerData.OrderWaitTimer)
        {
            case var t when t < satisfactionChangeLimit[0]:
                currentSatisfaction = Satisfaction.Middle;
                break;
            case var t when t < satisfactionChangeLimit[1]:
                currentSatisfaction = Satisfaction.Low;
                CurrentStatus = ConsumerState.AfterPay;
                break;
        }
    }
    private void UpdateEatting()
    {
        //�Ļ����� ����.
    }
    private void UpdateBeforePay()
    {
        //�Ļ簡 ���� �� ����� �̵��� ��, ����� �Ϸ�ɶ������� ����.
        //���⼭�� ��ٸ��� �������� ������ ��ġ�� ����.
        if (agent.remainingDistance <= 0.1f)
        {
            consumerManager.OnPayStart(consumerData);
        }
    }
    private void UpdateAfterPay()
    {
        //��� �� �����ϴ� ����.
        //���� ������ƮǮ�� ��ȯ��.
        if (agent.IsArrive(consumerManager.spawnPoint))
        {
            consumerManager.consumerPool.Release(gameObject);
        }
    }
}
