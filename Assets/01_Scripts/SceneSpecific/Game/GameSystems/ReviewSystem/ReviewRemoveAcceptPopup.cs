using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviewRemoveAcceptPopup : PopUp
{
    public ReviewManager reviewManager;

    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancleButton;
    protected override void Start()
    {
        base.Start();
        acceptButton.interactable = true;
        cancleButton.interactable = true;
        cancleButton.onClick.AddListener(() =>
        {
            cancleButton.interactable = false;
            acceptButton.interactable = false;
            OnCancle();
        });
    }

    public void Init(Review review)
    {
        acceptButton.onClick.AddListener(() =>
        {
            acceptButton.interactable = false;
            cancleButton.interactable = false;
            review.Remove();
            OnCancle();
        });
    }

}
