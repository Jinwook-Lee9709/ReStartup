using System;
using UnityEngine;

public enum GuestType
{
    Guest, //�Ϲ�
    Regular1, //�ܰ�
    Regular2, //�ܰ�
    Regular3, //�ܰ�
    Influencer, //���÷��
    BadGuest, //������
    PromotionGuest,
}
[Serializable]
public class ConsumerData
{
    public int GuestId { get; set; }
    public int GuestNameKey {  get; set; }
    public ThemeIds Theme {  get; set; }
    public GuestType GuestType {  get; set; }
    public int SellTipPercent { get; set; }
    public BuffType BuffType { get; set; }
    public int BuffID1 { get; set; }
    public int BuffID2 { get; set; }
    public int LoveFoodId { get; set; }
    public string GuestPrefab { get; set; }
    //public string GuestEffect {  get; set; }

    public float orderWaitTimer;
    public readonly float MaxOrderWaitLimit = 30f;
    public readonly float MaxEattingLimit = 8f;
    public void Init()
    {
        orderWaitTimer = MaxOrderWaitLimit;
    }
}