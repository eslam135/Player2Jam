using player2_sdk;
using UnityEngine;

public class Level4Manager : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private Player2Npc Zarhakal;
    [SerializeField] private GameObject Kael;
    [SerializeField] private GameObject spawnPoint;

    void Start()
    {
        Kael.SetActive(false);
        Zarhakal.gameObject.SetActive(true);
        Instantiate(boss, spawnPoint.transform.position, Quaternion.identity);
    }

    void Update()
    {
        
    }
}
