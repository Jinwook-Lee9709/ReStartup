using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class ReviewUi : MonoBehaviour
{
    public enum ReviewUiState
    {
        Review = 0,
        Rank,
        Information
    }
    
    public GameObject reviewScrollView;
    public GameObject rankUpScrollView;
    public GameObject informationScrollView;

    [SerializedDictionary][SerializeField]
    private SerializedDictionary<ReviewUiState, GameObject> panels;
    
    [SerializedDictionary][SerializeField]
    private SerializedDictionary<ReviewUiState, Button> buttonDictionary;

    [SerializeField] private Button closeButton;
    
    private ReviewUiState currentState;

    private ReviewUiState CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;
            foreach (var pair in buttonDictionary)
            {
                if (pair.Key == currentState)
                {
                    pair.Value.gameObject.SetActive(false);
                    panels[pair.Key].SetActive(true);
                    closeButton.transform.SetSiblingIndex((int)pair.Key);
                }
                else
                {
                    panels[pair.Key].SetActive(false);
                    pair.Value.gameObject.SetActive(true);
                }
            }
            
        }
    }

    private void Start()
    {
        CurrentState = ReviewUiState.Rank;
    }
    
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
        CurrentState = ReviewUiState.Review;
    }

    public void OnClickRankUpButton()
    {
        CurrentState = ReviewUiState.Rank;
    }

    public void OnClickInformationButton()
    {
        CurrentState = ReviewUiState.Information;
    }
}