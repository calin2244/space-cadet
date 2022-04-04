using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine;

public class EventAlertSpawner : MonoBehaviour
{
        
    #region EnumState
    public enum DisSpawnState { SPAWNING, WAITING, COUNTING };
    
    #endregion

    #region Variables
 
    [System.Serializable]
    public class Disasters
    {
        public Transform asteroidRain;
        public Transform rocketRain;
        public int asteroidCount;
        public int rocketCount;
        public float asteroidRate;
        public float rocketRate;
    }
 
    private Disasters diaster1;
 
    public Disasters[] disaster;
    private int nextDisaster = 0;
    public int NextDisaster
    {
        get { return nextDisaster + 1; }
    }

    [System.Serializable]
    public class SpawnPoint
    {
        public Vector3 SpawnLocation;
        public Quaternion Rotation;
        public bool CanSpawn;
    }

    public SpawnPoint[] asteroidSpawnPoints;
    public SpawnPoint[] rocketSpawnPoints;
 
    public float timeBetweenWaves = 5;
 
    private float disasterWaveCountdown;
 
    public float DisasterWaveCountdown
    {
        get { return disasterWaveCountdown; }
    }
 
    private float searchCountdown = 1f;
 
    private DisSpawnState state = DisSpawnState.COUNTING;
 
    public DisSpawnState State
    {
        get { return state; }
    }
 
    public float xBoundary = -12.5f;
 
    private int randomSpawnPoint;

    int newIndex = 0;
    public GameObject rocketSprite;
    public GameObject asteroidSprite;
    [SerializeField] private int minAstCount, maxAstCount;

    #endregion
 
    #region UnityStartUpdate
    
    private void Awake() 
    {
        StartCoroutine(RandIndex());
    }
    void Start()
    {
        if (asteroidSpawnPoints.Length == 0)
        {
            Debug.LogError("Disaster spawn Points missing");
        }
        
        if(rocketSpawnPoints.Length == 0)
        {
            Debug.LogError("Rocket Rain SpawnPoints missing");
        }
 
        //timeBetweenWaves = Random.Range(25,40);
        disasterWaveCountdown = timeBetweenWaves;
    }
 
    // Update is called once per frame
    void Update()
    {
        if (state == DisSpawnState.WAITING)
        {
            if (!DisasterIsAlive())
            {
                WaveCompleted();
            }
            else
            {
                return;
            }
        }
 
        if (disasterWaveCountdown <= 0)
        {
            if (state != DisSpawnState.SPAWNING)
            {
                StartCoroutine(SpawnAlertWaves(disaster[nextDisaster]));
            }
        }
        else
        {
            disasterWaveCountdown -= Time.deltaTime;
        }
        //Debug.Log("The time between waves is " + timeBetweenWaves);
 
    }
    #endregion
 
    #region WaveCompleted
    void WaveCompleted()
    {
        Debug.Log("Wave Completed");
 
        asteroidSprite.GetComponent<SpriteRenderer>().enabled = false;
        rocketSprite.GetComponent<SpriteRenderer>().enabled = false;
        timeBetweenWaves = Random.Range(10,14);
        state = DisSpawnState.COUNTING;
        disasterWaveCountdown = timeBetweenWaves;
        
    }
    #endregion
    
    #region CheckingObjectsWithDisasterTag
    bool DisasterIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Disasters") == null)
            {
                return false;
            }
        }
        return true;
    }
    #endregion
 
    #region Spawning
    IEnumerator SpawnAlertWaves(Disasters _disaster)
    {       
        state = DisSpawnState.SPAWNING;
        _disaster.asteroidCount = Random.Range(minAstCount, maxAstCount);
        Debug.Log("Asteroid Count is " + _disaster.asteroidCount);

        if(newIndex % 2 ==0)
        {
            for (int i = 0; i <= _disaster.asteroidCount; i++)
            {
                asteroidSprite.GetComponent<SpriteRenderer>().enabled = true;
                StartCoroutine(SpawnWaveAsteroid(_disaster.asteroidRain));
                yield return new WaitForSeconds(Random.Range(5.5f, 8f) / _disaster.asteroidRate);
            }
        }
        else if(newIndex%2!=0)
        {
            for (int i = 0; i <= _disaster.rocketCount; i++)
            {
                rocketSprite.GetComponent<SpriteRenderer>().enabled = true;
                StartCoroutine(SpawnWaveRocket(_disaster.rocketRain));
                yield return new WaitForSeconds(Random.Range(5.5f, 8f) / _disaster.rocketRate);
            }
        }
            
        if(GameObject.FindGameObjectWithTag("Disasters") == null && state == DisSpawnState.WAITING)
        {
            WaveCompleted();
            StartCoroutine(RandIndex());
        }
        //RandomIndex();
        
        
 
        state = DisSpawnState.WAITING;
        //StopCoroutine(SpawnWaveAsteroid(_disaster.asteroidRain));
        //StopCoroutine(SpawnWaveRocket(_disaster.rocketRain));
        yield break;
    }

    IEnumerator SpawnWaveAsteroid(Transform _disastr)
    {
 
        randomSpawnPoint = Random.Range(0, asteroidSpawnPoints.Length);
        var _sp = asteroidSpawnPoints[randomSpawnPoint];

        if (_sp.CanSpawn)
        {
            Debug.Log("Spawning Asteroid");
            Instantiate(_disastr, _sp.SpawnLocation, _sp.Rotation);
            _sp.CanSpawn = false;
            yield return new WaitForSeconds(1);
            _sp.CanSpawn = true;
        }
    }

    IEnumerator SpawnWaveRocket(Transform _disastr)
    {
 
        randomSpawnPoint = Random.Range(0, rocketSpawnPoints.Length);
        var _sp = rocketSpawnPoints[randomSpawnPoint];

        if (_sp.CanSpawn)
        {
            Debug.Log("Spawning Rockets");
            Instantiate(_disastr, _sp.SpawnLocation, _sp.Rotation);
            _sp.CanSpawn = false;
            yield return new WaitForSeconds(1);
            _sp.CanSpawn = true;
        }
    }

    int RandomIndex()
    {
        newIndex = Random.Range(1,21);
        return newIndex;
    }

    private IEnumerator RandIndex()
    {
        newIndex = Random.Range(1,21);
        yield break;
    }

    #endregion
}