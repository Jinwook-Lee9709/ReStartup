using System;
using UnityEngine;

public class CameraPositionSetting : MonoBehaviour
{
    enum CameraPosition
    {
        Hall,
        Kitchen
    }
    
    [SerializeField] private CameraPosition cameraPosition;
    private void Start()
    {
        var leftPos = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height * 0.5f, 0f));
        var rightPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height * 0.5f, 0f));
        
        var screenWorldWidth = rightPos.x - leftPos.x;

        switch (cameraPosition)
        {
            case CameraPosition.Hall:
                transform.position = new Vector3(-screenWorldWidth * 0.5f, transform.position.y, transform.position.z);
                break;
            case CameraPosition.Kitchen:
                transform.position = new Vector3(screenWorldWidth * 0.5f, transform.position.y, transform.position.z);
                break;
        }
            
    }
}