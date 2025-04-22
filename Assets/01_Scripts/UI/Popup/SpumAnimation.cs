using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpumAnimation : MonoBehaviour
{
    [SerializeField] private SPUM_Prefabs model;
    [SerializeField] private PlayerState animationCategory;
    [SerializeField] private int animationID;
    [SerializeField] private int animationInterval;

    private float timer = 0;
    
    private CancellationTokenSource cts;

    private void OnEnable()
    {
        cts = new CancellationTokenSource();
        PlayAnimationAsync(cts.Token).Forget();
    }

    private void OnDisable()
    {
        if (cts != null)
        {
            cts.Cancel(); // 취소 요청
            cts.Dispose(); // 메모리 정리
            cts = null;
        }
    }

    private async UniTaskVoid PlayAnimationAsync(CancellationToken token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(animationInterval), cancellationToken: token); 
        while (!token.IsCancellationRequested)
        {
            model.PlayAnimation(animationCategory, animationID);
            await UniTask.Delay(TimeSpan.FromSeconds(animationInterval), cancellationToken: token);
        }
    }

    private void Start()
    {
        model.OverrideControllerInit();
    }
    
}
