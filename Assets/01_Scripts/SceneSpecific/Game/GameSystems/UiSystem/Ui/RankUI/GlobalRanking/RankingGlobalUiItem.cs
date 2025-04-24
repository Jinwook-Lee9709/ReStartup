using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RankingGlobalUiItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI restaurantNameText;
    [SerializeField] private TextMeshProUGUI rankPointText;

    private string uuid;
    
    
    public void SetInfo(int rank, string userName, string restaurantName, int rankPoint, string uuid)
    {
        if (string.IsNullOrEmpty(this.uuid))
        {
            this.uuid = uuid;
            SetText(rank, userName, restaurantName, rankPoint);
            return;
        }
        if(this.uuid == uuid)
        {
            return;
        }
        this.uuid = uuid;
        SetTextWithAnimation(rank, userName, restaurantName, rankPoint).Forget();

    }

    private void SetText(int rank, string userName, string restaurantName, int rankPoint)
    {
        rankText.text = rank.ToString();
        userNameText.text = userName;
        restaurantNameText.text = restaurantName;
        rankPointText.text = rankPoint.ToString();
    }
    
    private UniTask SetTextWithAnimation( int rank, string userName, string restaurantName, int rankPoint)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DORotate(new Vector3(180, 0, 0), 0.4f).SetEase(Ease.InOutElastic));
        sequence.AppendCallback(()=>SetText(rank, userName, restaurantName, rankPoint));
        sequence.Append(transform.DORotate(new Vector3(0, 0, 0), 0.4f).SetEase(Ease.InOutElastic));
        return UniTask.CompletedTask;
    }
}
