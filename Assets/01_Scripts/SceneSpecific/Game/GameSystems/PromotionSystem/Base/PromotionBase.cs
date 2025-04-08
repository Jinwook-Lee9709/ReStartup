using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class PromotionBase
{
    public int consumeGold;
    public int goldConsumeCnt;

    public int consumeGem;
    public int gemConsumeCnt;

    public int maxAdViewCnt;
    public int remainingAdViewCnt;

    public abstract void Init();
    public abstract void Excute(BuffManager buffManager);
}
