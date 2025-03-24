using System.Collections;
using System.Collections.Generic;
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
            //�ӽ÷� �������� ��������Ʈ �ٲٴ°� ���ƺ���
            Vector3 currentRotation = smartPhoneButton.transform.rotation.eulerAngles;
            currentRotation.z += 10f;
            smartPhoneButton.transform.rotation = Quaternion.Euler(currentRotation);
            smartPhoneIncludedButton.SetActive(true);
            includedUiSet = true;
        }
    }
}
