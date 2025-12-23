using UnityEngine;

public class CatPickup : MonoBehaviour
{
    [SerializeField] AudioClip[] catSFXArray;
    AudioSource catSource;
    public int catScoreAmount = 10;

    [Header("Movement")]
    public float speed = 10f;

    [Header("Lifetime")]
    public float destroyZ = -20f;  // when it passes behind the player
    public float maxLifeTime = 20f;

    private float _lifeTimer;

    private void Awake()
    {
        catSource = GetComponent<AudioSource>();
    }
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
        if (other.GetComponent<PlayerMovement>() != null)
        {
            CatCollected();
        }
    }
    public void CatCollected()
    {
        int pick = Random.Range(0, catSFXArray.Length);
        catSource.clip = catSFXArray[pick];
        catSource.Play();

        //score things
        WitchScoreSystem.instance.score += catScoreAmount;

        Destroy(gameObject, 1f);
    }
}
