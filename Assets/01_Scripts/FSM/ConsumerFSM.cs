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
        //빈자리가 없어 대기하는 상태.
        //앞사람이 사라지면 앞사람 자리로 이동.
    }
    private void UpdateBeforeOrder()
    {
        //빈자리가 생겨 빈자리로 이동 후 주문.
        //직원이 주문을 받아가기 전까지의 상태.
        if(agent.remainingDistance < 0.1f)
        {
            OnSeatEvent?.Invoke(GetComponent<Consumer>());
        }
    }
    private void UpdateAfterOrder()
    {
        //주문을 받아간 후, 음식이 나오기까지의 상태.
        //deltaTime을 적산하여 만족도 상태를 갱신.
        orderWaitTimer -= Time.deltaTime;
        Debug.Log(orderWaitTimer);
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
        //식사중인 상태.
    }
    private void UpdateBeforePay()
    {
        //식사가 끝난 후 계산대로 이동한 후, 계산이 완료될때까지의 상태.
        //여기서의 기다림은 만족도에 영향을 미치지 않음.
    }
    private void UpdateAfterPay()
    {
        //계산 후 퇴장하는 상태.
        //이후 오브젝트풀에 반환됨.
        if (agent.remainingDistance <= 0.1f)
        {
            consumerManager.consumerPool.Release(gameObject);
        }
    }

    private void UpdateDisappoint()
    {
        //주문 후 30초 내로 음식이 나오지 않아 불만족 상태로 퇴장하는 상태.
        //agent가 목적지에 도달하면 오브젝트풀에 반환됨
        if(agent.remainingDistance <= 0.1f)
        {
            consumerManager.consumerPool.Release(gameObject);
        }
    }
}
