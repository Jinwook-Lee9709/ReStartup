using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffManager : MonoBehaviour
{
    private readonly Dictionary<BuffType, Buff> buffs = new();

    [SerializeField] private Button buffInfoButton, buffInfoBackground;
    [SerializeField] private GameObject popupParent, buffPopupPrefab, buffInfoPanel, buffInfoPrefabParent, buffInfoPrefab;
    [SerializeField] private TextMeshProUGUI buffCountText;
    public List<BuffInfoUI> buffInfoUIList = new();

    public event Action<Buff> OnBuffUsed;
    public event Action<Buff> OnBuffExpired;

    private void Start()
    {
        buffInfoButton.onClick.RemoveAllListeners();
        buffInfoBackground.onClick.RemoveAllListeners();

        buffInfoButton.onClick.AddListener(OnBuffInfoButtonClick);
        buffInfoBackground.onClick.AddListener(OnBuffInfoCancleButton);
        foreach (var data in UserDataManager.Instance.CurrentUserData.BuffSaveData.Values)
        {
            StartBuff(DataTableManager.Get<BuffDataTable>(DataTableIds.Buff.ToString()).GetBuffForBuffID(data.id), data.remainTime);
        }
    }

    public void OnBuffInfoCancleButton()
    {
        buffInfoPanel.SetActive(false);
        buffInfoBackground.gameObject.SetActive(false);
    }

    private void OnBuffInfoButtonClick()
    {
        buffInfoPanel.SetActive(true);
        buffInfoBackground.gameObject.SetActive(true);
    }
    private void UpdateBuffInfoUIList()
    {
        buffInfoUIList.Sort();
        for (int i = 0; i < buffInfoUIList.Count; i++)
        {
            buffInfoUIList[i].transform.SetSiblingIndex(i);
        }
    }
    public void UpdateBuffCntUI()
    {
        buffCountText.text = string.Format(Strings.buffCountFormat, buffInfoUIList.Count);
    }
    private void Update()
    {
        buffInfoButton.interactable = buffs.Count != 0;
        UpdateBuffCntUI();
        if (buffs.Count == 0)
        {
            return;
        }
        foreach (var buff in buffs.Values)
        {
            float prev = buff.remainBuffTime;
            buff.remainBuffTime -= Time.deltaTime;
            int currentInterval = (int)(buff.remainBuffTime / Constants.BUFF_SAVE_INTERVAL);
            int previousInterval = (int)(prev / Constants.BUFF_SAVE_INTERVAL);
            if (currentInterval != previousInterval)
            {
                UserDataManager.Instance.SaveRemainBuffTime(buff, buff.remainBuffTime).Forget();
            }
            if (buff.remainBuffTime < 0)
            {
                UserDataManager.Instance.OnBuffExpired(buff).Forget();
                OnBuffExpired?.Invoke(buff);
                buff.isOnBuff = false;
                buffs.Remove(buff.BuffType);
                Debug.Log("Buff Off");
                return;
            }
        }

    }
    public void BuffOverridePopup(Buff newBuff, Func<UniTask> limitCountAction, bool needAd = false)
    {
        var buffPopup = Instantiate(buffPopupPrefab, popupParent.transform, false);
        buffPopup.GetComponent<BuffOverrideAcceptPopup>().needAd = needAd;
        buffPopup.GetComponent<BuffOverrideAcceptPopup>().Init(buffs[newBuff.BuffType], newBuff, async () =>
        {
            foreach (var buffUI in buffInfoUIList)
            {
                if (buffUI.currentBuff == buffs[newBuff.BuffType])
                {
                    Destroy(buffUI.gameObject);
                    buffInfoUIList.Remove(buffUI);
                    break;
                }
            }

            buffs[newBuff.BuffType] = newBuff;
            var buffInfoUI = Instantiate(buffInfoPrefab, buffInfoPrefabParent.transform, false).GetComponent<BuffInfoUI>();
            await buffInfoUI.Init(newBuff, this);
            buffInfoUIList.Add(buffInfoUI);
            UpdateBuffInfoUIList();
            if (limitCountAction != null)
                await limitCountAction();
        });
    }

    public async void StartBuff(Buff newBuff, Func<UniTask> limitCountAction = null, bool needAd = false)
    {
        if (ContainType(newBuff.BuffType))
        {
            if (ContainID(newBuff))
            {
                ExtendBuffTimer(newBuff,async () =>
                {
                    if (limitCountAction != null)
                        await limitCountAction();
                    UserDataManager.Instance.OnUseBuff(newBuff).Forget();
                }, needAd);
            }
            else
            {
                BuffOverridePopup(newBuff,async () =>
                {
                    if (limitCountAction != null)
                        await limitCountAction();
                    UserDataManager.Instance.OnUseBuff(newBuff).Forget();
                }, needAd);
            }
        }
        else
        {
            if (needAd)
            {
                AdvertisementManager.Instance.ShowRewardedAd(async () =>
                {
                    UserDataManager.Instance.OnUseBuff(newBuff).Forget();
                    buffs[newBuff.BuffType] = newBuff;
                    var buffInfoUI = Instantiate(buffInfoPrefab, buffInfoPrefabParent.transform, false).GetComponent<BuffInfoUI>();
                    await buffInfoUI.Init(newBuff, this);
                    buffInfoUIList.Add(buffInfoUI);
                    UpdateBuffInfoUIList();
                    if (limitCountAction != null)
                        await limitCountAction();
                });
            }
            else
            {
                UserDataManager.Instance.OnUseBuff(newBuff).Forget();
                buffs[newBuff.BuffType] = newBuff;
                var buffInfoUI = Instantiate(buffInfoPrefab, buffInfoPrefabParent.transform, false).GetComponent<BuffInfoUI>();
                await buffInfoUI.Init(newBuff, this);
                buffInfoUIList.Add(buffInfoUI);
                UpdateBuffInfoUIList();
                limitCountAction?.Invoke();
            }
        }

    }

    public async void StartBuff(Buff buff)
    {
        if (ContainType(buff.BuffType))
        {
            if (ContainID(buff))
            {
                UserDataManager.Instance.OnUseBuff(buff).Forget();
                OnBuffUsed?.Invoke(buff);
                buffs[buff.BuffType].remainBuffTime += buff.remainBuffTime;
                UpdateBuffInfoUIList();
                return;
            }
        }
        else
        {
            buff.Init();
            buffs[buff.BuffType] = buff;
            UserDataManager.Instance.OnUseBuff(buff).Forget();
            OnBuffUsed?.Invoke(buff);
        }

        var buffInfoUI = Instantiate(buffInfoPrefab, buffInfoPrefabParent.transform, false).GetComponent<BuffInfoUI>();
        await buffInfoUI.Init(buff, this);
        buffInfoUIList.Add(buffInfoUI);
        UpdateBuffInfoUIList();
    }

    public async void StartBuff(Buff buff, float remainTime)
    {
        buff.Init();
        buffs[buff.BuffType] = buff;

        OnBuffUsed?.Invoke(buff);

        buff.remainBuffTime = remainTime;
        var buffInfoUI = Instantiate(buffInfoPrefab, buffInfoPrefabParent.transform, false).GetComponent<BuffInfoUI>();
        await buffInfoUI.Init(buff, this);
        buffInfoUIList.Add(buffInfoUI);
        UpdateBuffInfoUIList();
    }

    public bool RemoveBuff(BuffType buffType)
    {
        if (buffs.ContainsKey(buffType))
        {
            return buffs.Remove(buffType);
        }
        return false;
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
    public void ExtendBuffTimer(Buff buff, Func<UniTask> limitCountAction, bool needAd)
    {
        if (needAd)
        {
            AdvertisementManager.Instance.ShowRewardedAd(async () =>
            {
                buffs[buff.BuffType].remainBuffTime += buff.remainBuffTime;
                UpdateBuffInfoUIList();
                if (limitCountAction != null)
                    await limitCountAction();
            });
        }
        else
        {
            buffs[buff.BuffType].remainBuffTime += buff.remainBuffTime;
            UpdateBuffInfoUIList();
            limitCountAction?.Invoke();
        }
    }
}