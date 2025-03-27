using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FailText : MonoBehaviour
{
    public TextMeshProUGUI failText;
    private float fadeDuration = 2f;
    private Color defultColor;

    private void Start()
    {
        if (failText == null)
        {
            failText = GetComponent<TextMeshProUGUI>();
            defultColor = failText.color;
        }
        failText.color = defultColor;
        StartFadeOut();
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        Color originalColor = failText.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            failText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        failText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        gameObject.SetActive(false);
    }
}
