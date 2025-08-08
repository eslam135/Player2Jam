using player2_sdk;
using UnityEngine;

public class Level4Manager : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private Player2Npc Zarhakal;
    [SerializeField] private GameObject zarhkalChat;
    [SerializeField] private GameObject spawnPoint;

    void Start()
    {

        // Kael.SetActive(false);
        Zarhakal.gameObject.SetActive(true);
        _ = Zarhakal.SendChatMessageAsync("This is a system message: The player and Kael just arrived in front of you after defeating the goblin army", zarhkalChat);

        boss.SetActive(true);
        Camera.main.GetComponent<CinemachineBossFocus>()?.FocusOnBossOnce();
    }

    void Update()
    {
        
    }
}
