using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorkFlowController : MonoBehaviour
{
    [SerializeField] private WorkManager workManager;

    private InteractableObjectManager<Table> tableManager = new();
    private InteractableObjectManager<CookingStation> cookingStationManager = new();
    private InteractableObjectManager<FoodPickupCounter> foodPickupCounterManager = new();

    private Queue<Consumer> customerQueue = new();
    private Queue<MainLoopWorkContext> orderQueue = new();
    private Queue<MainLoopWorkContext> foodPickupCounterQueue = new();


#if UNITY_EDITOR
    public Table tempTable;
    public CookingStation tempStation;
    public FoodPickupCounter tempCounter;
#endif

    private void Awake()
    {
        tableManager.Enqueue(tempTable);
        cookingStationManager.Enqueue(tempStation);
        foodPickupCounterManager.Enqueue(tempCounter);
        customerQueue = new Queue<Consumer>();
    }

    private void Start()
    {
        tableManager.ObjectAvailableEvent += OnTableVacated;
        cookingStationManager.ObjectAvailableEvent += OnCookingStationVacated;
        foodPickupCounterManager.ObjectAvailableEvent += OnCookingStationVacated;
    }

    #region CustomerLogic
    
    public bool RegisterCustomer(Consumer consumer)
    {
        if (tableManager.IsAvailableObjectExist)
        {
            consumer.SetTable(tableManager.GetAvailableObject());
            AssignGetOrderWork(consumer);
            return true;
        }
        else
        {
            customerQueue.Enqueue(consumer);
            return false;
        }
    }

    private void OnTableVacated()
    {
        if (customerQueue.Count > 0)
        {
            var consumer = customerQueue.Dequeue();
            //TODO:Assign이 아니라 손님을 이동
            AssignGetOrderWork(consumer);
        }
    }

    public void AssignGetOrderWork(Consumer consumer)
    {
        var table = consumer.currentTable; //Customer의 테이블 받아오기
        WorkGetOrder work = new WorkGetOrder(workManager, WorkType.Hall);
        MainLoopWorkContext context = new MainLoopWorkContext(consumer, this);
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
        {
            AssignOrderWork(context);
        }
        else
        {
            orderQueue.Enqueue(context);
        }
    }

    private void OnCookingStationVacated()
    {
        if (orderQueue.Count > 0)
        {
            MainLoopWorkContext context = orderQueue.Dequeue();
            AssignOrderWork(context);
        }
    }

    private void AssignOrderWork(MainLoopWorkContext context)
    {
        var cookingStation = cookingStationManager.GetAvailableObject();
        WorkCooking work = new WorkCooking(workManager, WorkType.Kitchen);
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

    public void RegisterFoodToHall(MainLoopWorkContext context)
    {
        foodPickupCounterQueue.Enqueue(context);
    }

    public FoodPickupCounter GetEmptyFoodCounter()
    {
        return foodPickupCounterManager.Dequeue();
    }

    private void OnFoodPickupCounterVacated()
    {
        if (foodPickupCounterQueue.Count > 0)
        {
            MainLoopWorkContext context = foodPickupCounterQueue.Dequeue();
            
        }
    }
    public void ReturnFoodPickupCounter(FoodPickupCounter counter)
    {
        foodPickupCounterManager.ReturnObject(counter);
    }
    
    
    #endregion
    
    //손님 대기열
    //Table에 자리가 나면 손님할당
    //주문 대기
    //화구에 자리가 생기면 음식 할당
    //음식 놓는곳도 생각해야함
}