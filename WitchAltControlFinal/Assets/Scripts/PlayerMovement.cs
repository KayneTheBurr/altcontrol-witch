using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    public ArduinoConnector arduino;  // drag your ArduinoConnector object here in Inspector
    public bool manualMove = false;
    public bool inMenu = true;

    AudioSource myAudiosource;
    public GameObject gameOverPanel;
    public TMP_Text endScoreLabel;
    public GameObject gameStartPanel;

    [Header("PlayerStats")]
    public int playerHealth = 3;
    public Image heart1, heart2, heart3;

    [Header("Movement Range")]
    public float xMin = -5f;  // left edge of lane area
    public float xMax = 5f;  // right edge of lane area
    private void Awake()
    {
        myAudiosource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        gameStartPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (!manualMove && !inMenu)
        {
            //if (arduino == null) return;

            // Use the smoothed value for nicer motion
            float t = ArduinoConnector.instance.smoothedRunner01; // 0 (right) -> 1 (left)

            // Map 0..1 to xMin..xMax
            float x = Mathf.Lerp(xMin, xMax, t);

            Vector3 pos = transform.position;
            pos.x = x;
            transform.position = pos;
        }
        if(inMenu)
        {
            if (gameStartPanel.activeSelf)
            {
                if (ArduinoConnector.instance.extraButtonPressed)
                {
                    StartGame();
                }
            }
            else if (gameOverPanel.activeSelf)
            {
                if (ArduinoConnector.instance.extraButtonPressed)
                {
                    RestartGame();
                }
            }
                
            
        }
    }
    public void TakeDamage()
    {
        myAudiosource.Play();

        switch (playerHealth)
        {
            case 3:
                heart3.gameObject.SetActive(false);
                playerHealth--;
                break;
            case 2:
                heart2.gameObject.SetActive(false);
                playerHealth--;
                break;
            case 1:
                heart1.gameObject.SetActive(false);
                playerHealth--;
                PlayerDead();
                break;
            default:

                break;
        }

    }

    public void StartGame()
    {
        if (!inMenu) return;
        Debug.Log("START GAME");
        gameStartPanel.SetActive(false);
        inMenu = false;
        ObstacleSpawner.instance.playTime = 0;
    }
    
    public void PlayerDead()
    {
        Debug.Log("YOU DIED");
        endScoreLabel.text = $"Final Score: {WitchScoreSystem.instance.score}";
        gameOverPanel.SetActive(true);
        inMenu = true;
    }
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
