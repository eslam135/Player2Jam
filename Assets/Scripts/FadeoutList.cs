using System.Collections;
using UnityEngine;

public class FadeoutList : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] renderersToFade;
    [SerializeField] private SpriteRenderer[] renderersToShow;

    [SerializeField] private float fadeDuration = 1.0f;

    [SerializeField] private float targetAlpha = 0f;

    [SerializeField] private float targetShowAlpha = 1f;


    private bool hasFaded = false;

    private bool hasFadedIn = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasFaded && collision.CompareTag("Player"))
        {
            foreach (var sr in renderersToFade)
            {
                StartCoroutine(FadeOut(sr, fadeDuration, targetAlpha));
            }

            hasFaded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!hasFadedIn && collision.CompareTag("Player"))
        {
            foreach (var sr in renderersToShow)
            {
                StartCoroutine(FadeIn(sr, fadeDuration, targetShowAlpha));
            }

            hasFadedIn = true;
        }
    }
    IEnumerator FadeOut(SpriteRenderer sr, float duration, float targetAlpha)
    {
        float startAlpha = sr.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            yield return null;
        }

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, targetAlpha);
    }
    IEnumerator FadeIn(SpriteRenderer sr, float duration, float targetAlpha)
    {
        float startAlpha = sr.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            yield return null;
        }

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, targetAlpha);
        Debug.Log(sr.color.a);
    }
}
