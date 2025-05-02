using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RankingGlobalUiItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI rankPointText;

    private string uuid;
    private int rankPoint;
    
    
    public void SetInfo(int rank, string userName,int rankPoint, string uuid)
    {
        if (string.IsNullOrEmpty(this.uuid))
        {
            this.uuid = uuid;
            this.rankPoint = rankPoint;
            SetText(rank, userName, rankPoint);
            return;
        }
        if(this.uuid == uuid && rankPoint == this.rankPoint)
        {
            return;
        }
        this.rankPoint = rankPoint;
        this.uuid = uuid;
        SetTextWithAnimation(rank, userName,  rankPoint).Forget();

    }

    private void SetText(int rank, string userName, int rankPoint)
    {
        rankText.text = rank.ToString();
        userNameText.text = userName;
        rankPointText.text = rankPoint.ToString();
    }
    
    private UniTask SetTextWithAnimation( int rank, string userName, int rankPoint)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DORotate(new Vector3(180, 0, 0), 0.4f).SetEase(Ease.InOutElastic));
        sequence.AppendCallback(()=>SetText(rank, userName, rankPoint));
        sequence.Append(transform.DORotate(new Vector3(0, 0, 0), 0.4f).SetEase(Ease.InOutElastic));
        return UniTask.CompletedTask;
    }
}
