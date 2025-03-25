using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ConsumerFSM : MonoBehaviour
{
    public ConsumerManager consumerManager;
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
    private float orderWaitTimer;
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
            currentStatus = value;

            switch (currentStatus)
            {
                case ConsumerState.Waiting:
                    break;
                case ConsumerState.BeforeOrder:
                    agent.SetDestination(GetComponent<Consumer>().currentTable.InteractablePoints[1].position);
                    break;
                case ConsumerState.AfterOrder:
                    break;
                case ConsumerState.Eatting:
                    //StartCoroutine();
                    break;
                case ConsumerState.BeforePay:
                    break;
                case ConsumerState.AfterPay:
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
                    agent.SetDestination(consumerManager.spawnPoint.position);
                    break;
            }
        }
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        currentSatisfaction = Satisfaction.High;
        orderWaitTimer = 0f;
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
        if(agent.remainingDistance < 0.1f)
        {
            OnSeatEvent?.Invoke(GetComponent<Consumer>());
            CurrentStatus = ConsumerState.AfterOrder;
        }
    }
    private void UpdateAfterOrder()
    {
        //�ֹ��� �޾ư� ��, ������ ����������� ����.
        //deltaTime�� �����Ͽ� ������ ���¸� ����.
        orderWaitTimer -= Time.deltaTime;
        // Debug.Log(orderWaitTimer);
        switch (orderWaitTimer)
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
        if(agent.remainingDistance <= 0.1f)
        {
            consumerManager.consumerPool.Release(gameObject);
        }
    }
}
