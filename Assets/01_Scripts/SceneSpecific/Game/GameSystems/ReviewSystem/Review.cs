using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Review : MonoBehaviour
{
    public ReviewData data;

    public ReviewManager reviewManager;

    [SerializeField] private Button removeButton;
    [SerializeField] private TextMeshProUGUI starText;
    [SerializeField] private TextMeshProUGUI reviewScriptText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI rankPointText;

    public event Action OnRemoveAdEvent;

    public void Init(ReviewData data)
    {
        this.data = data;
        starText.text = $"별점 : {data.stars}점";
        reviewScriptText.text = data.reviewMessage;
        dateText.text = data.date;
        rankPointText.text = $"{data.addPoint:▲0;▼0;0} 점";

        removeButton.gameObject.SetActive(data.addPoint < 0);
        if(removeButton.enabled )
        {
            removeButton.onClick.AddListener(() => reviewManager.OnRemoveButtonClick(this));
        }

        OnRemoveAdEvent += () =>
        {
            reviewManager.RemoveAt(gameObject);
            UserDataManager.Instance.AddRankPointWithSave(-data.addPoint).Forget();
        };
    }

    public void Remove()
    {
        AdvertisementManager.Instance.ShowRewardedAd(OnRemoveAdEvent);
    }

}
