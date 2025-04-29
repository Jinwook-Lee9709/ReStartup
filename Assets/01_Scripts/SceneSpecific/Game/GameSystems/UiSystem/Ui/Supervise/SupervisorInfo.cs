using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupervisorInfo
{
    //Flag for confirm prev supervisor is Hired
    public bool isHireable { get; set; }
    public bool isHired { get; set; }
    public int number { get; set; }
    public int cost { get; set; }
    public string name { get; set; }
    public Sprite icon { get; set; }

}
