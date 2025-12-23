using UnityEngine;

public class CatSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject catPrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public float spawnZOffset = 50f;
    public float xMin = -10f;
    public float xMax = 10f;

    public float spawnInterval = 1.5f;
    public float spawnIntervalRandom = 0.5f;

    [Header("Tree Movement")]
    public float catSpeed = 10f;

    private float _spawnTimer;

    void Update()
    {
        if (!player.GetComponent<PlayerMovement>().inMenu)
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                SpawnCat();
                // next spawn time random
                _spawnTimer = spawnInterval + Random.Range(-spawnIntervalRandom, spawnIntervalRandom);
                if (_spawnTimer < 0.1f) _spawnTimer = 0.1f;
            }
        }
       
    }

    void SpawnCat()
    {
        if (catPrefab == null || player == null) return;

        float x = Random.Range(xMin, xMax);
        float y = player.position.y;
        float z = player.position.z + spawnZOffset;

        Vector3 spawnPos = new Vector3(x, y, z);
        Quaternion spawnRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
        GameObject tree = Instantiate(catPrefab, spawnPos, Quaternion.identity);

        // setmovement speed
        CatPickup obstacle = tree.GetComponent<CatPickup>();
        if (obstacle != null)
        {
            obstacle.speed = catSpeed;
        }
    }
}
