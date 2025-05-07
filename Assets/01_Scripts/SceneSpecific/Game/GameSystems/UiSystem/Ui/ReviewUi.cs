using System;
using UnityEngine;

public class ReviewUi : MonoBehaviour
{
    public GameObject reviewScrollView;
    public GameObject rankUpScrollView;
    public GameObject informationScrollView;

    private void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.OnClickButtonExitReviewUi();
            }
        }
    }

    public void OnClickReviewButton()
    {
        reviewScrollView.SetActive(true);
        rankUpScrollView.SetActive(false);
        informationScrollView.SetActive(false);
    }

    public void OnClickRankUpButton()
    {
        reviewScrollView.SetActive(false);
        rankUpScrollView.SetActive(true);
        informationScrollView.SetActive(false);
    }

    public void OnClickInformationButton()
    {
        reviewScrollView.SetActive(false);
        rankUpScrollView.SetActive(false);
        informationScrollView.SetActive(true);
    }
}