using player2_sdk;
using TMPro;
using UnityEngine;

public class Level1Manager : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;
    [SerializeField] private GameObject passwordParent;
    [SerializeField] private TMP_InputField[] passwordFields;
    [SerializeField] private TMP_Text result;
    [SerializeField] private Player2Npc npc;
    private bool nearHouse;
    private bool finishedTask;
    public static bool inText;
    readonly private string password = "4154";
    private void Start()
    {
        textField.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (nearHouse)
            {
                passwordParent.SetActive(!inText);
                inText = !inText;
            }
        }
    }

    public void onCheckButtonPress()
    {
        bool correct = true;
        for (int i = 0; i < passwordFields.Length; i++)
        {
            if (passwordFields[i].text[0] != password[i])
            {
                correct = false;
                break;
            }
        }
        if (correct)
        {
            result.color = Color.green;
            result.text = "Congratulations! you got it right";
            passwordParent.SetActive(false);
            _ = npc.SendChatMessageAsync("The player just entered the correct password and found the memory fragment finishing phase 1");
            finishedTask = true;
            GameManager.Instance.ChangeState(GameState.Level2);
        }
        else
        {
            result.color = Color.red;
            result.text = "Sorry, try again!";
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!finishedTask)
        {
            if (collision.gameObject.tag == "Player")
            {
                textField.text = "Press E";
                nearHouse = true;
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!finishedTask)
        {
            if (collision.gameObject.tag == "Player")
            {
                textField.text = "";
                nearHouse = false;
            }
        }
    }
}
