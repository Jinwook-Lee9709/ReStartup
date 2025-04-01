using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingDot : MonoBehaviour
{
    [SerializeField] private List<GameObject> dots;

    private void Start()
    {
        transform.DOScale(1f, 0.5f);
        for (int i = 0; i < dots.Count; i++)
        {
            StartCoroutine(FlashDot(i));
        }
    }

    public void StopDoTween()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].GetComponent<Image>().DOKill();
        }
    }

    private IEnumerator FlashDot(int idx)
    {
        yield return new WaitForSeconds(idx * 0.2f);
        dots[idx].GetComponent<Image>().DOColor(Color.black, 1f).SetLoops(-1,LoopType.Yoyo);
    }
}
