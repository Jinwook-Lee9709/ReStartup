using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatListController : MonoBehaviour
{
    [SerializeField] private List<HatController> hatList;

    public void SetHat(int rank)
    {
        var totalCount = hatList.Count;
        int fillAmount = rank % totalCount == 0 ? totalCount : rank % totalCount; 
        HatController.ColorType colorType = (HatController.ColorType)((rank - 1) / totalCount);
        for (int i = 0; i < hatList.Count; i++)
        {
            hatList[i].ChangeFrameColor(colorType);
            hatList[i].SetFill(i + 1 <= fillAmount);
        }
    }
}
