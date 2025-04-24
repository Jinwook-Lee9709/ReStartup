using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffInfoUI : MonoBehaviour, IComparable<BuffInfoUI>
{
    public Buff currentBuff;
    [SerializeField] private GameObject buffImage;
    [SerializeField] private TextMeshProUGUI buffRemainTimeText, buffInfoText;

    public int CompareTo(BuffInfoUI other)
    {
        return currentBuff.remainBuffTime > other.currentBuff.remainBuffTime ? 1 : -1;
    }

    public void Init(Buff buff)
    {
        currentBuff = buff;
        //TODO : buffImage
        buffRemainTimeText.text = $"{Mathf.CeilToInt(currentBuff.remainBuffTime)} 초";
        buffInfoText.text = currentBuff.buffDescription;
    }

    private void Update()
    {
        buffRemainTimeText.text = $"{Mathf.CeilToInt(currentBuff.remainBuffTime)} 초";

        if (currentBuff.remainBuffTime < 0f)
        {
            Destroy(gameObject);
        }
    }
}
