using UnityEngine;

public enum GuestType
{
    Guest, //�Ϲ�
    Regular1, //�ܰ�
    Regular2, //�ܰ�
    Regular3, //�ܰ�
    Influencer, //���÷��
    BadGuest //������
}
public class ConsumerData
{
    public int GuestId { get; set; }
    public int GuestNameKey {  get; set; }
    public ThemeIds Theme {  get; set; }
    public GuestType GuestType {  get; set; }
    public int SellTipPercent { get; set; }
    public BuffType BuffType { get; set; }
    public int BuffId { get; set; }
    public int LoveFoodId { get; set; }
    public Sprite GuestPrefab { get; set; }
    public ParticleSystem GuestEffect {  get; set; }

    public float orderWaitTimer;
    public readonly float MaxOrderWaitLimit = 18f;
    public readonly float MaxEattingLimit = 8f;
    public void Init()
    {
        orderWaitTimer = MaxOrderWaitLimit;
    }
}