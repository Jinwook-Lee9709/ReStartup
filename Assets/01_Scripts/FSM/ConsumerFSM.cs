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
    public enum ConsumerState
    {
        Waiting,
        BeforeOrder,
        AfterOrder,
        Eatting,
        BeforePay,
        AfterPay,

        Disappoint,
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


    [SerializeField] private ConsumerState currentStatus = ConsumerState.Waiting;
    private Satisfaction currentSatisfaction = Satisfaction.High;

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
                    consumerManager.OnWaitingLineUpdate(consumer);
                    agent.SetDestination(consumer.currentTable.InteractablePoints[1].position);
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
                case ConsumerState.Disappoint:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.Disappoint);
                    agent.SetDestination(consumerManager.spawnPoint.position);
                    break;
            }
            currentStatus = value;
        }
    }

    public void OnEndPayment()
    {
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
            case ConsumerState.Disappoint:
                UpdateDisappoint();
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
        if (agent.remainingDistance < 0.1f)
        {
            OnSeatEvent?.Invoke(consumer);
        }
    }
    private void UpdateAfterOrder()
    {
        //�ֹ��� �޾ư� ��, ������ ����������� ����.
        //deltaTime�� �����Ͽ� ������ ���¸� ����.
        consumerData.OrderWaitTimer -= Time.deltaTime;
        // Debug.Log(orderWaitTimer);
        switch (consumerData.OrderWaitTimer)
        {
            case var t when t < satisfactionChangeLimit[0]:
                currentSatisfaction = Satisfaction.Middle;
                break;
            case var t when t < satisfactionChangeLimit[1]:
                currentSatisfaction = Satisfaction.Low;
                currentStatus = ConsumerState.Disappoint;
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
    }
    private void UpdateAfterPay()
    {
        //��� �� �����ϴ� ����.
        //���� ������ƮǮ�� ��ȯ��.
        if (agent.remainingDistance <= 0.1f)
        {
            consumerManager.consumerPool.Release(gameObject);
        }
    }

    private void UpdateDisappoint()
    {
        //�ֹ� �� 30�� ���� ������ ������ �ʾ� �Ҹ��� ���·� �����ϴ� ����.
        //agent�� �������� �����ϸ� ������ƮǮ�� ��ȯ��
        if (agent.remainingDistance <= 0.1f)
        {
            consumerManager.consumerPool.Release(gameObject);
        }
    }
}
