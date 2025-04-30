using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeHpAllRecoveryButtons : MonoBehaviour
{
    private float widthRatio = 0.45f;
    private float heightRatio = 0.4f;
    [SerializeField] private RectTransform parentTransform;
    void Start()
    {
        StartCoroutine(DelayedSetup());
    }
    IEnumerator DelayedSetup()
    {
        yield return null; // 한 프레임 대기

        var gridLayout = GetComponent<GridLayoutGroup>();
        float newWidth = parentTransform.rect.size.x * widthRatio;
        float newHeight = parentTransform.rect.size.y * heightRatio;

        gridLayout.cellSize = new Vector2(newWidth, newHeight);
    }
}
