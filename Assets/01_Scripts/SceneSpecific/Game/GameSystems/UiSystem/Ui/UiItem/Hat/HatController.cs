using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatController : MonoBehaviour
{
    public enum ColorType
    {
        Grey = 0,
        Yellow = 1,
        Pink = 2
    }

    private List<String> hatColors = new List<String>()
    {
        "#A8A8A8",
        "#FFE700",
        "#FFA6B1",
    };
    
    [SerializeField] private Image frame;
    [SerializeField] private Image fill;

    public void ChangeFrameColor(ColorType colorType)
    {
        ColorUtility.TryParseHtmlString(hatColors[(int)colorType], out Color color);
        frame.color = color;
    }

    public void SetFill(bool isActive)
    {
        fill.gameObject.SetActive(isActive);
    }
    
}
