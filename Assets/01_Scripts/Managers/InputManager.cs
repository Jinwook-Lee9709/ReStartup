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

    private bool soundOn = false;
    private float currentTime = 0f;
    private float swipeTime = 2f;

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
        slowTouchAction.started += OnSlowTouchStarted;

        slowTouchAction.performed += OnSlowTouchPerformed;

        touchAction = InputSystem.actions.FindAction("TouchAction");

        touchAction.canceled += OnTouchActionCanceled;

        player.UpdateIdleArea(hollCamera.activeSelf);
    }
    private void Update()
    {
        if (currentTime >= swipeTime)
        {
            soundOn = true;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }
    private void OnSlowTouchStarted(InputAction.CallbackContext callbackContext)
    {
        AudioManager.Instance.PlaySFX("Touch");
        if (IsPointerOverUI())
            return;

        isPressed = true;
    }

    private void OnSlowTouchPerformed(InputAction.CallbackContext callbackContext)
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
        if (soundOn)
        {
            if (isCameraOnHall)
            {
                AudioManager.Instance.PlaySFX("ChangeHall");
            }
            else
            {
                AudioManager.Instance.PlaySFX("ChangeKitchen");
            }
        }
        hollCamera.SetActive(isCameraOnHall);
        player.UpdateIdleArea(isCameraOnHall);
    }

    private void OnTouchActionCanceled(InputAction.CallbackContext callbackContext)
    {
        if (IsPointerOverUI()) return;

        if (slowTouchDetected)
        {
            slowTouchDetected = false;
            return;
        }

        player.OnTouch(pos);
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
    private void OnDestroy()
    {
        slowTouchAction.started -= OnSlowTouchStarted;

        slowTouchAction.performed -= OnSlowTouchPerformed;

        touchAction = InputSystem.actions.FindAction("TouchAction");

        touchAction.canceled -= OnTouchActionCanceled;
    }
}