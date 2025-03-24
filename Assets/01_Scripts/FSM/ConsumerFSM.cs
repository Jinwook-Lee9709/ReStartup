using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ConsumerFSM : MonoBehaviour
{
    //TODO : �ֹ��� ����, ���� ���̺�
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
        //���ڸ��� ���� ����ϴ� ����.
        //�ջ���� ������� �ջ�� �ڸ��� �̵�.
    }
    private void UpdateBeforeOrder()
    {
        //���ڸ��� ���� ���ڸ��� �̵� �� �ֹ�.
        //������ �ֹ��� �޾ư��� �������� ����.
    }
    private void UpdateAfterOrder()
    {
        //�ֹ��� �޾ư� ��, ������ ����������� ����.
        //deltaTime�� �����Ͽ� ������ ���¸� ����
    }
    private void UpdateEatting()
    {
        //�Ļ����� ����
    }
    private void UpdateBeforePay()
    {
        //�Ļ簡 ���� �� ����� �̵��� ��, ����� ��ٸ��� ����.
        //���⼭�� ��ٸ��� �������� ������ ��ġ�� ����
    }
    private void UpdateAfterPay()
    {
        //��� �� �����ϴ� ����.
        //AfterOrder������ �� ���������� ���ŵ� �������� UserData�� ������
        //���� ������ƮǮ�� ��ȯ��
    }
}
