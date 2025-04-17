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
    private bool isPaid;

    private bool IsPaid
    {
        get => isPaid;
        set
        {
            isPaid = value;
            mainButtonText.text = value ? "확인" : "업그레이드";
        }
    }
    public void Start()
    {
        background.onClick.RemoveAllListeners();
        background.onClick.AddListener(OnClose);
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(OnMainButtonTouched);
    }

    public void SetInfo(EmployeeUIItem card , Sprite image)
    {
        IsPaid = false;
        currentCard = card;
        var data = card.employeeData;
        int upgradeCount = UserDataManager.Instance.CurrentUserData.EmployeeSaveData[data.StaffID].level;
        employeeIcon.sprite = image;
        currentLevelText.text = $"{upgradeCount} LV";
        nextLevelText.text = $"{upgradeCount + 1} LV";
        nameText.text = LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, data.StaffID));
        priceText.text = (data.Cost * (upgradeCount + 1)).ToString();
        currentMoveSpeedValue.text = data.MoveSpeed.ToString();
        currentWorkSpeedValue.text = data.WorkSpeed.ToString();
        currentHealthValue.text = data.Health.ToString();
        nextLevelMoveSpeedValue.text = data.MoveSpeed.ToString();
        nextLevelWorkSpeedValue.text = data.WorkSpeed.ToString();
        nextLevelHealthValue.text = data.Health.ToString();
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


    private void OnMainButtonTouched()
    {
        if (!IsPaid)
        {
            IsPaid = true;
            currentCard.OnBuy();
        }
        else
        {
            OnClose();
        }
    }

    private void OnClose()
    {
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () => { gameObject.SetActive(false); });
    }
}
