using UnityEngine;

public class Buff
{
    private readonly string buffNameFormat = "Buff{0}Name";
    private readonly string buffDescriptionFormat = "Buff{0}Description";
    public int BuffID { get; set; }
    public BuffType BuffType { get; set; }
    public float BuffTime { get; set; }
    public float BuffEffect { get; set; }
    public string BuffIcon { get; set; }

    public string buffName;
    public string buffDescription;

    public float remainBuffTime;
    public bool isOnBuff;
    public bool needAd;
    public Buff()
    {

    }
    public Buff(Buff newBuff)
    {
        BuffID = newBuff.BuffID;
        BuffType = newBuff.BuffType;
        BuffTime = newBuff.BuffTime;
        BuffEffect = newBuff.BuffEffect;
        BuffIcon = newBuff.BuffIcon;
    }
    public void Init()
    {
        remainBuffTime = BuffTime;
        isOnBuff = true;
        buffName = LZString.GetUIString(string.Format(buffNameFormat, BuffID));
        buffDescription = LZString.GetUIString(string.Format(buffDescriptionFormat, BuffID));
    }
}