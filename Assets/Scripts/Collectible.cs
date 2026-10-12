using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager scoreManager =
                FindFirstObjectByType<ScoreManager>();
            
            if (scoreManager != null)
            {
                scoreManager.AddScore();
            }

            Destroy(gameObject);
        }
    }
}
