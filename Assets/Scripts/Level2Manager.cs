using UnityEngine;

public class Level2Manager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; 
    [SerializeField] private float minSpawnDistance = 2.5f;
    [SerializeField] private float maxSpawnDistance = 5f;
    [SerializeField] private float spawnInterval = 0.8f;

    private float timer;

    private void Update()
    {
        if (GameManager.Instance.isTalking) return;

            timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRandomEnemy();
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

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
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
