using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class GuidePopup : MonoBehaviour
{
    private static readonly string titleStringFormat = "EntryDesc{0}";
    
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button background;
    [SerializeField] private Image panel;
    
    public void SetInfo(GuideElementData data)
    {
        var sprite = Addressables.LoadAssetAsync<Sprite>(data.Resource).WaitForCompletion();
        string descriptionStringKey = String.Format(titleStringFormat, data.EntryId);
        var description = LZString.GetUIString(descriptionStringKey);
        image.sprite = sprite;
        descriptionText.text = description;
    }

    public void Open()
    {
        transform.PopupAnimation();
    }
        
    
    private void OnClose()
    {
        background.interactable = false;
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () => { gameObject.SetActive(false); });
    }
}