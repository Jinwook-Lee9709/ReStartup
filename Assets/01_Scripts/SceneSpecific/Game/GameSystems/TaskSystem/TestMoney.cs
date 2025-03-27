using UnityEngine;

public static class TestMoney
{
    public static int money;

    public static void GetMoney(int moneyEarned)
    {
        money = moneyEarned;
        Debug.Log($"{money} = {money} + {moneyEarned}");
    }
}