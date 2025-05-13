using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeUpgradePopup : MonoBehaviour
{
    [SerializeField] private float backgroundOpacity = 0.8f;
    [SerializeField] private Button background;
    [SerializeField] private Button mainButton;
    [SerializeField] private Image panel;
    [SerializeField] private Image employeeIcon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI mainButtonText;
    [SerializeField] private TextMeshProUGUI priceText;

    [SerializeField] private TextMeshProUGUI currentMoveSpeedValue;
    [SerializeField] private TextMeshProUGUI currentWorkSpeedValue;
    [SerializeField] private TextMeshProUGUI currentHealthValue; 
    
    [SerializeField] private TextMeshProUGUI nextLevelMoveSpeedValue;
    [SerializeField] private TextMeshProUGUI nextLevelWorkSpeedValue;
    [SerializeField] private TextMeshProUGUI nextLevelHealthValue;

    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;

    private EmployeeUIItem currentCard;

    public void Start()
    {
        background.onClick.RemoveAllListeners();
        background.onClick.AddListener(OnClose);
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(OnMainButtonTouched);
    }

    public void SetInfo(EmployeeUIItem card , Sprite image)
    {
        currentCard = card;
        var data = card.employeeData;
        int upgradeCount = UserDataManager.Instance.CurrentUserData.EmployeeSaveData[data.StaffID].level;
        employeeIcon.sprite = image;
        currentLevelText.text = $"{upgradeCount} LV";
        nextLevelText.text = $"{upgradeCount + 1} LV";
        nameText.text = LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, data.StaffID));
        priceText.text = data.Cost.ToString();
        currentMoveSpeedValue.text = data.MoveSpeed.ToString();
        currentWorkSpeedValue.text = data.WorkSpeed.ToString();
        currentHealthValue.text = data.Health.ToString();
        nextLevelMoveSpeedValue.text = (data.MoveSpeed - data.upgradeMoveSpeed).ToString();
        nextLevelWorkSpeedValue.text = (data.WorkSpeed - data.upgradeWorkSpeed).ToString();
        nextLevelHealthValue.text = (data.Health + data.upgradeHealth).ToString();
    }
    private void OnEnable()
    {
        background.interactable = false;
        if (background != null)
        {
            var backgroundImage = background.GetComponent<Image>();
            backgroundImage.FadeInAnimation();
        }

        if (panel != null)
        {
            panel.transform.PopupAnimation(onComplete:() => background.interactable = true);
        }
    }


    private void OnMainButtonTouched()
    {
        currentCard.OnBuy();
        OnClose();
    }

    private void OnClose()
    {
        background.interactable = false;
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () => { gameObject.SetActive(false); });
    }
}
