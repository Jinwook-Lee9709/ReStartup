using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class IncreaseRankPoint : MonoBehaviour
{
    public void IncreaseRankpoint()
    {
        UserDataManager.Instance.AddRankPointWithSave(100000).Forget();
    }
}
