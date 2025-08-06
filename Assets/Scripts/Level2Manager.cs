using player2_sdk;
using UnityEngine;

public class Level2Manager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject kaelChat;
    private Transform enemyParent;
    [SerializeField] private float minSpawnDistance = 2.5f;
    [SerializeField] private float maxSpawnDistance = 5f;
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private Player2Npc Kael;
    public static int foundFragments = 0;

    private float timer;

    private void Start()
    {
        GameObject parentObj = new GameObject("Enemies");
        enemyParent = parentObj.transform;
    }


    private void Update()
    {
        if (GameManager.Instance.isTalking) return;

        if (foundFragments < 3)
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnRandomEnemy();
            }
        }
        else
        {
            Destroy(enemyParent.gameObject);
            _ = Kael.SendChatMessageAsync("This is a system message: you and the player just collected all the three fragments", kaelChat);
            GameManager.Instance.ChangeState(GameState.Level3);
        }
    }

    private void SpawnRandomEnemy()
    {
        GameObject player = GameObject.Find("Player");
        if (!player)
        {
            Debug.LogWarning("Player not found!");
            return;
        }

        if (!enemyPrefab)
        {
            Debug.LogWarning("No enemy prefabs assigned!");
            return;
        }

        Vector2 spawnPos = GetRandomPositionAround(player.transform.position, minSpawnDistance, maxSpawnDistance);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity, enemyParent.transform);
    }

    private Vector2 GetRandomPositionAround(Vector2 center, float minDist, float maxDist)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float radius = Random.Range(minDist, maxDist);
        float x = center.x + Mathf.Cos(angle) * radius;
        float y = center.y + Mathf.Sin(angle) * radius;
        return new Vector2(x, y);
    }

}
