using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//주문할 음식이 뭔지
//현재 Table

//나 자리에 앉았어요 -> ConsumerManager -> AssignGetOrderWork(consumer)

public class WorkFlowController : MonoBehaviour
{
    [SerializeField] private WorkManager workManager;

    private InteractableObjectManager<Table> tableManager;
    private InteractableObjectManager<CookingStation> cookingStationManager;

    private Queue<int> customerQueue;
    private Queue<int> orderQueue;
    
    private void Start()
    {
        tableManager.ObjectAvailableEvent += OnTableVacated;
        cookingStationManager.ObjectAvailableEvent += OnCookingStationVacated;
    }

    #region CustomerLogic
    
    public void RegisterCustomer(int customerId)
    {
        if (tableManager.IsAvailableObjectExist)
        {
            //TODO:Assign이 아니라 손님을 이동
            //consumer.Table = tableManager.GetAvailableObject();
            //consuMer.SetTable(tableManager.GetAvailableObject());
            
            AssignGetOrderWork(customerId);
        }
        else
        {
            customerQueue.Enqueue(customerId);
        }
    }

    private void OnTableVacated()
    {
        if (customerQueue.Count > 0)
        {
            int customerId = customerQueue.Dequeue();
            //TODO:Assign이 아니라 손님을 이동
            AssignGetOrderWork(customerId);
        }
    }

    public void AssignGetOrderWork(int customerId)
    {
        //TODO:손님이 할당 받은 TABLED을 Interactable로 사용하도록 변경
        var table = tableManager.GetAvailableObject(); //Customer의 테이블 받아오기
        WorkGetOrder work = new WorkGetOrder(workManager, WorkType.Hall, 0);
        work.SetInteractable(table);
        workManager.AddWork(work);
    }

    #endregion

    #region CookingLogic

    public void RegisterOrder(int orderId)
    {
        if (cookingStationManager.IsAvailableObjectExist)
        {
            AssignOrderWork(orderId);
        }
        else
        {
            orderQueue.Enqueue(orderId);
        }
    }

    private void OnCookingStationVacated()
    {
        if (orderQueue.Count > 0)
        {
            int orderId = orderQueue.Dequeue();
            AssignOrderWork(orderId);
        }
    }

    private void AssignOrderWork(int orderId)
    {
        var cookingStation = cookingStationManager.GetAvailableObject();
        WorkCooking work = new WorkCooking(workManager, WorkType.Kitchen, 5);
        work.SetInteractable(cookingStation);
        workManager.AddWork(work);
    }
    
    #endregion
    
    
    //손님 대기열
    //Table에 자리가 나면 손님할당
    //주문 대기
    //화구에 자리가 생기면 음식 할당
    //음식 놓는곳도 생각해야함
}