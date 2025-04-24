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
    private Camera mainCamera;

    public Action OnBubblePopEvent;
    public void Init(string text, Action action)
    {
        transform.localPosition = new Vector3(-0.5f, 1f, 0f);
        this.text.text = text;
        this.text.transform.localScale = this.text.transform.root.localScale.x < 0f ? new Vector3(-1f, 1f, 1f) : Vector3.one;
        OnBubblePopEvent += action;
    }

    private void OnEnable()
    {
        mainCamera = Camera.main;
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
