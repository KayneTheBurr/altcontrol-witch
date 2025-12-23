using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner instance;
    [Header("References")]
    public GameObject treePrefab;       
    public Transform player;            

    [Header("Spawn Settings")]
    public float spawnZOffset = 50f; 
    public float xMin = -10f;
    public float xMax = 10f;

    public float timeSpawnRateScalar = 10;
    public float timeSpeedScalar = 5;
    public int maxSpeed = 100;
    public float playTime = 0;

    public float spawnInterval = 1.5f; 
    public float spawnIntervalRandom = 0.5f; 

    [Header("Tree Movement")]
    public float treeSpeed = 20f;

    private float _spawnTimer;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playTime = 0;
    }
    void Update()
    {
        playTime += Time.deltaTime;

        if (!player.GetComponent<PlayerMovement>().inMenu)
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                SpawnObstacle();
                // next spawn time random
                _spawnTimer = (spawnInterval - playTime / timeSpawnRateScalar) + Random.Range(-spawnIntervalRandom, spawnIntervalRandom);
                if (_spawnTimer < 0.01f) _spawnTimer = 0.01f;
            }
        }
            
    }

    void SpawnObstacle()
    {
        if (treePrefab == null || player == null) return;

        float x = Random.Range(xMin, xMax);
        float y = player.position.y;
        float z = player.position.z + spawnZOffset;

        Vector3 spawnPos = new Vector3(x, y, z);
        Quaternion spawnRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
        GameObject tree = Instantiate(treePrefab, spawnPos, Quaternion.identity);

        // setmovement speed
        TreeObstacle obstacle = tree.GetComponent<TreeObstacle>();
        if (obstacle != null)
        {
            obstacle.speed = Mathf.Min(maxSpeed, treeSpeed + (playTime / timeSpeedScalar));
        }
    }
}
