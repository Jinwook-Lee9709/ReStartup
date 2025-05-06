using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreferencesUpdatePopup : MonoBehaviour
{
    [SerializeField] private float backgroundOpacity = 0.8f;
    [SerializeField] private Button background;
    [SerializeField] private Image panel;

    private FoodUpgradeUIItem currentCard;
    private bool isPaid;

    private bool IsPaid
    {
        get => isPaid;
        set
        {
            isPaid = value;
        }
    }
    public void Start()
    {
        background.onClick.RemoveAllListeners();
        background.onClick.AddListener(OnClose);
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
            panel.transform.PopupAnimation(onComplete: () => background.interactable = true);
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
