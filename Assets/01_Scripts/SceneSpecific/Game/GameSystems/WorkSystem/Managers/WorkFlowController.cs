using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorkFlowController : MonoBehaviour
{
    [SerializeField] private AssetReference foodPrefab;
    [SerializeField] private WorkManager workManager;
    [SerializeField] private CashierCounter cashierCounter;
    private readonly Queue<Consumer> cashierQueue = new();
    private readonly InteractableObjectManager<CookingStation> cookingStationManager = new();
    private readonly InteractableObjectManager<FoodPickupCounter> foodPickupCounterManager = new();
    private readonly Queue<(MainLoopWorkContext, CookingStation)> foodPickupCounterQueue = new();
    private readonly Queue<MainLoopWorkContext> orderQueue = new();

    private readonly InteractableObjectManager<Table> tableManager = new();


    private Queue<Consumer> customerQueue = new();

    private void Awake()
    {
        CreateFoodObjectPool();
        customerQueue = new Queue<Consumer>();
        cookingStationManager.InsertObject(tempStation);
        foodPickupCounterManager.InsertObject(tempCounter);
    }

    private void CreateFoodObjectPool()
    {
        var handle = foodPrefab.LoadAssetAsync<GameObject>();
        handle.WaitForCompletion();
        GameManager gameManager = ServiceLocator.Instance.GetSceneService<GameManager>();
        var foodGameObject = handle.Result;
        var foodObject = foodGameObject.GetComponent<FoodObject>();
        gameManager.ObjectPoolManager.CreatePool(foodObject);
    }

    private void Start()
    {
        tableManager.ObjectAvailableEvent += OnTableVacated;
        cookingStationManager.ObjectAvailableEvent += OnCookingStationVacated;
        foodPickupCounterManager.ObjectAvailableEvent += OnFoodPickupCounterVacated;
    }

    public void AddTable(Table table)
    {
        tableManager.InsertObject(table);
    }

    #region TableCleanLogic()

    public void OnEatComplete(Table table)
    {
        var work = new WorkCleanTable(workManager, WorkType.Hall);
        work.SetInteractable(table);
        work.SetContext(this);
        table.SetWork(work);
        var food = table.GetFood();
        var sprite = food.GetComponent<SpriteRenderer>();
        sprite.color = Color.red;
        workManager.AddWork(work);
    }

    #endregion
    
    public CookingStation tempStation;
    public FoodPickupCounter tempCounter;


    #region CustomerLogic

    public bool RegisterCustomer(Consumer consumer)
    {
        if (tableManager.IsAvailableObjectExist)
        {
            var table = tableManager.GetAvailableObject();
            consumer.SetTable(table);
            //if (consumer.pairData != null)
            //{
            //    consumer.pairData.pairTable = consumer.currentTable;
            //    consumer.pairData.partner.SetTable(consumer.currentTable);
            //}
            return true;
        }

        customerQueue.Enqueue(consumer);
        return false;
    }

    private void OnTableVacated()
    {
        if (customerQueue.Count > 0)
        {
            var consumer = customerQueue.Dequeue();
            consumer.SetTable(tableManager.GetAvailableObject());
            consumer.OnTableVacated();
            //TODO:Assign이 아니라 손님을 이동
        }
    }

    public void AssignGetOrderWork(Consumer consumer)
    {
        var table = consumer.currentTable;
        var work = new WorkGetOrder(workManager, WorkType.Hall);
        var context = new MainLoopWorkContext(consumer, this);
        table.SetWork(work);
        work.SetContext(context);
        work.SetInteractable(table);
        workManager.AddWork(work);
    }

    public void ReturnTable(Table table)
    {
        tableManager.ReturnObject(table);
    }

    #endregion

    #region CookingLogic

    public void RegisterOrder(MainLoopWorkContext context)
    {
        if (cookingStationManager.IsAvailableObjectExist)
            AssignOrderWork(context);
        else
            orderQueue.Enqueue(context);
    }

    private void OnCookingStationVacated()
    {
        if (orderQueue.Count > 0)
        {
            var context = orderQueue.Dequeue();
            AssignOrderWork(context);
        }
    }

    private void AssignOrderWork(MainLoopWorkContext context)
    {
        Debug.Log("AssignOrderWork");
        var cookingStation = cookingStationManager.GetAvailableObject();
        var work = new WorkCooking(workManager, WorkType.Kitchen);
        cookingStation.SetWork(work);
        work.SetContext(context);
        work.SetInteractable(cookingStation);
        workManager.AddWork(work);
    }

    public void ReturnCookingStation(CookingStation station)
    {
        cookingStationManager.ReturnObject(station);
    }

    #endregion

    #region FoodPickupLogic

    public bool IsFoodCounterAvailable()
    {
        return foodPickupCounterManager.IsAvailableObjectExist;
    }

    public void RegisterFoodToHall(MainLoopWorkContext context, CookingStation target)
    {
        foodPickupCounterQueue.Enqueue((context, target));
    }

    public FoodPickupCounter GetEmptyFoodCounter()
    {
        return foodPickupCounterManager.Dequeue();
    }

    private void OnFoodPickupCounterVacated()
    {
        if (foodPickupCounterQueue.Count > 0)
        {
            var counter = GetEmptyFoodCounter();
            var (context, target) = foodPickupCounterQueue.Dequeue();
            var work = new WorkGotoCookingStation(workManager, WorkType.Kitchen);
            target.SetWork(work);
            work.SetInteractable(target);
            work.SetContext(context, counter);
            workManager.AddWork(work);
        }
    }

    public void ReturnFoodPickupCounter(FoodPickupCounter counter)
    {
        foodPickupCounterManager.ReturnObject(counter);
    }

    #endregion

    #region CashierLogic

    public int AssignCashier(Consumer consumer)
    {
        cashierQueue.Enqueue(consumer);
        if (cashierQueue.Count > 1) return cashierQueue.Count - 1;

        consumer.NextTargetTransform =
            cashierCounter.GetInteractablePoints(InteractPermission.Consumer).First().transform;
        consumer.FSM.OnStartPayment();
        return 0;
    }

    public int OnCashierFinished()
    {
        var firstConsumer = cashierQueue.Dequeue();
        firstConsumer.FSM.OnEndPayment();

        if (cashierQueue.Count == 0)
            return 0;
        var nextConsumer = cashierQueue.Peek();
        nextConsumer.FSM.OnStartPayment();

        var buf = firstConsumer.transform;
        foreach (var consumer in cashierQueue)
        {
            consumer.NextTargetTransform = buf;
            buf = consumer.transform;
        }

        return cashierQueue.Count;
    }

    public void RegisterPayment()
    {
        var work = new WorkPayment(workManager, WorkType.Payment);
        var context = new MainLoopWorkContext(cashierQueue.First(), this);
        cashierCounter.SetWork(work);
        work.SetContext(context);
        work.SetInteractable(cashierCounter);
        workManager.AddWork(work);
    }

    public CashierCounter GetCashierCounter()
    {
        return cashierCounter;
    }

    #endregion

    //손님 대기열
    //Table에 자리가 나면 손님할당
    //주문 대기
    //화구에 자리가 생기면 음식 할당
    //음식 놓는곳도 생각해야함
}