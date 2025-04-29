using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantSuperviseUIManager : MonoBehaviour
{
    [SerializeField] RestaurantListPanel restaurantListPanel;
    [SerializeField] SupervisorListPanel supervisorListPanel;
    
    
    public void InitRestaurantListPanel(Action action)
    {
        restaurantListPanel.Init(action);
    }
    
    public void InitSupervisorListPanel(Action<int, int> action)
    {
        supervisorListPanel.Init(action);
    }



}
