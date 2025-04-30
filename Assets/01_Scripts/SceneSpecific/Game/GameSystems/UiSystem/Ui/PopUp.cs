using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopUp : MonoBehaviour
{
    [SerializeField] protected Button backGround;
    [SerializeField] protected GameObject popupUi;
    protected virtual void Start()
    {
        backGround.interactable = false;
        popupUi.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutElastic).OnComplete(() => backGround.interactable = true);
        backGround.onClick.AddListener(OnCancle);
    }

    protected virtual void OnCancle()
    {
        StopAllCoroutines();
        backGround.interactable = false;
        popupUi.transform.DOScale(0f, 0.5f).SetEase(Ease.InOutElastic).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }


}
