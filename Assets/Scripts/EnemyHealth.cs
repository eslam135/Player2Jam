using player2_sdk;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public int health = 1;
    private int hitPoint;
    private int maxHitPoint;

    [Header("UI (Boss Only)")]
    [Tooltip("Parent GameObject containing the health bar Image & Text.")]
    [SerializeField] private GameObject healthBarContainer;
    [Tooltip("The Image that fills/depletes.")]
    [SerializeField] private Image currentHealthBar;
    [Tooltip("The Text/TMP_Text showing “x / y”.")]
    [SerializeField] private Text healthText;

    [Header("Audio")]
    [Tooltip("Played when boss dies.")]
    [SerializeField] private AudioClip deathClip;
    [Tooltip("Played each time boss takes damage.")]
    [SerializeField] private AudioClip damageClip;
    private AudioSource audioSource;

    private Animator anim;
    private bool isDying = false;
    private static readonly int HashDeath = Animator.StringToHash("Death");

    [Header("Loot & Chat")]
    [SerializeField] private GameObject fragment;
    [SerializeField] public GameObject Kalecanva;
    [SerializeField] public Player2Npc player2npc;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        hitPoint = health;
        maxHitPoint = health;

        // start hidden
        if (healthBarContainer != null)
            healthBarContainer.SetActive(false);
    }

    private void ShowHealthUI()
    {
        if (healthBarContainer != null)
            healthBarContainer.SetActive(true);
    }

    private void UpdateHealthBar()
    {
        float ratio = (float)hitPoint / maxHitPoint;
        float fullWidth = currentHealthBar.rectTransform.rect.width;

        currentHealthBar.rectTransform.localPosition =
            new Vector3(fullWidth * ratio - fullWidth, 0f, 0f);

        healthText.text = $"{hitPoint:0} / {maxHitPoint:0}";
    }

    public void TakeDamage(int amount)
    {
        if (isDying) return;

        // reveal bar on first hit
        if (hitPoint == maxHitPoint)
            ShowHealthUI();

        hitPoint -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {hitPoint}");

        // play the damage sound
        if (damageClip != null && audioSource != null)
            audioSource.PlayOneShot(damageClip);

        // update UI
        if (healthBarContainer != null)
            UpdateHealthBar();

        if (hitPoint <= 0)
        {
            isDying = true;
            StopEnemyMovement();
            anim.SetTrigger(HashDeath);
        }
    }

    private void StopEnemyMovement()
    {
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }
        if (TryGetComponent<Enemy_Movement>(out var move)) move.enabled = false;
        if (TryGetComponent<EnemyAttack>(out var atk)) atk.enabled = false;
    }

    public void PlayDeathSound()
    {
        if (deathClip != null && audioSource != null)
            audioSource.PlayOneShot(deathClip);
    }

    public void OnDeathAnimationFinished()
    {
        if (GameManager.Instance.getCurrState() == GameState.Level3)
            Level3Manager.killed_enemies += 1;

        spawnFragment();
        Destroy(gameObject);
    }

    private void spawnFragment()
    {
        if (GameManager.Instance.getCurrState() == GameState.Level2 &&
            Random.value < 0.2f)
        {
            Instantiate(fragment, transform.position, Quaternion.identity);
        }
    }

    // private void OnBossDeath()
    // {
    //     if (player2npc != null && Kalecanva != null)
    //     {
    //         _ = player2npc.SendChatMessageAsync(
    //             "This is a system message: The player just entered the correct password and found the memory fragment finishing phase 1",
    //             Kalecanva
    //         );
    //     }
    // }
    [SerializeField] private GameObject winScreen;
    [SerializeField] private AudioClip winSound;
    public void OnBossDeath()
    {
        StartCoroutine(ShowWinScreenAndGoToMenu());
    }

    private IEnumerator ShowWinScreenAndGoToMenu()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);

            if (winSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(winSound);
            }
            else
            {
                Debug.LogWarning("Win sound AudioClip or AudioSource not assigned!");
            }

            yield return new WaitForSeconds(2f);
            if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogWarning("Win screen GameObject is not assigned!");
        }
    }
}
