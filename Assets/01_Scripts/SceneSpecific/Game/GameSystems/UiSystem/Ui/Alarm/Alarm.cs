using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alarm : MonoBehaviour
{
    [SerializeField] private Button hallAlarm, kitchenAlarm;
    [SerializeField] private CinemachineVirtualCamera hallCamera;
    private CinemachineBrain mainBrain;
    private void Start()
    {
        kitchenAlarm.onClick.AddListener(KitchenAlarmTouch);
        hallAlarm.onClick.AddListener(HallAlarmTouch);
        hallAlarm.gameObject.SetActive(false);
        kitchenAlarm.gameObject.SetActive(false);
        mainBrain = Camera.main.GetComponent<CinemachineBrain>();
    }
    private void PopUpAlarm()
    {
        switch (mainBrain.ActiveVirtualCamera?.VirtualCameraGameObject.GetComponent<CameraPositionSetting>().GetCameraPosition())
        {
            case CameraPositionSetting.CameraPosition.Hall:
                kitchenAlarm.transform.PopupAnimation();
                kitchenAlarm.interactable = true;
                break;
            case CameraPositionSetting.CameraPosition.Kitchen:
                hallAlarm.transform.PopupAnimation();
                hallAlarm.interactable = true;
                break;
            default:
                break;
        }
    }
    private void PopDownAlarm()
    {
        switch (mainBrain.ActiveVirtualCamera?.VirtualCameraGameObject.GetComponent<CameraPositionSetting>().GetCameraPosition())
        {
            case CameraPositionSetting.CameraPosition.Hall:
                kitchenAlarm.transform.PopdownAnimation();
                kitchenAlarm.interactable = false;
                break;
            case CameraPositionSetting.CameraPosition.Kitchen:
                hallAlarm.transform.PopdownAnimation();
                hallAlarm.interactable = false;
                break;
            default:
                break;
        }
    }

    private void HallAlarmTouch()
    {
        hallCamera.gameObject.SetActive(true);
        PopDownAlarm();
    }

    private void KitchenAlarmTouch()
    {
        hallCamera.gameObject.SetActive(false);
        PopDownAlarm();
    }
}
