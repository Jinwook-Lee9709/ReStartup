using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class ConsumerManager : MonoBehaviour
{
    private readonly string consumerDataTableFileName = "guesttable";
    private readonly string consumerSpawnPercentCsvFileName = "guestspawnratetable";
    private readonly string waitingTextStringID = "WaitingNumber";
    [SerializeField] private BuffManager buffManager;
    [SerializeField] private int maxConsumerCnt;
    [SerializeField] private int maxWaitingSeatCnt;
    [SerializeField] private GameObject consumerPrefab;
    [SerializeField] public Transform spawnPoint;
    private int pairProb = 95;
    [SerializeField] private TextMeshProUGUI waitingText;
    public WorkFlowController workFlowController;
    private ConsumerDataTable consumerDataTable;
    private Dictionary<int, List<int>> consumerSpawnPercent;
    [SerializeField] private List<Transform> waitingConsumerSeats;
    [SerializeField] private List<Transform> payWaitingPivots;


    private readonly Dictionary<ConsumerFSM.ConsumerState, List<Consumer>> currentSpawnedConsumerDictionary = new();
    private int waitOutsideConsumerCnt = 0;
    private string waitingNumberText = "대기 인원";

    public List<int> foodIds;
    public Queue<ConsumerData> promotionWatings = new();

    public bool IsPayWaitingLineVacated
    {
        get
        {
            return workFlowController.GetCashierQueue().Count < payWaitingPivots.Count - 1;
        }
    }
    public void AddPromotionConsumerWaitingLine(ConsumerData data)
    {
        promotionWatings.Enqueue(data);
        waitOutsideConsumerCnt++;
        UpdateWaitingText();
    }
    private void Awake()
    {
        workFlowController = ServiceLocator.Instance.GetSceneService<GameManager>().WorkFlowController;
        var pivotManager = ServiceLocator.Instance.GetSceneService<GameManager>().ObjectPivotManager;
        spawnPoint = pivotManager.GetConsumerSpawnPoint();
        waitingConsumerSeats = pivotManager.GetWatingLines();
        payWaitingPivots = pivotManager.GetPayWaitingPivots();
    }

    private void UpdateWaitingText()
    {
        waitingText.text = (waitOutsideConsumerCnt + currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count).ToString();
    }

    [ContextMenu("Consumer Spawn")]
    public async UniTask SpawnConsumer()
    {
        var consumer = consumerPool.Get().GetComponent<Consumer>();
        await consumer.FSM.SetModel();
        AfterSpawnInit(consumer);
    }

    public async UniTask SpawnConsumer(ConsumerData consumerData)
    {
        var spawnConsumer = consumerPool.Get().GetComponent<Consumer>();
        spawnConsumer.FSM.consumerData = consumerData;
        spawnConsumer.FSM.consumerData.Init();
        await spawnConsumer.FSM.SetModel();
        AfterSpawnInit(spawnConsumer);
    }

    [ContextMenu("Pair Consumer Spawn")]
    public async UniTask SpawnPairConsumer()
    {
        var consumer1 = consumerPool.Get().GetComponent<Consumer>();
        var consumer2 = consumerPool.Get().GetComponent<Consumer>();
        SetPairData(consumer1, consumer2);
        await consumer1.FSM.SetModel();
        await consumer2.FSM.SetModel();
        consumer2.FSM.CurrentStatus = ConsumerFSM.ConsumerState.None;
        AfterSpawnInit(consumer1);
    }

    public async UniTask SpawnPairConsumer(ConsumerData owner, ConsumerData partner)
    {
        var consumer1 = consumerPool.Get().GetComponent<Consumer>();
        var consumer2 = consumerPool.Get().GetComponent<Consumer>();
        SetPairData(consumer1, consumer2);
        consumer1.FSM.consumerData = owner;
        consumer2.FSM.consumerData = partner;
        SetFood(ref consumer1);
        SetFood(ref consumer2);
        await consumer1.FSM.SetModel();
        await consumer2.FSM.SetModel();
        consumer2.FSM.CurrentStatus = ConsumerFSM.ConsumerState.None;
        AfterSpawnInit(consumer1);
    }

    public void StartSpawnRoutine()
    {
        StopAllCoroutines();
        StartCoroutine(SpawnCoroutine());
    }


    private void SetFood(ref Consumer consumer)
    {
        var loveFoodId = consumer.FSM.consumerData.LoveFoodId;
        if (foodIds.Contains(loveFoodId))
        {
            var rand = UnityEngine.Random.Range(0, 5);
            if (rand < 4)
            {
                var foods = foodIds.Where((id) => id != loveFoodId).ToList();
                if (foods.Count < 1)
                {
                    consumer.needFood = DataTableManager.Get<FoodDataTable>(DataTableIds.Food.ToString()).GetFoodData(loveFoodId);
                    return;
                }
                var food = foods[UnityEngine.Random.Range(0, foods.Count)];
                consumer.needFood = DataTableManager.Get<FoodDataTable>(DataTableIds.Food.ToString()).GetFoodData(food);
            }
            else
            {
                consumer.needFood = DataTableManager.Get<FoodDataTable>(DataTableIds.Food.ToString()).GetFoodData(loveFoodId);
            }
        }
        else
        {
            consumer.needFood = DataTableManager.Get<FoodDataTable>(DataTableIds.Food.ToString()).GetFoodData(foodIds[UnityEngine.Random.Range(0, foodIds.Count)]);
        }
    }
    public bool CanSpawnConsumer()
    {
        return currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count < 3;
    }
    
    private void SetPairData(Consumer owner, Consumer partner)
    {
        owner.pairData = new ConsumerPairData();
        owner.pairData.owner = owner;
        owner.pairData.partner = partner;

        partner.pairData = owner.pairData;

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

        owner.FSM.consumerData.Init();
        partner.FSM.consumerData.Init();
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            if (currentSpawnedConsumerDictionary[ConsumerFSM.ConsumerState.Waiting].Count < maxWaitingSeatCnt)
            {
                int pairProb = UnityEngine.Random.Range(0, 100);
                if (buffManager.GetBuff(BuffType.PairSpawn)?.isOnBuff ?? false)
                {
                    if (pairProb > this.pairProb)
                        SpawnConsumer();
                    else
                        SpawnPairConsumer();
                }
                else
                {
                    if (pairProb < this.pairProb)
                        SpawnConsumer();
                    else
                        SpawnPairConsumer();
                }
            }
            else if (waitOutsideConsumerCnt < 99)
            {
                waitOutsideConsumerCnt++;
            }
            var buff = buffManager.GetBuff(BuffType.FootTraffic);
            var basicTime = 40f;
            basicTime *= buff?.BuffEffect ?? 1f;
            var inflowRate = UserDataManager.Instance.CurrentUserData.InflowRate;
            basicTime /= inflowRate == 0 ? 1 : (1 + 1 / (float)UserDataManager.Instance.CurrentUserData.InflowRate);

            UpdateWaitingText();
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
        {
            if (consumer.pairData?.owner == consumer)
            {
                consumer.GetComponent<NavMeshAgent>().SetDestination(payWaitingPivots[Mathf.Clamp(cnt - 1, 0, payWaitingPivots.Count - 1)].position);
            }
            else
            {
                consumer.GetComponent<NavMeshAgent>().SetDestination(payWaitingPivots[cnt - 1].position);
            }
        }
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
            , false,
            maxConsumerCnt);
        
        foreach (ConsumerFSM.ConsumerState consumerState in Enum.GetValues(typeof(ConsumerFSM.ConsumerState)))
            currentSpawnedConsumerDictionary[consumerState] = new List<Consumer>();

        consumerDataTable = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString());
        consumerSpawnPercent = CsvToDictionaryLoader.LoadCsvToDictionary(consumerSpawnPercentCsvFileName);

        UpdateWaitingText();
    }
    public void OnChangeConsumerState(Consumer consumer, ConsumerFSM.ConsumerState state)
    {
        var prevState = consumer.FSM.CurrentStatus;
        if (prevState == state)
            return;
        currentSpawnedConsumerDictionary[state].Add(consumer);
        currentSpawnedConsumerDictionary[prevState].Remove(consumer);
        UpdateWaitingText();
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

                if (promotionWatings.Count > 0)
                {
                    if (buffManager.GetBuff(BuffType.PairSpawn)?.isOnBuff ?? false)
                    {
                        if (pairProb < this.pairProb && waitOutsideConsumerCnt >= 2)
                        {
                            var owner = promotionWatings.Dequeue();
                            var list = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetConsumerWithoutBadType(ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme);
                            var partner = promotionWatings.Count != 0 ? promotionWatings.Dequeue() : list[UnityEngine.Random.Range(0, list.Count)];

                            SpawnPairConsumer(owner, partner);

                            waitOutsideConsumerCnt -= 2;
                        }
                        else
                        {
                            var promotionConsumer = promotionWatings.Dequeue();
                            SpawnConsumer(promotionConsumer);
                            waitOutsideConsumerCnt--;
                        }
                    }
                    else
                    {
                        if (pairProb > this.pairProb && waitOutsideConsumerCnt >= 2)
                        {
                            var owner = promotionWatings.Dequeue();
                            var list = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetConsumerWithoutBadType(ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme);
                            var partner = promotionWatings.Count != 0 ? promotionWatings.Dequeue() : list[UnityEngine.Random.Range(0, list.Count)];

                            SpawnPairConsumer(owner, partner);
                            waitOutsideConsumerCnt -= 2;
                        }
                        else
                        {
                            var promotionConsumer = promotionWatings.Dequeue();
                            SpawnConsumer(promotionConsumer);
                            waitOutsideConsumerCnt--;
                        }
                    }

                    UpdateWaitingText();
                    return;
                }

                if (buffManager.GetBuff(BuffType.PairSpawn)?.isOnBuff ?? false)
                {
                    if (pairProb < this.pairProb && waitOutsideConsumerCnt >= 2)
                    {
                        SpawnPairConsumer();
                        waitOutsideConsumerCnt -= 2;
                    }
                    else
                    {
                        SpawnConsumer();
                        waitOutsideConsumerCnt--;
                    }
                }
                else
                {
                    if (pairProb > this.pairProb && waitOutsideConsumerCnt >= 2)
                    {
                        SpawnPairConsumer();
                        waitOutsideConsumerCnt -= 2;
                    }
                    else
                    {
                        SpawnConsumer();
                        waitOutsideConsumerCnt--;
                    }
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
        consumer.GetComponent<NavMeshAgent>().Warp(spawnPoint.position);
        consumer.consumerManager = this;
        consumer.FSM.buffManager = buffManager;
        consumer.NextTargetTransform = null;
        consumer.pairData = null;
        consumer.FSM.consumerManager = this;
        ConsumerData data = new(consumerDataTable.GetConsumerData(consumerSpawnPercent[UserDataManager.Instance.CurrentUserData.CurrentRank]));
        consumer.FSM.consumerData = data;
        consumer.FSM.consumerData.Init();
        consumer.FSM.satisfactionIcon.gameObject.SetActive(false);
        consumer.FSM.CurrentStatus = ConsumerFSM.ConsumerState.Debug;
        consumer.FSM.CurrentSatisfaction = ConsumerFSM.Satisfaction.None;
        SetFood(ref consumer);
        consumer.isEndMeal = false;
        consumer.isFoodReady = false;
        consumer.FSM.SetCashierCounter(workFlowController.GetCashierCounter());
        consumer.FSM.OnSeatEvent -= workFlowController.AssignGetOrderWork;
        consumer.FSM.OnSeatEvent += workFlowController.AssignGetOrderWork;
    }

    private void OnTakeConsumer(GameObject consumer)
    {
        InitConsumer(consumer.GetComponent<Consumer>());
        consumer.SetActive(true);
        var currentConsumerData = consumer.GetComponent<Consumer>().FSM.consumerData;
        if (currentConsumerData.GuestType == GuestType.Influencer)
        {
            var timerBuff = DataTableManager.Get<BuffDataTable>(DataTableIds.Buff.ToString()).GetBuffForBuffID(currentConsumerData.BuffId1);
            buffManager.StartBuff(timerBuff);
        }

    }

    private void OnReturnConsumer(GameObject consumer)
    {
        foreach (var list in currentSpawnedConsumerDictionary.Values)
            if (list.Contains(consumer.GetComponent<Consumer>()))
                list.Remove(consumer.GetComponent<Consumer>());

        Destroy(consumer.GetComponentInChildren<TextBubble>()?.gameObject);
        if(consumer.GetComponent<ConsumerFSM>().Model != null)
            Destroy(consumer.GetComponent<ConsumerFSM>().Model.gameObject);
        consumer.SetActive(false);
    }

    private void OnDestroyConsumer(GameObject consumer)
    {
        Destroy(consumer);
    }

    private void AfterSpawnInit(Consumer consumer)
    {
        consumer.gameObject.SetActive(true);
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