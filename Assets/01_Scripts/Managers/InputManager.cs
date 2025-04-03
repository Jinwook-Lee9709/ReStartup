using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject hollCamera;

    [SerializeField] private float minSwipeDistance = 10f; // ��ũ�� �ȼ� ���� �ּ� �Ÿ�

    [SerializeField] private float swipeTolerance = 0.5f; // ���� �Ǵܿ� ����� ����

    public Player player;

    public LayerMask clickableLayers;
    private Vector2 endPos;

    private bool isPressed;

    private Vector2 pos;

    private InputAction slowTouchAction;

    private bool slowTouchDetected;
    private bool isSwipe;
    private Vector2 startPos;
    private InputAction touchAction;

    [SerializeField][Range(0f, 1f)] private float swipeDistanceCalcParam = 0.2f;

    [ContextMenu("SwipeTest")]
    public void SwipeTest()
    {
        minSwipeDistance = Screen.width * swipeDistanceCalcParam;
    }
    private void OnEnable()
    {
        slowTouchAction = InputSystem.actions.FindAction("SlowTouchAction");
        slowTouchAction.Enable();
    }

    private void Start()
    {
        minSwipeDistance = Screen.width * swipeDistanceCalcParam;
        slowTouchAction = InputSystem.actions.FindAction("SlowTouchAction");
        slowTouchAction.started += ctx =>
        {
            if (IsPointerOverUI()) 
                return;

            isPressed = true;
        };

        slowTouchAction.performed += ctx =>
        {
            if (IsPointerOverUI()) return;
            slowTouchDetected = true;
            endPos = pos;
            var distance = endPos.x - startPos.x;
            if (MathF.Abs(distance) < minSwipeDistance)
            {
                player.OnTouch(pos);
                return;
            }
            bool isCameraOnHall = distance > 0;
            hollCamera.SetActive(isCameraOnHall);
            player.UpdateIdleArea(isCameraOnHall);
        };

        touchAction = InputSystem.actions.FindAction("TouchAction");

        touchAction.canceled += ctx =>
        {
            if (IsPointerOverUI()) return;

            if (slowTouchDetected)
            {
                slowTouchDetected = false;
                return;
            }

            player.OnTouch(pos);
        };
        
        player.UpdateIdleArea(hollCamera.activeSelf);
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
    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Pointer.current.position.ReadValue() // 현재 포인터 위치 가져오기
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0; // UI 요소를 감지했으면 true 반환
    }
}