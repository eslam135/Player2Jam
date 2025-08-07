using player2_sdk;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool isTalking;

    [SerializeField] private Player2Npc npc;
    [SerializeField] private GameObject[] npcChats;
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

    }

    private void Update()
    {

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(i == (int)currentState);
        }
        if (isTalking)
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
    public GameState getCurrState()
    {
        return currentState;
    }
}

public enum GameState
{
    Level1,
    Level2,
    Level3,
    Level4,
}
