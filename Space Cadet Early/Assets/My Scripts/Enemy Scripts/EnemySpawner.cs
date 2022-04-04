using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    private int nextWave = 0;
    public int NextWave
    {
        get { return nextWave + 1; }
    }

    public List<Transform> spawnPoints;

    public float timeBetweenWaves = 5f;
    private float waveCountdown;
    public float WaveCountdown
    {
        get { return waveCountdown; }
    }

    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;
    public SpawnState State
    {
        get { return state; }
    }

    private float maxLength;
    private float currentWave = 0;
    public bool canSpawn = true;

    void Start()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points referenced.");
        }

        waveCountdown = timeBetweenWaves;
        maxLength = waves.Length;
    }

    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            }
            else
            {
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING && currentWave!=maxLength)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
                currentWave++;
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }

        if(currentWave >= maxLength)
        {
            Debug.Log("All waves from the script have been finished");
        }
    }
    
    void WaveCompleted()
    {
        Debug.Log("Wave Completed!");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length)
        {
            nextWave = 0;
 
        }
        else
        {
            nextWave++;
        }
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        //Debug.Log("Spawning Wave: " + _wave.name);
        state = SpawnState.SPAWNING;

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        state = SpawnState.WAITING;

        yield break;
    }

    void SpawnEnemy(Transform _enemy)
    {
        //Debug.Log("Spawning Enemy: " + _enemy.name);

        List<Transform> freeSpawnPoints = new List<Transform>(spawnPoints);
        if(freeSpawnPoints.Count <= 0)
        {
            return;
        }

        int randomSpawnPoint = Random.Range(0, freeSpawnPoints.Count);
        var _sp = freeSpawnPoints[randomSpawnPoint];
        freeSpawnPoints.RemoveAt(randomSpawnPoint);
        Instantiate(_enemy, _sp.position, _sp.rotation);      
    }

}
