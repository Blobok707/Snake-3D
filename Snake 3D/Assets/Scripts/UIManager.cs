using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public Text scoreText;
    public Text gameOverText;
    public Text startText;

    void Start()
    {
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        if (startText != null)
        {
            startText.gameObject.SetActive(true);
            startText.text = "Baslamak icin ok tusuna bas";
        }
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        // Skor guncelle
        if (scoreText != null)
            scoreText.text = "Skor: " + GameManager.Instance.score;

        // Baslangic yazisini gizle
        Snake snake = FindFirstObjectByType<Snake>();
        if (snake != null && snake.HasStarted() && startText != null)
        {
            startText.gameObject.SetActive(false);
        }

        // Game Over goster
        if (GameManager.Instance.isGameOver && gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "OYUN BITTI!\nSkor: " + GameManager.Instance.score +
                               "\nYeniden baslamak icin R'ye bas";
        }
    }
}