using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Mission
{
    private int id;
    private int targetId;
    private int count;
    public int ID => id;
    public int Count => count;
    public int goalCount;
    public void Init(int goal, int id, int targetId = 0)
    {
        this.goalCount = goal;
        this.id = id;
        this.targetId = targetId;
    }
    public bool OnEventInvoked(int args = 1, int targetId = 0)
    {
        if (targetId != this.targetId)
        {
            return false;
        }
        count += args;
        bool isCleared = goalCount <= count;
        SavePrgoress(false,this).Forget();
        return goalCount <= count;
    }
    
    public static async UniTask SavePrgoress(bool isCleared, Mission mission)
    {
        MissionSaveData data = new MissionSaveData();
        data.id = mission.ID;
        data.count = mission.Count;
        data.isCleared = isCleared;
        await MissionSaveDataDAC.UpdateMissionSaveData(data);
    }

    public void SetCount(int count)
    {
        this.count = count;
    }
}
