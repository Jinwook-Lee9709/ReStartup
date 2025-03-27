using UnityEngine;

public class CameraPositionSetting : MonoBehaviour
{
    void Start()
    {
        Vector3 screenWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 1.5f, Screen.height * 0.5f, 0f));
        transform.position = screenWidth;
    }
}
