using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    public void FadeToBlack()
    {
        StartCoroutine(FadeToBlackCoroutine());
    }

    private IEnumerator FadeToBlackCoroutine()
    {
        if (fadeCanvasGroup == null)
        {
            yield break;
        }

        fadeCanvasGroup.blocksRaycasts = true;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            fadeCanvasGroup.alpha = Mathf.Lerp(
                0f,
                1f,
                elapsedTime / fadeDuration
            );

            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }
}