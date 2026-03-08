using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [Header("Movement")]
    public float moveInterval = 0.3f; // Her kac saniyede bir hareket etsin
    private float moveTimer;

    [Header("Direction")]
    private Vector3 currentDirection = Vector3.right; // Baslangicta saga git
    private Vector3 inputDirection = Vector3.right;

    [Header("Body")]
    public GameObject bodyPrefab;
    public List<Transform> bodyParts = new List<Transform>();

    [Header("Materials")]
    public Material headMaterial;
    public Material bodyMaterial;

    void Start()
    {
        // Basi listeye ekle
        bodyParts.Add(transform);
        moveTimer = moveInterval;

        // Basa materyal ata
        if (headMaterial != null)
            GetComponent<Renderer>().material = headMaterial;
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver)
            return;

        HandleInput();

        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            Move();
            moveTimer = moveInterval;
        }
    }

    void HandleInput()
    {
        // Ters yone gidemez (ornegin saga giderken sola donemez)
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentDirection != Vector3.back)
            inputDirection = Vector3.forward;
        else if (Input.GetKeyDown(KeyCode.DownArrow) && currentDirection != Vector3.forward)
            inputDirection = Vector3.back;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentDirection != Vector3.right)
            inputDirection = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentDirection != Vector3.left)
            inputDirection = Vector3.right;
        else if (Input.GetKeyDown(KeyCode.W) && currentDirection != Vector3.back)
            inputDirection = Vector3.forward;
        else if (Input.GetKeyDown(KeyCode.S) && currentDirection != Vector3.forward)
            inputDirection = Vector3.back;
        else if (Input.GetKeyDown(KeyCode.A) && currentDirection != Vector3.right)
            inputDirection = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.D) && currentDirection != Vector3.left)
            inputDirection = Vector3.right;
    }

    void Move()
    {
        currentDirection = inputDirection;

        // Onceki pozisyonlari kaydet
        List<Vector3> previousPositions = new List<Vector3>();
        foreach (Transform part in bodyParts)
        {
            previousPositions.Add(part.position);
        }

        // Basi hareket ettir
        Vector3 newHeadPos = bodyParts[0].position + currentDirection;

        // Sinir kontrolu (duvardan gecme)
        int gw = GameManager.Instance.gridWidth;
        int gh = GameManager.Instance.gridHeight;

        if (newHeadPos.x < 0 || newHeadPos.x >= gw ||
            newHeadPos.z < 0 || newHeadPos.z >= gh)
        {
            GameManager.Instance.GameOver();
            return;
        }

        // Kendine carpma kontrolu
        for (int i = 1; i < bodyParts.Count; i++)
        {
            if (Vector3.Distance(newHeadPos, bodyParts[i].position) < 0.5f)
            {
                GameManager.Instance.GameOver();
                return;
            }
        }

        // Basi yeni pozisyona tasi
        bodyParts[0].position = newHeadPos;

        // Govde parcalarini takip ettir
        for (int i = 1; i < bodyParts.Count; i++)
        {
            bodyParts[i].position = previousPositions[i - 1];
        }

        // Yemek kontrolu
        CheckFoodCollision();
    }

    void CheckFoodCollision()
    {
        GameObject food = GameObject.FindGameObjectWithTag("Food");
        if (food != null)
        {
            float dist = Vector3.Distance(bodyParts[0].position, food.transform.position);
            if (dist < 0.5f)
            {
                Grow();
                GameManager.Instance.AddScore();
                GameManager.Instance.SpawnFood();
            }
        }
    }

    void Grow()
    {
        // Son parcayi bul ve onun pozisyonuna yeni parca ekle
        Transform lastPart = bodyParts[bodyParts.Count - 1];
        GameObject newPart = Instantiate(bodyPrefab, lastPart.position, Quaternion.identity);

        if (bodyMaterial != null)
            newPart.GetComponent<Renderer>().material = bodyMaterial;

        bodyParts.Add(newPart.transform);
    }
}