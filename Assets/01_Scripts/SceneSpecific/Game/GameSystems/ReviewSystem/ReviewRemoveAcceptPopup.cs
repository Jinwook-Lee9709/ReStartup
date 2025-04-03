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
        cancleButton.onClick.AddListener(OnCancle);
    }

    public void Init(Review review)
    {
        acceptButton.onClick.AddListener(() =>
        {
            review.Remove();
            OnCancle();
        });
    }

}
