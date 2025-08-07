using player2_sdk;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool isTalking;
    public bool isPaused;

    [SerializeField] private Player2Npc npc;
    [SerializeField] private GameObject[] npcChats;
    [SerializeField] private GameObject pauseParent;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject[] levels;
    [SerializeField] private GameState currentState = GameState.Level1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(i == (int)currentState);
        }
        pauseParent.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            isTalking = true;
            npcChats[0].SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseParent.SetActive(isPaused);
        }
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(i == (int)currentState);
        }
        if (isTalking || isPaused)
        {
            playerInput.DeactivateInput();
        }
        else
        {
            playerInput.ActivateInput();
        }
    }

    public void OnCloseButtonPress(GameObject obj)
    {
        // hide the UI panel
        obj.SetActive(false);

        isTalking = false;

        if (playerInput != null && !playerInput.enabled)
            playerInput.enabled = true;
    }

    public void ChangeState(GameState state)
    {
        currentState = state;
    }
    public void ResumeGame()
{
    isPaused = false;
    pauseParent.SetActive(false);

    // Reactivate player input
    if (playerInput != null)
        playerInput.ActivateInput();
}
    public GameState getCurrState()
    {
        return currentState;
    }
    public void backToMainMenu()
{
    Destroy(gameObject);
    Instance = null;

    SceneManager.LoadScene(0);
}
}

public enum GameState
{
    Level1,
    Level2,
    Level3,
    Level4,
}
