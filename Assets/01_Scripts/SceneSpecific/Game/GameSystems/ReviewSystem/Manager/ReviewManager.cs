using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReviewManager : MonoBehaviour
{
    public LinkedList<Review> reviews = new LinkedList<Review>();
    public GameObject reviewContent;
    public GameObject reviewPrefab;

    private readonly int maxReviewCnt = 20;

    private void Awake()
    {
        DOTween.Init();
        for (int i = 0; i < maxReviewCnt; i++)
        {
            AddBestReview();
        }
    }

    public void AddReview(bool isBest)
    {
        //ReviewData tempData = new();
        //tempData.Init(isBest);
        //var reviewObj = Instantiate(reviewPrefab).GetComponent<Review>();
        //reviewObj.Init(tempData);

        //reviews.Add(reviewObj);
        //UpdateReviews();


        if (isBest)
            AddBestReview();
        else
            AddWorstReview();
    }

    [ContextMenu("Add Best Review")]
    public void AddBestReview()
    {
        ReviewData tempData = new();
        tempData.Init(true);
        var gameObj = Instantiate(reviewPrefab);
        gameObj.transform.SetParent(reviewContent.transform);
        gameObj.transform.localPosition = new Vector2(reviewContent.GetComponent<RectTransform>().sizeDelta.x * 0.5f, gameObj.GetComponent<RectTransform>().sizeDelta.y);
        gameObj.transform.SetSiblingIndex(0);



        var reviewObj = gameObj.GetComponent<Review>();
        reviewObj.Init(tempData);

        UserDataManager.Instance.CurrentUserData.CurrentRankPoint += 10;

        reviews.AddFirst(reviewObj);

        if (reviews.Count > maxReviewCnt)
        {
            RemoveAt(reviews.Last());
        }

        UpdateReviews();
    }

    [ContextMenu("Add Worst Review")]
    public void AddWorstReview()
    {
        ReviewData tempData = new();
        tempData.Init(false);
        var gameObj = Instantiate(reviewPrefab);
        gameObj.transform.SetParent(reviewContent.transform);
        gameObj.transform.localPosition = new Vector2(reviewContent.GetComponent<RectTransform>().sizeDelta.x * 0.5f, gameObj.GetComponent<RectTransform>().sizeDelta.y);
        gameObj.transform.SetSiblingIndex(0);

        var reviewObj = gameObj.GetComponent<Review>();
        reviewObj.Init(tempData);

        UserDataManager.Instance.CurrentUserData.CurrentRankPoint -= 10;

        reviews.AddFirst(reviewObj);

        if (reviews.Count > maxReviewCnt)
        {
            RemoveAt(reviews.Last());
        }

        UpdateReviews();
    }

    public void RemoveAt(Review removeReview)
    {
        removeReview.GetComponent<RectTransform>().DOScale(0f, 1f).SetEase(Ease.InOutElastic).OnComplete(() =>
        {
            Destroy(removeReview.gameObject);
            reviews.Remove(removeReview);
            UpdateReviews();
        });
    }

    public void UpdateReviews()
    {
        int idx = 0;
        foreach (var review in reviews)
        {
            review.GetComponent<RectTransform>().DOAnchorPos(new Vector2(reviewContent.GetComponent<RectTransform>().sizeDelta.x * 0.5f, idx * -review.GetComponent<RectTransform>().sizeDelta.y), 1f).SetEase(Ease.InOutCubic);
            idx++;
        }
        var contentSize = reviewContent.GetComponent<RectTransform>().sizeDelta;
        reviewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(contentSize.x, reviewPrefab.GetComponent<RectTransform>().sizeDelta.y * idx);
    }

}
