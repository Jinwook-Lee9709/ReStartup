using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class HorizontalLayoutPaddingCalculator : MonoBehaviour
{
    [SerializeField] private float elementWidth;
    [SerializeField] private RectTransform viewPort;

    [Foldout("Variables")] 
    [ShowInInspector] private float viewPortWidth;
    [ShowInInspector] private float padding;
    private void Start()
    {
        AdjustPaddingAsync().Forget();
    }

    [VInspector.Button]
    private void AdjustPadding()
    {
        var layoutGroup = GetComponent<HorizontalLayoutGroup>();
        viewPortWidth = viewPort.rect.size.x;
        padding = (viewPortWidth - elementWidth) / 2;
        layoutGroup.padding = new RectOffset((int)padding, (int)padding, 0, 0);
        layoutGroup.spacing = padding;
    }
    
    private async UniTask AdjustPaddingAsync()
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        AdjustPadding();
    }
}
