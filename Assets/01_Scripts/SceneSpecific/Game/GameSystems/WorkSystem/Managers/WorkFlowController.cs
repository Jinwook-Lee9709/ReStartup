using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = System.Object;
using Random = UnityEngine.Random;

public class WorkFlowController
{
    private static readonly string FoodObjectName = "FoodObject";
    private LinkedList<Consumer> cashierQueue = new();
    private Dictionary<CookwareType, InteractableObjectManager<CookingStation>> cookingStationManagers = new();
    private InteractableObjectManager<FoodPickupCounter> foodPickupCounterManager = new();
    private LinkedList<(MainLoopWorkContext, CookingStation)> foodPickupCounterQueue = new();
    private Dictionary<CookwareType, LinkedList<MainLoopWorkContext>> orderQueue = new();

    private Dictionary<ObjectArea, TrashCan> trashCanDictionary = new();
    private int isTrashExist = 0;

    private InteractableObjectManager<Table> tableManager = new();
    private CashierCounter cashierCounter;
    private GameManager gameManager;
    private LinkedList<Consumer> waitingConsumerQueue = new();
    private WorkManager workManager;

    public TrayReturnCounter TrayReturnCounter { get; private set; }

    public SinkingStation SinkingStation { get; private set; }

    public event Action<CookwareType> OnCookingStationAdded;

