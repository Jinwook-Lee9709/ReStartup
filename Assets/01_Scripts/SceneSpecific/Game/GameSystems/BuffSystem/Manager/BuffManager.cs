using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    private readonly Dictionary<BuffType, Buff> buffs = new();
    [SerializeField] private GameObject buffPopupPrefab;
    [SerializeField] private GameObject popupParent;
    private void Start()
    {
    }

    private void Update()
    {
        if (buffs.Count == 0)
            return;

        foreach (var buff in buffs.Values)
        {
            buff.remainBuffTime -= Time.deltaTime;
            if (buff.remainBuffTime < 0)
            {
                buff.isOnBuff = false;
                buffs.Remove(buff.BuffType);
                Debug.Log("Buff Off");
                return;
            }
        }
    }
    public void BuffOverridePopup(Buff newBuff, Action limitCountAction, bool needAd = false)
    {
        var buffPopup = Instantiate(buffPopupPrefab, popupParent.transform, false);
        buffPopup.GetComponent<BuffOverrideAcceptPopup>().needAd = needAd;
        buffPopup.GetComponent<BuffOverrideAcceptPopup>().Init(buffs[newBuff.BuffType], newBuff, () =>
        {
            buffs[newBuff.BuffType] = newBuff;
            limitCountAction?.Invoke();
        });
    }

    public void StartBuff(Buff newBuff, Action limitCountAction = null, bool needAd = false)
    {
        if (ContainType(newBuff.BuffType))
        {
            if (ContainID(newBuff))
            {
                ExtendBuffTimer(newBuff, () => limitCountAction?.Invoke(), needAd);
            }
            else
            {
                BuffOverridePopup(newBuff, () => limitCountAction?.Invoke(), needAd);
            }
        }
        else
        {
            if (needAd)
            {
                AdvertisementManager.Instance.ShowRewardedAd(() =>
                {
                    buffs[newBuff.BuffType] = newBuff;
                    limitCountAction?.Invoke();
                });
            }
            else
            {
                buffs[newBuff.BuffType] = newBuff;
                limitCountAction?.Invoke();
            }
        }
    }

    public void StartBuff(Buff buff)
    {
        buffs[buff.BuffType] = buff;
    }

    public Buff GetBuff(BuffType type)
    {
        if (!buffs.ContainsKey(type))
            return null;
        return buffs[type];
    }

    public bool ContainType(BuffType type)
    {
        return buffs.ContainsKey(type);
    }
    public bool ContainID(Buff buff)
    {
        return buffs[buff.BuffType].BuffID == buff.BuffID;
    }
    public void ExtendBuffTimer(Buff buff, Action limitCountAction, bool needAd)
    {
        if (needAd)
        {
            AdvertisementManager.Instance.ShowRewardedAd(() =>
            {
                buffs[buff.BuffType].remainBuffTime += buff.remainBuffTime;
                limitCountAction.Invoke();
            });
        }
        else
        {
            buffs[buff.BuffType].remainBuffTime += buff.remainBuffTime;
            limitCountAction.Invoke();
        }
    }

    [ContextMenu("TempBuffOn!!!")]
    public void TempBuffOn()
    {
        var buffDatas = DataTableManager.Get<BuffDataTable>("Buff");
        var buff = buffDatas.GetBuffForBuffID(990100);
        buff.Init();
        StartBuff(buff);
        Debug.Log($"Buff On{buff.BuffType.ToString()}");
    }
}