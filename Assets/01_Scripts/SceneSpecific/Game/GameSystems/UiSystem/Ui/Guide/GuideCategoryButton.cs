using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

public class GuideCategoryButton : MonoBehaviour
{
    private static readonly string titleStringFormat = "Category{0}";
    
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button button;
    [SerializeField] private Image icon;

    public void Init(GuideCategoryData data, UnityAction<int> onClick)
    {
        var titleStringKey = String.Format(titleStringFormat, data.CategoryId);
        var titleString = LZString.GetUIString(titleStringKey);

        var iconSprite = Addressables.LoadAssetAsync<Sprite>(data.CategoryIcon).WaitForCompletion();
        
        title.text = titleString;
        icon.sprite = iconSprite;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(data.CategoryId));
    }
}
