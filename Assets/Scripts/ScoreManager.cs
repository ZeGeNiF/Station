using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject victoryText;

    private int score = 0;
    private int totalModules = 10;

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore()
    {
        score++;
        UpdateScoreText();

        if (score >= totalModules)
        {
            victoryText.SetActive(true);
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "Энергомодули: " + score + "/" + totalModules;
    }
}

