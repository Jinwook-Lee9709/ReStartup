using System.Collections;
using TMPro;
using UnityEngine;

public class FailText : MonoBehaviour
{
    public TextMeshProUGUI failText;
    private readonly float fadeDuration = 2f;
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
        var originalColor = failText.color;
        var elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            var alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            failText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        failText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        gameObject.SetActive(false);
    }
}