using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class ConsumerManager : MonoBehaviour
{
    [SerializeField] private WorkFlowController workFlowController;
    [SerializeField] private int maxConsumerCnt;
    [SerializeField] private int maxWaitingSeatCnt;
    [SerializeField] private GameObject consumerPrefab;
    [SerializeField] public Transform spawnPoint;

    /// <summary>
    /// 현재 게임신에 스폰되어있는 손님들을 상태별로 관리하는 딕셔너리
    /// </summary>
    private Dictionary<ConsumerFSM.ConsumerState, List<Consumer>> currentSpawnedConsumerDictionary = new();


    /// <summary>
    /// 대기열 자리
    /// </summary>
    [SerializeField] private List<Transform> waitingConsumerSeats;


    #region ConsumerObjPoolling
    /// <summary>
    /// 손님 오브젝트 풀
    /// </summary>
    public IObjectPool<GameObject> consumerPool;
    private void Start()
    {
        maxWaitingSeatCnt = waitingConsumerSeats.Count;
        consumerPool = new ObjectPool<GameObject>(() =>
        {
            return OnCreateConsumer();
        }
        , OnTakeConsumer
        , OnReturnConsumer
        , OnDestroyConsumer
        , false);
        foreach (ConsumerFSM.ConsumerState consumerState in System.Enum.GetValues(typeof(ConsumerFSM.ConsumerState)))
        {
            currentSpawnedConsumerDictionary[consumerState] = new();
        }
    }

    public void OnChangeConsumerState(Consumer consumer, ConsumerFSM.ConsumerState state)
    {
        var prevState = consumer.FSM.CurrentStatus;
        currentSpawnedConsumerDictionary[state].Add(consumer);
        currentSpawnedConsumerDictionary[prevState].Remove(consumer);
    }

    public void OnWaitingLineUpdate(Consumer consumer)
    {
        if (currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count > 0)
        {
            for (int i = 0; i < currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count; i++)
            {
                currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting][i].NextTargetTransform = waitingConsumerSeats[i];
            }
        }
    }

    private GameObject OnCreateConsumer()
    {
        var obj = Instantiate(consumerPrefab);
        return obj;
    }

    private void InitConsumer(Consumer consumer)
    {
        //TODO : 손님을 오브젝트풀에서 꺼낼 때 설정 (원하는 음식, 손님 타입, 줄설 자리 등)
        consumer.transform.position = spawnPoint.position;
        consumer.consumerManager = this;
        consumer.FSM.consumerManager = this;
        consumer.FSM.SetCashierCounter(workFlowController.GetCashierCounter());

        consumer.FSM.OnSeatEvent += workFlowController.AssignGetOrderWork;

        if (workFlowController.RegisterCustomer(consumer))
        {
            consumer.FSM.CurrentStatus = ConsumerFSM.ConsumerState.BeforeOrder;
            currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.BeforeOrder].Add(consumer);
        }
        else
        {
            consumer.NextTargetTransform = waitingConsumerSeats[currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count];
            currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Add(consumer);
        }
    }

    private void OnTakeConsumer(GameObject consumer)
    {
        InitConsumer(consumer.GetComponent<Consumer>());
        consumer.SetActive(true);
    }
    private void OnReturnConsumer(GameObject consumer)
    {
        foreach (var list in currentSpawnedConsumerDictionary.Values)
        {
            if (list.Contains(consumer.GetComponent<Consumer>()))
            {
                list.Remove(consumer.GetComponent<Consumer>());
            }
        }
        consumer.SetActive(false);
    }
    private void OnDestroyConsumer(GameObject consumer)
    {
        Destroy(consumer);
    }
    #endregion
    [ContextMenu("Consumer Spawn")]
    public void SpawnConsumer()
    {
        if (currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count < maxWaitingSeatCnt)
        {
            consumerPool.Get();
            Debug.Log(currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count);
        }
        else
        {
            Debug.Log("No More WaitingSeat");
        }
    }

    public void OnEndMeal(Consumer consumer)
    {
        int cnt = workFlowController.AssignCashier(consumer);
        if (cnt != 0)
        {
            consumer.transform.position = workFlowController.GetCashierCounter().InteractablePoints[0].position + new Vector3(-1, 0, 0) * cnt;
        }
    }
}
