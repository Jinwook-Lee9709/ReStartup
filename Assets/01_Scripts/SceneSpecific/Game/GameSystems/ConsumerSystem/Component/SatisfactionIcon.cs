using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatisfactionIcon : MonoBehaviour
{
    [SerializeField] private Sprite[] icons = new Sprite[3];
    private Color targetColor = new Color(1,1,1, 0);
    private SpriteRenderer icon;
    public void SetIcon(ConsumerFSM.Satisfaction satisfaction)
    {
        icon = GetComponent<SpriteRenderer>();
        gameObject.SetActive(true);
        icon.sprite = icons[(int)Enum.Parse(typeof(ConsumerFSM.Satisfaction), satisfaction.ToString())];
        icon.DOColor(targetColor, 1f).SetLoops(5, LoopType.Yoyo).OnComplete(() =>
        {
            icon.color = Color.white;
            if(satisfaction != ConsumerFSM.Satisfaction.Low)
                gameObject.SetActive(false);
        });
    }
}
