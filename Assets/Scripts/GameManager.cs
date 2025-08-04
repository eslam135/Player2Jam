using player2_sdk;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player2Npc npc;
    [SerializeField] private GameObject npcChat;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject[] levels;
    private GameState currentState = GameState.Level1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    private void Update()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(i == (int)currentState);
        }
        if (npcChat.activeInHierarchy)
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
        obj.SetActive(false);
    }

    public void ChangeState(GameState state)
    {
        currentState = state;
    }
}

public enum GameState
{
    Level1,
    Level2,
    Level3
}
