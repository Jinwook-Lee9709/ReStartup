using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class EmployeeTableGetData
{
    public int upgradeCount = 0;
    public float upgradeSpeed = 0.5f;
    public int StaffID { get; set; }
    public int StaffNameKey { get; set; }
    public int Description { get; set; }
    public int Theme { get; set; }
    public int StaffType { get; set; }
    public float WorkSpeed { get; set; }
    public float MoveSpeed { get; set; }
    public int Health { get; set; }
    public int RankPoint { get; set; }
    public int Cost { get; set; }
    public string Icon { get; set; }
    public string Resouces { get; set; }

    public event Action OnUpgradeEvent;
    public int currentHealth;

    public void OnUpgrade()
    {
        OnUpgradeEvent?.Invoke();
    }
}