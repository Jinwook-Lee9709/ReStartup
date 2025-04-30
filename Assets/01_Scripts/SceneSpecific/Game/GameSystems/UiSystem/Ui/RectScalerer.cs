using Cysharp.Threading.Tasks;
using GoogleMobileAds.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectScalerer : MonoBehaviour
{
    [SerializeField][Range(0f,1f)] private float ratio;
    [SerializeField] private RectTransform parent;
    void Start()
    {
        RectAdjust().Forget();
    }

    private async UniTask RectAdjust()
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(parent.sizeDelta.x * ratio, rect.sizeDelta.y);
    }
}
