using UnityEngine;

public class HeadsUpDisplayUi : MonoBehaviour
{
    public GameObject smartPhoneButton;
    public GameObject smartPhoneIncludedButton;

    public bool includedUiSet = false;

    public void OnClickButtonSmartPhoneButton()
    {
        if (!includedUiSet)
        {
            var currentRotation = smartPhoneButton.transform.rotation.eulerAngles;
            currentRotation.z += 10f;
            smartPhoneButton.transform.rotation = Quaternion.Euler(currentRotation);
            smartPhoneIncludedButton.SetActive(true);
            includedUiSet = true;
        }
    }

    public void OnClickButtonCloseSmartPhoneButton()
    {
        includedUiSet = false;
        smartPhoneIncludedButton.SetActive(false);
        var currentRotation = smartPhoneButton.transform.rotation.eulerAngles;
        currentRotation.z -= 10f;
        smartPhoneButton.transform.rotation = Quaternion.Euler(currentRotation);
    }
}