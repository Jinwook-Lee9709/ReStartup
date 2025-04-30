using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateNameWarningPopup : PopUp
{
    [SerializeField] TextMeshProUGUI warningText;
    public void Init(string text)
    {
        warningText.text = text;
        StartCoroutine(popupCoroutine());
    }

    private IEnumerator popupCoroutine()
    {
        yield return new WaitForSeconds(2f);
        OnCancle();
    }
}
