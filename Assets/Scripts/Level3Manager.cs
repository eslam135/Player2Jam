using UnityEngine;
using System.Collections;
using player2_sdk;

public class Level3Manager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject KaelChat;
    [SerializeField] private float minSpawnDistance = 2.5f;
    [SerializeField] private float maxSpawnDistance = 5f;
    [SerializeField] private float spawnInterval = 0.3f;      
    [SerializeField] private float waveInterval = 4f;
    [SerializeField] private Player2Npc Kael;
    [SerializeField] private GameObject Firesoul;
    [SerializeField] private GameObject SkillSCD;
    private int totalToSpawn;
    private int totalSpawned = 0;
    private int waveNumber = 1;

    private bool isSpawning = false;

    public static int killed_enemies = 0;
    private void Start()
    {
        Firesoul.SetActive(true);
        SkillSCD.SetActive(true);
        totalToSpawn = Random.Range(90, 120);  
        StartCoroutine(SpawnWaves());
    }
    private void Update()
    {
        if (killed_enemies == totalToSpawn)
        {
            GameManager.Instance.ChangeState(GameState.Level4);
        }
    }

    private IEnumerator SpawnWaves()
    {
        while (totalSpawned < totalToSpawn)
        {
            if (GameManager.Instance.isTalking)
            {
                yield return null;
                continue;
            }

            isSpawning = true;

            int enemiesThisWave = Mathf.Min(5 + waveNumber * 3, 20); 
            enemiesThisWave = Mathf.Min(enemiesThisWave, totalToSpawn - totalSpawned); 

            for (int i = 0; i < enemiesThisWave; i++)
            {
                SpawnRandomEnemy();
                totalSpawned++;
                yield return new WaitForSeconds(spawnInterval);
            }

            isSpawning = false;
            waveNumber++;

            yield return new WaitForSeconds(waveInterval);
        }

        Debug.Log("All goblins spawned.");
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
            Debug.LogWarning("No enemy prefab assigned!");
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
