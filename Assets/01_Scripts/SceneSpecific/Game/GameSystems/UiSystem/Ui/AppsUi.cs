using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AppsUi : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    void Start()
    {
        timeText.text = DateTime.Now.ToString(("HH:mm"));
    }

    void Update()
    {
        
    }
}
