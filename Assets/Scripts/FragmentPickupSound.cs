using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FragmentPickupSound : MonoBehaviour
{
    [Header("Pickup SFX")]
    [Tooltip("Sound to play when the fragment is picked up.")]
    [SerializeField] private AudioClip pickupClip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; // don’t play automatically
    }

    /// <summary>
    /// Call this method via an Animation Event on your pickup animation.
    /// </summary>
    public void PlayPickupSound()
    {
        if (pickupClip != null)
            audioSource.PlayOneShot(pickupClip);
    }
}
