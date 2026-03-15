using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [Header("Movement")]
    public float moveInterval = 0.3f;
    private float moveTimer;

    [Header("Smooth Movement")]
    public float smoothSpeed = 15f;

    [Header("Start Settings")]
    public int startBodyCount = 2; // Baslangicta kac govde parcasi olsun
    private bool hasStarted = false;

    [Header("Direction")]
    private Vector3 currentDirection = Vector3.right;
    private Vector3 inputDirection = Vector3.right;

    [Header("Body")]
    public GameObject bodyPrefab;
    public List<Transform> bodyParts = new List<Transform>();
    private List<Vector3> targetPositions = new List<Vector3>();

    [Header("Materials")]
    public Material headMaterial;
    public Material bodyMaterial;

    void Start()
    {
        // Basi ekle
        bodyParts.Add(transform);
        targetPositions.Add(transform.position);

        if (headMaterial != null)
            GetComponent<Renderer>().material = headMaterial;

        // Baslangic govde parcalarini olustur (basindan geriye dogru)
        for (int i = 1; i <= startBodyCount; i++)
        {
            Vector3 partPos = transform.position + Vector3.left * i;
            GameObject newPart = Instantiate(bodyPrefab, partPos, Quaternion.identity);

            if (bodyMaterial != null)
                newPart.GetComponent<Renderer>().material = bodyMaterial;

            bodyParts.Add(newPart.transform);
            targetPositions.Add(partPos);
        }

        moveTimer = moveInterval;
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver)
            return;

        // Oyun henuz baslamadiysa tus bekleme
        if (!hasStarted)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                HandleInput();
                hasStarted = true;
                moveTimer = moveInterval;
            }
            return;
        }

        HandleInput();

        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            Move();
            moveTimer = moveInterval;
        }

        SmoothMove();
    }

    void HandleInput()
    {
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && currentDirection != Vector3.back)
            inputDirection = Vector3.forward;
        else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && currentDirection != Vector3.forward)
            inputDirection = Vector3.back;
        else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && currentDirection != Vector3.right)
            inputDirection = Vector3.left;
        else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && currentDirection != Vector3.left)
            inputDirection = Vector3.right;
    }

    void Move()
    {
        currentDirection = inputDirection;

        List<Vector3> previousTargets = new List<Vector3>(targetPositions);

        Vector3 newHeadPos = targetPositions[0] + currentDirection;

        // Sinir kontrolu
        int gw = GameManager.Instance.gridWidth;
        int gh = GameManager.Instance.gridHeight;

        if (newHeadPos.x < 0 || newHeadPos.x >= gw ||
            newHeadPos.z < 0 || newHeadPos.z >= gh)
        {
            SnapAllToTarget();
            GameManager.Instance.GameOver();
            return;
        }

        // Kendine carpma kontrolu
        for (int i = 1; i < targetPositions.Count; i++)
        {
            if (Vector3.Distance(newHeadPos, targetPositions[i]) < 0.5f)
            {
                SnapAllToTarget();
                GameManager.Instance.GameOver();
                return;
            }
        }

        targetPositions[0] = newHeadPos;

        for (int i = 1; i < targetPositions.Count; i++)
        {
            targetPositions[i] = previousTargets[i - 1];
        }

        CheckFoodCollision();
    }

    void SmoothMove()
    {
        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (bodyParts[i] != null)
            {
                bodyParts[i].position = Vector3.Lerp(
                    bodyParts[i].position,
                    targetPositions[i],
                    Time.deltaTime * smoothSpeed
                );
            }
        }
    }

    void SnapAllToTarget()
    {
        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (bodyParts[i] != null)
                bodyParts[i].position = targetPositions[i];
        }
    }

    void CheckFoodCollision()
    {
        GameObject food = GameObject.FindGameObjectWithTag("Food");
        if (food != null)
        {
            float dist = Vector3.Distance(targetPositions[0], food.transform.position);
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
        Transform lastPart = bodyParts[bodyParts.Count - 1];
        Vector3 lastTarget = targetPositions[targetPositions.Count - 1];

        GameObject newPart = Instantiate(bodyPrefab, lastPart.position, Quaternion.identity);

        if (bodyMaterial != null)
            newPart.GetComponent<Renderer>().material = bodyMaterial;

        bodyParts.Add(newPart.transform);
        targetPositions.Add(lastTarget);
    }

    public bool HasStarted()
    {
        return hasStarted;
    }
}