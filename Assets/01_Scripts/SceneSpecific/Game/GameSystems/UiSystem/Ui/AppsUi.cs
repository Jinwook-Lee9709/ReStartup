using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppsUi : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    [SerializeField]private GameObject missionNewImage;

    private void Start()
    {
        timeText.text = DateTime.Now.ToString("HH:mm");
    }
    private void Update()
    {
    }
    public void MissionNewImageON()
    {
        missionNewImage.SetActive(true);
    }
    public void MissionNewImageOFF()
    {
        missionNewImage.SetActive(false);
    }
}