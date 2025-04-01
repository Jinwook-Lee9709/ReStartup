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
        for (int i = 0; i < dots.Count; i++)
        {
            StartCoroutine(FlashDot(i));
        }

        StartCoroutine(OnStartCoroutine());
    }

    private IEnumerator OnStartCoroutine()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private IEnumerator FlashDot(int idx)
    {
        yield return new WaitForSeconds(idx * 0.2f);
        dots[idx].GetComponent<Image>().DOColor(Color.black, 1f).SetLoops(-1,LoopType.Yoyo);
    }
}
