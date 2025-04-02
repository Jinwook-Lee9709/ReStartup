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
        popupUi.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutElastic);
        backGround.onClick.AddListener(OnCancle);
    }

    protected virtual void OnCancle()
    {
        popupUi.transform.DOScale(0f, 0.5f).SetEase(Ease.InOutElastic).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }


}
