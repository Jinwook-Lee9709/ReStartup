using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BuffInfoUI : MonoBehaviour, IComparable<BuffInfoUI>
{
    public static readonly string BuffIconFormat = "BuffIcon{0}";
    public Buff currentBuff;
    private BuffManager buffManager;
    [SerializeField] private Image buffImage;
    [SerializeField] private TextMeshProUGUI buffRemainTimeText, buffInfoText;

    public int CompareTo(BuffInfoUI other)
    {
        return currentBuff.remainBuffTime > other.currentBuff.remainBuffTime ? 1 : -1;
    }

    public async UniTask Init(Buff buff, BuffManager buffManager)
    {
        currentBuff = buff;
        this.buffManager = buffManager;
        var handle = Addressables.LoadAssetAsync<Sprite>(string.Format(BuffIconFormat, (int)buff.BuffType));
        var sprite = await handle;
        buffImage.sprite = sprite;
        buffRemainTimeText.text = $"{Mathf.CeilToInt(currentBuff.remainBuffTime)} 초";
        buffInfoText.text = currentBuff.buffDescription;
    }

    private void Update()
    {
        buffRemainTimeText.text = $"{Mathf.CeilToInt(currentBuff.remainBuffTime)} 초";

        if (currentBuff.remainBuffTime < 0f)
        {
            OnBuffTimeOver();
        }
    }

    private void OnBuffTimeOver()
    {
        GetComponent<RectTransform>().DOMoveX(-GetComponent<RectTransform>().sizeDelta.x, 1.5f).OnComplete(() =>
        {
            transform.DOKill();
            buffManager.buffInfoUIList.Remove(this);
            buffManager.UpdateBuffCntUI();
            Destroy(gameObject);
        });
    }
}
