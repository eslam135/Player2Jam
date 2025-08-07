using System.Collections;
using UnityEngine;

public class FadeoutList : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] renderersToFade;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float fadeDistance = 5f;

    private Transform player;
    private bool isFading = false;
    private bool isVisible = true; // True = currently shown

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the Player object has the 'Player' tag.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > fadeDistance && isVisible && !isFading)
        {
            // Player is far → fade out
            StartCoroutine(FadeAllTo(0f));
            isVisible = false;
        }
        else if (distance <= fadeDistance && !isVisible && !isFading)
        {
            // Player is close → fade in
            StartCoroutine(FadeAllTo(1f));
            isVisible = true;
        }
    }

    IEnumerator FadeAllTo(float targetAlpha)
    {
        isFading = true;

        foreach (var sr in renderersToFade)
        {
            StartCoroutine(FadeToAlpha(sr, fadeDuration, targetAlpha));
        }

        yield return new WaitForSeconds(fadeDuration);
        isFading = false;
    }

    IEnumerator FadeToAlpha(SpriteRenderer sr, float duration, float targetAlpha)
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
}
