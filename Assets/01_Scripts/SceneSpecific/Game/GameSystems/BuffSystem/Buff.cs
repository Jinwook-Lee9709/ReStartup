public abstract class Buff
{
    public BuffType Type { get; set; }
    public bool IsOnBuff { get; set; }
    public float RemainBuffTime { get; set; }
}