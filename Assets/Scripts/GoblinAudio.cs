using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GoblinAudio : MonoBehaviour
{
    [Header("Activation SFX")]
    [Tooltip("Sound to play when the goblin activates/enables.")]
    [SerializeField] private AudioClip activationClip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // We don’t want it playing on Awake automatically:
        audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        // Play the activation clip whenever this GameObject becomes active
        if (activationClip != null)
            audioSource.PlayOneShot(activationClip);
    }
}
