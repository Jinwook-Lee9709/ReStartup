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

    public Action OnBubblePopdownEvent;
    public void Init(string text, Action startAction = null, Action endAction = null)
    {
        transform.localPosition = new Vector3(transform.root.GetComponent<ConsumerFSM>().CurrentStatus == ConsumerFSM.ConsumerState.Paying ? 0.5f : -0.5f, 1f, 0f);
        this.text.text = text;
        this.text.transform.localScale = this.text.transform.root.localScale.x < 0f ? new Vector3(-1f, 1f, 1f) : Vector3.one;
        if (this.text.transform.root.GetComponent<ConsumerFSM>().CurrentStatus == ConsumerFSM.ConsumerState.Paying)
            this.text.transform.localScale = new Vector3(-1f, 1f, 1f);
        startAction?.Invoke();
        OnBubblePopdownEvent += endAction;
    }

    private void OnEnable()
    {
        transform.PopupAnimation(transform.root.GetComponent<ConsumerFSM>().CurrentStatus == ConsumerFSM.ConsumerState.Paying ? new Vector3(-2f, 2f, 2f) : new Vector3(2f, 2f, 2f), 0.5f,
            () => StartCoroutine(ShowText()));
    }
    private IEnumerator ShowText()
    {
        yield return new WaitForSeconds(destroyTime);

        transform.PopdownAnimation(0f, 1f, () =>
        {
            OnBubblePopdownEvent?.Invoke();
            StopAllCoroutines();
            Destroy(gameObject);
        });
    }
}
