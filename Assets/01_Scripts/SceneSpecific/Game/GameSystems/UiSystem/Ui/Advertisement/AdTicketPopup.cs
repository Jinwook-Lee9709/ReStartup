using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdTicketPopup : PopUp
{
    private readonly string cntFormat = "남은 티켓 갯수 : {0}";
    [SerializeField] private TextMeshProUGUI adTicketCheckText, adTicketCnt;
    [SerializeField] private Button acceptButton, cancelButton;

    public void Init(Func<UniTask> adCallback, Func<UniTask> afterEvent = null)
    {
        var ticketCnt = UserDataManager.Instance.CurrentUserData.AdTicket;
        adTicketCnt.text = string.Format(cntFormat, ticketCnt < 999 ? ticketCnt : "+999");
        acceptButton.onClick.AddListener(() =>
        {
            UniTask.Void(async () =>
            {
                await adCallback();
            });
            if (afterEvent != null)
                afterEvent();

            UserDataManager.Instance.AdjustAdTicketWithSave(-1).Forget();
            OnCancle();
        });
        cancelButton.onClick.AddListener(() => {
            AdvertisementManager.Instance.ShowRewardedAdDirect(adCallback, afterEvent);
            OnCancle();
            });
    }
}
