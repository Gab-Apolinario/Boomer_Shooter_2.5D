using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public struct WaveData
    {
        public int meleeCount;
        public int rangedCount;
    }

    [Header("Waves")]
    public List<WaveData> waves;

    [Header("Prefabs")]
    public GameObject meleeEnemyPrefab;
    public GameObject rangedEnemyPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Configuração")]
    public float timeBetweenWaves = 3f;

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;

    private void OnEnable()
    {
        Acoes.OnEnemyDie += HandleEnemyDie;
    }

    private void OnDisable()
    {
        Acoes.OnEnemyDie -= HandleEnemyDie;
    }

    private void Start()
    {
        StartCoroutine(StartWaveWithDelay(0f));
    }

    private void HandleEnemyDie()
    {
        enemiesAlive--;

        if (enemiesAlive > 0)
        {
            return;
        }

        //Daqui para baixo é quando a wave acaba (ultimo inimigo morreu)
        bool isLastWave = currentWaveIndex >= waves.Count - 1;

        if (isLastWave)
        {
            Acoes.OnLastWaveFinished?.Invoke();
        }
        else
        {
            Acoes.OnAllEnemiesDead?.Invoke();
            currentWaveIndex++;
            StartCoroutine(StartWaveWithDelay(timeBetweenWaves));
        }
    }

    private IEnumerator StartWaveWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnWave(waves[currentWaveIndex]);
    }

    private void SpawnWave(WaveData wave)
    {
        enemiesAlive = wave.meleeCount + wave.rangedCount;

        List<int> availablePoints = GetShuffledSpawnIndices();
        int spawnIndex = 0;

        for (int i = 0; i < wave.meleeCount; i++)
        {
            SpawnEnemy(meleeEnemyPrefab, availablePoints[spawnIndex % availablePoints.Count]);
            spawnIndex++;
        }

        for (int i = 0; i < wave.rangedCount; i++)
        {
            SpawnEnemy(rangedEnemyPrefab, availablePoints[spawnIndex % availablePoints.Count]);
            spawnIndex++;
        }
    }

    private void SpawnEnemy(GameObject prefab, int pointIndex)
    {
        Transform point = spawnPoints[pointIndex];
        Instantiate(prefab, point.position, point.rotation);
    }

    private List<int> GetShuffledSpawnIndices()
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
            indices.Add(i);

        for (int i = indices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        return indices;
    }
}