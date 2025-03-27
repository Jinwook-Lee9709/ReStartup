using System;
using TMPro;
using UnityEngine;

public class AppsUi : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    private void Start()
    {
        timeText.text = DateTime.Now.ToString("HH:mm");
    }

    private void Update()
    {
    }
}