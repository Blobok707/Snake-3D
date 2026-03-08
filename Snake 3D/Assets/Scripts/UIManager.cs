using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public Text scoreText;
    public Text gameOverText;

    void Start()
    {
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        // Skor guncelle
        if (scoreText != null)
            scoreText.text = "Skor: " + GameManager.Instance.score;

        // Game Over goster
        if (GameManager.Instance.isGameOver && gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "OYUN BITTI!\nSkor: " + GameManager.Instance.score +
                               "\nYeniden baslamak icin R'ye bas";
        }
    }
}