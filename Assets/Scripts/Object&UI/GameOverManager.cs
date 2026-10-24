using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Экран поражения")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameplayHUD;

    private bool gameEnded = false;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (gameOverPanel == null)
        {
            Debug.LogError("GameOverPanel не назначена! ", this);
            return;
        }

        gameOverPanel.SetActive(false);
        
        if (gameplayHUD == null)
        {
            Debug.LogError("GameplayHUD не назначен!", this);
        }
    }

    public void ShowGameOver()
    {
        if (gameEnded)
            return;

        if (gameOverPanel == null)
        {
            Debug.LogError("GameOverPanel не назначена!", this);
            return;
        }

        gameEnded = true;

        if (gameplayHUD != null)
            gameplayHUD.SetActive(false);

        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        Debug.Log("Game Over");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Выход из игры");
        Application.Quit();
    }
}
