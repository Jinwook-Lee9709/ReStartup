using UnityEngine.InputSystem;
using UnityEngine;
using System;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hollCamera;
    [SerializeField]
    private float minSwipeDistance = 10f; // ��ũ�� �ȼ� ���� �ּ� �Ÿ�
    [SerializeField]
    private float swipeTolerance = 0.5f; // ���� �Ǵܿ� ����� ����

    private Vector2 startPos;
    private Vector2 endPos;

    private InputAction touchAction;

    private Vector2 pos;

    private bool isPressed;
    private bool isReleased;

    private void Start()
    {
        minSwipeDistance = Screen.width * 0.5f;
        touchAction = InputSystem.actions.FindAction("TouchAction");

        touchAction.started += (ctx) =>
        {
            isPressed = true;
        };

        touchAction.performed += (ctx) =>
        {
            endPos = pos;
            var distance = endPos.x - startPos.x;
            if (MathF.Abs(distance) < minSwipeDistance)
            {
                return;
            }
            hollCamera.SetActive(distance > 0);
        };
    }
    public void GetPos(InputAction.CallbackContext callbackContext)
    {
        pos = callbackContext.ReadValue<Vector2>();

        if (isPressed)
        {
            isPressed = false;
            startPos = pos;
        }
    }
}