using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumerData
{
    public float OrderWaitTimer { get; set; }
    public float MaxOrderWaitLimit { get; set; } = 30f;
    public float MaxEattingLimit { get; set; } = 10f;
}
