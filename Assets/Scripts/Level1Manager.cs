using player2_sdk;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;   

public class Level1Manager : MonoBehaviour
{
    [Header("UI & Password")]
    [SerializeField] private TMP_Text textField;
    [SerializeField] private GameObject passwordParent;
    [SerializeField] private TMP_InputField[] passwordFields;
    [SerializeField] private TMP_Text result;

    [Header("Player & NPC")]
    [Tooltip("Drag your Player object here (with PlayerInput component).")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Player2Npc npc;
    [SerializeField] private GameObject kaelChat;

    private bool nearHouse;
    private bool finishedTask;
    public static bool inText;
    
    private readonly string password = "4154";

    private void Start()
    {
        textField.text = "";

        // Configure each input field to only allow one digit
        foreach (var field in passwordFields)
        {
            field.characterLimit = 1;
            field.contentType    = TMP_InputField.ContentType.IntegerNumber;
            field.text           = "";
        }

        // ensure UI is hidden and player can move
        passwordParent.SetActive(false);
        inText = false;
        if (playerInput != null)
            playerInput.enabled = true;
    }

    private void Update()
    {
        // Toggle on E when near house and not already finished
        if (Input.GetKeyUp(KeyCode.E) && nearHouse && !finishedTask)
            TogglePasswordUI();
    }

    private void TogglePasswordUI()
    {
        inText = !inText;
        passwordParent.SetActive(inText);

        // disable all player input when typing
        if (playerInput != null)
            playerInput.enabled = !inText;

      
    }

    public void onCheckButtonPress()
    {
        bool correct = true;
        for (int i = 0; i < passwordFields.Length; i++)
        {
            var txt = passwordFields[i].text;
            if (string.IsNullOrEmpty(txt) || txt[0] != password[i])
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            result.color = Color.green;
            result.text  = "Congratulations! you got it right";
            TogglePasswordUI();

            _ = npc.SendChatMessageAsync(
                "This is a system message: The player just entered the correct password and found the memory fragment finishing phase 1",
                kaelChat
            );

            finishedTask = true;
            GameManager.Instance.ChangeState(GameState.Level2);
        }
        else
        {
            result.color = Color.red;
            result.text  = "Sorry, try again!";
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!finishedTask && collision.CompareTag("Player"))
        {
            textField.text = "Press E";
            nearHouse = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!finishedTask && collision.CompareTag("Player"))
        {
            textField.text = "";
            nearHouse = false;

            // also ensure UI closes if player walks away
            if (inText)
                TogglePasswordUI();
        }
    }
}
