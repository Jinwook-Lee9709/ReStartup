using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantSuperviseManager : MonoBehaviour
{
    [SerializeField] RestaurantSuperviseUIManager restaurantSuperviseUIManager;
    private void Start()
    {
        restaurantSuperviseUIManager.InitRestaurantListPanel(OnThemeChanged);
        restaurantSuperviseUIManager.InitSupervisorListPanel(OnHireEmployee);
    }

    private void OnThemeChanged()
    {
        
    }

    private void OnHireEmployee(int theme, int number)
    {
        
    }
}
