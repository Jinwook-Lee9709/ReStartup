using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeHpAllRecoveryUIItem : MonoBehaviour
{
    private readonly string recovery = "Recovery";
    [SerializeField] private int recoveryValue;
    [SerializeField] private EmployeeHpUi employeeHpUi;
    [SerializeField] private CostType costType;
    [SerializeField] private int costValue;
    [SerializeField] private TextMeshProUGUI costValueText, nameText, recoveryValueText;
    [SerializeField] private string itemName;
    [SerializeField] private RectTransform objectsTransform;
    [SerializeField] private Image image;
    private float widthRatio = 0.35f;
    private float heigthRatio = 0.4f;
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
        costValueText.text = costValue.ToString();
        nameText.text = LZString.GetUIString(itemName);
        recoveryValueText.text = $"{recoveryValue.ToString()} {LZString.GetUIString(recovery)}";
        StartCoroutine(DelayedSetup());
    }
    private void OnButtonClick()
    {
        employeeHpUi.EmployeeAllRecovery(recoveryValue, costType, costValue);
    }
    public void Update()
    {
    }
    IEnumerator DelayedSetup()
    {
        yield return null;
        yield return null;

        float newWidth = GetComponent<RectTransform>().rect.size.x * widthRatio;
        float newheigth = GetComponent<RectTransform>().rect.size.y * heigthRatio;
        Vector2 newSize = new(newWidth, objectsTransform.rect.size.y);
        objectsTransform.sizeDelta = newSize;
        float newXPos = newSize.x / 2f;
        Vector2 newImageSize = new(newheigth, newheigth);
        image.GetComponent<RectTransform>().sizeDelta = newImageSize;
    }
}
