using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ConsumerFSM : MonoBehaviour
{
    //TODO : 주문한 음식, 현재 테이블
    public Table currentTable = null;
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

    private ConsumerState currentStatus = ConsumerState.Waiting;

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
                    break;
                case ConsumerState.AfterOrder:
                    break;
                case ConsumerState.Eatting:
                    break;
                case ConsumerState.BeforePay:
                    break;
                case ConsumerState.AfterPay:
                    break;
            }
        }
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
        //빈자리가 없어 대기하는 상태.
        //앞사람이 사라지면 앞사람 자리로 이동.
    }
    private void UpdateBeforeOrder()
    {
        //빈자리가 생겨 빈자리로 이동 후 주문.
        //직원이 주문을 받아가기 전까지의 상태.
    }
    private void UpdateAfterOrder()
    {
        //주문을 받아간 후, 음식이 나오기까지의 상태.
        //deltaTime을 적산하여 만족도 상태를 갱신
    }
    private void UpdateEatting()
    {
        //식사중인 상태
    }
    private void UpdateBeforePay()
    {
        //식사가 끝난 후 계산대로 이동한 후, 계산을 기다리는 상태.
        //여기서의 기다림은 만족도에 영향을 미치지 않음
    }
    private void UpdateAfterPay()
    {
        //계산 후 퇴장하는 상태.
        //AfterOrder상태일 때 마지막으로 갱신된 만족도가 UserData에 누적됨
        //이후 오브젝트풀에 반환됨
    }
}
