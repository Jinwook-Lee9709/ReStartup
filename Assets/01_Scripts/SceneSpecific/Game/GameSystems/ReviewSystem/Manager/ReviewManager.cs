using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ReviewManager : MonoBehaviour
{
    [SerializeField] private ReviewRemoveAcceptPopup popup;
    [SerializeField] private ReviewUi popupParent;
    [SerializeField] private GameObject adPanel;
    public LinkedList<GameObject> reviews = new LinkedList<GameObject>();
    public GameObject loadingDot;
    public GameObject reviewContent;
    public GameObject reviewPrefab;

    private readonly int maxReviewCnt = 20;

    private void Awake()
    {
        AdvertisementManager.Instance.Init();
        UserDataManager.Instance.OnReviewCntFullEvent += AddReview;
    }

    private void Start()
    {
        var query = UserDataManager.Instance.CurrentUserData.ReviewSaveData.OrderBy(x => x.orderIndex);
        foreach (var data in query)
        {
            CreateReviewObject(data);
        }

        UpdateReviews();
    }

    private void CreateReviewObject(ReviewSaveData data)
    {
        ReviewData tempData = new();

        tempData.Init(data.isPositive, data.createdTime, data.reviewId);
        var gameObj = Instantiate(reviewPrefab);
        gameObj.transform.SetParent(reviewContent.transform);
        gameObj.transform.localPosition = Vector2.zero;
        gameObj.transform.rotation = Quaternion.Euler(Vector3.zero);
        gameObj.transform.SetSiblingIndex(0);
        gameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(reviewContent.GetComponent<RectTransform>().sizeDelta.x, gameObj.GetComponent<RectTransform>().sizeDelta.y);
        gameObj.transform.localScale = Vector3.one;
        var reviewObj = gameObj.GetComponent<Review>();
        reviewObj.Init(tempData);
        reviewObj.reviewManager = this;
        reviews.AddFirst(reviewObj.gameObject);
    }

    private void AddReview(bool isBest)
    {
        StartCoroutine(AddReviewCoroutine(isBest));
    }
    [VInspector.Button]
    public void AddPositiveReview()
    {
        StartCoroutine(AddReviewCoroutine(true));
    }

    [VInspector.Button]
    public void AddNegativeReview()
    {
        StartCoroutine(AddReviewCoroutine(false));
    }

    public IEnumerator AddReviewCoroutine(bool isBest)
    {
        var loadingObj = Instantiate(loadingDot);

        reviews.AddFirst(loadingObj);
        loadingObj.transform.SetParent(reviewContent.transform);
        loadingObj.transform.localPosition = Vector2.zero;
        loadingObj.transform.SetSiblingIndex(0);
        loadingDot.GetComponent<RectTransform>().sizeDelta = new Vector2(reviewContent.GetComponent<RectTransform>().sizeDelta.x, loadingObj.GetComponent<RectTransform>().sizeDelta.y);
        UpdateReviews();


        yield return new WaitForSeconds(3f);

        reviews.Remove(loadingObj);
        loadingObj.transform.DOScale(0f, 0.5f).SetEase(Ease.InOutElastic).OnComplete(() =>
        {
            loadingObj.GetComponent<LoadingDot>().StopDoTween();
            Destroy(loadingObj);
            ReviewData tempData = new();
            DateTime timeStamp = DateTime.Now;
            tempData.Init(isBest, timeStamp);
            var gameObj = Instantiate(reviewPrefab);
            gameObj.transform.SetParent(reviewContent.transform);
            gameObj.transform.localPosition = Vector2.zero;
            gameObj.transform.SetSiblingIndex(0);
            gameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(reviewContent.GetComponent<RectTransform>().sizeDelta.x, gameObj.GetComponent<RectTransform>().sizeDelta.y);
            gameObj.transform.DORotate(Vector3.zero, 1f).SetEase(Ease.OutBounce);
            gameObj.transform.localScale = Vector3.one;
            var reviewObj = gameObj.GetComponent<Review>();
            reviewObj.Init(tempData);
            reviewObj.reviewManager = this;

            ReviewSaveDataDAC.InsertReviewData(isBest, tempData.stringID, timeStamp).Forget();
            UserDataManager.Instance.AddRankPointWithSave(reviewObj.data.addPoint);

            reviews.AddFirst(reviewObj.gameObject);

            if (reviews.Count > maxReviewCnt)
            {
                RemoveAt(reviews.Last());
            }

            UpdateReviews();
        });

    }

    public void OnRemoveButtonClick(Review review)
    {
        var removePopup = Instantiate(popup, popupParent.transform, false);
        removePopup.reviewManager = this;
        removePopup.Init(review);
    }

    public void RemoveAt(GameObject removeReview)
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
            review.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, idx * -review.GetComponent<RectTransform>().sizeDelta.y), 1f).SetEase(Ease.InOutCubic);
            idx++;
        }
        var contentSize = reviewContent.GetComponent<RectTransform>().sizeDelta;
        reviewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(contentSize.x, reviewPrefab.GetComponent<RectTransform>().sizeDelta.y * idx);
    }

}
