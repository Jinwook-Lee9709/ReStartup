using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Alarm : MonoBehaviour
{
    [SerializeField] private Button hallAlarm, kitchenAlarm;
    [SerializeField] private CinemachineVirtualCamera hallCamera;
    private Player player;
    private CinemachineBrain mainBrain;
    private bool isTouchedFrame = false;
    private void Start()
    {
        kitchenAlarm.onClick.AddListener(KitchenAlarmTouch);
        hallAlarm.onClick.AddListener(HallAlarmTouch);
        hallAlarm.interactable = false;
        kitchenAlarm.interactable = false;
        mainBrain = Camera.main.GetComponent<CinemachineBrain>();
        player = GameObject.FindGameObjectWithTag(Strings.PlayerTag).GetComponent<Player>();
    }

    public void UpdateAlarm(HashSet<WorkBase> needHallAlarmWorks, HashSet<WorkBase> needKitchenAlarmWorks)
    {
        if(needHallAlarmWorks.Count > 0)
        {
            AlarmActive(WorkType.Hall);
        }
        else if(needKitchenAlarmWorks.Count > 0)
        {
            AlarmActive(WorkType.Kitchen);
        }
        else
        {
            AlarmUnActive(WorkType.Hall);
            AlarmUnActive(WorkType.Kitchen);
        }

        AlarmUnActive();
    }



    public void AfterUpdateAlarm(Dictionary<WorkType, SortedSet<WorkBase>> workQueue)
    {
        foreach (var workType in workQueue.Values)
        {
            switch (mainBrain.ActiveVirtualCamera?.VirtualCameraGameObject.GetComponent<CameraPositionSetting>().GetCameraPosition())
            {
                case CameraPositionSetting.CameraPosition.Hall:
                    if (kitchenAlarm.interactable == false)
                        return;
                    if (workQueue[WorkType.Kitchen].Count <= 0)
                    {
                        AlarmUnActive(WorkType.Kitchen);
                    }
                    break;

                case CameraPositionSetting.CameraPosition.Kitchen:

                    if (hallAlarm.interactable == false)
                        return;
                    if (workQueue[WorkType.Payment].Count <= 0 && workQueue[WorkType.Hall].Count <= 0)
                    {
                        AlarmUnActive(WorkType.Hall);
                    }
                    break;
            }
        }
    }

    private void AlarmActive(WorkType type)
    {
        switch (type)
        {
            case WorkType.Payment:
            case WorkType.Hall:
                if (hallAlarm.interactable == true || isTouchedFrame)
                {
                    isTouchedFrame = !isTouchedFrame;
                    return;
                }
                if (mainBrain.ActiveVirtualCamera?.VirtualCameraGameObject.GetComponent<CameraPositionSetting>().GetCameraPosition() == CameraPositionSetting.CameraPosition.Kitchen)
                {
                    hallAlarm.transform.PopupAnimation();
                    hallAlarm.interactable = true;
                }
                break;
            case WorkType.Kitchen:
                if (kitchenAlarm.interactable == true || isTouchedFrame)
                {
                    isTouchedFrame = !isTouchedFrame;
                    return;
                }

                if (mainBrain.ActiveVirtualCamera?.VirtualCameraGameObject.GetComponent<CameraPositionSetting>().GetCameraPosition() == CameraPositionSetting.CameraPosition.Hall)
                {
                    kitchenAlarm.transform.PopupAnimation();
                    kitchenAlarm.interactable = true;
                }
                break;
        }
    }

    private void AlarmActive(WorkBase work)
    {
        switch (work.workType)
        {
            case WorkType.Payment:
            case WorkType.Hall:
                if (hallAlarm.interactable == true || isTouchedFrame)
                {
                    isTouchedFrame = !isTouchedFrame;
                    return;
                }
                if (mainBrain.ActiveVirtualCamera?.VirtualCameraGameObject.GetComponent<CameraPositionSetting>().GetCameraPosition() == CameraPositionSetting.CameraPosition.Kitchen)
                {
                    hallAlarm.transform.PopupAnimation();
                    hallAlarm.interactable = true;
                }

                break;
            case WorkType.Kitchen:
                if (kitchenAlarm.interactable == true || isTouchedFrame)
                {
                    isTouchedFrame = !isTouchedFrame;
                    return;
                }

                if (mainBrain.ActiveVirtualCamera?.VirtualCameraGameObject.GetComponent<CameraPositionSetting>().GetCameraPosition() == CameraPositionSetting.CameraPosition.Hall)
                {
                    kitchenAlarm.transform.PopupAnimation();
                    kitchenAlarm.interactable = true;
                }
                break;
        }
    }
    private void AlarmUnActive()
    {
        switch (mainBrain.ActiveVirtualCamera?.VirtualCameraGameObject.GetComponent<CameraPositionSetting>().GetCameraPosition())
        {
            case CameraPositionSetting.CameraPosition.Hall:
                if (hallAlarm.interactable == false)
                    return;
                hallAlarm.transform.PopdownAnimation();
                hallAlarm.interactable = false;
                break;
            case CameraPositionSetting.CameraPosition.Kitchen:
                if (kitchenAlarm.interactable == false)
                    return;
                kitchenAlarm.transform.PopdownAnimation();
                kitchenAlarm.interactable = false;
                break;
            default:
                break;
        }
    }

    private void AlarmUnActive(WorkType type)
    {
        switch (type)
        {
            case WorkType.Payment:
            case WorkType.Hall:
                if (hallAlarm.interactable == false)
                    return;
                hallAlarm.transform.PopdownAnimation();
                hallAlarm.interactable = false;
                break;
            case WorkType.Kitchen:
                if (kitchenAlarm.interactable == false)
                    return;
                kitchenAlarm.transform.PopdownAnimation();
                kitchenAlarm.interactable = false;
                break;
        }
    }

    public void PopDownAlarm()
    {
        switch (mainBrain.ActiveVirtualCamera?.VirtualCameraGameObject.GetComponent<CameraPositionSetting>().GetCameraPosition())
        {
            case CameraPositionSetting.CameraPosition.Hall:
                if (kitchenAlarm.interactable == false)
                    return;
                kitchenAlarm.transform.PopdownAnimation();
                kitchenAlarm.interactable = false;
                break;
            case CameraPositionSetting.CameraPosition.Kitchen:
                if (hallAlarm.interactable == false)
                    return;
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
        isTouchedFrame = true;
        player.UpdateIdleArea(true);
    }

    private void KitchenAlarmTouch()
    {
        hallCamera.gameObject.SetActive(false);
        PopDownAlarm();
        isTouchedFrame = true;
        player.UpdateIdleArea(false);
    }
}
