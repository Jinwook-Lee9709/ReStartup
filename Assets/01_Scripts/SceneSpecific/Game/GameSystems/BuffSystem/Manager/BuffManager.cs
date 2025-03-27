using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [SerializeField] private Dictionary<BuffType, Buff> buffs = new();
    private void Start()
    {

    }

    private void StartBuff(Buff newBuff)
    {
        if (buffs.ContainsKey(newBuff.Type))
        {
            buffs[newBuff.Type] = newBuff;
        }
        else
        {
            buffs.Add(newBuff.Type, newBuff);
        }
    }

    public T GetBuff<T>(BuffType type) where T : Buff
    {
        if (!buffs.ContainsKey(type))
            return null;
        return buffs[type] as T;
    }

    [ContextMenu("TempBuffOn!!!")]
    public void TempBuffOn()
    {
        InfluencerBuff buff = new();
        buff.IsOnBuff = true;
        buff.RemainBuffTime = 5f;
        StartBuff(buff);
        Debug.Log("Buff On");
    }

    private void Update()
    {
        if (buffs.Count == 0)
            return;

        foreach (var buff in buffs.Values)
        {
            buff.RemainBuffTime -= Time.deltaTime;
            if (buff.RemainBuffTime < 0)
            {
                buff.IsOnBuff = false;
                buffs.Remove(buff.Type);
                Debug.Log("Buff Off");
                return;
            }
        }
    }
}
