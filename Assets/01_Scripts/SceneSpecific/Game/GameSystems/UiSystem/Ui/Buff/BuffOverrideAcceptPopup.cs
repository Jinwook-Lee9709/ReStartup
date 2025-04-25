using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffOverrideAcceptPopup : PopUp
{
    private Buff currentBuff, nextBuff;
    [SerializeField] private Button cancleButton, acceptButton;
    [SerializeField] private TextMeshProUGUI currentBuffText, nextBuffText;
    [SerializeField] private GameObject adPanel;
    public bool needAd = false;
    public void Init(Buff currentBuff, Buff nextBuff, Action callBack)
    {
        cancleButton.interactable = true;
        acceptButton.interactable = true;
        adPanel.SetActive(needAd);
        this.currentBuff = currentBuff;
        this.nextBuff = nextBuff;

        if (needAd)
        {
            acceptButton.onClick.AddListener(() =>
            {
                AdvertisementManager.Instance.ShowRewardedAd(() =>
                {
                    callBack.Invoke();
                    cancleButton.interactable = false;
                    acceptButton.interactable = false;
                    OnCancle();
                });
            });
        }
        else
        {
            acceptButton.onClick.AddListener(() =>
            {
                callBack.Invoke();
                cancleButton.interactable = false;
                acceptButton.interactable = false;
                OnCancle();
            });
        }
    }
    private void Update()
    {
        currentBuffText.text = $"{Mathf.Clamp(currentBuff.remainBuffTime, 0f,float.MaxValue)} / {currentBuff.buffName} / {currentBuff.buffDescription}";
        nextBuffText.text = $"{nextBuff.remainBuffTime} / {nextBuff.buffName} / {nextBuff.buffDescription}";
    }
    private void OnEnable()
    {
        cancleButton.onClick.RemoveAllListeners();
        acceptButton.onClick.RemoveAllListeners();
        cancleButton.onClick.AddListener(() =>
        {
            cancleButton.interactable = false;
            acceptButton.interactable = false;
            OnCancle();
        });
    }


}
