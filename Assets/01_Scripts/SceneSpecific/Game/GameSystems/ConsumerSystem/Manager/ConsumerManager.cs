using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class ConsumerManager : MonoBehaviour
{
    [SerializeField] public WorkFlowController workFlowController;
    [SerializeField] private int maxConsumerCnt;
    [SerializeField] private int maxWaitingSeatCnt;
    [SerializeField] private GameObject consumerPrefab;
    [SerializeField] public Transform spawnPoint;

    /// <summary>
    /// ���� ���ӽſ� �����Ǿ��ִ� �մԵ��� ���º��� �����ϴ� ��ųʸ�
    /// </summary>
    private Dictionary<ConsumerFSM.ConsumerState, List<Consumer>> currentSpawnedConsumerDictionary = new();


    /// <summary>
    /// ��⿭ �ڸ�
    /// </summary>
    [SerializeField] private List<Transform> waitingConsumerSeats;


    #region ConsumerObjPoolling
    /// <summary>
    /// �մ� ������Ʈ Ǯ
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

        StartCoroutine(SpawnCoroutine());
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
        //TODO : �մ��� ������ƮǮ���� ���� �� ���� (���ϴ� ����, �մ� Ÿ��, �ټ� �ڸ� ��)
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
            consumer.FSM.CurrentStatus = ConsumerFSM.ConsumerState.Waiting;
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

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            SpawnConsumer();
            yield return new WaitForSeconds(5f);
        }
    }

    public void OnEndMeal(Consumer consumer)
    {
        workFlowController.OnEatComplete(consumer.currentTable);
        int cnt = workFlowController.AssignCashier(consumer);
        if (cnt != 0)
        {
            consumer.transform.position = workFlowController.GetCashierCounter().InteractablePoints[0].position + new Vector3(-1, 0, 0) * cnt;
        }


    }

    public void OnPayStart()
    {
        workFlowController.RegisterPayment();
    }
}
