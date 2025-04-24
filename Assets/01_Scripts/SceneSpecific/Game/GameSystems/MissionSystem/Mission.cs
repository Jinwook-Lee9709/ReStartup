using System.Collections;
using System.Collections.Generic;
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
        return goalCount <= count;
    }
}
