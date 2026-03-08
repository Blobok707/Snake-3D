using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 10;

    [Header("References")]
    public GameObject foodPrefab;

    [Header("Game State")]
    public int score = 0;
    public bool isGameOver = false;

    private GameObject currentFood;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        SpawnFood();
    }

    void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void SpawnFood()
    {
        if (currentFood != null)
            Destroy(currentFood);

        int x, z;
        int attempts = 0;

        // Yilanin uzerinde olmayan bir pozisyon bul
        do
        {
            x = Random.Range(0, gridWidth);
            z = Random.Range(0, gridHeight);
            attempts++;
        }
        while (IsPositionOccupied(x, z) && attempts < 100);

        Vector3 spawnPos = new Vector3(x, 0.5f, z);
        currentFood = Instantiate(foodPrefab, spawnPos, Quaternion.identity);
    }

    bool IsPositionOccupied(int x, int z)
    {
        Snake snake = FindFirstObjectByType<Snake>();
        if (snake == null) return false;

        foreach (Transform segment in snake.bodyParts)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.z) == z)
                return true;
        }
        return false;
    }

    public void AddScore()
    {
        score += 10;
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER! Skor: " + score + " | R tusuna bas ile yeniden basla");
    }
}