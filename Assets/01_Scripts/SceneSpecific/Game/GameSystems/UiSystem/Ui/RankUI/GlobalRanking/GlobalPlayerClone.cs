using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalPlayerClone : MonoBehaviour
{
    public ScrollRect rect;
    public UserRankData playerData;
    [SerializeField] private Image playerImage;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerRank;
    [SerializeField] private TextMeshProUGUI playerRankPoint;
    private int rankerCount = 0;
    
    
    private bool isActive = true;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickClone);
    }

    public void SetRankerCount(int count)
    {
        rankerCount = count;
    }

    public void UpdatePlayerData(UserRankData data)
    {
        playerData = data;
        playerName.text = LZString.GetUIString(Strings.PlayerTag);
        playerRank.text = data.rank.ToString();
        playerRankPoint.text = data.rankPoint.ToString();
    }
    public void OnActive()
    {
        if (!gameObject.activeSelf)
        {
            isActive = true;
            gameObject.SetActive(true);
            transform.DOScale(1f, 0.5f).SetEase(Ease.InOutElastic);
        }
    }
    public void OnUnActive()
    {
        if (isActive)
        {
            isActive = false;
            transform.DOKill();
            transform.DOScale(0f, 0.5f).SetEase(Ease.InOutElastic).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }

    public void OnClickClone()
    {
        if (playerData.rank > 50)
            return;
        rect.DOVerticalNormalizedPos(Mathf.InverseLerp(rankerCount, 0, playerData.rank),1f);
    }


}
