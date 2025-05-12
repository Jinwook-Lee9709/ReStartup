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
    
    private static readonly float maxScaleRatio = 1.5f;
    
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button background;
    [SerializeField] private Image panel;

    [SerializeField] private RectTransform canvas;
    
    public void Start()
    {
        background.onClick.RemoveAllListeners();
        background.onClick.AddListener(OnClose);
        CalculateSize();
    }

    [VInspector.Button]
    private void CalculateSize()
    {
        Vector2 size = canvas.rect.size;
        float sizeRatio = size.x / 645;
        sizeRatio = Mathf.Clamp(sizeRatio, 1f, maxScaleRatio);
        panel.transform.localScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);
    }
    
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
        gameObject.SetActive(true);
        panel.transform.PopupAnimation(onComplete: () => {background.interactable = true;});
        if (background != null)
        {
            var backgroundImage = background.GetComponent<Image>();
            backgroundImage.FadeInAnimation();
        }
    }
        
    
    private void OnClose()
    {
        background.interactable = false;
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () => { gameObject.SetActive(false); });
    }
}