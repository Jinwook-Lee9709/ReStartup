using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNamePopup : PopUp
{
    [SerializeField] private NameRegister nameRegister;
    public Action OnCancleAction;
    public async UniTask Init(GameObject parent)
    {
        await UniTask.NextFrame();
        backGround.onClick.RemoveAllListeners();
        nameRegister.parent = this;
        nameRegister.rootParent = parent;
        OnCancleAction = OnCancle;
    }
}
