using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumerData
{
    public enum ConsumerType
    {
        Regular,        //�ܰ�
        Influencer,     //���÷��
        Obnoxious       //������
    }
    public ConsumerType Type { get; set; } = ConsumerType.Obnoxious;
    public float OrderWaitTimer { get; set; }
    public float MaxOrderWaitLimit { get; set; } = 30f;
    public float MaxEattingLimit { get; set; } = 3f;
}
