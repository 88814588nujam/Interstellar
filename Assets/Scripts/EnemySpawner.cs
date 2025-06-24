using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    public enum EnemyType
    {
        EnemyPlatform = 0,
        Suicide = 1,
        Comet = 2,
        Boss = 3
    }

    [System.Serializable]
    public class EnemyWave
    {
        public GameObject enemyPrefab;
        public EnemyType enemyType;
        public float spawnAreaX = 0f;
        public float spawnAreaY = 0f;
        public float delayBeforeNextWave = 1f;
    }

    [Header("Basic Setting")]
    public GameObject bossPrefab;
    public float delayBossWave = 0f;
    public float startDelay = 0f;
    public List<EnemyWave> enemyWaves;
    public AudioSource racingAudio;
    public AudioSource emergencyAudio;
    public AudioSource warningAudio;

    [Header("Loop Setting")]
    public bool loopWaves = false;
    public int loopTimes = 0;

    [Header("Test Setting")]
    public int startIndex = 0;

    private bool gameEnd = false;
    private float timer = 0f;
    private bool hasExecuted = false;

    void Update()
    {
        if (!hasExecuted)
        {
            timer += Time.deltaTime;
            if (timer >= startDelay)
            {
                StartCoroutine(SpawnWaves());
                hasExecuted = true;
            }
        }
    }

    IEnumerator SpawnWaves()
    {
        do
        {
            if (loopTimes > 0) loopTimes--;
            for(int i = startIndex; i < enemyWaves.Count; i++)
            {
                PlayerController playerController = FindFirstObjectByType<PlayerController>();
                if (playerController == null) {
                    gameEnd = true;
                    loopWaves = false;
                    break;
                }
                var wave = enemyWaves[i];
                Vector2 spawnPos = new Vector2(wave.spawnAreaX, wave.spawnAreaY);
                GameObject enemy = Instantiate(wave.enemyPrefab, spawnPos, Quaternion.identity);
                switch (wave.enemyType)
                {
                    case EnemyType.EnemyPlatform:
                    case EnemyType.Suicide:
                    case EnemyType.Comet:
                    case EnemyType.Boss:
                    default: break;
                }
                yield return new WaitForSeconds(wave.delayBeforeNextWave);
            }
            if (loopTimes == 0) loopWaves = false;
        } while (loopWaves);
        if (!gameEnd) { 
            racingAudio.Pause();
            warningAudio.Play();
            emergencyAudio.Play();
            yield return new WaitForSeconds(delayBossWave);
            GameObject boss = Instantiate(bossPrefab, new Vector3(0f, 7f, 0f), Quaternion.identity);
        }
    }

    public void chageMusic() {
        emergencyAudio.Pause();
        racingAudio.Play();
    }
}