    private int foodSellStack = 0;
    
    
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
        var gameManager = ServiceLocator.Instance.GetSceneService<GameManager>();
        var foodGameObject = handle.Result;
        var foodObject = foodGameObject.GetComponent<FoodObject>();
        gameManager.ObjectPoolManager.CreatePool(foodObject, onRelease: ResetFoodAppearance);
    }

    private void InitCookingStation()
    {
        var table = DataTableManager.Get<CookwareDataTable>(DataTableIds.Cookware.ToString());
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
        foreach (var manager in cookingStationManagers) manager.Value.ObjectAvailableEvent += OnCookingStationVacated;
    }

    private void ResetFoodAppearance(FoodObject foodObject)
    {
    }

    public void AddTable(Table table)
    {
        tableManager.InsertObject(table);
    }

    public void AddCookingStation(CookingStation station)
    {
        var cookwareType = station.cookwareType;
        cookingStationManagers[cookwareType].InsertObject(station);
        UserDataManager.Instance.CurrentUserData.CookWareUnlock[gameManager.CurrentTheme][cookwareType]++;
        OnCookingStationAdded?.Invoke(cookwareType);
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
        TrayReturnCounter = counter;
    }

    public void SetSinkingStation(SinkingStation station)
    {
        SinkingStation = station;
        SinkingStation.OnSinkVacated += OnSinkVacated;
    }

    #region TableCleanLogic()

    public void OnEatComplete(Table table, bool isPair = false)
    {
        CreateCleanTableWork(table, isPair);
    }

    public void CreateCleanTableWork(Table table, bool isPair = false)
    {
        var work = new WorkCleanTable(workManager, WorkType.Hall);
        work.SetInteractable(table);
        work.SetContext(this, isPair);
        table.SetWork(work);

        workManager.AddWork(work);
    }

    public void CreateDirtyOnTable(Table table)
    {
        table.FoodToTray();
    }

    #endregion

    #region CustomerLogic

    public bool RegisterCustomer(Consumer consumer)
    {
        if (tableManager.IsAvailableObjectExist)
        {
            var table = tableManager.GetAvailableObject();
            consumer.SetTable(table);
            return true;
        }

        waitingConsumerQueue.AddLast(consumer);
        return false;
    }

    private void OnTableVacated(Table availableTable)
    {
        if (waitingConsumerQueue.Count > 0)
        {
            var table = tableManager.GetAvailableObject();
            var consumer = waitingConsumerQueue.First();
            waitingConsumerQueue.RemoveFirst();
            consumer.SetTable(table);
            consumer.OnTableVacated();
        }
    }

    public void AssignGetOrderWork(Consumer consumer)
    {
        var table = consumer.currentTable;
        if (consumer.pairData != null)
        {
            if (consumer.pairData.owner != consumer)
                return;
            var work = new WorkGetPairOrder(workManager, WorkType.Hall);
            var firstContext = new MainLoopWorkContext(consumer, this);
            var secondContext = new MainLoopWorkContext(consumer.pairData.partner, this);
            table.SetWork(work);
            work.SetContext(firstContext, secondContext);
            work.SetInteractable(table);
            workManager.AddWork(work, consumer);
        }
        else
        {
            var work = new WorkGetOrder(workManager, WorkType.Hall);
            var context = new MainLoopWorkContext(consumer, this);
            table.SetWork(work);
            work.SetContext(context);
            work.SetInteractable(table);
            workManager.AddWork(work, consumer);
        }
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
        var isCookingStationAvailable = cookingStationManagers[food.CookwareType].IsAvailableObjectExist;
        var isSinkFull = SinkingStation.IsSinkFull;
        if (isCookingStationAvailable && !isSinkFull)
            AssignOrderWork(context);
        else
            orderQueue[food.CookwareType].AddLast(context);
    }

    private void OnCookingStationVacated(CookingStation availableStation)
    {
        if (orderQueue[availableStation.cookwareType].Count > 0)
        {
            if (SinkingStation.IsSinkFull) return;
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
            while (orderQueue[pair.Key].Count > 0)
            {
                if (!pair.Value.IsAvailableObjectExist) break;
                var context = orderQueue[pair.Key].First();
                orderQueue[pair.Key].RemoveFirst();
                AssignOrderWork(context);
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

    private void AddDeliverFoodWork(InteractableObjectBase target, MainLoopWorkContext context,
        FoodPickupCounter counter)
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
        IncreaseSellStack();
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

    public LinkedList<Consumer> GetCashierQueue()
    {
        return cashierQueue;
    }

    #endregion

    #region TrashCanLogic
    
    private void IncreaseSellStack()
    {
        foodSellStack++;
        if (foodSellStack >= Constants.TRASH_CREATE_STACK)
        {
            HandleTrashCreation();
            foodSellStack = 0;
        }
    }

    private void HandleTrashCreation()
    {
        switch (isTrashExist)
        {
            case 0b00:
            {
                if (Random.value < 0.5f)
                    CreateTrash(ObjectArea.Hall);
                else
                    CreateTrash(ObjectArea.Kitchen);
                UserDataManager.Instance.negativeReviewProbability = 0.65f;
                break;
            }
            case 0b01:
                CreateTrash(ObjectArea.Hall);
                UserDataManager.Instance.negativeReviewProbability = 0.7f;
                break;
            case 0b10:
                CreateTrash(ObjectArea.Kitchen);
                UserDataManager.Instance.negativeReviewProbability = 0.7f;
                break;
            case 0b11:
                break;
        }
    }

    public void RegisterTrashCan(ObjectArea area, TrashCan trashCan)
    {
        trashCanDictionary.TryAdd(area, trashCan);
    }
    
    public void CreateTrash(ObjectArea area)
    {
        if (area == ObjectArea.Hall)
        {
            isTrashExist |= 0b10;
        }
        else
        {
            isTrashExist |= 0b01;
        }
        var work = new WorkCleanTrashCan(workManager, area == ObjectArea.Hall ? WorkType.Hall : WorkType.Kitchen, 0);
        work.SetContext(this, area);
        work.SetInteractable(trashCanDictionary[area]);
        work.OnWorkRegistered();
        trashCanDictionary[area].SetWork(work);
    }

    public void OnCleanTrash(ObjectArea area)
    {
        if (area == ObjectArea.Hall)
        {
            isTrashExist &= 0b01;
        }
        else
        {
            isTrashExist &= 0b10;
        }

        if (isTrashExist == 0)
        {
            UserDataManager.Instance.negativeReviewProbability = 0.6f;
        }
        else
        {
            UserDataManager.Instance.negativeReviewProbability = 0.65f;
        }

        UserDataManager.Instance.AddRankPointWithSave(Constants.TRASH_CLEAN_BONUS);
    }

    #endregion

    //손님 대기열
    //Table에 자리가 나면 손님할당
    //주문 대기
    //화구에 자리가 생기면 음식 할당
    //음식 놓는곳도 생각해야함
}