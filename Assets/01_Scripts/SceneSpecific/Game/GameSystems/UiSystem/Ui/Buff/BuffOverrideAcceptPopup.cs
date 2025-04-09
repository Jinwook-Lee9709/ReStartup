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
                    OnCancle();
                });
            });
        }
        else
        {
            acceptButton.onClick.AddListener(() =>
            {
                callBack.Invoke();
                OnCancle();
            });
        }
    }
    private void Update()
    {
        currentBuffText.text = $"{currentBuff.remainBuffTime} / {currentBuff.BuffType.ToString()} / {currentBuff.BuffEffect}";
        nextBuffText.text = $"{nextBuff.remainBuffTime} / {nextBuff.BuffType.ToString()} / {nextBuff.BuffEffect}";
    }
    private void OnEnable()
    {
        cancleButton.onClick.RemoveAllListeners();
        acceptButton.onClick.RemoveAllListeners();
        cancleButton.onClick.AddListener(OnCancle);
    }


}
