using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SoonsoonData;

public class FoodResearchNotifyPopup : MonoBehaviour
{
    private string firstRequirementString = "달성 랭킹 포인트";
    private string secondRequirementString = "필요한 가구";

    [SerializeField] private GameObject panel;
    [SerializeField] private Button background;

    [SerializeField] private Color satisfiedColor;
    [SerializeField] private Color unsatisfiedColor;

    [SerializeField] private TextMeshProUGUI firstRequirementNameText;
    [SerializeField] private TextMeshProUGUI firstRequirementValueText;
    [SerializeField] private TextMeshProUGUI secondRequirementNameText;
    [SerializeField] private TextMeshProUGUI secondRequirementValueText;
    [SerializeField] private GameObject secondRequirementFrame;

    public void Start()
    {
        background.onClick.RemoveAllListeners();
        background.onClick.AddListener(OnClose);
    }
    private void OnEnable()
    {
        if (background != null)
        {
            var backgroundImage = background.GetComponent<Image>();
            backgroundImage.FadeInAnimation();
        }

        if (panel != null)
        {
            panel.transform.PopupAnimation();
        }
    }

    public void SetRequirementText(FoodData data, bool currentCookwareAmount)
    {
        secondRequirementFrame.SetActive(!currentCookwareAmount);
        if (!currentCookwareAmount)
        {
            secondRequirementValueText.text = $"{LZString.GetUIString(data.CookwareType.ToString())} {data.CookwareNB}";

            var secondTextColor = !currentCookwareAmount ? unsatisfiedColor : satisfiedColor;
            secondRequirementNameText.color = secondTextColor;
            secondRequirementValueText.color = secondTextColor;
        }
        firstRequirementValueText.text = data.Requirements.ToString();

        var firstTextColor = data.Requirements < UserDataManager.Instance.CurrentUserData.CurrentRankPoint ? satisfiedColor : unsatisfiedColor;
        firstRequirementNameText.color = firstTextColor;
        firstRequirementValueText.color = firstTextColor;
    }

    private void OnClose()
    {
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () => { gameObject.SetActive(false); });
    }
}
