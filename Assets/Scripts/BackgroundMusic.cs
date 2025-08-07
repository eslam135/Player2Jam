using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    [Header("BGM Settings")]
    [Tooltip("Assign your background music clip here.")]
    [SerializeField] private AudioClip musicClip;
    [Tooltip("Volume of the background music.")]
    [Range(0f,1f)]
    [SerializeField] private float volume = 0.5f;

    private AudioSource audioSource;

    private void Awake()
    {
        // Enforce singleton so we don't stack multiple music players
        var existing = FindObjectsOfType<BackgroundMusic>();
        if (existing.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        if (musicClip != null)
            audioSource.Play();
        else
            Debug.LogWarning("[BackgroundMusic] No music clip assigned!", this);
    }

    /// <summary>
    /// Fade the music volume over time (optional utility).
    /// </summary>
    public void FadeTo(float targetVolume, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(targetVolume, duration));
    }

    private IEnumerator FadeCoroutine(float targetVolume, float duration)
    {
        float startVol = audioSource.volume;
        float timer    = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, targetVolume, timer / duration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }
}
