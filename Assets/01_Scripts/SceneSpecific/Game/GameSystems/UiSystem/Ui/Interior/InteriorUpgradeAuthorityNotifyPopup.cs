using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InteriorUpgradeAuthorityNotifyPopup : MonoBehaviour
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
    
    public void SetRequirementText(InteriorData data)
    {
        bool isSecondRequirementExist = data.Requirements2 != 0;
        secondRequirementFrame.SetActive(isSecondRequirementExist);
        if (isSecondRequirementExist)
        {
            var table =DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
            var interiorData = table.GetData(data.Requirements2);
            
            secondRequirementValueText.text = LZString.GetUIString(interiorData.Name);
            
            var secondTextColor = data.CheckSecondRequirement() ? satisfiedColor : unsatisfiedColor;
            secondRequirementNameText.color = secondTextColor;
            secondRequirementValueText.color = secondTextColor;
        }
        firstRequirementValueText.text = data.Requirements1.ToString();
        
        var firstTextColor = data.CheckFirstRequirement() ? satisfiedColor : unsatisfiedColor;
        firstRequirementNameText.color = firstTextColor;
        firstRequirementValueText.color = firstTextColor;
    }
    
    private void OnClose()
    {
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete:() =>{gameObject.SetActive(false);});
    }
}
