using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class Review : MonoBehaviour
{
    public ReviewData data;

    public ReviewManager reviewManager;
    [SerializeField] private Button removeButton;
    [SerializeField] private Image profileIcon;
    [SerializeField] private Image star;
    [SerializeField] private TextMeshProUGUI reviewScriptText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI rankPointText;
    [SerializeField] private TextMeshProUGUI reviewUserID;
 
    public event Func<UniTask> OnRemoveAdEvent;

    public void Init(ReviewData data)
    {
        this.data = data;
        var starhandler = Addressables.LoadAssetAsync<Sprite>(data.addPoint > 0 ? Strings.positiveReviewFileName : Strings.negativeReviewFileName);
        var iconhandler = Addressables.LoadAssetAsync<Sprite>(data.iconID);
        starhandler.WaitForCompletion();
        iconhandler.WaitForCompletion();
        star.sprite = starhandler.Result;
        profileIcon.sprite = iconhandler.Result;
        reviewScriptText.text = data.reviewMessage;
        dateText.text = data.date;
        rankPointText.text = $"{data.addPoint:▲0;▼0;0} 점";
        reviewUserID.text = data.userID;

        removeButton.gameObject.SetActive(data.addPoint < 0);
        if(removeButton.enabled )
        {
            removeButton.onClick.AddListener(() => reviewManager.OnRemoveButtonClick(this));
        }

        OnRemoveAdEvent += async () =>
        {
            
            int count = 0;
            foreach (var review in reviewManager.reviews)
            {
                if (review == gameObject)
                {
                    await ReviewSaveDataDAC.DeleteReviewData(count);
                    break;
                }
                count++;
            }

            reviewManager.RemoveAt(gameObject);
            UserDataManager.Instance.AddRankPointWithSave(-data.addPoint).Forget();
            UserDataManager.Instance.OnNegativeReviewRemove();
            ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.BadReviewDelete, 1);
        };
    }

    public void Remove()
    {
        AdvertisementManager.Instance.ShowRewardedAd(OnRemoveAdEvent);
    }

}
