using TMPro;
using UnityEngine;

public class WitchScoreSystem : MonoBehaviour
{
    public static WitchScoreSystem instance;

    public int score = 0;
    public TMP_Text scoreText;
    float scoreTimer = 1;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void AddScore(int scoreAdded)
    {
        score += scoreAdded;
        UpdateScoreText();
    }
    private void Update()
    {
        //add 1 score per second
        if (scoreTimer > 0)
        {
            scoreTimer -= Time.deltaTime;
        }
        else
        {
            AddScore(1);
            scoreTimer = 1;
        }
    }
    public void UpdateScoreText()
    {
        scoreText.text = $"Score: {score}";
    }
}
