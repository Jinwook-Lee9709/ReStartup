using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumerData
{
    public enum ConsumerType
    {
        Normal,         //일반
        Regular,        //단골
        Influencer,     //인플루언서
        Obnoxious       //개진상
    }
    public ConsumerType Type { get; set; } = ConsumerType.Obnoxious;
    public float OrderWaitTimer { get; set; }
    public float MaxOrderWaitLimit { get; set; } = 20f;
    public float MaxEattingLimit { get; set; } = 3f;

    //임시 팁 확률
    public float TempTipProb => 0.5f;
    public float SellTipPercent => 22f;

    public void Init()
    {
        OrderWaitTimer = MaxOrderWaitLimit;

    }
}
