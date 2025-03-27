using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UiItemData
{
    public int ID { get; set; }
    public int Cost { get; set; }
    public string Icon { get; set; }
    public Action OnUpgrade { get; set; }
}
