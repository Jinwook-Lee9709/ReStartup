using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumerData
{
    public enum ConsumerType
    {
        Normal,         //�Ϲ�
        Regular,        //�ܰ�
        Influencer,     //���÷��
        Obnoxious       //������
    }
    public ConsumerType Type { get; set; } = ConsumerType.Obnoxious;
    public float OrderWaitTimer { get; set; }
    public float MaxOrderWaitLimit { get; set; } = 100f;
    public float MaxEattingLimit { get; set; } = 3f;

    //�ӽ� �� Ȯ��
    public float TempTipProb => 0.5f;
    public float SellTipPercent => 22f;

    public void Init()
    {
        OrderWaitTimer = MaxOrderWaitLimit;

    }
}
