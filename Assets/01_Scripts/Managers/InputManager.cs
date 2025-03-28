using System;
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
    private bool isReleased;

    private Vector2 pos;

    private InputAction slowTouchAction;

    private bool slowTouchDetected;
    private Vector2 startPos;
    private InputAction touchAction;

    [SerializeField][Range(0f, 1f)] private float swipeDistanceCalcParam = 0.05f;

    [ContextMenu("SwipeTest")]
    public void SwipeTest()
    {
        minSwipeDistance = Screen.width * swipeDistanceCalcParam;
    }

    private void Start()
    {
        minSwipeDistance = Screen.width * swipeDistanceCalcParam;
        slowTouchAction = InputSystem.actions.FindAction("SlowTouchAction");
        slowTouchAction.started += ctx => { isPressed = true; };

        slowTouchAction.performed += ctx =>
        {
            slowTouchDetected = true;
            endPos = pos;
            var distance = endPos.x - startPos.x;
            if (MathF.Abs(distance) < minSwipeDistance)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(pos); // ���콺 ��ǥ�� ���� ��ǥ�� ��ȯ
                var hit = Physics2D.RaycastAll(worldPoint, Vector2.zero);

                if (EventSystem.current.IsPointerOverGameObject()) return;
                var cheakWork = false;
                for (var i = 0; i < hit.Length; i++)
                    if (hit[i].collider != null)
                        if (hit[i].collider.CompareTag("Work"))
                            cheakWork = true;

                player.OnMoveOrWork(cheakWork, worldPoint);
                return;
            }

            hollCamera.SetActive(distance > 0);
        };

        touchAction = InputSystem.actions.FindAction("TouchAction");

        touchAction.canceled += ctx =>
        {
            if (slowTouchDetected)
            {
                slowTouchDetected = false; // ���ο� ��ġ�� ����Ǿ����� �Ϲ� ��ġ ����
                return;
            }

            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(pos); // ���콺 ��ǥ�� ���� ��ǥ�� ��ȯ
            var hit = Physics2D.RaycastAll(worldPoint, Vector2.zero);

            if (EventSystem.current.IsPointerOverGameObject()) return;
            var cheakWork = false;
            for (var i = 0; i < hit.Length; i++)
                if (hit[i].collider != null)
                    if (hit[i].collider.CompareTag("Work"))
                        cheakWork = true;

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