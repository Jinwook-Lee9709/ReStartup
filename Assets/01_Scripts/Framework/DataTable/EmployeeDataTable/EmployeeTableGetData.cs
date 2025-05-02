using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class EmployeeTableGetData
{
    private float upgradeCostValue = 1.5f;
    public float upgradeMoveSpeed = 0.5f;
    public float upgradeWorkSpeed = 0.4f;
    public int upgradeHealth = 10;
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
    public float defaultWorkSpeed;
    public float defaultMoveSpeed;
    public int defaultHealth;
    public float defaultCost;
    public float defaultRankPoint;

    public void OnUpgrade()
    {
        OnUpgradeEvent?.Invoke();
    }
    public void StartValueSet()
    {
        defaultWorkSpeed = WorkSpeed;
        defaultMoveSpeed = MoveSpeed;
        defaultHealth = Health;
        defaultCost = Cost;
        defaultRankPoint = RankPoint;
    }
    public void UpdateUpgradeStats(int upgradeCount)
    {
        WorkSpeed = defaultWorkSpeed - (upgradeCount * upgradeWorkSpeed);
        MoveSpeed = defaultMoveSpeed + (upgradeCount * upgradeMoveSpeed);
        Health = defaultHealth + (upgradeCount * upgradeHealth);
        float upgradeSquareValue = upgradeCostValue;
        for (int i = 1; i < upgradeCount; i++)
        {
            upgradeSquareValue = upgradeSquareValue * upgradeCostValue;
        }
        Cost = (int)(defaultCost * upgradeSquareValue);
        RankPoint = (int)(defaultRankPoint * upgradeSquareValue);
    }
}