using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class ConsumerManager : MonoBehaviour
{
    private readonly string consumerDataTableFileName = "guesttable";
    private readonly string consumerSpawnPercentCsvFileName = "guestspawnratetable";
    [SerializeField] private BuffManager buffManager;
    [SerializeField] private int maxConsumerCnt;
    [SerializeField] private int maxWaitingSeatCnt;
    [SerializeField] private GameObject consumerPrefab;
    [SerializeField] public Transform spawnPoint;
    private int tempPairProb = 50;
    [SerializeField] private TextMeshProUGUI waitingText;

    public WorkFlowController workFlowController;

    private ConsumerDataTable consumerDataTable;
    private Dictionary<int, List<int>> consumerSpawnPercent;

    /// <summary>
    ///     ��⿭ �ڸ�
    /// </summary>
    [SerializeField] private List<Transform> waitingConsumerSeats;

    /// <summary>
    ///     ���� ���ӽſ� �����Ǿ��ִ� �մԵ��� ���º��� �����ϴ� ��ųʸ�
    /// </summary>
    private readonly Dictionary<ConsumerFSM.ConsumerState, List<Consumer>> currentSpawnedConsumerDictionary = new();
    private int waitOutsideConsumerCnt = 0;

    public List<int> foodIds;

    private void Awake()
    {
        workFlowController = ServiceLocator.Instance.GetSceneService<GameManager>().WorkFlowController;
        var pivotManager = ServiceLocator.Instance.GetSceneService<GameManager>().ObjectPivotManager;
        spawnPoint = pivotManager.GetConsumerSpawnPoint();
        waitingConsumerSeats = pivotManager.GetWatingLines();
    }


    private void UpdateWaitingText()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"대기 인원 : {waitOutsideConsumerCnt}");
        waitingText.text = sb.ToString();
    }

    [ContextMenu("Consumer Spawn")]
    public void SpawnConsumer()
    {
        var consumer = consumerPool.Get().GetComponent<Consumer>();
        AfterSpawnInit(consumer);
    }

    [ContextMenu("Pair Consumer Spawn")]
    public void SpawnPairConsumer()
    {
        var consumer1 = consumerPool.Get().GetComponent<Consumer>();
        var consumer2 = consumerPool.Get().GetComponent<Consumer>();
        SetPairData(consumer1, consumer2);
        AfterSpawnInit(consumer1);
    }
    public bool CanSpawnConsumer()
    {
        return currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count < 3;
    }

    public void SpawnConsumer(ConsumerData consumerData)
    {
        var spawnConsumer = consumerPool.Get().GetComponent<Consumer>();
        spawnConsumer.FSM.consumerData = consumerData;
        AfterSpawnInit(spawnConsumer);
    }

    private void SetPairData(Consumer owner, Consumer partner)
    {
        owner.pairData = new ConsumerPairData();
        owner.pairData.owner = owner;
        owner.pairData.partner = partner;

        partner.pairData = owner.pairData;
        partner.FSM.CurrentStatus = ConsumerFSM.ConsumerState.None;

        if (owner.FSM.consumerData.GuestType == GuestType.BadGuest)
        {
            var withoutBadTypeList = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetConsumerWithoutBadType(ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme);
            owner.FSM.consumerData = withoutBadTypeList[UnityEngine.Random.Range(0, withoutBadTypeList.Count)];
        }
        else if (partner.FSM.consumerData.GuestType == GuestType.BadGuest)
        {
            var withoutBadTypeList = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetConsumerWithoutBadType(ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme);
            partner.FSM.consumerData = withoutBadTypeList[UnityEngine.Random.Range(0, withoutBadTypeList.Count)];
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            if (currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count < maxWaitingSeatCnt)
            {
                int cnt = 0;
                foreach (var consumers in currentSpawnedConsumerDictionary.Values)
                {
                    foreach (var consumer in consumers)
                    {
                        cnt++;
                    }
                }
                if (cnt < 9)
                {
                    int pairProb = UnityEngine.Random.Range(0, 100);
                    if (pairProb < tempPairProb)
                        SpawnConsumer();
                    else
                        SpawnPairConsumer();
                }
            }
            else if (waitOutsideConsumerCnt < 99)
            {
                waitOutsideConsumerCnt++;
                UpdateWaitingText();
            }
            var buff = buffManager.GetBuff(BuffType.FootTraffic);
            var basicTime = 5f;
            basicTime *= buff?.BuffEffect ?? 1f;

            yield return new WaitForSeconds(basicTime);
        }
    }

    public void OnEndMeal(Consumer consumer)
    {
        if (consumer.pairData == null)
        {
            workFlowController.OnEatComplete(consumer.currentTable);
        }
        else if (consumer.pairData.owner == consumer)
        {
            workFlowController.OnEatComplete(consumer.currentTable, true);
        }

        var cnt = workFlowController.AssignCashier(consumer);
        if (cnt != 0)
            consumer.GetComponent<NavMeshAgent>().SetDestination(
                workFlowController.GetCashierCounter().GetInteractablePoints(InteractPermission.Consumer)[0].transform
                    .position + new Vector3(-0.5f, 0, 0) * cnt);
    }

    public void OnPayStart(ConsumerData consumerData)
    {
        workFlowController.RegisterPayment();
    }


    #region ConsumerObjPoolling

    /// <summary>
    ///     �մ� ������Ʈ Ǯ
    /// </summary>
    public IObjectPool<GameObject> consumerPool;

    private void Start()
    {
        maxWaitingSeatCnt = waitingConsumerSeats.Count;
        consumerPool = new ObjectPool<GameObject>(() => { return OnCreateConsumer(); }
            , OnTakeConsumer
            , OnReturnConsumer
            , OnDestroyConsumer
            , false);
        foreach (ConsumerFSM.ConsumerState consumerState in Enum.GetValues(typeof(ConsumerFSM.ConsumerState)))
            currentSpawnedConsumerDictionary[consumerState] = new List<Consumer>();

        consumerDataTable = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString());
        consumerSpawnPercent = CsvToDictionaryLoader.LoadCsvToDictionary(consumerSpawnPercentCsvFileName);


        StartCoroutine(SpawnCoroutine());
    }

    public void OnChangeConsumerState(Consumer consumer, ConsumerFSM.ConsumerState state)
    {
        var prevState = consumer.FSM.CurrentStatus;
        if (prevState == state)
            return;
        currentSpawnedConsumerDictionary[state].Add(consumer);
        currentSpawnedConsumerDictionary[prevState].Remove(consumer);
    }

    public void OnWaitingLineUpdate(Consumer consumer)
    {
        if (currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count > 0)
        {
            for (var i = 0; i < currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count; i++)
            {
                currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting][i].NextTargetTransform =
                    waitingConsumerSeats[i];
                if (currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting][i].pairData != null)
                {
                    var ownerDst = currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting][i].GetComponent<NavMeshAgent>().destination;
                    var partnerDst = new Vector3(ownerDst.x + 0.5f, ownerDst.y, ownerDst.z);
                    currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting][i].pairData.partner.GetComponent<NavMeshAgent>()
                        .SetDestination(partnerDst);
                }
            }
            if (waitOutsideConsumerCnt > 0)
            {
                int pairProb = UnityEngine.Random.Range(0, 100);
                if (pairProb > tempPairProb && waitOutsideConsumerCnt >= 2)
                {
                    SpawnPairConsumer();
                    waitOutsideConsumerCnt -= 2;
                }
                else
                {
                    SpawnConsumer();
                    waitOutsideConsumerCnt--;
                }
                UpdateWaitingText();
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
        consumer.NextTargetTransform = null;
        consumer.pairData = null;
        consumer.FSM.consumerManager = this;
        consumer.FSM.consumerData = consumerDataTable.GetConsumerData(consumerSpawnPercent[14]);
        consumer.FSM.consumerData.Init();
        consumer.isEndMeal = false;
        consumer.FSM.SetCashierCounter(workFlowController.GetCashierCounter());
        consumer.FSM.OnSeatEvent -= workFlowController.AssignGetOrderWork;
        consumer.FSM.OnSeatEvent += workFlowController.AssignGetOrderWork;
    }

    private void OnTakeConsumer(GameObject consumer)
    {
        InitConsumer(consumer.GetComponent<Consumer>());
        consumer.SetActive(true);
    }

    private void OnReturnConsumer(GameObject consumer)
    {
        foreach (var list in currentSpawnedConsumerDictionary.Values)
            if (list.Contains(consumer.GetComponent<Consumer>()))
                list.Remove(consumer.GetComponent<Consumer>());

        if (consumer.GetComponent<ConsumerFSM>().consumerData.GuestType == GuestType.Influencer)
        {
            //TODO : Make InfluencerBuff on consumerData based
            buffManager.TempBuffOn();
        }

        //TODO : SPUM 초기화
        consumer.SetActive(false);
    }

    private void OnDestroyConsumer(GameObject consumer)
    {
        Destroy(consumer);
    }

    private void AfterSpawnInit(Consumer consumer)
    {
        if (workFlowController.RegisterCustomer(consumer))
        {
            consumer.FSM.CurrentStatus = ConsumerFSM.ConsumerState.BeforeOrder;
        }
        else
        {
            consumer.FSM.CurrentStatus = ConsumerFSM.ConsumerState.Waiting;
            consumer.NextTargetTransform =
                waitingConsumerSeats[currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count];
            currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Add(consumer);
            if (consumer.pairData != null)
            {
                var ownerDst = consumer.GetComponent<NavMeshAgent>().destination;
                var partnerDst = new Vector3(ownerDst.x + 0.5f, ownerDst.y, ownerDst.z);
                consumer.pairData.partner.GetComponent<NavMeshAgent>().SetDestination(partnerDst);
            }
        }
    }

    #endregion
}