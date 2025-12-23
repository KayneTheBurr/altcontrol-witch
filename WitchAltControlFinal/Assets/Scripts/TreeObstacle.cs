using UnityEngine;

public class TreeObstacle : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;  

    [Header("Lifetime")]
    public float destroyZ = -20f;  // when it passes behind the player
    public float maxLifeTime = 20f;

    private float _lifeTimer;

    void Update()
    {
        // Move along -Z
        transform.Translate(0f, 0f, -speed * Time.deltaTime, Space.World);

        _lifeTimer += Time.deltaTime;

        // Destroy if too far behind or too old
        if (transform.position.z < destroyZ || _lifeTimer > maxLifeTime)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerMovement>() != null)
        {
            other.GetComponent<PlayerMovement>().TakeDamage();
        }
    }
}
