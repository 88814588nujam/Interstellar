using UnityEngine;
using System.Collections.Generic;

public class CometCreater : MonoBehaviour
{
    public Vector2 spawnAreaMin = new Vector2(-8, -4);
    public Vector2 spawnAreaMax = new Vector2(8, 4);
    public int maxComets = 35;
    public float spawnRadius = 0.5f;
    public int maxAttempts = 50;
    public List<GameObject> cometPrefabs;

    private List<Vector2> spawnedPositions = new List<Vector2>();

    void Start()
    {
        SpawnNonOverlappingComets();
    }

    void SpawnNonOverlappingComets()
    {
        if (cometPrefabs == null || cometPrefabs.Count == 0) return;
        int successfullySpawned = 0;
        int attempts = 0;
        while (successfullySpawned < maxComets && attempts < maxComets * maxAttempts)
        {
            attempts++;
            Vector2 spawnPos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );
            if (!IsOverlapping(spawnPos))
            {
                int cometType = Random.Range(0, cometPrefabs.Count);
                GameObject comet = Instantiate(
                    cometPrefabs[cometType],
                    spawnPos,
                    Quaternion.Euler(0, 0, Random.Range(0, 360)),
                    this.transform
                );
                spawnedPositions.Add(spawnPos);
                successfullySpawned++;
            }
        }
    }

    private bool IsOverlapping(Vector2 position)
    {
        foreach (Vector2 existingPos in spawnedPositions)
        {
            if (Vector2.Distance(position, existingPos) < spawnRadius * 2)
                return true;
        }
        return false;
    }
}