using UnityEngine.InputSystem;
using UnityEngine;
using System;
using Unity.VisualScripting;
using JetBrains.Annotations;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hollCamera;
    [SerializeField]
    private float minSwipeDistance = 10f; // 스크린 픽셀 기준 최소 거리
    [SerializeField]
    private float swipeTolerance = 0.5f; // 방향 판단에 허용할 오차

    public Player player;
    private Vector2 startPos;
    private Vector2 endPos;

    private InputAction slowTouchAction;
    private InputAction touchAction;

    private Vector2 pos;

    private bool isPressed;
    private bool isReleased;

    private bool slowTouchDetected = false;

    public LayerMask clickableLayers;

    private void Start()
    {
        minSwipeDistance = Screen.width * 0.5f;
        slowTouchAction = InputSystem.actions.FindAction("SlowTouchAction");
        slowTouchAction.started += (ctx) =>
        {
            isPressed = true;
        };

        slowTouchAction.performed += (ctx) =>
        {
            slowTouchDetected = true;
            endPos = pos;
            var distance = endPos.x - startPos.x;
            if (MathF.Abs(distance) < minSwipeDistance)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(pos); // 마우스 좌표를 월드 좌표로 변환
                RaycastHit2D[] hit = Physics2D.RaycastAll(worldPoint, Vector2.zero);

                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                bool cheakWork = false;
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].collider != null)
                    {
                        if (hit[i].collider.CompareTag("Work"))
                        {
                            cheakWork = true;
                        }
                    }
                }
                player.OnMoveOrWork(cheakWork, worldPoint);
                return;
            }
            hollCamera.SetActive(distance > 0);
        };

        touchAction = InputSystem.actions.FindAction("TouchAction");

        touchAction.canceled += (ctx) =>
        {
            if (slowTouchDetected)
            {
                slowTouchDetected = false; // 슬로우 터치가 실행되었으면 일반 터치 무시
                return;
            }

            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(pos); // 마우스 좌표를 월드 좌표로 변환
            RaycastHit2D[] hit = Physics2D.RaycastAll(worldPoint, Vector2.zero);

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            bool cheakWork = false;
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider != null)
                {
                    if (hit[i].collider.CompareTag("Work"))
                    {
                        cheakWork = true;
                    }
                }
            }
            player.OnMoveOrWork(cheakWork, worldPoint);
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