using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorkFlowController
{
    static readonly string FoodObjectName = "FoodObject";
    private GameManager gameManager;
    private WorkManager workManager;
    private CashierCounter cashierCounter;
    private TrayReturnCounter trayReturnCounter;
    private SinkingStation sinkingStation;
    private readonly LinkedList<Consumer> cashierQueue = new();
    private readonly Dictionary<CookwareType, InteractableObjectManager<CookingStation>> cookingStationManagers = new();
    private readonly InteractableObjectManager<FoodPickupCounter> foodPickupCounterManager = new();
    private readonly LinkedList<(MainLoopWorkContext, CookingStation)> foodPickupCounterQueue = new();
    private readonly Dictionary<CookwareType, LinkedList<MainLoopWorkContext>> orderQueue = new();


    private readonly InteractableObjectManager<Table> tableManager = new();
    private LinkedList<Consumer> waitingConsumerQueue = new();
    
    public TrayReturnCounter TrayReturnCounter => trayReturnCounter;
    public SinkingStation SinkingStation => sinkingStation;

    public void Init(GameManager gameManager, WorkManager workManager)
    {
        this.gameManager = gameManager;
        this.workManager = workManager;
        CreateFoodObjectPool();
        InitCookingStation();
        InitEventListeners();
    }

    private void CreateFoodObjectPool()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(FoodObjectName);
        handle.WaitForCompletion();
        GameManager gameManager = ServiceLocator.Instance.GetSceneService<GameManager>();
        var foodGameObject = handle.Result;
        var foodObject = foodGameObject.GetComponent<FoodObject>();
        gameManager.ObjectPoolManager.CreatePool(foodObject, onRelease: ResetFoodAppearance);
    }

    private void InitCookingStation()
    {
        var table = DataTableManager.Get<FoodDataTable>(DataTableIds.Food.ToString());
        var kvpList = table.GetSceneFoodDataList(gameManager.CurrentTheme);
        var cookwareTypeList = kvpList.Select(pair => pair.Value.CookwareType).Distinct().ToList();
        foreach (var cookwareType in cookwareTypeList)
        {
            var cookingStationManager = new InteractableObjectManager<CookingStation>();
            cookingStationManagers.Add(cookwareType, cookingStationManager);
            var list = new LinkedList<MainLoopWorkContext>();
            orderQueue.Add(cookwareType, list);
        }
    }


    private void InitEventListeners()
    {
        tableManager.ObjectAvailableEvent += OnTableVacated;
        foodPickupCounterManager.ObjectAvailableEvent += OnFoodPickupCounterVacated;
        foreach (var manager in cookingStationManagers)
        {
            manager.Value.ObjectAvailableEvent += OnCookingStationVacated;
        }
    }

    private void ResetFoodAppearance(FoodObject foodObject)
    {
        foodObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void AddTable(Table table)
    {
        tableManager.InsertObject(table);
    }

    public void AddCookingStation(CookingStation station)
    {
        var cookwareType = station.cookwareType;
        cookingStationManagers[cookwareType].InsertObject(station);
    }

    public void AddFoodPickupCounter(FoodPickupCounter counter)
    {
        foodPickupCounterManager.InsertObject(counter);
    }

    public void SetCashierCounter(CashierCounter counter)
    {
        cashierCounter = counter;
    }

    public void SetTrayReturnCounter(TrayReturnCounter counter)
    {
        trayReturnCounter = counter;
    }

    public void SetSinkingStation(SinkingStation station)
    {
        sinkingStation = station;
        sinkingStation.OnSinkVacated += OnSinkVacated;
    }
    
    #region TableCleanLogic()

    public void OnEatComplete(Table table)
    {
        CreateCleanTableWork(table);
        CreateDirtyOnTable(table);
    }
    private void CreateCleanTableWork(Table table)
    {
        var work = new WorkCleanTable(workManager, WorkType.Hall);
        work.SetInteractable(table);
        work.SetContext(this);
        table.SetWork(work);
        table.FoodToTray();
        workManager.AddWork(work);
    }

    private static void CreateDirtyOnTable(Table table)
    {
        var food = table.GetFood();
        var sprite = food.GetComponent<SpriteRenderer>();
        sprite.color = Color.red;
    }


    #endregion

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

        waitingConsumerQueue.AddLast(consumer);
        return false;
    }

    private void OnTableVacated(Table availableTable)
    {
        if (waitingConsumerQueue.Count > 0)
        {
            var consumer = waitingConsumerQueue.First();
            waitingConsumerQueue.RemoveFirst();
            consumer.SetTable(tableManager.GetAvailableObject());
            consumer.OnTableVacated();
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
        workManager.AddWork(work, consumer);
    }

    public void CancelOrder(Consumer consumer)
    {
        workManager.OnWorkCanceled(consumer);
        RemoveConsumerFromQueues(consumer);
    }

    private void RemoveConsumerFromQueues(Consumer consumer)
    {
        var node1 = foodPickupCounterQueue.FirstOrDefault(x => x.Item1.Consumer == consumer);
        if (node1 != default)
            foodPickupCounterQueue.Remove(node1);
        foreach (var pair in orderQueue)
        {
            var node2 = pair.Value.FirstOrDefault(x => x.Consumer == consumer);
            if (node2 != null)
            {
                pair.Value.Remove(node2);
                break;
            }
        }
        var node3 = waitingConsumerQueue.FirstOrDefault(x => x == consumer);
        if (node3 != null)
            waitingConsumerQueue.Remove(node3);
    }

    public void ReturnTable(Table table)
    {
        tableManager.ReturnObject(table);
    }

    #endregion

    #region CookingLogic

    public void RegisterOrder(MainLoopWorkContext context)
    {
        var food = context.Consumer.needFood;
        bool isCookingStationAvailable = cookingStationManagers[food.CookwareType].IsAvailableObjectExist;
        bool isSinkFull = sinkingStation.IsSinkFull;
        if (isCookingStationAvailable && !isSinkFull)
            AssignOrderWork(context);
        else
            orderQueue[food.CookwareType].AddLast(context);
    }

    private void OnCookingStationVacated(CookingStation availableStation)
    {
        if (orderQueue[availableStation.cookwareType].Count > 0)
        {
            if (sinkingStation.IsSinkFull)
            {
                return;
            }
            var context = orderQueue[availableStation.cookwareType].First();
            orderQueue[availableStation.cookwareType].RemoveFirst();
            AssignOrderWork(context);
        }
    }

    private void AssignOrderWork(MainLoopWorkContext context)
    {
        var food = context.Consumer.needFood;
        var cookingStation = cookingStationManagers[food.CookwareType].GetAvailableObject();
        var work = new WorkCooking(workManager, WorkType.Kitchen);
        cookingStation.SetWork(work);
        work.SetContext(context);
        work.SetInteractable(cookingStation);
        workManager.AddWork(work, context.Consumer);
    }

    public void ReturnCookingStation(CookingStation station)
    {
        var cookwareType = station.cookwareType;
        cookingStationManagers[cookwareType].ReturnObject(station);
    }

    public void OnSinkVacated()
    {
        foreach (var pair in cookingStationManagers)
        {
            while (orderQueue[pair.Key].Count > 0)
            {
                if (!pair.Value.IsAvailableObjectExist)
                {
                    break;
                }
                var context = orderQueue[pair.Key].First();
                orderQueue[pair.Key].RemoveFirst();
                AssignOrderWork(context);
            }
        }
    }
    
    #endregion

    #region FoodPickupLogic

    public bool IsFoodCounterAvailable()
    {
        return foodPickupCounterManager.IsAvailableObjectExist;
    }

    public void RegisterFoodToHall(MainLoopWorkContext context, CookingStation target)
    {
        if (!IsFoodCounterAvailable())
        {
            foodPickupCounterQueue.AddLast((context, target));
        }
        else
        {
            var counter = GetEmptyFoodCounter();
            var work = new WorkGotoCookingStation(workManager, WorkType.Kitchen);
            target.SetWork(work);
            work.SetInteractable(target);
            work.SetContext(context, counter);
            workManager.AddWork(work, context.Consumer);
        }
    }

    public FoodPickupCounter GetEmptyFoodCounter()
    {
        return foodPickupCounterManager.Dequeue();
    }

    private void OnFoodPickupCounterVacated(FoodPickupCounter availableCounter)
    {
        if (foodPickupCounterQueue.Count > 0)
        {
            var counter = GetEmptyFoodCounter();
            var (context, target) = foodPickupCounterQueue.First();
            foodPickupCounterQueue.RemoveFirst();
            AddDeliverFoodWork(target, context, counter);
            
        }
    }
    private void AddDeliverFoodWork(InteractableObjectBase target, MainLoopWorkContext context, FoodPickupCounter counter)
    {
        var work = new WorkGotoCookingStation(workManager, WorkType.Kitchen);
        target.SetWork(work);
        work.SetInteractable(target);
        work.SetContext(context, counter);
        workManager.AddWork(work, context.Consumer);
    }
    
    public void ReturnFoodPickupCounter(FoodPickupCounter counter)
    {
        foodPickupCounterManager.ReturnObject(counter);
    }

    #endregion

    #region CashierLogic

    public int AssignCashier(Consumer consumer)
    {
        cashierQueue.AddLast(consumer);
        if (cashierQueue.Count > 1) return cashierQueue.Count - 1;

        consumer.NextTargetTransform =
            cashierCounter.GetInteractablePoints(InteractPermission.Consumer).First().transform;
        consumer.FSM.OnStartPayment();
        return 0;
    }

    public int OnCashierFinished()
    {
        var firstConsumer = cashierQueue.First();
        cashierQueue.RemoveFirst();
        firstConsumer.FSM.OnEndPayment();

        if (cashierQueue.Count == 0)
            return 0;
        var nextConsumer = cashierQueue.First();
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