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
        restaurantListPanel.Init(action, this);
    }
    
    public void InitSupervisorListPanel(Action<int, int> action)
    {
        supervisorListPanel.Init(action);
    }

    public void UpdateRestaurantButton()
    {
        restaurantListPanel.UpdateInteractable();
    }

    public void OnSupervisorHire()
    {
        supervisorListPanel.OnSupervisorHire();
    }

    [VInspector.Button]
    public void ChangeSupervisorList(int themeID)
    {
        supervisorListPanel.ChangeSupervisor(themeID);
    }

}
