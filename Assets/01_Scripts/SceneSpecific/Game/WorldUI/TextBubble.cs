using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBubble : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    private float destroyTime = 2f;

    public Action OnBubblePopEvent;
    public void Init(string text, Action action)
    {
        transform.localPosition = new Vector3(0.5f, 1f, 0f);
        this.text.text = text;
        OnBubblePopEvent += action;
    }

    private void OnEnable()
    {
        transform.PopupAnimation(2f, 0.5f,
            () => StartCoroutine(ShowText()));
    }
    private IEnumerator ShowText()
    {
        yield return new WaitForSeconds(destroyTime);

        transform.PopdownAnimation(0f, 1f, () =>
        {
            OnBubblePopEvent.Invoke();
            StopAllCoroutines();
            Destroy(gameObject);
        });
    }
}
