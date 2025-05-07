using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdTicketPopup : PopUp
{
    private readonly string cntFormat = ": {0}";
    [SerializeField] private TextMeshProUGUI adTicketCheckText, adTicketCnt;
    [SerializeField] private Button acceptButton, cancelButton;

    public void Init(Func<UniTask> adCallback, Func<UniTask> afterEvent = null)
    {
        adTicketCheckText.text = "테스트 티켓 사용 텍스트";
        adTicketCnt.text = string.Format(cntFormat, UserDataManager.Instance.CurrentUserData.AdTicket);
        acceptButton.onClick.AddListener(() =>
        {
            UniTask.Void(async () =>
            {
                await adCallback();
            });
            if (afterEvent != null)
                afterEvent();

            UserDataManager.Instance.CurrentUserData.AdTicket--;
        });
        cancelButton.onClick.AddListener(() => AdvertisementManager.Instance.ShowRewardedAdDirect(adCallback, afterEvent));
    }
}
